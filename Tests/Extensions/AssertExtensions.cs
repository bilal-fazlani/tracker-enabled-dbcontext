using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Extensions
{
    public static class AssertExtensions
    {
        public static void AssertIsNotZero(this int value)
        {
            Assert.AreNotEqual(0, value, "given value is zero");
        }
        public static void AssertIsNotZero(this long value)
        {
            Assert.AreNotEqual(0, value, "given value is zero");
        }
        public static void AssertIsNotZero(this double value)
        {
            Assert.AreNotEqual(0, value, "given value is zero");
        }
    }
}
