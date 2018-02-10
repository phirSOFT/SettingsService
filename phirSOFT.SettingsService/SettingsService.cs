namespace phirSOFT.SettingsService
{
    /// <summary>
    ///     Provides extension methods for an <see cref="ISettingsService" />.
    /// </summary>
    public static class SettingsService
    {
        /// <summary>
        ///     Wraps an <see cref="IReadOnlySettingsService" /> in an read only instance.
        /// </summary>
        /// <param name="service">The service to wrap.</param>
        /// <returns>A pure read only settings service.</returns>
        public static IReadOnlySettingsService AsReadOnly(this IReadOnlySettingsService service)
        {
            return !(service is ISettingsService) ? service : new ReadOnlySettingsService(service);
        }
    }
}