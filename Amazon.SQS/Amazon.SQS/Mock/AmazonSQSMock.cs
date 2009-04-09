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
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Amazon.SQS.Model;

namespace Amazon.SQS.Mock
{

    /// <summary>
    /// AmazonSQSMock is the implementation of AmazonSQS based
    /// on the pre-populated set of XML files that serve local data. It simulates 
    /// responses from Amazon SQS service.
    /// </summary>
    /// <remarks>
    /// Use this to test your application without making a call to 
    /// Amazon SQS 
    /// 
    /// Note, current Mock Service implementation does not valiadate requests
    /// </remarks>
    public  class AmazonSQSMock : AmazonSQS {
    

        // Public API ------------------------------------------------------------//
    
        
        /// <summary>
        /// Create Queue 
        /// </summary>
        /// <param name="request">Create Queue  request</param>
        /// <returns>Create Queue  Response from the service</returns>
        /// <remarks>
        /// 
        /// The CreateQueue action creates a new queue. You must provide a queue name that is unique within the scope of the queues you own. The queue is assigned a queue URL; you must use this URL when performing actions on the queue.  When you create a queue, if a queue with the same name already exists, CreateQueue returns the queue URL with an error indicating that the queue already exists.
        ///   
        /// </remarks>
        public CreateQueueResponse CreateQueue(CreateQueue request) 
        {
            return Invoke<CreateQueueResponse>("CreateQueueResponse.xml");
        }
        
        /// <summary>
        /// List Queues 
        /// </summary>
        /// <param name="request">List Queues  request</param>
        /// <returns>List Queues  Response from the service</returns>
        /// <remarks>
        /// 
        /// The ListQueues action returns a list of your queues.
        ///   
        /// </remarks>
        public ListQueuesResponse ListQueues(ListQueues request) 
        {
            return Invoke<ListQueuesResponse>("ListQueuesResponse.xml");
        }
        
        /// <summary>
        /// Delete Message 
        /// </summary>
        /// <param name="request">Delete Message  request</param>
        /// <returns>Delete Message  Response from the service</returns>
        /// <remarks>
        /// The DeleteMessage action unconditionally removes the specified message from the specified queue. Even if the message is locked by another reader due to the visibility timeout setting, it is still deleted from the queue.
        ///   
        /// </remarks>
        public DeleteMessageResponse DeleteMessage(DeleteMessage request) 
        {
            return Invoke<DeleteMessageResponse>("DeleteMessageResponse.xml");
        }
        
        /// <summary>
        /// Delete Queue 
        /// </summary>
        /// <param name="request">Delete Queue  request</param>
        /// <returns>Delete Queue  Response from the service</returns>
        /// <remarks>
        /// 
        /// This action unconditionally deletes the queue specified by the queue URL. Use this operation WITH CARE!  The queue is deleted even if it is NOT empty.
        ///   
        /// </remarks>
        public DeleteQueueResponse DeleteQueue(DeleteQueue request) 
        {
            return Invoke<DeleteQueueResponse>("DeleteQueueResponse.xml");
        }
        
        /// <summary>
        /// Get Queue Attributes 
        /// </summary>
        /// <param name="request">Get Queue Attributes  request</param>
        /// <returns>Get Queue Attributes  Response from the service</returns>
        /// <remarks>
        /// 
        /// Gets one or all attributes of a queue. Queues currently have two attributes you can get: ApproximateNumberOfMessages and VisibilityTimeout.
        ///   
        /// </remarks>
        public GetQueueAttributesResponse GetQueueAttributes(GetQueueAttributes request) 
        {
            return Invoke<GetQueueAttributesResponse>("GetQueueAttributesResponse.xml");
        }
        
        /// <summary>
        /// Receive Message 
        /// </summary>
        /// <param name="request">Receive Message  request</param>
        /// <returns>Receive Message  Response from the service</returns>
        /// <remarks>
        /// 
        /// Retrieves one or more messages from the specified queue, including the message body and message ID of each message. Messages returned by this action stay in the queue until you delete them. However, once a message is returned to a ReceiveMessage request, it is not returned on subsequent ReceiveMessage requests for the duration of the VisibilityTimeout. If you do not specify a VisibilityTimeout in the request, the overall visibility timeout for the queue is used for the returned messages.
        ///   
        /// </remarks>
        public ReceiveMessageResponse ReceiveMessage(ReceiveMessage request) 
        {
            return Invoke<ReceiveMessageResponse>("ReceiveMessageResponse.xml");
        }
        
        /// <summary>
        /// Send Message 
        /// </summary>
        /// <param name="request">Send Message  request</param>
        /// <returns>Send Message  Response from the service</returns>
        /// <remarks>
        /// The SendMessage action delivers a message to the specified queue.
        ///   
        /// </remarks>
        public SendMessageResponse SendMessage(SendMessage request) 
        {
            return Invoke<SendMessageResponse>("SendMessageResponse.xml");
        }
        
        /// <summary>
        /// Set Queue Attributes 
        /// </summary>
        /// <param name="request">Set Queue Attributes  request</param>
        /// <returns>Set Queue Attributes  Response from the service</returns>
        /// <remarks>
        /// 
        /// Sets an attribute of a queue. Currently, you can set only the VisibilityTimeout attribute for a queue.
        ///   
        /// </remarks>
        public SetQueueAttributesResponse SetQueueAttributes(SetQueueAttributes request) 
        {
            return Invoke<SetQueueAttributesResponse>("SetQueueAttributesResponse.xml");
        }

        // Private API ------------------------------------------------------------//

        private T Invoke<T>(String xmlResource)
        {
            XmlSerializer serlizer = new XmlSerializer(typeof(T));
            Stream xmlStream = Assembly.GetAssembly(this.GetType()).GetManifestResourceStream(xmlResource);
            return (T)serlizer.Deserialize(xmlStream);
        }
    }
}