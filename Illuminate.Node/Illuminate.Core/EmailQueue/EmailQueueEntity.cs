using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Illuminate.EmailQueue
{
    public class EmailQueueEntity : Illuminate.EmailQueue.IEmailQueueEntity
    {
		#region Protected Members

		protected int _mailId = 0;
		protected DateTime _mailDate = new DateTime();
		protected string _to = string.Empty;
		protected string _from = string.Empty;
		protected string _subject = string.Empty;
		protected string _body = string.Empty;

		#endregion

		#region Public Properties

		public int MailId
		{
			get
			{
				return _mailId;
			}
			set
			{
				_mailId = value;
			}
		}

		public DateTime MailDate
		{
			get
			{
				return _mailDate;
			}
			set
			{
				_mailDate = value;
			}
		}

		public string To
		{
			get
			{
				return _to;
			}
			set
			{
				_to = value;
			}
		}

		public string From
		{
			get
			{
				return _from;
			}
			set
			{
				_from = value;
			}
		}

		public string Subject
		{
			get
			{
				return _subject;
			}
			set
			{
				_subject = value;
			}
		}

		public string Body
		{
			get
			{
				return _body;
			}
			set
			{
				_body = value;
			}
		}

		#endregion

		#region Constructors

		public EmailQueueEntity()
		{

		}

		public EmailQueueEntity(int mailId, DateTime mailDate, string to, string from, string subject, string body)
		{
			_mailId = mailId;
			_mailDate = MailDate;
			_to = to;
			_from = from;
			_subject = subject;
			_body = body;
		}

		#endregion

		#region Bind

		/// <summary>
		/// Binds a datarow to an entity
		/// </summary>
		/// <param name="Dr">The datarow you want to bind</param>
		/// <param name="GS">Reference to the Gazaro Service</param>
		/// <returns>A binded entity</returns>
		public static IEmailQueueEntity Bind(DataRow Dr)
		{
			int MailId = 0;
			DateTime MailDate = new DateTime();
			string To = string.Empty;
			string From = string.Empty;
			string Subject = string.Empty;
			string Body = string.Empty;

			if (Dr.Table.Columns.Contains("MailId")) MailId = int.Parse(Dr["MailId"].ToString());
			if (Dr.Table.Columns.Contains("MailDate")) DateTime.TryParse(Dr["MailDate"].ToString(), out MailDate);
			if (Dr.Table.Columns.Contains("MailTo")) To = Dr["MailTo"].ToString();
			if (Dr.Table.Columns.Contains("MailFrom")) From = Dr["MailFrom"].ToString();
			if (Dr.Table.Columns.Contains("MailSubject")) Subject = Dr["MailSubject"].ToString();
			if (Dr.Table.Columns.Contains("MailBody")) Body = Dr["MailBody"].ToString();

			EmailQueueEntity EmailQueue = new EmailQueueEntity(MailId, MailDate, To, From, Subject, Body);

			return EmailQueue;

		}

		#endregion
    }
}
