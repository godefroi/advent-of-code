using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace aoc_2019.Intcode
{
	internal abstract class Computer
	{
		public Computer()
		{
			Instructions = new Dictionary<int, Instruction>();

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

		protected int[] Core { get; set; }

		protected InputStream<int> Input { get; set; }

		protected Dictionary<int, Instruction> Instructions { get; }

		public List<int> Output { get; private set; }

		public virtual int[] Run(int[] program, IEnumerable<int> input = null)
		{
			Core   = program;
			Input  = input == null ? null : new InputStream<int>(input);
			Output = new List<int>();

			var pos = 0;

			while( true ) {
				// make sure we didn't blow past the end of the program somehow
				if( pos > Core.Length - 1 )
					throw new InvalidOperationException("Executed past end of program");

				// opcode 99 is program end
				if( Core[pos] == 99 )
					return Core;

				// get the instruction and build the array of parameters
				var (inst, modes) = DecodeInstruction(Core[pos]);
				var prms = new int[inst.ParameterCount];

				// set the paramters based on the mode for each
				for( var i = 0; i < prms.Length; i++ ) {
					prms[i] = modes[i] switch {
						ParameterMode.Immediate => Core[pos + 1 + i],
						ParameterMode.Position  => Core[Core[pos + 1 + i]],
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
		}

		protected (Instruction Instruction, List<ParameterMode> Modes) DecodeInstruction(int opcode)
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
			foreach( var ap in instr.AddressParameters )
				modes[ap] = ParameterMode.Immediate;

			return (instr, modes);
		}

		[AttributeUsage(AttributeTargets.Method)]
		protected class OpCodeAttribute : Attribute
		{
			public OpCodeAttribute(int opCode)
			{
				OpCode = opCode;
			}

			public int OpCode { get; }
		}

		[AttributeUsage(AttributeTargets.Parameter)]
		protected class AddressAttribute : Attribute { }

		protected class Instruction
		{
			private object     m_computer;
			private MethodInfo m_method;
			private bool       m_outp;

			public Instruction(Computer computer, MethodInfo method)
			{
				var parms = method.GetParameters();
				var aps   = new List<int>();

				ParameterCount = parms.Length;
				m_computer     = computer;
				m_method       = method;

				for( var i = 0; i < parms.Length; i++ ) {
					if( parms[i].GetCustomAttribute<AddressAttribute>() != null )
						aps.Add(i);

					if( parms[i].IsOut) {
						if( i < parms.Length - 1 )
							throw new InvalidOperationException("Output parameter for IP change is only supported as the final parameter");

						ParameterCount = parms.Length - 1;
						m_outp         = true;
					}
				}

				AddressParameters = aps.ToArray();
			}

			public int ParameterCount { get; }

			public int[] AddressParameters { get; }

			public int? Execute(int[] parameters)
			{
				if( m_outp ) {
					var prms = new object[parameters.Length + 1];

					Array.Copy(parameters, 0, prms, 0, parameters.Length);

					m_method.Invoke(m_computer, prms);

					return (int?)prms[prms.Length - 1];
				} else {
					m_method.Invoke(m_computer, parameters.Select(i => (object)i).ToArray());

					return null;
				}
			}
		}

		protected class InputStream<T>
		{
			private IEnumerable<T> m_input;
			private IEnumerator<T> m_enumerator;

			public InputStream(IEnumerable<T> input)
			{
				m_input      = input;
				m_enumerator = input.GetEnumerator();
			}

			public T Next()
			{
				if( m_enumerator.MoveNext() )
					return m_enumerator.Current;
				else
					throw new InvalidOperationException("No more input available");
			}
		}

		protected enum ParameterMode
		{
			Position  = 0,
			Immediate = 1,
		}
	}
}
