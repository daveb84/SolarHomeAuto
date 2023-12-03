namespace SolarHomeAuto.Tests.AssertHelpers
{
    public static class AssetCollectionHelper
    {
        public static void AreSame<T>(ICollection<T> expected, ICollection<T> actual, bool ignoreOrder, params Func<T, object>[] propertiesToCompare)
            => Compare(expected, actual, ignoreOrder, propertiesToCompare, null, true);

        public static void AreSame<T>(ICollection<T> expected, ICollection<T> actual, bool ignoreOrder, Func<T, T, bool> comparer, params Func<T, object>[] propertiesToCompare)
            => Compare(expected, actual, ignoreOrder, propertiesToCompare, comparer, true);

        public static bool CheckAreSame<T>(ICollection<T> expected, ICollection<T> actual, bool ignoreOrder, params Func<T, object>[] propertiesToCompare)
            => Compare(expected, actual, ignoreOrder, propertiesToCompare, null, false);

        private static bool Compare<T>(ICollection<T> expected, ICollection<T> actual, bool ignoreOrder, IEnumerable<Func<T, object>> propertiesToCompare, Func<T, T, bool> customComparer, bool throwOnFail)
        {
            var compare = GetComparer(propertiesToCompare, customComparer);

            if (throwOnFail)
            {
                Assert.AreEqual(expected.Count, actual.Count);
            }
            else if (expected.Count != actual.Count)
            {
                return false;
            }

            int? failedIndex = null;

            if (ignoreOrder)
            {
                for (int i = 0; i < expected.Count; i++)
                {
                    var ex = expected.ElementAt(i);

                    var hasMatch = actual.Any(x => compare(ex, x));
                    if (!hasMatch)
                    {
                        failedIndex = i;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < expected.Count; i++)
                {
                    var ex = expected.ElementAt(i);
                    var ac = actual.ElementAt(i);

                    if (!compare(ex, ac))
                    {
                        failedIndex = i;
                        break;
                    }
                }
            }

            if (failedIndex != null)
            {
                if (throwOnFail)
                {
                    Assert.Fail($"Expected item at index {failedIndex} could not be found");
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private static Func<T, T, bool> GetComparer<T>(IEnumerable<Func<T, object>> propertiesToCompare, Func<T, T, bool> customComparer)
        {
            Func<T, T, bool> compare = (ex, ac) =>
            {
                var equal = customComparer?.Invoke(ex, ac) ?? true;

                if (equal && propertiesToCompare?.Any() == true)
                {
                    var exProps = propertiesToCompare.Select(x => x(ex)).ToList();
                    var acProps = propertiesToCompare.Select(x => x(ac)).ToList();

                    equal = ValuesAreEqual(exProps, acProps);
                }

                return equal;
            };

            return compare;
        }

        private static bool ValuesAreEqual(List<object> expected, List<object> actual)
        {
            for (int i = 0; i < expected.Count; i++)
            {
                if (!object.Equals(expected[i], actual[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
