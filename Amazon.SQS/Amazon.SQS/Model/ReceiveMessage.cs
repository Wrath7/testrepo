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
using System.Text;


namespace Amazon.SQS.Model
{
    [XmlTypeAttribute(Namespace = "http://queue.amazonaws.com/doc/2008-01-01/")]
    [XmlRootAttribute(Namespace = "http://queue.amazonaws.com/doc/2008-01-01/", IsNullable = false)]
    public class ReceiveMessage
    {
    
        private String queueNameField;

        private Decimal? maxNumberOfMessagesField;

        private Decimal? visibilityTimeoutField;


        /// <summary>
        /// Gets and sets the QueueName property.
        /// </summary>
        [XmlElementAttribute(ElementName = "QueueName")]
        public String QueueName
        {
            get { return this.queueNameField ; }
            set { this.queueNameField= value; }
        }



        /// <summary>
        /// Sets the QueueName property
        /// </summary>
        /// <param name="queueName">QueueName property</param>
        /// <returns>this instance</returns>
        public ReceiveMessage WithQueueName(String queueName)
        {
            this.queueNameField = queueName;
            return this;
        }



        /// <summary>
        /// Checks if QueueName property is set
        /// </summary>
        /// <returns>true if QueueName property is set</returns>
        public Boolean IsSetQueueName()
        {
            return  this.queueNameField != null;

        }


        /// <summary>
        /// Gets and sets the MaxNumberOfMessages property.
        /// </summary>
        [XmlElementAttribute(ElementName = "MaxNumberOfMessages")]
        public Decimal MaxNumberOfMessages
        {
            get { return this.maxNumberOfMessagesField.GetValueOrDefault() ; }
            set { this.maxNumberOfMessagesField= value; }
        }



        /// <summary>
        /// Sets the MaxNumberOfMessages property
        /// </summary>
        /// <param name="maxNumberOfMessages">MaxNumberOfMessages property</param>
        /// <returns>this instance</returns>
        public ReceiveMessage WithMaxNumberOfMessages(Decimal maxNumberOfMessages)
        {
            this.maxNumberOfMessagesField = maxNumberOfMessages;
            return this;
        }



        /// <summary>
        /// Checks if MaxNumberOfMessages property is set
        /// </summary>
        /// <returns>true if MaxNumberOfMessages property is set</returns>
        public Boolean IsSetMaxNumberOfMessages()
        {
            return  this.maxNumberOfMessagesField.HasValue;

        }


        /// <summary>
        /// Gets and sets the VisibilityTimeout property.
        /// </summary>
        [XmlElementAttribute(ElementName = "VisibilityTimeout")]
        public Decimal VisibilityTimeout
        {
            get { return this.visibilityTimeoutField.GetValueOrDefault() ; }
            set { this.visibilityTimeoutField= value; }
        }



        /// <summary>
        /// Sets the VisibilityTimeout property
        /// </summary>
        /// <param name="visibilityTimeout">VisibilityTimeout property</param>
        /// <returns>this instance</returns>
        public ReceiveMessage WithVisibilityTimeout(Decimal visibilityTimeout)
        {
            this.visibilityTimeoutField = visibilityTimeout;
            return this;
        }



        /// <summary>
        /// Checks if VisibilityTimeout property is set
        /// </summary>
        /// <returns>true if VisibilityTimeout property is set</returns>
        public Boolean IsSetVisibilityTimeout()
        {
            return  this.visibilityTimeoutField.HasValue;

        }





    }

}