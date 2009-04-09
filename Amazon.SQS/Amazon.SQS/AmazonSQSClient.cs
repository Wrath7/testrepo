/******************************************************************************* 
 *  Copyright 2007 Amazon Technologies, Inc.  
 *  Licensed under the Apache License, Version 2.0 (the "License"); 
 *  
 *  You may not use this file except in compliance with the License. 
 *  You may obtain a copy of the License at: http://aws.amazon.com/apache2.0
 *  This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
 *  CONDITIONS OF ANY KIND, either express or implied. See the License for the 
 *  specific language governing permissions and limitations under the License.
 * ***************************************************************************** 
 *    __  _    _  ___ 
 *   (  )( \/\/ )/ __)
 *   /__\ \    / \__ \
 *  (_)(_) \/\/  (___/
 * 
 *  Amazon SQS CSharp Library
 *  API Version: 2008-01-01
 *  Generated: Fri Feb 29 12:27:25 PST 2008 
 * 
 */

using System;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Globalization;
using System.Xml.Serialization;
using System.Collections.Generic;
using Amazon.SQS.Model;
using Attribute = Amazon.SQS.Model.Attribute;
using Amazon.SQS;

namespace Amazon.SQS
{


   /**

    *
    * AmazonSQSClient is an implementation of AmazonSQS
    *
    */
    public class AmazonSQSClient : AmazonSQS
    {

        private String awsAccessKeyId = null;
        private String awsSecretAccessKey = null;
        private AmazonSQSConfig config = null;

        /// <summary>
        /// Constructs AmazonSQSClient with AWS Access Key ID and AWS Secret Key
        /// </summary>
        /// <param name="awsAccessKeyId">AWS Access Key ID</param>
        /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
        public AmazonSQSClient(String awsAccessKeyId, String awsSecretAccessKey)
            : this(awsAccessKeyId, awsSecretAccessKey, new AmazonSQSConfig())
        {
        }


        /// <summary>
        /// Constructs AmazonSQSClient with AWS Access Key ID and AWS Secret Key
        /// </summary>
        /// <param name="awsAccessKeyId">AWS Access Key ID</param>
        /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
        /// <param name="config">configuration</param>
        public AmazonSQSClient(String awsAccessKeyId, String awsSecretAccessKey, AmazonSQSConfig config)
        {
            this.awsAccessKeyId = awsAccessKeyId;
            this.awsSecretAccessKey = awsSecretAccessKey;
            this.config = config;
        }


