using ElemarJR.FunctionalCSharp;

namespace GFT.DeveloperPracticum.Kernel.Helpers
{
    public static class HashCodeHelper
    {
        public const uint HASHING_BASE = 2166136261;
        public const int HASHING_MULTIPLIER = 16777619;
        public const int EMPTY_HASHING = 0;

        public static int CustomHashCode(Option<object[]> properties) =>
            properties.Match(
                some: p =>
                {
                    unchecked
                    {
                        int hash = (int)HASHING_BASE;

                        foreach (var clientObject in p)
                        {
                            hash = (hash * HASHING_MULTIPLIER)
                                ^ (!ReferenceEquals(null, clientObject)
                                    ? clientObject.GetHashCode()
                                    : 0);
                        }

                        return hash;
                    }
                },
                none: () => EMPTY_HASHING
        );
    }
}
