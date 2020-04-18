using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using phirSOFT.SettingsService.Abstractions;

namespace phirSOFT.SettingsService.Test
{
    [TestFixture]
    public class ReadOnlySettingsStackTests
    {
        private const string Key1 = "key1";

        [Test]
        public async Task GetSettingAsync_ReturnsValue()
        {
            // Arrange
            var readOnlySettingsStack = new ReadOnlySettingsStack { CreateReadOnlySettingsService() };
            Type type = typeof(string);

            // Act
            object? result = await readOnlySettingsStack.GetSettingAsync(
                Key1,
                type);

            // Assert
            Assert.IsInstanceOf<string>(result);
            Assert.AreEqual("value1", result);
        }

        [SetUp]
        public void SetUp()
        {
        }

        private static IReadOnlySettingsService CreateReadOnlySettingsService()
        {
            IReadOnlySettingsService settingsServiceMock = Substitute.For<IReadOnlySettingsService>();

            settingsServiceMock.IsRegisteredAsync(Arg.Is(Key1)).Returns(true);
            settingsServiceMock.GetSettingAsync(Arg.Is(Key1), Arg.Is(typeof(string))).Returns("value1");
            return settingsServiceMock;
        }
    }
}
