public static class Generic
{
	public static void Swap<T>(ref T first, ref T second)
	{
		T temp = first;
		first = second;
		second = temp;
	}
}