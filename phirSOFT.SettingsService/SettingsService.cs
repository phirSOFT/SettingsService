// <copyright file="SettingsService.cs" company="phirSOFT">
// Copyright (c) phirSOFT. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using phirSOFT.SettingsService.Abstractions;
using static phirSOFT.SettingsService.TypeHelper;

namespace phirSOFT.SettingsService
{
    /// <summary>
    ///     Provides extension methods for an <see cref="ISettingsService"/>.
    /// </summary>
    [PublicAPI]
    public static class SettingsService
    {
        /// <summary>
        ///     Wraps an <see cref="IReadOnlySettingsService"/> in an read only instance.
        /// </summary>
        /// <param name="service">The service to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/> is <see langword="null"/>.</exception>
        /// <returns>A pure read only settings service.</returns>
        [NotNull]
        public static IReadOnlySettingsService AsReadOnly([NotNull] this IReadOnlySettingsService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            return !(service is ISettingsService) ? service : new ReadOnlySettingsService(service);
        }

        /// <summary>
        ///     Gets the value of setting with a specific key.
        /// </summary>
        /// <typeparam name="T">The type of the setting to retrieve.</typeparam>
        /// <param name="service">The service to retrieve the setting from.</param>
        /// <param name="key">The key of the setting.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/> or <paramref name="key"/> is <see langword="null"/>.</exception>
        /// <returns>The value of the setting, if its present in this service.</returns>
        [ItemCanBeNull]
        public static async Task<T> GetSettingAsync<T>([NotNull] this IReadOnlySettingsService service, [NotNull]string key)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return (T) await service.GetSettingAsync(key, typeof(T));
        }

        /// <summary>
        ///     Registers a setting in this service.
        /// </summary>
        /// <typeparam name="T">The type of the setting.</typeparam>
        /// <param name="service">The service to register the setting within.</param>
        /// <param name="key">The key of the setting.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/> or <paramref name="key"/> is <see langword="null"/>.</exception>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task RegisterSettingAsync<T>([NotNull] this ISettingsService service, [NotNull] string key)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            Type type = typeof(T);
            object defaultValue = GetDefaultValue(type);

