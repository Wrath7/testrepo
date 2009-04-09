using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Illuminate.Node
{
	public class Configuration
	{
		#region Members

		private static bool _configurationRead = false;
		private static Dictionary<string, Dictionary<string, string>> _settings = new Dictionary<string, Dictionary<string, string>>();
		private static List<string> _agents = new List<string>();
		private static List<string> _mapReduce = new List<string>();

		#endregion

		#region Properties

		public static bool ConfigurationRead
		{
			get
			{
				return _configurationRead;
			}
		}

		public static List<string> Agents
		{
			get
			{
				return _agents;
			}
		}

		public static List<string> MapServers
		{
			get
			{
				return _mapReduce;
			}
		}

		#endregion

		#region Read Configuration

		public static void ReadConfiguration()
		{
			try
			{
				bool inSettings = false;
				bool inMapReduce = false;
				string currentSection = string.Empty;
				string sectionType = string.Empty;

				XmlTextReader rdr = new XmlTextReader("Illuminate.conf");

				while (rdr.Read())
				{
					string name = rdr.Name.ToLower();
					XmlNodeType nodeType = rdr.NodeType;

					if (name == "settings" && nodeType == XmlNodeType.Element)
					{
						inSettings = true;
					}
					else if (name == "settings" && nodeType == XmlNodeType.EndElement)
					{
						currentSection = string.Empty;
						inSettings = false;
					}
					else if (name == "section" && nodeType == XmlNodeType.Element)
					{
						currentSection = rdr.GetAttribute("name").ToLower();
						_settings.Add(currentSection, new Dictionary<string, string>());

						if (rdr.AttributeCount > 1)
						{
							sectionType = rdr.GetAttribute("type").ToLower();

							if (sectionType == "agent")
							{
                                if (_agents.Contains(currentSection))
                                {
                                    throw new ArgumentException("Duplicate AgentId in Illuminate.conf");
                                }
								_agents.Add(currentSection);
							}
						}
					}
					else if (nodeType == XmlNodeType.Element && currentSection != string.Empty && inSettings)
					{
						rdr.Read();

						string value = rdr.Value.Trim();
						_settings[currentSection].Add(name.ToLower(), value);
					}
					else if (name == "mapreduce" && nodeType == XmlNodeType.Element)
					{
						inMapReduce = true;
					}
					else if (name == "mapreduce" && nodeType == XmlNodeType.EndElement)
					{
						inMapReduce = false;
					}
					else if (name == "server" && nodeType == XmlNodeType.Element && inMapReduce)
					{
						rdr.Read();
						_mapReduce.Add(rdr.Value.Trim());
					}
				}

				rdr.Close();
			}
			catch (System.IO.FileNotFoundException e)
			{
				throw new FileNotFoundException("Configuration file is missing.  A Configuraion.xml must be created.  See the Illuminate Documentation", e);
			}
			catch (System.InvalidOperationException e)
			{
				throw new FileNotFoundException("There was an error reading the Configuration.xml file.", e);
			}
			catch (Exception e)
			{
				throw new FileNotFoundException("There was an unknown problem reading the configuration file.", e);
			}

			_configurationRead = true;

			//#region Initiate Map Reduce Clients

			//Illuminate.MapReduce.Channels.InitiateClientChannel();

			//if (_mapReduce.Count > 0)
			//{
			//    for (int i = 0; i < _mapReduce.Count; i++)
			//    {
			//        string[] mapServer = _mapReduce[i].Split(':');
			//        Illuminate.MapReduce.Client svr = new Illuminate.MapReduce.Client(i, mapServer[0], int.Parse(mapServer[1]));
			//        Illuminate.MapReduce.Client.Clients.Add(svr);
			//    }
			//}

			//#endregion

		}

		#endregion

		#region Getters

		public static bool SettingExists(string section, string name)
		{
			if (!_settings.ContainsKey(section))
			{
				return false;
			}

			return true;
		}

		public static int GetSettingsInt(string section, string name)
		{
			string value = GetSettings(section, name);

			int returnValue = 0;
			bool validate = int.TryParse(value, out returnValue);

			if (!validate)
				throw new FormatException(value + " is not an integer");

			return returnValue;
		}

		public static long GetSettingsLong(string section, string name)
		{
			string value = GetSettings(section, name);

			long returnValue = 0;
			bool validate = long.TryParse(value, out returnValue);

			if (!validate)
				throw new FormatException(value + " is not a log");

			return returnValue;
		}

		public static double GetSettingsDouble(string section, string name)
		{
			string value = GetSettings(section, name);

			double returnValue = 0;
			bool validate = double.TryParse(value, out returnValue);

			if (!validate)
				throw new FormatException(value + " is not a double");

			return returnValue;
		}

		public static bool GetSettingsBool(string section, string name)
		{
			string value = GetSettings(section, name);

			bool returnValue;
			bool validate = bool.TryParse(value, out returnValue);

			if (!validate)
				throw new FormatException(value + " is not a boolean");

			return returnValue;
		}

		public static DateTime GetSettingsDateTime(string section, string name)
		{
			string value = GetSettings(section, name);

			DateTime returnValue;
			bool validate = DateTime.TryParse(value, out returnValue);

			if (!validate)
				throw new FormatException(value + " is not a Date/Time");

			return returnValue;
		}

		public static string GetSettings(string section, string name)
		{
			section = section.ToLower();
			name = name.ToLower();

			if (!_settings.ContainsKey(section))
				throw new System.ArgumentException(section + " does not exists in Node Configuration");

			if (!_settings[section].ContainsKey(name))
				throw new System.ArgumentException(name + " does not exists in " + name + " configuration section");

			return _settings[section][name];
		}

		#endregion
	}
}
