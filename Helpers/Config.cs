using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows;
using System.Windows.Forms;

namespace Helpers
{
    public static class Config
    {
        private static readonly object _lock = new object();

        public static bool ReadBoolAppSettings(string field, bool Default)
        {
            return Boolean.Parse(LeAppSettings(field, Default.ToString()));
        }

        public static void WriteBoolAppSettings(string field, bool Valor)
        {
            GravaAppSettings(field, Valor.ToString());
        }

        public static string ReadSettings()
        {
            lock (_lock)
            {
                StringBuilder ret = new StringBuilder();
                Configuration config = null;
                try
                {
                    config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                }
                catch
                {
                    config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming);
                }

                foreach (var item in config.AppSettings.Settings.AllKeys)
                {
                    ret.AppendLine(item.ToString() + " = " + config.AppSettings.Settings[item].Value.Trim());
                }
                return ret.ToString();
            }
        }

        /// <summary>
        /// Read an item from current AppSettings
        /// </summary>
        /// <param name="field">Field to read</param>
        /// <param name="defaultValue">Default Value, if exists</param>
        /// <returns>Field Value</returns>
        public static string LeAppSettings(string field, string defaultValue)
        {
            string valor = defaultValue.Trim();
            lock (_lock)
            {
                Configuration config = null;
                try
                {
                    config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                }
                catch
                {
                    config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming);
                }
                if (config.AppSettings.Settings.AllKeys.Contains(field))
                    valor = config.AppSettings.Settings[field].Value.Trim();
                else
                {
                    config.AppSettings.Settings.Add(field, defaultValue);
                    config.Save();
                }
                if (string.IsNullOrEmpty(valor) && !string.IsNullOrEmpty(defaultValue))
                {
                    valor = defaultValue.Trim();
                    config.AppSettings.Settings[field].Value = valor;
                    config.Save();
                }
            }
            return valor;
        }

        /// <summary>
        /// Write data from current AppSettings
        /// </summary>
        /// <param name="field">Field to write </param>
        /// <param name="value">Field value</param>
        public static void GravaAppSettings(string field, string value)
        {
            lock (_lock)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                if (config.AppSettings.Settings.AllKeys.Contains(field))
                    config.AppSettings.Settings[field].Value = value.Trim();
                else
                    config.AppSettings.Settings.Add(field, value);
                config.Save();
                try
                {
                    ConfigurationManager.RefreshSection("AppSettings");
                }
                catch { }
            }
        }

        public static ConnectionStringSettings ReadConnSetting(string Name)
        {
            return ReadConnSetting(Name, null);
        }

        public static ConnectionStringSettings ReadConnSetting(string Name, string Conn, string Provider)
        {
            return ReadConnSetting(Name, new ConnectionStringSettings(Name, Conn, Provider));
        }


        public static ConnectionStringSettings ReadConnSetting(string Name, ConnectionStringSettings Conn)
        {
            ConnectionStringSettings Ret = null;

            lock (_lock)
            {
                try
                {
                    Ret = ConfigurationManager.ConnectionStrings[Name];
                }
                catch { }

                if (Ret == null && Conn != null)
                {
                    try
                    {
                        ConfigurationManager.ConnectionStrings.Add(Conn);
                    }
                    catch { }
                    Ret = Conn;
                }
            }


            return Ret;
        }

        public static void WriteConnSetting(ConnectionStringSettings Conn)
        {
            ConnectionStringSettings Ret = null;

            lock (_lock)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                try
                {
                    Ret = config.ConnectionStrings.ConnectionStrings[Conn.Name];
                }
                catch { }

                if (Ret == null)
                {
                    config.ConnectionStrings.ConnectionStrings.Add(Conn);
                }
                else
                {
                    Ret.ProviderName = Conn.ProviderName;
                    Ret.ConnectionString = Conn.ConnectionString;
                }

                config.Save();
                try
                {
                    ConfigurationManager.RefreshSection("connectionStrings");
                }
                catch { }
            }
        }
    }
}
