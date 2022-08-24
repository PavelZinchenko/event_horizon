public static class DebugCommands
{
	public static int GetHashCode(string deviceId)
	{
		return 0;
	}

	public static int Decode(string command, int hash)
	{
        try
		{
            return System.Convert.ToInt32(command);
		}
		catch (System.Exception)
		{
			return -1;
		}
	}

	public static string Encode(int id, int hash)
	{
		return id.ToString();
	}
}
