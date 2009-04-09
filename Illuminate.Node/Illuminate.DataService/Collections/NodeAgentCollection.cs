using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.Node.Collections
{
	public class Agent : Illuminate.Collections.Collection, Illuminate.Node.Collections.IAgent
	{
		public void Add(Entities.IAgent agent)
		{
			InnerList.Add(agent);
		}

		public void Remove(Entities.IAgent agent)
		{
			InnerList.Remove(agent);
		}

		public Entities.IAgent this[int Index]
		{
			get
			{
				OutOfRangeCheck(Index);
				return (Entities.IAgent)InnerList[Index];
			}
			set
			{
				OutOfRangeCheck(Index);
				InnerList[Index] = value;
			}
		}

		public bool Contains(string agentType)
		{
			for (int i = 0; i < InnerList.Count; i++)
			{
				Entities.IAgent agent = (Entities.IAgent)InnerList[i];
				if (agent.AgentName == agentType)
					return true;
			}
			return false;
		}

	}
}