        // Public API ------------------------------------------------------------//

        
        /// <summary>
        /// Create Queue 
        /// </summary>
        /// <param name="request">Create Queue  Action</param>
        /// <returns>Create Queue  Response from the service</returns>
        /// <remarks>
        /// 
        /// The CreateQueue action creates a new queue. You must provide a queue name that is unique within the scope of the queues you own. The queue is assigned a queue URL; you must use this URL when performing actions on the queue.  When you create a queue, if a queue with the same name already exists, CreateQueue returns the queue URL with an error indicating that the queue already exists.
        ///   
        /// </remarks>
        public CreateQueueResponse CreateQueue(CreateQueue request)
        {
            return Invoke<CreateQueueResponse>(ConvertCreateQueue(request));
        }

        
        /// <summary>
        /// List Queues 
        /// </summary>
        /// <param name="request">List Queues  Action</param>
        /// <returns>List Queues  Response from the service</returns>
        /// <remarks>
        /// 
        /// The ListQueues action returns a list of your queues.
        ///   
        /// </remarks>
        public ListQueuesResponse ListQueues(ListQueues request)
        {
            return Invoke<ListQueuesResponse>(ConvertListQueues(request));
        }

        
        /// <summary>
        /// Delete Message 
        /// </summary>
        /// <param name="request">Delete Message  Action</param>
        /// <returns>Delete Message  Response from the service</returns>
        /// <remarks>
        /// The DeleteMessage action unconditionally removes the specified message from the specified queue. Even if the message is locked by another reader due to the visibility timeout setting, it is still deleted from the queue.
        ///   
        /// </remarks>
        public DeleteMessageResponse DeleteMessage(DeleteMessage request)
        {
            return Invoke<DeleteMessageResponse>(ConvertDeleteMessage(request));
        }

        
        /// <summary>
        /// Delete Queue 
        /// </summary>
        /// <param name="request">Delete Queue  Action</param>
        /// <returns>Delete Queue  Response from the service</returns>
        /// <remarks>
        /// 
        /// This action unconditionally deletes the queue specified by the queue URL. Use this operation WITH CARE!  The queue is deleted even if it is NOT empty.
        ///   
        /// </remarks>
        public DeleteQueueResponse DeleteQueue(DeleteQueue request)
        {
            return Invoke<DeleteQueueResponse>(ConvertDeleteQueue(request));
        }

        
        /// <summary>
        /// Get Queue Attributes 
        /// </summary>
        /// <param name="request">Get Queue Attributes  Action</param>
        /// <returns>Get Queue Attributes  Response from the service</returns>
        /// <remarks>
        /// 
        /// Gets one or all attributes of a queue. Queues currently have two attributes you can get: ApproximateNumberOfMessages and VisibilityTimeout.
        ///   
        /// </remarks>
        public GetQueueAttributesResponse GetQueueAttributes(GetQueueAttributes request)
        {
            return Invoke<GetQueueAttributesResponse>(ConvertGetQueueAttributes(request));
        }

        
        /// <summary>
        /// Receive Message 
        /// </summary>
        /// <param name="request">Receive Message  Action</param>
        /// <returns>Receive Message  Response from the service</returns>
        /// <remarks>
        /// 
        /// Retrieves one or more messages from the specified queue, including the message body and message ID of each message. Messages returned by this action stay in the queue until you delete them. However, once a message is returned to a ReceiveMessage request, it is not returned on subsequent ReceiveMessage requests for the duration of the VisibilityTimeout. If you do not specify a VisibilityTimeout in the request, the overall visibility timeout for the queue is used for the returned messages.
        ///   
        /// </remarks>
        public ReceiveMessageResponse ReceiveMessage(ReceiveMessage request)
        {
            return Invoke<ReceiveMessageResponse>(ConvertReceiveMessage(request));
        }

        
        /// <summary>
        /// Send Message 
        /// </summary>
        /// <param name="request">Send Message  Action</param>
        /// <returns>Send Message  Response from the service</returns>
        /// <remarks>
        /// The SendMessage action delivers a message to the specified queue.
        ///   
        /// </remarks>
        public SendMessageResponse SendMessage(SendMessage request)
        {
            return Invoke<SendMessageResponse>(ConvertSendMessage(request));
        }

        
        /// <summary>
        /// Set Queue Attributes 
        /// </summary>
        /// <param name="request">Set Queue Attributes  Action</param>
        /// <returns>Set Queue Attributes  Response from the service</returns>
        /// <remarks>
        /// 
        /// Sets an attribute of a queue. Currently, you can set only the VisibilityTimeout attribute for a queue.
        ///   
        /// </remarks>
        public SetQueueAttributesResponse SetQueueAttributes(SetQueueAttributes request)
        {
            return Invoke<SetQueueAttributesResponse>(ConvertSetQueueAttributes(request));
        }

        // Private API ------------------------------------------------------------//

        /**
         * Configure HttpClient with set of defaults as well as configuration
         * from AmazonSQSConfig instance
         */
        private HttpWebRequest ConfigureWebRequest(int contentLength, String path)
        {
            HttpWebRequest request = WebRequest.Create(config.ServiceURL + path) as HttpWebRequest;

            if (config.IsSetProxyHost())
            {
                request.Proxy = new WebProxy(config.ProxyHost, config.ProxyPort);
            }
            request.UserAgent = config.UserAgent;
            request.Method = "POST";
            request.Timeout = 50000;
            request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            request.ContentLength = contentLength;

            return request;
        }

