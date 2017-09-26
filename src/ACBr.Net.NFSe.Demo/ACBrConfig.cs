using ACBr.Net.Core.Extensions;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;

namespace ACBr.Net
{
    public class ACBrConfig
    {
        #region Fields

        private readonly Configuration config;

        #endregion Fields

        #region Constructors

        private ACBrConfig(Configuration config)
        {
            this.config = config;
        }

        #endregion Constructors

        #region Methods

        public void Set(string setting, object value)
        {
            var valor = string.Format(CultureInfo.InvariantCulture, "{0}", value);

            if (config.AppSettings.Settings[setting]?.Value != null)
                config.AppSettings.Settings[setting].Value = valor;
            else
                config.AppSettings.Settings.Add(setting, valor);
        }

        public T Get<T>(string setting, T defaultValue)
        {
            var type = typeof(T);
            var value = config.AppSettings.Settings[setting]?.Value;
            if (value.IsEmpty()) return defaultValue;

            try
            {
                if (type.IsEnum || type.IsGenericType && type.GetGenericArguments()[0].IsEnum)
                {
                    return (T)Enum.Parse(type, value);
                }

                return (T)Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public void Save()
        {
            config.Save(ConfigurationSaveMode.Minimal, true);
        }

        public static ACBrConfig CreateOrLoad(string fileName = "acbr.config")
        {
            if (!File.Exists(fileName))
            {
                var config = "<?xml version='1.0' encoding='utf-8' ?>" + Environment.NewLine +
                             "<configuration>" + Environment.NewLine +
                             "    <appSettings>" + Environment.NewLine +
                             "    </appSettings>" + Environment.NewLine +
                             "</configuration>";
                File.WriteAllText(fileName, config);
            }

            var configFileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = fileName
            };

            var configuration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            return new ACBrConfig(configuration);
        }

        #endregion Methods
    }
}