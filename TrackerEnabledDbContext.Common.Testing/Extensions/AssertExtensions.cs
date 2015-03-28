using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TrackerEnabledDbContext.Common.Testing.Extensions
{
    public static class AssertExtensions
    {
        public static bool AssertTrue(this bool value, string errorMessage = null)
        {
            Assert.IsTrue(value, errorMessage);
            return value;
        }

        public static int AssertIsNotZero(this int value, string errorMessage = null)
        {
            Assert.AreNotEqual(0, value, errorMessage ?? "given value is zero");
            return value;
        }

        public static long AssertIsNotZero(this long value, string errorMessage = null)
        {
            Assert.AreNotEqual(0, value, errorMessage ?? "given value is zero");
            return value;
        }

        public static double AssertIsNotZero(this double value, string errorMessage = null)
        {
            Assert.AreNotEqual(0, value, errorMessage ?? "given value is zero");
            return value;
        }

        public static IEnumerable<T> AssertCountIsNotZero<T>(this IEnumerable<T> collection, string errorMessage = null)
        {
            if (!collection.Any()) Assert.Fail(errorMessage ?? "collection has zero records");
            return collection;
        }

        public static IEnumerable<T> AssertAny<T>(this IEnumerable<T> collection, Func<T, bool> predicate,
            string errorMessage = null)
        {
            if (!collection.Any(predicate)) Assert.Fail(errorMessage);
            return collection;
        }

        public static IEnumerable<T> AssertCount<T>(this IEnumerable<T> collection, int expectedCount,
            string errorMessage = null)
        {
            Assert.AreEqual(expectedCount, collection.Count(), errorMessage);
            return collection;
        }

        public static T AssertIsNotNull<T>(this T value, string errorMessage = null)
        {
            Assert.IsNotNull(value, errorMessage);
            return value;
        }
    }
}