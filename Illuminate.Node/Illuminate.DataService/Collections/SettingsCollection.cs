using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.Node.Collections
{
	public class Setting : Illuminate.Collections.Collection, Illuminate.Node.Collections.ISetting
	{
		public void Add(Entities.ISetting setting)
		{
			InnerList.Add(setting);
		}

		public Entities.ISetting this[int Index]
		{
			get
			{
				OutOfRangeCheck(Index);
				return (Entities.ISetting)InnerList[Index];
			}
			set
			{
				OutOfRangeCheck(Index);
				InnerList[Index] = value;
			}
		}
	}
}
