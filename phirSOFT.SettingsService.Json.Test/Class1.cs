using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace phirSOFT.SettingsService.Json.Test
{
    [TestFixture]
    public class JsonSettingsServiceTest
    {
        [Test]
        public async Task TestSerialisation()
        {
            var file = Path.GetTempFileName();
            var service = await JsonSettingsService.Create(file);

            await service.RegisterSettingAsync("integer", 0);
            await service.RegisterSettingAsync("string", "string");
            await service.RegisterSettingAsync<object>("object");

            await service.StoreAsync();

            var service2 = await JsonSettingsService.Create(file);
            Assert.AreEqual(0, await service2.GetSettingAsync<int>("integer"));
            Assert.AreEqual("string", await service2.GetSettingAsync<string>("string"));
            Assert.True(await service2.IsRegisterdAsync("object"));


        }
    }
}
