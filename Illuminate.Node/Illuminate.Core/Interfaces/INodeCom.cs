using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.Interfaces
{
	public interface INodeCom : IIlluminateObject
	{
		event Illuminate.Communication.Communicator.OnDataOutDelegate OnDataOut;
		event Illuminate.Communication.Communicator.OnDataInDelegate OnDataIn;

		void RaiseDataIn(Illuminate.Communication.Command Command);
		void RaiseDataOut(Illuminate.Communication.Command Command);

	}
}
