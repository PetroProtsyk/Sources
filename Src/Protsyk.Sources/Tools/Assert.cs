using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protsyk.Common.UnitTests
{
    public static class Assert
    {
        public static void AreEqual<T>(T expected, T actual)
        {
            AreEqual(expected, actual, EqualityComparer<T>.Default);
        }

        public static void AreEqual<T>(T expected, T actual, IEqualityComparer<T> comparer)
        {
            if (!comparer.Equals(expected, actual))
                throw new Exception($"Values are not equal {expected} != {actual}");
        }

        public static void IsTrue(bool value)
        {
            AreEqual(true, value);
        }

        public static void IsFalse(bool value)
        {
            AreEqual(false, value);
        }

        public static void AreEqualSequences<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            AreEqualSequences(expected, actual, EqualityComparer<T>.Default);
        }

        public static void AreEqualSequences<T>(IEnumerable<T> expected, IEnumerable<T> actual, IEqualityComparer<T> comparer)
        {
            using (var expectedIterator = expected.GetEnumerator())
            {
                using (var actualIterator = actual.GetEnumerator())
                {
                    while (expectedIterator.MoveNext())
                    {
                        IsTrue(actualIterator.MoveNext());

                        AreEqual(expectedIterator.Current, actualIterator.Current, comparer);
                    }
                    IsFalse(actualIterator.MoveNext());
                }
            }
        }
    }
}
