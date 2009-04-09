using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.EmailQueue
{
    public interface IEmailQueue
    {
		IEmailQueueEntity Pop();
		void Push(DateTime mailDate, string mailTo, string mailFrom, string mailSubject, string mailBody);
    }
}