            return service.RegisterSettingAsync(key, defaultValue, defaultValue, type);
        }

        /// <summary>
        ///     Registers a setting in this service.
        /// </summary>
        /// <param name="service">The service to register the setting within.</param>
        /// <param name="key">The key of the setting.</param>
        /// <param name="type">The type of the setting.</param>
        /// <exception cref="ArgumentNullException">Thrown if at least one of <paramref name="service"/>, <paramref name="key"/> or <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task RegisterSettingAsync([NotNull] this ISettingsService service, [NotNull] string key,
            [NotNull] Type type)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            object defaultValue = GetDefaultValue(type);

            return service.RegisterSettingAsync(key, defaultValue, defaultValue, type);
        }

        /// <summary>
        ///     Registers a setting with a default value in this service .
        /// </summary>
        /// <typeparam name="T">The type of the setting.</typeparam>
        /// <param name="service">The service to register the setting within.</param>
        /// <param name="key">The key of this setting.</param>
        /// <param name="defaultValue">The value of this setting.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/> or <paramref name="key"/> is <see langword="null"/>.</exception>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task RegisterSettingAsync<T>([NotNull] this ISettingsService service, [NotNull] string key, [CanBeNull] T defaultValue)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return service.RegisterSettingAsync(key, defaultValue, defaultValue, typeof(T));
        }

        /// <summary>
        ///     Registers a setting with a default value in this service.
        /// </summary>
        /// <param name="service">The service to register the setting within.</param>
        /// <param name="key">The key of this setting.</param>
        /// <param name="defaultValue">The default value of this setting.</param>
        /// <remarks>
        ///     <para>The settings type will be determined automatically. This can lead to unexpected results.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/> or <paramref name="key"/> is <see langword="null"/>.</exception>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task RegisterSettingAsync([NotNull] this ISettingsService service, [NotNull] string key, [CanBeNull] object defaultValue)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return service.RegisterSettingAsync(key, defaultValue, defaultValue, defaultValue?.GetType() ?? typeof(object));
        }

        /// <summary>
        ///     Registers a setting with a default value in this service.
        /// </summary>
        /// <param name="service">The service to register the setting within.</param>
        /// <param name="key">The key of this setting.</param>
        /// <param name="defaultValue">The default value of this setting.</param>
        /// <param name="type">The type of the setting.</param>
        /// <exception cref="ArgumentNullException">Thrown if at least one of <paramref name="service"/>, <paramref name="key"/> or <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task RegisterSettingAsync(
            [NotNull] this ISettingsService service,
            [NotNull] string key,
            [CanBeNull] object defaultValue,
            [NotNull] Type type)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return service.RegisterSettingAsync(key, defaultValue, defaultValue, type);
        }

        /// <summary>
        ///     Registers a setting with a default and an initial value in this service.
        /// </summary>
        /// <typeparam name="T">The type of the setting.</typeparam>
        /// <param name="service">The service to register the setting within.</param>
        /// <param name="key">The key of this setting.</param>
        /// <param name="defaultValue">The default value of this setting.</param>
        /// <param name="initialValue">The initial value of this setting.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/> or <paramref name="key"/> is <see langword="null"/>.</exception>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task RegisterSettingAsync<T>(
            [NotNull] this ISettingsService service,
            [NotNull] string key,
            [CanBeNull] T defaultValue,
            [CanBeNull] T initialValue)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return service.RegisterSettingAsync(key, defaultValue, initialValue, typeof(T));
        }

        /// <summary>
        ///     Registers a setting with a default and an initial value in this service.
        /// </summary>
        /// <param name="service">The service to register the setting within.</param>
        /// <param name="key">The key of this setting.</param>
        /// <param name="defaultValue">The default value of this setting.</param>
        /// <param name="initialValue">The initial value of this setting.</param>
        /// <remarks>
        ///     <para>The settings type will be determined automatically. This can lead to unexpected results.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/> or <paramref name="key"/> is <see langword="null"/>.</exception>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task RegisterSettingAsync(
            [NotNull] this ISettingsService service,
            [NotNull] string key,
            object defaultValue,
            object initialValue)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            TypeInfo defaultType = defaultValue.GetType().GetTypeInfo();
            TypeInfo initialType = initialValue.GetType().GetTypeInfo();

            if (!AreAssignable(defaultType, initialType, out Type type) &&

                // The types are not derived from each other, so we have to find a common base type
                // We don't have to check for a common interface, because we can't deserialize an interface.
                // Maybe we can allow this for collection like interfaces, but that is a topic to cover later.
                !HaveCommonBaseType(initialType, defaultType, out type))
            {
                throw new ArgumentException($"`{nameof(defaultValue)}` ({defaultType}) and `{nameof(initialValue)}` ({initialType}) do not share a common base type");
            }

            return service.RegisterSettingAsync(key, defaultValue, initialValue, type);
        }

        /// <summary>
        ///     Sets a setting to a new value.
        /// </summary>
        /// <typeparam name="T">The type of the setting.</typeparam>
        /// <param name="service">The service to write the setting to.</param>
        /// <param name="key">The key of the setting.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/> or <paramref name="key"/> is <see langword="null"/>.</exception>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task SetSettingAsync<T>([NotNull] this ISettingsService service, [NotNull] string key, [CanBeNull] T value)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return service.SetSettingAsync(key, value, typeof(T));
        }

        /// <summary>
        ///     Sets a setting to a new value.
        /// </summary>
        /// <param name="service">The service to write the setting to.</param>
        /// <param name="key">The key of the setting.</param>
        /// <param name="value">The value of the setting.</param>
        /// <remarks>
        ///     <para>The settings type will be determined automatically. This can lead to unexpected results.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/> or <paramref name="key"/> is <see langword="null"/>.</exception>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task SetSettingAsync([NotNull] this ISettingsService service, [NotNull] string key, [CanBeNull] object value)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return service.SetSettingAsync(key, value, value?.GetType() ?? typeof(object));
        }
    }
}
