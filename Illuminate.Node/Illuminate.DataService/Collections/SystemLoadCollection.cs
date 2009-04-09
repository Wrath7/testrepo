using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.Node.Collections
{
	public class SystemLoad : Illuminate.Collections.Collection, Illuminate.Node.Collections.ISystemLoad
	{
		public void Add(Entities.ISystemLoad systemLoad)
		{
			InnerList.Add(systemLoad);
		}

		public Entities.ISystemLoad this[int Index]
		{
			get
			{
				OutOfRangeCheck(Index);
				return (Entities.ISystemLoad)InnerList[Index];
			}
			set
			{
				OutOfRangeCheck(Index);
				InnerList[Index] = value;
			}
		}

		public Entities.ISystemLoad this[string nodeName]
		{
			get
			{
				for (int i = 0; i < InnerList.Count; i++)
				{
					Entities.ISystemLoad sysLoad = this[i];
					if (sysLoad.NodeName == nodeName)
						return sysLoad;
				}
				return null;
			}
		}
	}
}
