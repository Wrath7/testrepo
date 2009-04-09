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
using System.Xml.Serialization;
using System.Collections.Generic;
using Amazon.SQS.Model;

namespace Amazon.SQS
{


    /// <summary>
    /// 
    /// Amazon Simple Queue Service (Amazon SQS) offers a reliable, highly scalable hosted queue for storing messages as they travel between computers. By using Amazon SQS, developers can simply move data between distributed application components performing different tasks, without losing messages or requiring each component to be always available.  Amazon SQS works by exposing Amazon's web-scale messaging infrastructure as a web service. Any computer on the Internet can add or read messages without any installed software or special firewall configurations. Components of applications using Amazon SQS can run independently, and do not need to be on the same network, developed with the same technologies, or running at the same time.
    /// 
    /// </summary>
    public interface  AmazonSQS {
    
                
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
        CreateQueueResponse CreateQueue(CreateQueue request);

                
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
        ListQueuesResponse ListQueues(ListQueues request);

                
        /// <summary>
        /// Delete Message 
        /// </summary>
        /// <param name="request">Delete Message  request</param>
        /// <returns>Delete Message  Response from the service</returns>
        /// <remarks>
        /// The DeleteMessage action unconditionally removes the specified message from the specified queue. Even if the message is locked by another reader due to the visibility timeout setting, it is still deleted from the queue.
        ///   
        /// </remarks>
        DeleteMessageResponse DeleteMessage(DeleteMessage request);

                
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
        DeleteQueueResponse DeleteQueue(DeleteQueue request);

                
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
        GetQueueAttributesResponse GetQueueAttributes(GetQueueAttributes request);

                
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
        ReceiveMessageResponse ReceiveMessage(ReceiveMessage request);

                
        /// <summary>
        /// Send Message 
        /// </summary>
        /// <param name="request">Send Message  request</param>
        /// <returns>Send Message  Response from the service</returns>
        /// <remarks>
        /// The SendMessage action delivers a message to the specified queue.
        ///   
        /// </remarks>
        SendMessageResponse SendMessage(SendMessage request);

                
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
        SetQueueAttributesResponse SetQueueAttributes(SetQueueAttributes request);

    }
}