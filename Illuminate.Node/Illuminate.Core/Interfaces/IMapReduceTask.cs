using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.Interfaces
{
	public interface IMapReduceTask
	{
		Illuminate.MapReduce.InputData Task(Illuminate.MapReduce.InputData id);
	}
}
