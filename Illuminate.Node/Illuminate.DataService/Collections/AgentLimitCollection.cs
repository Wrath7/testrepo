using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.Node.Collections
{
	public class AgentLimit : Illuminate.Collections.Collection, Illuminate.Node.Collections.IAgentLimit
	{
		public void Add(Entities.IAgentLimit agentLimit)
		{
			InnerList.Add(agentLimit);
		}

		public Entities.IAgentLimit this[int Index]
		{
			get
			{
				OutOfRangeCheck(Index);
				return (Entities.IAgentLimit)InnerList[Index];
			}
			set
			{
				OutOfRangeCheck(Index);
				InnerList[Index] = value;
			}
		}
	}
}