        /**
         * Invoke request and return response
         */
        private T Invoke<T>(IDictionary<String, String> parameters)
        {
            String actionName = parameters["Action"];
            T response = default(T);
            String responseBody = null;
            String queueName = parameters.ContainsKey("QueueName") ? parameters["QueueName"] : null;
            bool removeQueueNameFromParameters = queueName != null && !"CreateQueue".Equals(actionName);
            String queuepath = removeQueueNameFromParameters ? "/" + queueName : "";
            if (removeQueueNameFromParameters) {
                parameters.Remove("QueueName");
            }
            HttpStatusCode statusCode = default(HttpStatusCode);

            /* Add required request parameters */
            AddRequiredParameters(parameters);

            String queryString = GetParametersAsString(parameters);

            byte[] requestData = new UTF8Encoding().GetBytes(queryString);
            bool shouldRetry = true;
            int retries = 0;
            do
            {
                HttpWebRequest request = ConfigureWebRequest(requestData.Length, queuepath);
                /* Submit the request and read response body */
                try
                {
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(requestData, 0, requestData.Length);
                    }
                    using (HttpWebResponse httpResponse = request.GetResponse() as HttpWebResponse)
                    {
                        statusCode = httpResponse.StatusCode;
                        StreamReader reader = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8);
                        responseBody = reader.ReadToEnd();
                    }

                    /* Attempt to deserialize response into <Action> Response type */
                    XmlSerializer serlizer = new XmlSerializer(typeof(T));
                    response = (T)serlizer.Deserialize(new StringReader(responseBody));
                    shouldRetry = false;
                }
                /* Web exception is thrown on unsucessful responses */
                catch (WebException we)
                {
                    shouldRetry = false;
                    using (HttpWebResponse httpErrorResponse = (HttpWebResponse)we.Response as HttpWebResponse)
                    {
                        if (httpErrorResponse == null)
                        {
                            throw new AmazonSQSException(we);
                        }
                        statusCode = httpErrorResponse.StatusCode;
                        StreamReader reader = new StreamReader(httpErrorResponse.GetResponseStream(), Encoding.UTF8);
                        responseBody = reader.ReadToEnd();
                    }

                    if (statusCode == HttpStatusCode.InternalServerError || statusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        shouldRetry = true;
                        PauseOnRetry(++retries, statusCode);
                    }
                    else
                    {

                        /* Attempt to deserialize response into ErrorResponse type */
                        try
                        {
                            XmlSerializer serlizer = new XmlSerializer(typeof(ErrorResponse));
                            ErrorResponse errorResponse = (ErrorResponse)serlizer.Deserialize(new StringReader(responseBody));
                            Error error = errorResponse.Error[0];

                            /* Throw formatted exception with information available from the error response */
                            throw new AmazonSQSException(
                                error.Message,
                                statusCode,
                                error.Code,
                                error.Type,
                                errorResponse.RequestId,
                                errorResponse.ToXML());
                        }
                        /* Rethrow on deserializer error */
                        catch (Exception e)
                        {
                            if (e is AmazonSQSException)
                            {
                                throw e;
                            }
                            else
                            {
                                AmazonSQSException se = ReportAnyErrors(responseBody, statusCode, e);
                                throw se;
                            }
                        }
                    }
                }

                /* Catch other exceptions, attempt to convert to formatted exception, 
                 * else rethrow wrapped exception */
                catch (Exception e)
                {
                    throw new AmazonSQSException(e);
                }
            } while (shouldRetry);

