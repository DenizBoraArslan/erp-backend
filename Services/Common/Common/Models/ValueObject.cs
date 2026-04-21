namespace Common.Models;

public abstract class ValueObject
{
    protected static bool EqualOperator(ValueObject? left, ValueObject? right)
    {
        if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            return false;
        return ReferenceEquals(left, right) || left!.Equals(right);
    }

    protected static bool NotEqualOperator(ValueObject? left, ValueObject? right)
    {
        return !EqualOperator(left, right);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;
        return true;
    }

    public override int GetHashCode()
    {
        return GetAtomicValues()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    protected abstract IEnumerable<object> GetAtomicValues();
}

public static class ValueObjectExtensions
{
    public static T Map<T>(this IEnumerable<T> enumerable, Func<T, T> map)
    {
        return enumerable.Select(map).Aggregate((a, b) => b);
    }
}
