using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;
using Unity.Registration;

namespace phirSOFT.SettingsService.Unity.Test
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public void TestMethod()
        {
            var container = new UnityContainer();
            container.AddNewExtension<SettingsServiceContainerExtension>();

            container.RegisterType<IReadOnlySettingsService, ISettingsService>();
            container.RegisterType<ISettingsService, CallResponseService>();

            var instance = container.Resolve<TestSampleClass>();

            Assert.AreEqual("test", instance.Test);
            Assert.AreEqual("Test1", instance.Test1);
        }
    }

    internal class TestSampleClass
    {
        [global::Unity.Attributes.Dependency]
        [SettingValue("Test1")]
        public string Test1 { get; set; }

        public string Test { get; }

        public TestSampleClass([SettingValue("test")]string test)
        {
            Test = test;
        }
    }
}
