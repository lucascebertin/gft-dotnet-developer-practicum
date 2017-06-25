namespace GFT.DeveloperPracticum.Kernel.Helpers
{
    public static class ObjectComparisonHelper
    {
        public static bool Same<T>(T left, T right) where T : class =>
            (ReferenceEquals(left, right))
            || (!(ReferenceEquals(null, left))
                && (left.Equals(right)));

        public static bool NotSame<T>(T left, T right) where T : class =>
            !Same(left, right);
    }
}
