﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nito.AsyncEx;

namespace phirSOFT.SettingsService.Json
{
    public class JsonSettingsService : SettingsServiceBase
    {
        private readonly IDictionary<string, object> _defaultValues;
        private readonly string _filename;
        private readonly Task _initializerTask;
        private readonly AsyncReaderWriterLock _readerWriterLock = new AsyncReaderWriterLock();
        private readonly IDictionary<string, Type> _types;

        private readonly IDictionary<string, object> _values;

        private JsonSettingsService(string filename)
        {
            _filename = filename;
            _values = new ConcurrentDictionary<string, object>();
            _types = new ConcurrentDictionary<string, Type>();
            _defaultValues = new ConcurrentDictionary<string, object>();

            _initializerTask = Initialize();
        }

        private async Task Initialize()
        {
            using (await _readerWriterLock.WriterLockAsync())
            {
                var serializer = new JsonSerializer();
                using (var fs = new StreamReader(new FileStream(_filename, FileMode.Open, FileAccess.Read)))
                {
                    var reader = new JsonTextReader(fs);

                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        if (reader.TokenType != JsonToken.PropertyName) continue;

                        switch ((string) reader.Value)
                        {
                            case "types":
                                await ReadDictionary(reader, serializer, _types, key => typeof(Type));
                                break;
                            case "values":
                                await ReadDictionary(reader, serializer, _values, key => _types[key]);
                                break;
                            case "defaults":
                                await ReadDictionary(reader, serializer, _defaultValues, key => _types[key]);
                                break;
                        }
                    }
                }
            }
        }

        public static async Task<JsonSettingsService> Create(string filename)
        {
            var service = new JsonSettingsService(filename);
            await service._initializerTask.ConfigureAwait(false);
            return service;
        }

        private static async Task WriteDictionaryAsync<T>(JsonWriter writer, JsonSerializer serializer,
            IDictionary<string, T> dictionary, string key, Func<string, Type> typeResover)
        {
            await writer.WritePropertyNameAsync(key).ConfigureAwait(false);
            await writer.WriteStartObjectAsync().ConfigureAwait(false);

            foreach (var value in dictionary)
            {
                await writer.WritePropertyNameAsync(value.Key);
                serializer.Serialize(writer, value.Value, typeResover(value.Key));
            }

            writer.WriteEndObject();
        }

        private static async Task ReadDictionary<T>(JsonReader reader, JsonSerializer serializer,
            IDictionary<string, T> dictionary,
            Func<string, Type> typeResover)
        {
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (reader.TokenType == JsonToken.EndObject)
                    return;

                if (reader.TokenType != JsonToken.PropertyName)
                    continue;

                var key = reader.Value as string;

                var value = (T) serializer.Deserialize(reader, typeResover(key));
                dictionary.Add(key, value);
            }
        }

        public override async Task<object> GetSettingAsync(string key, Type type)
        {
            using (await _readerWriterLock.ReaderLockAsync().ConfigureAwait(false))
            {
                if (type != _types[key])
                    throw new ArgumentException();

                return _values[key];
            }
        }

        public override async Task SetSettingAsync(string key, object value, Type type)
        {
            using (await _readerWriterLock.ReaderLockAsync().ConfigureAwait(false))
            {
                if (!value.GetType().GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                    throw new ArgumentException();
                if (type != _types[key])
                    throw new ArgumentException();

                _values[key] = value;
            }
        }

        public override async Task RegisterSettingAsync(string key, object defaultValue, object initialValue, Type type)
        {
            using (await _readerWriterLock.ReaderLockAsync().ConfigureAwait(false))
            {
                _values.Add(key, initialValue);
                _defaultValues.Add(key, defaultValue);
                _types.Add(key, type);
            }
        }

        public override async Task UnregisterSettingAsync(string key)
        {
            using (await _readerWriterLock.ReaderLockAsync().ConfigureAwait(false))
            {
                _values.Remove(key);
                _types.Remove(key);
                _defaultValues.Remove(key);
            }
        }

        public override async Task<bool> IsRegisterdAsync(string key)
        {
            using (await _readerWriterLock.ReaderLockAsync())
            {
                return _values.ContainsKey(key);
            }
        }

        public override async Task StoreAsync()
        {
            using (await _readerWriterLock.WriterLockAsync().ConfigureAwait(false))
            {
                var serializer = new JsonSerializer();
                using (var fs = new StreamWriter(new FileStream(_filename, FileMode.Create, FileAccess.ReadWrite)))
                {
                    var writer = new JsonTextWriter(fs);
                    await writer.WriteStartObjectAsync().ConfigureAwait(false);

                    await WriteDictionaryAsync(writer, serializer, _types, "types", key => typeof(Type))
                        .ConfigureAwait(false);
                    await WriteDictionaryAsync(writer, serializer, _values, "values", key => _types[key])
                        .ConfigureAwait(false);
                    await WriteDictionaryAsync(writer, serializer, _defaultValues, "defaults", key => _types[key])
                        .ConfigureAwait(false);


                    await writer.WriteEndObjectAsync().ConfigureAwait(false);
                }
            }
        }

        public override async Task DiscardAsync()
        {
            using (await _readerWriterLock.WriterLockAsync().ConfigureAwait(false))
            {
                _values.Clear();
                _defaultValues.Clear();
                _types.Clear();
                await Initialize().ConfigureAwait(false);
            }
        }
    }
}