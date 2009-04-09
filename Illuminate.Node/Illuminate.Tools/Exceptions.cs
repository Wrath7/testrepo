using System;

namespace Illuminate
{
	/// <summary>
	/// Class which contains common exceptions for Illuminate
	/// </summary>
	public class Exceptions
	{
        /// <summary>
        /// Exception to handle any item not found when calling a GET method or binding objects
        /// </summary>
        public class ItemNotFoundException : Exception
        {
            /// <summary>
            /// Class to handle any item not found when calling a GET method or binding objects
            /// </summary>
            /// <param name="Message">Message to pass the exception handler</param>
            public ItemNotFoundException(string Message) : base(Message) { }

        }

		/// <summary>
		/// Exception which handles throwing Illuminate Critical errors for the logger, and exception framework to pickup
		/// </summary>
		public class DeterminiticException : Exception
		{
			/// <summary>
			/// Exception which handles throwing Illuminate Determinitic errors for the logger, and exception framework to pickup
			/// </summary>
			/// <param name="message">Message to pass the exception handler</param>
			public DeterminiticException(string message, Exception innerException) : base(message, innerException) { }
			public DeterminiticException(string message) : base(message) { }
		}

		/// <summary>
		/// Exception which handles throwing Illuminate Critical errors for the logger, and exception framework to pickup
		/// </summary>
		public class CriticalException : Exception
		{
			/// <summary>
			/// Exception which handles throwing Illuminate Critical errors for the logger, and exception framework to pickup
			/// </summary>
			/// <param name="message">Message to pass the exception handler</param>
			public CriticalException(string message) : base(message) { }
		}

		/// <summary>
		/// Exception which handles throwing Illuminate Error errors for the logger, and exception framework to pickup
		/// </summary>
		public class ErrorException : Exception
		{
			/// <summary>
			/// Exception which handles throwing Illuminate Error errors for the logger, and exception framework to pickup
			/// </summary>
			/// <param name="message">Message to pass the exception handler</param>
			public ErrorException(string message) : base(message) { }
		}

		/// <summary>
		/// Exception which handles throwing Illuminate Error errors for the logger, and exception framework to pickup
		/// </summary>
		public class InformationException : Exception
		{
			/// <summary>
			/// Exception which handles throwing Illuminate Error errors for the logger, and exception framework to pickup
			/// </summary>
			/// <param name="message">Message to pass the exception handler</param>
			public InformationException(string message) : base(message) { }
		}

		/// <summary>
		/// Exception which is thrown when a Map task is taking too long to complete.
		/// </summary>
		public class MapTimeoutException : Exception
		{
			/// <summary>
			/// Exception which is thrown when a Map task is taking too long to complete.
			/// </summary>
			/// <param name="message">Message to pass the exception handler</param>
			public MapTimeoutException(string message) : base(message) { }
		}

    }

}
