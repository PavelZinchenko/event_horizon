using UnityEngine;
using System;

public struct Rectangle
{
	public Rectangle(float minX, float minY, float maxX, float maxY)
	{
		_minX = clamp(minX, Position.AreaWidth);
		_maxX = clamp(maxX, Position.AreaWidth);
		_minY = clamp(minY, Position.AreaHeight);
		_maxY = clamp(maxY, Position.AreaHeight);
	}

	public Rectangle(Position min, Position max)
	{
		_minX = min.x;
		_maxX = max.x;
		_minY = min.y;
		_maxY = max.y;
	}

	public static Rectangle FromPoints(Position p1, Position p2)
	{
		var minX = p1.x;
		var minY = p1.y;
		var maxX = p2.x;
		var maxY = p2.y;

		if (Math.Abs(minX - maxX) > Position.AreaWidth/2 == minX < maxX)
		{
			var temp = minX;
			minX = maxX;
			maxX = temp;
		}
		
		if (Math.Abs(minY - maxY) > Position.AreaHeight/2 == minY < maxY)
		{
			var temp = minY;
			minY = maxY;
			maxY = temp;
		}

		return new Rectangle(minX, minY, maxX, maxY);
	}

	public bool Contain(Position position)
	{
		if (_maxX >= _minX)
		{
			if (position.x < _minX || position.x > _maxX)
				return false;
		}
		else
		{
			if (position.x > _maxX && position.x < _minX)
				return false;
		}

		if (_maxY >= _minY)
		{
			if (position.y < _minY || position.y > _maxY)
				return false;
		}
		else
		{
			if (position.y > _maxY && position.y < _minY)
				return false;
		}

		return true;
	}

	public Position Center
	{
		get
		{
			var x = _maxX >= _minX ? (_maxX + _minX)/2 : (_maxX + _minX + Position.AreaWidth)/2;
			var y = _maxY >= _minY ? (_maxY + _minY)/2 : (_maxY + _minY + Position.AreaHeight)/2;
			return new Position(x,y);
		}
	}

	public float Width { get { return _maxX >= _minX ? _maxX - _minX : _maxX - _minX + Position.AreaWidth; } }
	public float Height { get { return _maxY >= _minY ? _maxY - _minY : _maxY - _minY + Position.AreaHeight; } }

	public Position GetPointInRect(float x, float y)
	{
		var px = _minX + Mathf.Clamp01(x)*Width;
		if (px > Position.AreaWidth) px -= Position.AreaWidth;
		var py = _minY + Mathf.Clamp01(y)*Height;
		if (py > Position.AreaHeight) py -= Position.AreaHeight;
		return new Position(px,py);
	}

	public Vector2 Size
	{
		get
		{
			var x = _maxX >= _minX ? _maxX - _minX : Position.AreaWidth - (_minX - _maxX);
			var y = _maxY >= _minY ? _maxY - _minY : Position.AreaHeight - (_minY - _maxY);
			return new Vector2(x,y);
		}
	}

	//public bool Contain(Rectangle other)
	//{
	//	
	//}

	//public bool Intersect(Rectangle other)
	//{
	//}


	public static readonly Rectangle Zero = new Rectangle(0,0,0,0);

	public override string ToString()
	{
		return "(" + _minX + "," + _minY + ") - (" + _maxX + "," + _maxY + ")";
	}

	private static float clamp(float value, float max)
	{
		if (value < 0)
			return max + value % max;
		else if (value >= max)
			return value % max;
		else
			return value;
	}

	private float _minX;
	private float _maxX;
	private float _minY;
	private float _maxY;
}
