using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace phirSOFT.SettingsService.Test
{
    [TestFixture]
    public class TypeHelperTest
    {
        class Root { }
        class A : Root { }
        class B : Root { }
        
        class A1 : A { }

        class A2 : A { }

        class B1 : B { }
        class B2 : B { }

        class Other { }

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

        [Test]
        [TestCase(typeof(Root), typeof(Root), typeof(Root))]
        [TestCase(typeof(Root), typeof(A), typeof(Root))]
        [TestCase(typeof(Root), typeof(B), typeof(Root))]
        [TestCase(typeof(Root), typeof(A1), typeof(Root))]
        [TestCase(typeof(Root), typeof(A2), typeof(Root))]
        [TestCase(typeof(Root), typeof(B1), typeof(Root))]
        [TestCase(typeof(Root), typeof(B2), typeof(Root))]
        [TestCase(typeof(Root), typeof(Other), null)]
        [TestCase(typeof(A), typeof(A), typeof(A))]
        [TestCase(typeof(A), typeof(B), typeof(Root))]
        [TestCase(typeof(A), typeof(A1), typeof(A))]
        [TestCase(typeof(A), typeof(A2), typeof(A))]
        [TestCase(typeof(A), typeof(B1), typeof(Root))]
        [TestCase(typeof(A), typeof(B2), typeof(Root))]
        [TestCase(typeof(A), typeof(Other), null)]
        [TestCase(typeof(B), typeof(B), typeof(B))]
        [TestCase(typeof(B), typeof(A1), typeof(Root))]
        [TestCase(typeof(B), typeof(A2), typeof(Root))]
        [TestCase(typeof(B), typeof(B1), typeof(B))]
        [TestCase(typeof(B), typeof(B2), typeof(B))]
        [TestCase(typeof(B), typeof(Other), null)]
        [TestCase(typeof(A1), typeof(A1), typeof(A1))]
        [TestCase(typeof(A1), typeof(A2), typeof(A))]
        [TestCase(typeof(A1), typeof(B1), typeof(Root))]
        [TestCase(typeof(A1), typeof(B2), typeof(Root))]
        [TestCase(typeof(A1), typeof(Other), null)]
        [TestCase(typeof(A2), typeof(A2), typeof(A2))]
        [TestCase(typeof(A2), typeof(B1), typeof(Root))]
        [TestCase(typeof(A2), typeof(B2), typeof(Root))]
        [TestCase(typeof(A1), typeof(Other), null)]
        [TestCase(typeof(B1), typeof(B1), typeof(B1))]
        [TestCase(typeof(B1), typeof(B2), typeof(B))]
        [TestCase(typeof(B1), typeof(Other), null)]
        [TestCase(typeof(B2), typeof(B2), typeof(B2))]
        [TestCase(typeof(B2), typeof(Other), null)]
        [TestCase(typeof(Other), typeof(Other), typeof(Other))]
        public void TestHaveCommonBaseType(Type a, Type b,  Type expectedCommon)
        {
            if (expectedCommon == null)
            {
                Assert.IsFalse(TypeHelper.HaveCommonBaseType(a.GetTypeInfo(), b.GetTypeInfo(), out _));
            }
            else
            {
                Assert.IsTrue(TypeHelper.HaveCommonBaseType(a.GetTypeInfo(), b.GetTypeInfo(), out Type common));
                Assert.AreEqual(expectedCommon, common);
            }
        }

        [Test]
        [TestCase(typeof(Root), typeof(Root))]
        [TestCase(typeof(Root), typeof(A))]
        [TestCase(typeof(Root), typeof(B))]
        [TestCase(typeof(Root), typeof(A1))]
        [TestCase(typeof(Root), typeof(A2))]
        [TestCase(typeof(Root), typeof(B1))]
        [TestCase(typeof(Root), typeof(B2))]
        [TestCase(typeof(Root), typeof(Other))]
        [TestCase(typeof(A), typeof(A))]
        [TestCase(typeof(A), typeof(B))]
        [TestCase(typeof(A), typeof(A1))]
        [TestCase(typeof(A), typeof(A2))]
        [TestCase(typeof(A), typeof(B1))]
        [TestCase(typeof(A), typeof(B2))]
        [TestCase(typeof(A), typeof(Other))]
        [TestCase(typeof(B), typeof(B))]
        [TestCase(typeof(B), typeof(A1))]
        [TestCase(typeof(B), typeof(A2))]
        [TestCase(typeof(B), typeof(B1))]
        [TestCase(typeof(B), typeof(B2))]
        [TestCase(typeof(B), typeof(Other))]
        [TestCase(typeof(A1), typeof(A1))]
        [TestCase(typeof(A1), typeof(A2))]
        [TestCase(typeof(A1), typeof(B1))]
        [TestCase(typeof(A1), typeof(B2))]
        [TestCase(typeof(A1), typeof(Other))]
        [TestCase(typeof(A2), typeof(A2))]
        [TestCase(typeof(A2), typeof(B1))]
        [TestCase(typeof(A2), typeof(B2))]
        [TestCase(typeof(A1), typeof(Other))]
        [TestCase(typeof(B1), typeof(B1))]
        [TestCase(typeof(B1), typeof(B2))]
        [TestCase(typeof(B1), typeof(Other))]
        [TestCase(typeof(B2), typeof(B2))]
        [TestCase(typeof(B2), typeof(Other))]
        [TestCase(typeof(Other), typeof(Other))]
        public void TestHaveCommonBaseTypeSymmetrical(Type a, Type b)
        {
            Assert.AreEqual(
                TypeHelper.HaveCommonBaseType(a.GetTypeInfo(), b.GetTypeInfo(), out Type abCommon),
                TypeHelper.HaveCommonBaseType(b.GetTypeInfo(), a.GetTypeInfo(), out Type baCommon)
                );
            Assert.AreEqual(abCommon, baCommon);
        }

        [Test]
        [TestCase(typeof(Root), typeof(Root), typeof(Root))]
        [TestCase(typeof(Root), typeof(A), typeof(Root))]
        [TestCase(typeof(Root), typeof(B), typeof(Root))]
        [TestCase(typeof(Root), typeof(A1), typeof(Root))]
        [TestCase(typeof(Root), typeof(A2), typeof(Root))]
        [TestCase(typeof(Root), typeof(B1), typeof(Root))]
        [TestCase(typeof(Root), typeof(B2), typeof(Root))]
        [TestCase(typeof(Root), typeof(Other), null)]
        [TestCase(typeof(A), typeof(A), typeof(A))]
        [TestCase(typeof(A), typeof(B), null)]
        [TestCase(typeof(A), typeof(A1), typeof(A))]
        [TestCase(typeof(A), typeof(A2), typeof(A))]
        [TestCase(typeof(A), typeof(B1), null)]
        [TestCase(typeof(A), typeof(B2), null)]
        [TestCase(typeof(A), typeof(Other), null)]
        [TestCase(typeof(B), typeof(B), typeof(B))]
        [TestCase(typeof(B), typeof(A1), null)]
        [TestCase(typeof(B), typeof(A2), null)]
        [TestCase(typeof(B), typeof(B1), typeof(B))]
        [TestCase(typeof(B), typeof(B2), typeof(B))]
        [TestCase(typeof(B), typeof(Other), null)]
        [TestCase(typeof(A1), typeof(A1), typeof(A1))]
        [TestCase(typeof(A1), typeof(A2), null)]
        [TestCase(typeof(A1), typeof(B1), null)]
        [TestCase(typeof(A1), typeof(B2), null)]
        [TestCase(typeof(A1), typeof(Other), null)]
        [TestCase(typeof(A2), typeof(A2), typeof(A2))]
        [TestCase(typeof(A2), typeof(B1), null)]
        [TestCase(typeof(A2), typeof(B2), null)]
        [TestCase(typeof(A1), typeof(Other), null)]
        [TestCase(typeof(B1), typeof(B1), typeof(B1))]
        [TestCase(typeof(B1), typeof(B2), null)]
        [TestCase(typeof(B1), typeof(Other), null)]
        [TestCase(typeof(B2), typeof(B2), typeof(B2))]
        [TestCase(typeof(B2), typeof(Other), null)]
        [TestCase(typeof(Other), typeof(Other), typeof(Other))]
        public void TestAreAssignable(Type a, Type b, Type expectedAssignment)
        {
            if (expectedAssignment == null)
            {
                Assert.IsFalse(TypeHelper.AreAssignable(a.GetTypeInfo(), b.GetTypeInfo(), out _));
            }
            else
            {
                Assert.IsTrue(TypeHelper.AreAssignable(a.GetTypeInfo(), b.GetTypeInfo(), out Type common));
                Assert.AreEqual(expectedAssignment, common);
            }
        }

        [Test]
        [TestCase(typeof(Root), typeof(Root))]
        [TestCase(typeof(Root), typeof(A))]
        [TestCase(typeof(Root), typeof(B))]
        [TestCase(typeof(Root), typeof(A1))]
        [TestCase(typeof(Root), typeof(A2))]
        [TestCase(typeof(Root), typeof(B1))]
        [TestCase(typeof(Root), typeof(B2))]
        [TestCase(typeof(Root), typeof(Other))]
        [TestCase(typeof(A), typeof(A))]
        [TestCase(typeof(A), typeof(B))]
        [TestCase(typeof(A), typeof(A1))]
        [TestCase(typeof(A), typeof(A2))]
        [TestCase(typeof(A), typeof(B1))]
        [TestCase(typeof(A), typeof(B2))]
        [TestCase(typeof(A), typeof(Other))]
        [TestCase(typeof(B), typeof(B))]
        [TestCase(typeof(B), typeof(A1))]
        [TestCase(typeof(B), typeof(A2))]
        [TestCase(typeof(B), typeof(B1))]
        [TestCase(typeof(B), typeof(B2))]
        [TestCase(typeof(B), typeof(Other))]
        [TestCase(typeof(A1), typeof(A1))]
        [TestCase(typeof(A1), typeof(A2))]
        [TestCase(typeof(A1), typeof(B1))]
        [TestCase(typeof(A1), typeof(B2))]
        [TestCase(typeof(A1), typeof(Other))]
        [TestCase(typeof(A2), typeof(A2))]
        [TestCase(typeof(A2), typeof(B1))]
        [TestCase(typeof(A2), typeof(B2))]
        [TestCase(typeof(A1), typeof(Other))]
        [TestCase(typeof(B1), typeof(B1))]
        [TestCase(typeof(B1), typeof(B2))]
        [TestCase(typeof(B1), typeof(Other))]
        [TestCase(typeof(B2), typeof(B2))]
        [TestCase(typeof(B2), typeof(Other))]
        [TestCase(typeof(Other), typeof(Other))]
        public void TestAreAssignableSymmetrical(Type a, Type b)
        {
            Assert.AreEqual(
                TypeHelper.AreAssignable(a.GetTypeInfo(), b.GetTypeInfo(), out Type abCommon),
                TypeHelper.AreAssignable(b.GetTypeInfo(), a.GetTypeInfo(), out Type baCommon)
                );
            Assert.AreEqual(abCommon, baCommon);
        }
    }
}
