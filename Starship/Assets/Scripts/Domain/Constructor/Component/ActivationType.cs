using GameDatabase.Enums;

namespace Constructor
{
	//public enum ActivationType
	//{
	//	None,
	//	Manual,
	//	Mixed,
	//}

	public static class ActivationTypeExtension
	{
	    public static int ValidateKey(this ActivationType type, int key)
	    {
	        if (type == ActivationType.None)
	            return -1;
            if (type == ActivationType.Manual && key < 0)
                return 0;

            return key;
	    }
	}
}
