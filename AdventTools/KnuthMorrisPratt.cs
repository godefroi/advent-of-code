namespace AdventOfCode;

public class KnuthMorrisPratt
{
	public static void ComputeFailureFunction<T>(T[] pattern, int offset, int length, int[] failure) where T : IEquatable<T>
	{
		var idx = 1; // "i"
		var len = 0; // "j"

		failure[0] = 0; // "f"

		while (idx < length) {
			if (pattern[idx + offset].Equals(pattern[len + offset])) {
				failure[idx++] = ++len;
			} else {
				if (len != 0) {
					len = failure[len - 1];
				} else {
					failure[idx] = 0; // (len is 0, so this could be = len)
					idx++;
				}
			}
		}
	}
}
