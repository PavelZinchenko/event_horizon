using UnityEngine;
using System;

public struct Position
{
	public Position(float x, float y)
	{
		_x = 0;
		_y = 0;
		this.x = x;
		this.y = y;
	}

	public float x
	{
		get
		{
			return _x; 
		} 
		set 
		{
			if (value < 0)
				_x = AreaWidth + value % AreaWidth;
			else if (value >= AreaWidth)
				_x = value % AreaWidth;
			else
				_x = value;
		} 
	}

	public float y
	{
		get
		{
			return _y;
		} 
		set
		{
			if (value < 0)
				_y = AreaHeight + value % AreaHeight;
			else if (value >= AreaHeight)
				_y = value % AreaHeight;
			else
				_y = value;
		}
	}

	public Vector2 ToVector()
	{
		return new Vector2(_x,_y);
	}

	public Vector2 Direction(Position other)
	{
		var halfWidth = AreaWidth/2;
		var x = other._x - _x;
		if (x > halfWidth)
			x -= AreaWidth;
		else if (x < -halfWidth)
			x += AreaWidth;

		var halfHeight = AreaHeight/2;
		var y = other._y - _y;
		if (y > halfHeight)
			y -= AreaHeight;
		else if (y < -halfHeight)
			y += AreaHeight;

		return new Vector2(x,y);
	}

	public static Position Random(System.Random random)
	{
		return new Position(random.NextFloat()*AreaWidth,random.NextFloat()*AreaHeight);
	}	

	public static Position operator+(Position position, Vector2 vector)
	{
		position.x += vector.x;
		position.y += vector.y;
		return position;
	}

	public static Position operator-(Position position, Vector2 vector)
	{
		position.x -= vector.x;
		position.y -= vector.y;
		return position;
	}

	public static Position Lerp(Position first, Position second, float t)
	{
		var halfWidth = AreaWidth/2;
		var x = second._x - first._x;
		if (x > halfWidth)
			x -= AreaWidth;
		else if (x < -halfWidth)
			x += AreaWidth;
		
		var halfHeight = AreaHeight/2;
		var y = second._y - first._y;
		if (y > halfHeight)
			y -= AreaHeight;
		else if (y < -halfHeight)
			y += AreaHeight;

		first.x += x*t;
		first.y += y*t;
		return first;
	}

	public static float DistanceX(float x1, float x2)
	{
		var x = Math.Abs(x1 - x2);
		return Math.Min(x, AreaWidth - x);
	}

	public static float DistanceY(float y1, float y2)
	{
		var y = Math.Abs(y1 - y2);
		return Math.Min(y, AreaHeight - y);
	}
	
	public static float Distance(Position first, Position second)
	{
		return (float)Math.Sqrt(SqrDistance(first, second));
	}

	public static float SqrDistance(Position first, Position second)
	{
		var x = Math.Abs(first._x - second._x);
		x = Math.Min(x, AreaWidth - x);
		var y = Math.Abs(first._y - second._y);
		y = Math.Min(y, AreaHeight - y);
		return x*x + y*y;
	}

	public override string ToString()
	{
		return "(" + _x + "," + _y + ")";
	}

	public static float AreaWidth = 1;
	public static float AreaHeight = 1;
	public static readonly Position Zero = new Position(0,0);

	private float _x;
	private float _y;
}
