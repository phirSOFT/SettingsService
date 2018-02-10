using System.Threading.Tasks;

namespace phirSOFT.SettingsService
{
    public static class SettingsService
    {
        public static ReadOnlySettingsService AsReadOnly(this IReadOnlySettingsService service) => new ReadOnlySettingsService(service);
    }
}