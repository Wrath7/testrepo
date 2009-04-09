using System;
namespace Illuminate.MapReduce
{
	public interface IInputData
	{
		bool Done { get; set; }
		object IntermediateResult { get; set; }
		object Key { get; }
		object Value { get; }
		int ErrorCode { get; set; }
		string ErrorMessage { get; set; }
	}
}
