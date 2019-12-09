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
		private Thread m_thread;
		private bool   m_terminated;

		public Computer()
		{
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

		public bool AwaitingInput => Input.AwaitingInput;

		public bool Idle => Terminated || AwaitingInput;

		public long RelativeBase { get; protected set; }

		public bool OutputAvailable => Output.OutputAvailable;

		public void AddInput(long input) => Input.AddInput(input);

		public long GetOutput() => Output.Get();

		public virtual SparseArray<long> Run(long[] program, IEnumerable<long> input = null)
		{
			RunInThread(program, input);

			m_thread.Join();

			return Core;
		}

		public virtual void RunInThread(long[] program, IEnumerable<long> input = null)
		{
			Core       = new SparseArray<long>(program);
			Input      = input == null ? new InputStream<long>(new long[0]) : new InputStream<long>(input);
			Output     = new OutputStream<long>();
			Terminated = false;
			RelativeBase = 0;

			m_thread = new Thread(() => {
				var pos = 0L;

				while( true ) {
					// make sure we didn't blow past the end of the program somehow
					//if( pos > Core.Length - 1 )
					//	throw new InvalidOperationException("Executed past end of program");

					// opcode 99 is program end
					if( Core[pos] == 99 ) {
						Terminated = true;
						return;
					}

					// get the instruction and build the array of parameters
					var (inst, modes) = DecodeInstruction(Core[pos]);
					var prms = new long[inst.ParameterCount];

					// set the paramters based on the mode for each
					for( var i = 0; i < prms.Length; i++ ) {
						prms[i] = modes[i] switch
						{
							ParameterMode.Immediate => pos + 1 + i,
							ParameterMode.Position => Core[pos + 1 + i],
							ParameterMode.Relative => RelativeBase + Core[pos + 1 + i],
							_ => throw new InvalidOperationException($"Unknown parameter mode {modes[i]}")
						};
					}

					// execute the instruction
					var jmp = inst.Execute(prms);

					// either execute the jump, or advance past the opcode plus parameters
					if( jmp.HasValue )
						pos = jmp.Value;
					else
						pos += inst.ParameterCount + 1;
				}
			});

			m_thread.Start();
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

				for( var i = 0; i < parms.Length; i++ ) {
					if( parms[i].IsOut) {
						if( i < parms.Length - 1 )
							throw new InvalidOperationException("Output parameter for IP change is only supported as the final parameter");

						ParameterCount = parms.Length - 1;
						m_outp         = true;
					}
				}
			}

			public int ParameterCount { get; }

			public long? Execute(long[] parameters)
			{
				if( m_outp ) {
					var prms = new object[parameters.Length + 1];

					Array.Copy(parameters, 0, prms, 0, parameters.Length);

					m_method.Invoke(m_computer, prms);

					return (long?)prms[prms.Length - 1];
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
	}
}
