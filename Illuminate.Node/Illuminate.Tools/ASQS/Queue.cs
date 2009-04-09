using System;
using Amazon.SQS;

namespace Illuminate.Tools.ASQS
{
	public sealed class Queue
	{
		#region Private Members

		private AmazonSQSClient _client = null;
		private string _queueName = string.Empty;
		private bool _deleteMessageDefault = true;

		#endregion

		#region Constructor

		public Queue(string queueName, int visibilityTimeout, bool deleteMessageDefault)
		{
			_client = GetClient();
						
			_queueName = queueName;
			_deleteMessageDefault = deleteMessageDefault;

			#region Initialize Queue

			QueueResult openResult = OpenQueue();

			if (openResult.Success)
			{
				QueueResult visibilityResult = SetVisibilityAttribute(visibilityTimeout);

				if (!visibilityResult.Success)
				{
					throw new Exception(visibilityResult.ErrorMessage, visibilityResult.Exception);
				}
			}
			else
			{
				throw new Exception(openResult.ErrorMessage, openResult.Exception);
			}

			#endregion
		}

		#endregion

		#region Initialization Methods

		private QueueResult SetVisibilityAttribute(int visibilityTimeout)
		{
			Amazon.SQS.Model.SetQueueAttributes request = new Amazon.SQS.Model.SetQueueAttributes();
			Amazon.SQS.Model.SetQueueAttributesResponse response = null;
			QueueResult result = new QueueResult();

			try
			{
				request.QueueName = _queueName;

				Amazon.SQS.Model.Attribute visibilityAttribute = new Amazon.SQS.Model.Attribute();
				visibilityAttribute.Name = "VisibilityTimeout";
				visibilityAttribute.Value = visibilityTimeout.ToString();
				request.Attribute.Add(visibilityAttribute);

				response = _client.SetQueueAttributes(request);
				result.Success = true;
			}
			catch (Amazon.SQS.AmazonSQSException e)
			{
				result.isError = true;
				result.Exception = e;
				result.ErrorMessage = e.Message;
				result.ErrorType = QueueResult.GazaroASQSErrorType.Amazon;
			}
			catch (Exception e)
			{
				result.isError = true;
				result.Exception = e;
				result.ErrorMessage = e.Message;
				result.ErrorType = QueueResult.GazaroASQSErrorType.dotNet;
			}

			return result;

		}

		/// <summary>
		/// Open Queue Creates or Opens a Queue on Amazon SQS
		/// </summary>
		/// <param name="queueName">The name of the queue you want to create or open</param>
		/// <returns></returns>
		private QueueResult OpenQueue()
		{
			Amazon.SQS.Model.CreateQueueResponse response = null;
			Amazon.SQS.Model.CreateQueue request = new Amazon.SQS.Model.CreateQueue();
			QueueResult result = new QueueResult();

			try
			{
				request.QueueName = _queueName;
				response = _client.CreateQueue(request);

				result.Success = true;
				result.ResponseData = response.CreateQueueResult.QueueUrl;
			}
			catch (Amazon.SQS.AmazonSQSException e)
			{
				result.isError = true;
				result.Exception = e;
				result.ErrorMessage = e.Message;
				result.ErrorType = QueueResult.GazaroASQSErrorType.Amazon;
			}
			catch (Exception e)
			{
				result.isError = true;
				result.Exception = e;
				result.ErrorMessage = e.Message;
				result.ErrorType = QueueResult.GazaroASQSErrorType.dotNet;
			}

			return result;
		}

		#endregion

		#region Static Methods

		private static AmazonSQSClient GetClient()
		{
			string awsId = System.Configuration.ConfigurationManager.AppSettings["AWSId"].ToString();
			string awsSecretKey = System.Configuration.ConfigurationManager.AppSettings["AWSSecretKey"].ToString();

			AmazonSQSConfig config = new AmazonSQSConfig();
			config.UserAgent = "Illuminate v0.2";

			AmazonSQSClient client = new AmazonSQSClient(awsId, awsSecretKey, config);

			return client;
		}

		public static QueueResult DeleteQueue(string queueName)
		{
			AmazonSQSClient client = GetClient();

			Amazon.SQS.Model.DeleteQueueResponse response = null;
			Amazon.SQS.Model.DeleteQueue request = new Amazon.SQS.Model.DeleteQueue();
			QueueResult result = new QueueResult();

			try
			{
				request.QueueName = queueName;
				response = client.DeleteQueue(request);

				result.Success = true;
			}
			catch (Amazon.SQS.AmazonSQSException e)
			{
				result.isError = true;
				result.Exception = e;
				result.ErrorMessage = e.Message;
				result.ErrorType = QueueResult.GazaroASQSErrorType.Amazon;
			}
			catch (Exception e)
			{
				result.isError = true;
				result.Exception = e;
				result.ErrorMessage = e.Message;
				result.ErrorType = QueueResult.GazaroASQSErrorType.dotNet;
			}

			return result;
		}

		#endregion

		#region Public Methods

		public QueueResult Push(string message)
		{
			Amazon.SQS.Model.SendMessageResponse response = null;
			Amazon.SQS.Model.SendMessage request = new Amazon.SQS.Model.SendMessage();
			QueueResult result = new QueueResult();

			try
			{
				request.MessageBody = message;
				request.QueueName = _queueName;
				response = _client.SendMessage(request);

				result.Success = true;
				result.ResponseData = response.SendMessageResult.MessageId;
			}
			catch (Amazon.SQS.AmazonSQSException e)
			{
				result.isError = true;
				result.Exception = e;
				result.ErrorMessage = e.Message;
				result.ErrorType = QueueResult.GazaroASQSErrorType.Amazon;
			}
			catch (Exception e)
			{
				result.isError = true;
				result.Exception = e;
				result.ErrorMessage = e.Message;
				result.ErrorType = QueueResult.GazaroASQSErrorType.dotNet;
			}

			return result;
		}

		public QueueResult Pop()
		{
			return Pop(_deleteMessageDefault);
		}

		public QueueResult Pop(bool deleteMessage)
		{
			Amazon.SQS.Model.ReceiveMessageResponse response = null;
			Amazon.SQS.Model.ReceiveMessage request = new Amazon.SQS.Model.ReceiveMessage();
			QueueResult result = new QueueResult();

			try
			{
				request.QueueName = _queueName;
				response = _client.ReceiveMessage(request);

				result.Success = true;

				if (response.ReceiveMessageResult.Message.Count > 0)
				{
					result.ResponseData = response.ReceiveMessageResult.Message[0].Body;

					if (deleteMessage)
					{
						Amazon.SQS.Model.DeleteMessage deleteRequest = new Amazon.SQS.Model.DeleteMessage();

						deleteRequest.QueueName = _queueName;
						deleteRequest.ReceiptHandle = response.ReceiveMessageResult.Message[0].ReceiptHandle;

						_client.DeleteMessage(deleteRequest);
					}
				}
			}
			catch (Amazon.SQS.AmazonSQSException e)
			{
				result.isError = true;
				result.Exception = e;
				result.ErrorMessage = e.Message;
				result.ErrorType = QueueResult.GazaroASQSErrorType.Amazon;
			}
			catch (Exception e)
			{
				result.isError = true;
				result.Exception = e;
				result.ErrorMessage = e.Message;
				result.ErrorType = QueueResult.GazaroASQSErrorType.dotNet;
			}

			return result;
		}

		#endregion
	}
}
