using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using aoc_2019.Intcode.Infrastructure;

namespace aoc_2019.Intcode
{
	internal abstract class Computer
	{
		private bool m_terminated;
		private long m_ip;

		public Computer()
		{
			Core         = new SparseArray<long>(Enumerable.Empty<long>());
			Input        = new InputStream<long>(Enumerable.Empty<long>());
			Output       = new OutputStream<long>();
			Instructions = new Dictionary<int, Instruction>();
			Terminated   = true;

			Console.WriteLine($"Initializing Computer implementation {GetType().Name}");

			foreach( var mi in GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) ) {
				var oca = mi.GetCustomAttribute<OpCodeAttribute>();

				if( oca != null ) {
					Console.WriteLine($"\tOpcode: {oca.OpCode} for method {mi.Name}({string.Join(", ", mi.GetParameters().Select(p => p.ParameterType.Name))})");
					Instructions.Add(oca.OpCode, new Instruction(this, mi));
				}
			}

			Console.WriteLine();
		}

		protected SparseArray<long> Core { get; set; }

		protected InputStream<long> Input { get; set; }

		protected Dictionary<int, Instruction> Instructions { get; }

		protected OutputStream<long> Output { get; private set; }

		public event EventHandler<int>? OutputReady;

		public bool Terminated
		{
			get {
				lock( this )
					return m_terminated;
			}
			private set {
				lock( this )
					m_terminated = value;
			}
		}

		public long RelativeBase { get; protected set; }

		public bool OutputAvailable => Output.OutputAvailable;

		public void AddInput(long input) => Input.Add(input);

		public long GetOutput() => Output.Read();

		public virtual void Initialize(long[] program, IEnumerable<long>? input = null)
		{
			Core         = new SparseArray<long>(program);
			Input        = input == null ? new InputStream<long>(Array.Empty<long>()) : new InputStream<long>(input);
			Output       = new OutputStream<long>();
			Terminated   = false;
			RelativeBase = 0;
			m_ip         = 0;
		}

		public virtual InterruptType Run()
		{
			while( true ) {
				// make sure we didn't blow past the end of the program somehow
				if( m_ip > Core.Length - 1 )
					throw new InvalidOperationException("Executed past end of program");

				// opcode 99 is program end
				if( Core[m_ip] == 99 ) {
					Terminated = true;
					return InterruptType.Terminated;
				}

				// get the instruction and build the array of parameters
				var (inst, modes) = DecodeInstruction(Core[m_ip]);
				var prms = new long[inst.ParameterCount];

				// set the paramters based on the mode for each
				for( var i = 0; i < prms.Length; i++ ) {
					prms[i] = modes[i] switch
					{
						ParameterMode.Immediate => m_ip + 1 + i,
						ParameterMode.Position => Core[m_ip + 1 + i],
						ParameterMode.Relative => RelativeBase + Core[m_ip + 1 + i],
						_ => throw new InvalidOperationException($"Unknown parameter mode {modes[i]}")
					};
				}

				var jmp  = default(long?);
				var outp = false;

				// execute the instruction
				try {
					jmp = inst.Execute(prms);
				} catch( TargetInvocationException ex ) {
					switch( ex.InnerException ) {
						case InputNeededException _:
							return InterruptType.Input;
						case OutputReadyException _:
							outp = true;
							break;
						default:
							throw ex.InnerException ?? new Exception();
					}
				}

				// either execute the jump, or advance past the opcode plus parameters
				if( jmp.HasValue )
					m_ip = jmp.Value;
				else
					m_ip += inst.ParameterCount + 1;

				// currently this won't happen, but we can make it happen if we need it
				if( outp )
					return InterruptType.Output;
			}
		}

		protected (Instruction Instruction, List<ParameterMode> Modes) DecodeInstruction(long opcode)
		{
			var ocstr   = opcode.ToString().PadLeft(2, '0');
			var inst_id = Convert.ToInt32(ocstr.Substring(ocstr.Length - 2));
			var modes   = ocstr.Reverse().Skip(2).Select(c => (ParameterMode)Convert.ToInt32(c.ToString())).ToList();

			// make sure we understand the opcode
			if( !Instructions.ContainsKey(inst_id) )
				throw new InvalidOperationException($"The opcode {inst_id} is not valid for this implementation.");

			// get the instruction
			var instr = Instructions[inst_id];

			// add enough default modes to cover all the parameters
			while( modes.Count < instr.ParameterCount )
				modes.Add(ParameterMode.Position);

			// address parameters we treat as immediate
			//foreach( var ap in instr.AddressParameters )
			//	modes[ap] = ParameterMode.Immediate;

			return (instr, modes);
		}

		protected virtual void OnOutputReady(int cnt) => OutputReady?.Invoke(this, cnt);

		public static long[] Parse(string program) => program.Split(',').Select(a => Convert.ToInt64(a)).ToArray();

		[AttributeUsage(AttributeTargets.Method)]
		protected class OpCodeAttribute : Attribute
		{
			public OpCodeAttribute(int opCode)
			{
				OpCode = opCode;
			}

			public int OpCode { get; }
		}

		protected class Instruction
		{
			private object     m_computer;
			private MethodInfo m_method;
			private bool       m_outp;

			public Instruction(Computer computer, MethodInfo method)
			{
				var parms = method.GetParameters();

				ParameterCount = parms.Length;
				m_computer     = computer;
				m_method       = method;

				if( method.ReturnType == typeof(long?) )
					m_outp = true;
			}

			public int ParameterCount { get; }

			public long? Execute(long[] parameters)
			{
				if( m_outp ) {
					return m_method.Invoke(m_computer, parameters.Select(i => (object)i).ToArray()) as long?;
				} else {
					m_method.Invoke(m_computer, parameters.Select(i => (object)i).ToArray());

					return null;
				}
			}
		}

		protected enum ParameterMode
		{
			Position  = 0,
			Immediate = 1,
			Relative  = 2,
		}

		public enum InterruptType
		{
			Input,
			Output,
			Terminated
		}
	}
}
