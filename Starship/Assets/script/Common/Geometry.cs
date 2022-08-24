using UnityEngine;

public static class Geometry
{
	public static float Point2VectorDistance(Vector2 point, Vector2 vector)
	{
		var direction = vector.normalized;
		var len = Vector2.Dot(point, direction);

		if (len < 0)
			return point.magnitude;

		if (len > vector.magnitude)
			return Vector2.Distance(point, vector);

		return Vector2.Distance(point, len*direction);
	}

	public static Vector2 VectorSphereIntersection(Vector2 vector, Vector2 center, float radius)
	{
		var direction = vector.normalized;
		var len = Vector2.Dot(center, direction);
		var h2 = (len*direction - center).sqrMagnitude;
		var offset = Mathf.Sqrt(radius*radius - h2);

		var p1 = len - offset;
		var p2 = len + offset;
		var size = vector.magnitude;

		if (p1 >= 0 && p1 < size) 
			return p1*direction;

		if (p2 >= 0 && p2 < size) 
			return p2*direction;

		return Vector2.zero;
	}

	public static Vector2 Bezier(Vector2 start, Vector2 control, Vector2 end, float t)
	{
		return (((1-t)*(1-t)) * start) + (2 * t * (1 - t) * control) + ((t * t) * end);
	}

	public static Vector2 BezierTangent(Vector2 start, Vector2 control, Vector2 end, float t)
	{
		return (2*t-2)*start + (2-4*t)*control + (2*t)*end;
	}

	public static bool GetTargetPosition(
		Position enemyPosition, Vector2 enemyVelocity, Position gunPosition, float bulletSpeed,
		out Position position, out float timeInterval)
	{
		var v0 = enemyVelocity;
		var p = enemyPosition.Direction(gunPosition);
		var v = bulletSpeed;
		
		var a = v0.x*v0.x + v0.y*v0.y - v*v;
		var b = -2*p.x*v0.x - 2*p.y*v0.y;
		var c = p.x*p.x + p.y*p.y;
		
		float t1, t2;
		if (SolveQuadraticEquation(a,b,c, out t1, out t2) && (t1 >= 0 || t2 >= 0))
		{
			timeInterval = t1 < 0 ? t2 : t2 < 0 ? t1 : Mathf.Min(t1,t2);
			position = enemyPosition + v0 * timeInterval;
			return true;
		}
		
		position = Position.Zero;
		timeInterval = 0;
		return false;
	}

	public static bool GetTargetPosition(
		Vector2 enemyPosition, Vector2 enemyVelocity, Vector2 gunPosition, float bulletSpeed,
		out Vector2 position, out float timeInterval)
	{
		var v0 = enemyVelocity;
		var p0 = enemyPosition;
		var p = gunPosition;
		var v = bulletSpeed;
		
		var a = v0.x*v0.x + v0.y*v0.y - v*v;
		var b = 2*(p0.x - p.x)*v0.x + 2*(p0.y - p.y)*v0.y;
		var c = (p0.x - p.x)*(p0.x - p.x) + (p0.y - p.y)*(p0.y - p.y);
		
		float t1, t2;
		if (SolveQuadraticEquation(a,b,c, out t1, out t2) && (t1 >= 0 || t2 >= 0))
		{
			timeInterval = t1 < 0 ? t2 : t2 < 0 ? t1 : Mathf.Min(t1,t2);
			position = p0 + v0 * timeInterval;
			return true;
		}

		position = Vector2.zero;
		timeInterval = 0;
		return false;
	}

	public static void ElasticCollision(Vector2 position1, Vector2 velocity1, float weight1,
	                                    Vector2 position2, Vector2 velocity2, float weight2,
	                                    out Vector2 impulse1, out Vector2 impulse2)
	{
		if (Mathf.Approximately(weight1, 0f) || Mathf.Approximately(weight2, 0f))
		{
			impulse1 = Vector2.zero;
			impulse2 = Vector2.zero;
			return;
		}

		var dir = (position1 - position2).normalized;
		var v1 = Vector2.Dot(velocity1, dir);
		var v2 = Vector2.Dot(velocity2, dir);
		var force1 = 2*(v2 - v1) * weight2 / (weight1 + weight2);
		var force2 = 2*(v2 - v1) * weight1 / (weight1 + weight2);
		
		impulse1 = force1 > 0 ? dir * force1 : Vector2.zero;
		impulse2 = force2 > 0 ? -dir * force2 : Vector2.zero;
	}

	public static bool IsCounterClockwise(Vector2 p1, Vector2 p2, Vector2 p3)
	{
		return (p2.x - p1.x)*(p3.y - p1.y) - (p2.y - p1.y)*(p3.x - p1.x) > 0;
	}

	private static bool SolveQuadraticEquation(float a, float b, float c, out float x1, out float x2)
	{
		x1 = 0;
		x2 = 0;
		
		var d = b*b - 4*a*c;
		
		if (d < 0) 
			return false;
		
		x1 = (-b - Mathf.Sqrt(d))/(2*a);
		x2 = (-b + Mathf.Sqrt(d))/(2*a);
		
		return true;
	}
}
