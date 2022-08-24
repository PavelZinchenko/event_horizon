public class ValueWrapper<T> where T: struct
{
	public static implicit operator T(ValueWrapper<T> wrapper) { return wrapper._value; } 
	
	public ValueWrapper(T value = default(T))
	{
		_value = value;
	}
	
	public T Value { get { return _value; } set { _value = value; } }
	
	public override string ToString() { return _value.ToString(); } 
	
	private T _value;
}
