using System;
using System.Data;

namespace Illuminate.Node.Entities
{
	public class Setting : Illuminate.Entities.Entity, Illuminate.Node.Entities.ISetting
	{
		#region Protected Area

		protected string _settingName;
		protected string _settingValue;

		#endregion

		#region Public Properties

		public string SettingName
		{
			get
			{
				return _settingName;
			}
			set
			{
				_settingName = value;
			}
		}

		public string SettingValue
		{
			get
			{
				return _settingValue;
			}
			set
			{
				_settingValue = value;
			}
		}

		#endregion

		#region Constuctors

		public Setting(string settingName, string settingValue, Illuminate.Node.Service GS)
		{
			_settingName = settingName;
			_settingValue = settingValue;
			this.GS = GS;
		}

		public Setting()
		{

		}

		#endregion

		#region Bind

		/// <summary>
		/// Binds a datatable to a collection of entities
		/// </summary>
		/// <param name="Dt">The datatable to bind</param>
		/// <param name="GS">Reference to the Illuminate Service</param>
		/// <returns>Collection of Entities</returns>
		public static Collections.Setting Bind(DataTable Dt, Illuminate.Node.Service GS)
		{
			Collections.Setting settings = new Collections.Setting();

			for (int i = 0; i < Dt.Rows.Count; i++)
			{
				DataRow Dr = Dt.Rows[i];
				Setting setting = Bind(Dr, GS);

				settings.Add(setting);
			}

			return settings;
		}

		/// <summary>
		/// Binds a datarow to an entity
		/// </summary>
		/// <param name="Dr">The datarow you want to bind</param>
		/// <param name="GS">Reference to the Illuminate Service</param>
		/// <returns>A binded entity</returns>
		public static Setting Bind(DataRow Dr, Illuminate.Node.Service GS)
		{
			string settingName = string.Empty;
			string settingValue = string.Empty;

			if (Dr.Table.Columns.Contains("SettingName")) settingName = Dr["SettingName"].ToString();
			if (Dr.Table.Columns.Contains("SettingValue")) settingValue = Dr["SettingValue"].ToString();

			return new Setting(settingName, settingValue, GS);
		}

		#endregion
	}
}