            return response;
        }


        /**
         * Look for additional error strings in the response and return formatted exception
         */
        private AmazonSQSException ReportAnyErrors(String responseBody, HttpStatusCode status, Exception e)
        {

            AmazonSQSException ex = null;

            if (responseBody != null && responseBody.StartsWith("<"))
            {
                Match errorMatcherOne = Regex.Match(responseBody, "<RequestId>(.*)</RequestId>.*<Error>" +
                        "<Code>(.*)</Code><Message>(.*)</Message></Error>.*(<Error>)?", RegexOptions.Multiline);
                Match errorMatcherTwo = Regex.Match(responseBody, "<Error><Code>(.*)</Code><Message>(.*)" +
                        "</Message></Error>.*(<Error>)?.*<RequestID>(.*)</RequestID>", RegexOptions.Multiline);

                if (errorMatcherOne.Success)
                {
                    String requestId = errorMatcherOne.Groups[1].Value;
                    String code = errorMatcherOne.Groups[2].Value;
                    String message = errorMatcherOne.Groups[3].Value;

                    ex = new AmazonSQSException(message, status, code, "Unknown", requestId, responseBody);

                }
                else if (errorMatcherTwo.Success)
                {
                    String code = errorMatcherTwo.Groups[1].Value;
                    String message = errorMatcherTwo.Groups[2].Value;
                    String requestId = errorMatcherTwo.Groups[4].Value;

                    ex = new AmazonSQSException(message, status, code, "Unknown", requestId, responseBody);
                }
                else
                {
                    ex = new AmazonSQSException("Internal Error", status);
                }
            }
            else
            {
                ex = new AmazonSQSException("Internal Error", status);
            }
            return ex;
        }

        /**
         * Exponential sleep on failed request
         */
        private void PauseOnRetry(int retries, HttpStatusCode status)
        {
            if (retries <= config.MaxErrorRetry)
            {
                int delay = (int)Math.Pow(4, retries) * 100;
                System.Threading.Thread.Sleep(delay);
            }
            else
            {
                throw new AmazonSQSException("Maximum number of retry attempts reached : " + (retries - 1), status);
            }
        }

        /**
         * Add authentication related and version parameters
         */
        private void AddRequiredParameters(IDictionary<String, String> parameters)
        {
            parameters.Add("AWSAccessKeyId", this.awsAccessKeyId);
            parameters.Add("Timestamp", GetFormattedTimestamp());
            parameters.Add("Version", config.ServiceVersion);
            parameters.Add("SignatureVersion", config.SignatureVersion);
            parameters.Add("Signature", SignParameters(parameters, this.awsSecretAccessKey));
        }

        /**
         * Convert Disctionary of paremeters to Url encoded query string
         */
        private string GetParametersAsString(IDictionary<String, String> parameters)
        {
            StringBuilder data = new StringBuilder();
            foreach (String key in (IEnumerable<String>)parameters.Keys)
            {
                String value = parameters[key];
                if (value != null && value.Length > 0)
                {
                    data.Append(key);
                    data.Append('=');
                    data.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
                    data.Append('&');
                }    
            }
            String stringData = data.ToString();
            if (stringData.EndsWith("&")) 
            {
                stringData = stringData.Remove(stringData.Length - 1, 1);
            }
            return stringData;
        }

        /**
         * Computes RFC 2104-compliant HMAC signature for request parameters
         * Implements AWS Signature, as per following spec:
         *
         * If Signature Version is 0, it signs concatenated Action and Timestamp
         *
         * If Signature Version is 1, it performs the following:
         *
         * Sorts all  parameters (including SignatureVersion and excluding Signature,
         * the value of which is being created), ignoring case.
         *
         * Iterate over the sorted list and append the parameter name (in original case)
         * and then its value. It will not URL-encode the parameter values before
         * constructing this string. There are no separators.
         */
        private String SignParameters(IDictionary<String, String> parameters, String key)
        {
            String signatureVersion = parameters["SignatureVersion"];
            StringBuilder data = new StringBuilder();

            if ("0".Equals(signatureVersion))
            {
                data.Append(parameters["Action"]).Append(parameters["Timestamp"]);
            }
            else if ("1".Equals(signatureVersion))
            {
                IDictionary<String, String> sorted =
                    new SortedDictionary<String, String>(parameters, StringComparer.InvariantCultureIgnoreCase);
                parameters.Remove("Signature");
                foreach (KeyValuePair<String, String> pair in sorted)
                {
                    if (pair.Value != null && pair.Value.Length > 0)
                    {
                        data.Append(pair.Key);
                        data.Append(pair.Value);
                    }
                }
            }
            else
            {
                throw new Exception("Invalid Signature Version specified");
            }
            return Sign(data.ToString(), key);
        }

        /**
         * Computes RFC 2104-compliant HMAC signature.
         */
        private String Sign(String data, String key)
        {
            Encoding encoding = new UTF8Encoding();
            HMACSHA1 signature = new HMACSHA1(encoding.GetBytes(key));
            return Convert.ToBase64String(signature.ComputeHash(
                encoding.GetBytes(data.ToCharArray())));
        }


        /**
         * Formats date as ISO 8601 timestamp
         */
        private String GetFormattedTimestamp()
        {
            DateTime dateTime = DateTime.Now;
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                                 dateTime.Hour, dateTime.Minute, dateTime.Second,
                                 dateTime.Millisecond
                                 , DateTimeKind.Local
                               ).ToUniversalTime().ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z",
                                CultureInfo.InvariantCulture);
        }

        
        /**
         * Convert CreateQueue to name value pairs
         */
        private IDictionary<String, String> ConvertCreateQueue(CreateQueue request) 
        {
            
            IDictionary<String, String> parameters = new Dictionary<String, String>();
            parameters.Add("Action", "CreateQueue");
            if (request.IsSetQueueName()) 
            {
                parameters.Add("QueueName", request.QueueName);
            }
            if (request.IsSetDefaultVisibilityTimeout()) 
            {
                parameters.Add("DefaultVisibilityTimeout", request.DefaultVisibilityTimeout + "");
            }

            return parameters;
        }
        
                        
        /**
         * Convert ListQueues to name value pairs
         */
        private IDictionary<String, String> ConvertListQueues(ListQueues request) 
        {
            
            IDictionary<String, String> parameters = new Dictionary<String, String>();
            parameters.Add("Action", "ListQueues");
            if (request.IsSetQueueNamePrefix()) 
            {
                parameters.Add("QueueNamePrefix", request.QueueNamePrefix);
            }

            return parameters;
        }
        
                        
        /**
         * Convert DeleteMessage to name value pairs
         */
        private IDictionary<String, String> ConvertDeleteMessage(DeleteMessage request) 
        {
            
            IDictionary<String, String> parameters = new Dictionary<String, String>();
            parameters.Add("Action", "DeleteMessage");
            if (request.IsSetQueueName()) 
            {
                parameters.Add("QueueName", request.QueueName);
            }
            if (request.IsSetReceiptHandle()) 
            {
                parameters.Add("ReceiptHandle", request.ReceiptHandle);
            }

            return parameters;
        }
        
                        
        /**
         * Convert DeleteQueue to name value pairs
         */
        private IDictionary<String, String> ConvertDeleteQueue(DeleteQueue request) 
        {
            
            IDictionary<String, String> parameters = new Dictionary<String, String>();
            parameters.Add("Action", "DeleteQueue");
            if (request.IsSetQueueName()) 
            {
                parameters.Add("QueueName", request.QueueName);
            }

            return parameters;
        }
        
                        
        /**
         * Convert GetQueueAttributes to name value pairs
         */
        private IDictionary<String, String> ConvertGetQueueAttributes(GetQueueAttributes request) 
        {
            
            IDictionary<String, String> parameters = new Dictionary<String, String>();
            parameters.Add("Action", "GetQueueAttributes");
            if (request.IsSetQueueName()) 
            {
                parameters.Add("QueueName", request.QueueName);
            }
            List<String> attributeNameList  =  request.AttributeName;
            foreach  (String attributeName in attributeNameList)
            { 
                parameters.Add("AttributeName" + "."  + (attributeNameList.IndexOf(attributeName) + 1), attributeName);
            }	

            return parameters;
        }
        
                        
        /**
         * Convert ReceiveMessage to name value pairs
         */
        private IDictionary<String, String> ConvertReceiveMessage(ReceiveMessage request) 
        {
            
            IDictionary<String, String> parameters = new Dictionary<String, String>();
            parameters.Add("Action", "ReceiveMessage");
            if (request.IsSetQueueName()) 
            {
                parameters.Add("QueueName", request.QueueName);
            }
            if (request.IsSetMaxNumberOfMessages()) 
            {
                parameters.Add("MaxNumberOfMessages", request.MaxNumberOfMessages + "");
            }
            if (request.IsSetVisibilityTimeout()) 
            {
                parameters.Add("VisibilityTimeout", request.VisibilityTimeout + "");
            }

            return parameters;
        }
        
                        
        /**
         * Convert SendMessage to name value pairs
         */
        private IDictionary<String, String> ConvertSendMessage(SendMessage request) 
        {
            
            IDictionary<String, String> parameters = new Dictionary<String, String>();
            parameters.Add("Action", "SendMessage");
            if (request.IsSetQueueName()) 
            {
                parameters.Add("QueueName", request.QueueName);
            }
            if (request.IsSetMessageBody()) 
            {
                parameters.Add("MessageBody", request.MessageBody);
            }

            return parameters;
        }
        
                        
        /**
         * Convert SetQueueAttributes to name value pairs
         */
        private IDictionary<String, String> ConvertSetQueueAttributes(SetQueueAttributes request) 
        {
            
            IDictionary<String, String> parameters = new Dictionary<String, String>();
            parameters.Add("Action", "SetQueueAttributes");
            if (request.IsSetQueueName()) 
            {
                parameters.Add("QueueName", request.QueueName);
            }
            List<Attribute> attributeList = request.Attribute;
            foreach (Attribute attribute in attributeList) 
            {
                if (attribute.IsSetName()) 
                {
                    parameters.Add("Attribute" + "."  + (attributeList.IndexOf(attribute) + 1) + "." + "Name", attribute.Name);
                }
                if (attribute.IsSetValue()) 
                {
                    parameters.Add("Attribute" + "."  + (attributeList.IndexOf(attribute) + 1) + "." + "Value", attribute.Value);
                }

            }

            return parameters;
        }
        
                                                                                                
    }
}