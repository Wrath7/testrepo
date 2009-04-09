using System;
namespace Illuminate.MapReduce
{
	public interface IJob
	{
		object Execute(System.Collections.Generic.Dictionary<string, object> input);
		System.Collections.Generic.List<string> FunctionalKeys { get; }
		int TimeoutInterval { get; set; }
	}
}
