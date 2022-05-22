namespace Pantrymony.back.Extensions;

internal static class NullCheckExtensions
{
    internal static T ThrowIfNull<T>(this T? target, Exception ex) where T: class
    {
        if (target == null)
        {
            throw ex;
        }

        return target;
    }

    internal static T ThrowIf<T>(this T target, Predicate<T> isTrue, Exception ex)
    {
        if (isTrue(target))
            throw ex;
        return target;
    }
}