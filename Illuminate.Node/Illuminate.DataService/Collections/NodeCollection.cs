using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.Node.Collections
{
	public class Node : Illuminate.Collections.Collection, Illuminate.Node.Collections.INode
	{
		public void Add(Entities.INode node)
		{
			InnerList.Add(node);
		}

		public Entities.INode this[int Index]
		{
			get
			{
				OutOfRangeCheck(Index);
				return (Entities.INode)InnerList[Index];
			}
			set
			{
				OutOfRangeCheck(Index);
				InnerList[Index] = value;
			}
		}
	}
}
