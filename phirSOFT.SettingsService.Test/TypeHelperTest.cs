using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace phirSOFT.SettingsService.Test
{
    [TestFixture]
    public class TypeHelperTest
    {
        [Test()]
        [TestCase(typeof(object), default(object))]
        [TestCase(typeof(string), default(string))]
        [TestCase(typeof(byte), default(byte))]
        [TestCase(typeof(sbyte), default(sbyte))]
        [TestCase(typeof(short), default(short))]
        [TestCase(typeof(ushort), default(ushort))]
        [TestCase(typeof(int), default(int))]
        [TestCase(typeof(uint), default(uint))]
        [TestCase(typeof(long), default(long))]
        [TestCase(typeof(ulong), default(ulong))]
        [TestCase(typeof(float), default(float))]
        [TestCase(typeof(double), default(double))]
        public void TestDefaultType(Type type, object expectedDefaultValue)
        {
            object defaultValue = TypeHelper.GetDefaultValue(type);
            Assert.AreEqual(expectedDefaultValue, defaultValue);
        }
    }
}
