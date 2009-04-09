using System;

namespace Illuminate.Node.Entities
{
	public interface ISetting
	{
		string SettingName { get; set; }
		string SettingValue { get; set; }
	}
}
