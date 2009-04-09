using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Illuminate.Tools.Data.MySql;

namespace Illuminate.EmailQueue
{
	public sealed class Queue : Illuminate.EmailQueue.IEmailQueue
	{
		private static Queue _queue = new Queue();

		public static Queue Instance
		{
			get
			{
				return _queue;
			}
		}

		private string _emailConnection = null;

		public Queue()
		{
			_emailConnection = System.Configuration.ConfigurationManager.ConnectionStrings["EmailQueueConnection"].ToString();
		}

		public void Push(DateTime mailDate, string mailTo, string mailFrom, string mailSubject, string mailBody)
		{
			Query q = new Query("insert into EmailQueue (MailDate, MailTo, MailFrom, MailSubject, MailBody) values (?MailDate, ?MailTo, ?MailFrom, ?MailSubject, ?MailBody)", "PushIntoEmailQueue", _emailConnection);
			q.Parameters.Add("?MailDate", mailDate, ParameterCollection.FieldType.DateTime);
			q.Parameters.Add("?MailTo", mailTo, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?MailFrom", mailFrom, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?MailSubject", mailSubject, ParameterCollection.FieldType.Text);
			q.Parameters.Add("?MailBody", mailBody, ParameterCollection.FieldType.Text);
			q.RunQueryNoResult();
		}

		public IEmailQueueEntity Pop()
		{
			Query q = new Query("select * from EmailQueue order by MailDate asc limit 1", "PopFromEmailQueue", _emailConnection);
			DataTable Dt = q.RunQuery();

			if (Dt.Rows.Count == 0)
			{
				return null;
			}

			IEmailQueueEntity queueItem = EmailQueueEntity.Bind(Dt.Rows[0]);

			DeleteItem(queueItem);

			return queueItem;
		}

		private void DeleteItem(IEmailQueueEntity queueItem)
		{
			Query q = new Query("delete from EmailQueue where MailId = ?MailId", "DeleteFromEmailQueue", _emailConnection);
			q.Parameters.Add("?MailId", queueItem.MailId, ParameterCollection.FieldType.Numeric);
			q.RunQueryNoResult();
		}
	}
}
