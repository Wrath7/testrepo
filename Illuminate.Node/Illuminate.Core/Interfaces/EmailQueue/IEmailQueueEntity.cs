using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.EmailQueue
{
    public interface IEmailQueueEntity
    {
		int MailId { get; set; }
		string To { get; set; }
		string From { get; set; }
		string Subject { get; set; }
		string Body { get; set; }
    }
}
