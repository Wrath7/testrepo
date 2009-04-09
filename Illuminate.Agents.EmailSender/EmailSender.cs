using System;
using System.Collections.Generic;
using System.Text;
using Illuminate.Tools;
using Illuminate.EmailQueue;

namespace Illuminate.Agents.EmailSender
{
	public class EmailSender : Illuminate.Core.Node.AgentStandard, Illuminate.Interfaces.IAgent
	{
		#region IAgent Members

		public new void InitializeAgent(Illuminate.Contexts.AgentContext agentContext)
		{
			//initialize email service or publisher
			base.InitializeAgent(agentContext);

			_context.Interval = 1000;
		}

		public new void Run()
		{
			base.Run();

			//check for any instant push emails
			IEmailQueueEntity email = Illuminate.EmailQueue.Queue.Instance.Pop();

			if (email != null)
			{
				Logger.WriteLine("Sending an email to: " + email.To + " -- Subject: " + email.Subject, Logger.Severity.Information, LOGNAME);

				//Send Email
				Illuminate.Tools.EmailService es = new Illuminate.Tools.EmailService();
				es.OnError += new EmailService.EmailServiceErrorDelegate(es_OnError);
				es.SendEmail(email.From, email.From, email.To, email.Subject, email.Body);

			}
		}

		void es_OnError(string ErrorMessage, string StackTrace)
		{
			Logger.WriteLine("There was an error sending mail: " + ErrorMessage + " -- " + StackTrace, Logger.Severity.Error, LOGNAME);
		}

		#endregion
	}
}
