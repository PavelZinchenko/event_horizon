using System;

public static class RandomExtension
{
	public static float Next(this Random random, float minValue, float maxValue)
	{
		return minValue + (maxValue - minValue)*(random.Next(1000)/1000f);
	}

    public static int Next2(this Random random, int value)
    {
        return random.Next(random.Next(value+1));
    }

    public static int Next3(this Random random, int value)
    {
        return random.Next(random.Next(random.Next(value + 2)));
    }

    public static bool Percentage(this Random random, int value)
    {
        if (value < 0)
            return false;
        if (value > 100)
            return true;

        return random.Next(100) < value;
    }

    public static float NextFloat(this Random random)
    {
        return (float)random.NextDouble();
    }

    public static float NextFloatSigned(this Random random)
    {
        return 2f*((float)random.NextDouble()) - 1f;
    }

    public static float NextFloatNormal(this Random random)
    {
        return (float)((2*random.NextDouble() - 1)*(2*random.NextDouble() - 1));
    }

    public static int Range(this Random random, int min, int max)
    {
        if (min > max)
        {
            var t = min;
            min = max;
            max = t;
        }

        return random.Next(min, max + 1);
    }

    public static int SquareRange(this Random random, int min, int max)
    {
        if (min > max)
        {
            var t = min;
            min = max;
            max = t;
        }

        return random.Next(min, random.Next(min, max + 1) + 1);
    }
}
