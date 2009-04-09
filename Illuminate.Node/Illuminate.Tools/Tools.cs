using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Illuminate.Tools
{
	/// <summary>
	/// Set of Helper Tools
	/// </summary>
    public class Tools
    {
		public static string GetUnderline(int Count)
		{
			string Text = "";

			for (int i = 0; i < Count; i++)
			{
				Text = Text + "-";
			}

			return Text;
		}

		public static string GetString(string Text, int MaxLength)
		{
			if (Text.Length > MaxLength)
			{
				return Text.Substring(0, MaxLength);
			}
			else if (Text.Length == MaxLength)
			{
				return Text;
			}
			else
			{
				int Diff = MaxLength - Text.Length;

				for (int i = 0; i < Diff; i++)
				{
					Text = Text + " ";
				}
			}

			return Text;
		}

		static string CleanCharacters(string Input)
		{
			char[] charsToReplace = new char[] { (char)8216, (char)8217, (char)8220, (char)8221, (char)8230, (char)8211, (char)8212, (char)8226, (char)8482 };
			string[] charsAsciiValues = new string[] { ((char)39).ToString(), ((char)39).ToString(), ((char)34).ToString(), ((char)34).ToString(), "...", "-", "-", ".", "tm" };

			for (int i = 0; i < charsToReplace.Length; i++)
			{
				Input = Input.Replace(charsToReplace[i].ToString(), charsAsciiValues[i]);
			}

			return Input;
		}

		public static string ConvertToUFT8(Encoding enc, string Value)
		{
			Encoding utf8 = Encoding.UTF8;

			byte[] isoBytes = enc.GetBytes(Value.ToString());
			byte[] utf8Bytes = Encoding.Convert(enc, utf8, isoBytes);

			Value = System.Text.UTF8Encoding.UTF8.GetString(utf8Bytes);

			return Value;
		}

		public static string ConvertToUFT8(string Value)
		{
			Encoding iso = Encoding.GetEncoding("iso-8859-1");
			Encoding utf8 = Encoding.UTF8;

			byte[] isoBytes = iso.GetBytes(Value.ToString());
			byte[] utf8Bytes = Encoding.Convert(iso, utf8, isoBytes);

			Value = System.Text.UTF8Encoding.UTF8.GetString(utf8Bytes);

			return Value;
		}

		/// <summary>
		/// Checks if a string is numeric
		/// </summary>
		/// <param name="value">The string you want to check</param>
		/// <returns>Whether the string is numeric or not</returns>
		public static bool IsNumeric(string value)
		{
			if (value == null) return false;
            if (value.ToLower() == "nan") return false;

			double Test = 0;
			bool Result = double.TryParse(value.ToString(), out Test);

			return Result;
		}

		/// <summary>
		/// Determines is a column is part of a datatable
		/// </summary>
		/// <param name="Dt">The datatable you want to check againsts</param>
		/// <param name="ColumnName">The name of the column</param>
		/// <returns>Whether the column exists in the datatable or not</returns>
		public static bool IsColumn(DataTable Dt, string ColumnName)
		{
			if (Dt == null)
			{
				return false;
			}

			for (int i = 0; i < Dt.Columns.Count; i++)
			{
				if (Dt.Columns[i].ColumnName.ToLower() == ColumnName.ToLower())
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Generate a new date of 1/1/1900
		/// </summary>
		/// <returns></returns>
		public static DateTime GetDefaultDate()
		{
			return new DateTime(1900, 1, 1);
		}

		/// <summary>
		/// Generates of unique filename (not thread safe)
		/// </summary>
		/// <param name="Extension">The extension you want the filename to have</param>
		/// <returns></returns>
		public static string GetUniqueFileName(string Extension)
		{
			string FileName = string.Empty;
			FileName = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Ticks.ToString() + "." + Extension;

			return FileName;
		}

		/// <summary>
		/// Trucates characters of a string starting from the left.
		/// </summary>
		/// <param name="Text">The text you want to truncate</param>
		/// <param name="Length">The length of the text you want to truncatee</param>
		/// <returns></returns>
		public static string Left(string Text, int Length)
		{
			if (Length > 0 && Text.Length > Length)
			{
				return Text.Substring(0, Length);
			}

			return Text;
		}

		/// <summary>
		/// Trucates characters of a string starting from the left using the last space as the cutting point.
		/// </summary>
		/// <param name="Text">The text you want to truncate</param>
		/// <param name="Length">The length of the text you want to truncatee</param>
		/// <returns></returns>
		public static string LeftSpaces(string Text, int Length)
		{
			if (Length > 0 && Text.Length > Length)
			{
				MatchCollection Mc = Regex.Matches(Text, @"\s");

				if (Mc.Count == 0) return Text.Substring(0, Length) + "...";
				
				for (int i = 0; i < Mc.Count; i++)
				{
					if (Mc[i].Index > Length)
					{
						if (i == 0)
						{
							return Text.Substring(0, Length) + "...";
						}
						else
						{
							string ReturnText = Text.Substring(0, Mc[i - 1].Index);

							ReturnText = StripLastCharForLeftSpaceReturn(ReturnText).Trim();

							return ReturnText + "...";
						}
					}
				}
			}

			return Text;
		}

		private static string StripLastCharForLeftSpaceReturn(string Text)
		{
			if (Text.Length == 1) return Text;

			string Char = Text.Substring(Text.Length - 1);

			if (!Regex.Match(Char, @"\w", RegexOptions.IgnoreCase).Success)
			{
				return StripLastCharForLeftSpaceReturn(Text.Substring(0, Text.Length-1));
			}

			return Text;
		}

		/// <summary>
		/// Gets a text date for display on deal markers.  (e.g. 15 hours old, 2 days old).
		/// </summary>
		/// <param name="CreatedOn"></param>
		/// <returns></returns>
		public static string GetTimeSpanText(DateTime CreatedOn)
		{
			TimeSpan Ts = DateTime.Now.Subtract(CreatedOn);

			int DaysOld = (int)Ts.TotalDays;

			if (DaysOld == 1)
			{
				return DaysOld.ToString() + " day ago";
			}
			else if (DaysOld == 0)
			{
				DaysOld = (int)Ts.TotalHours;

				if (DaysOld == 0)
				{
					DaysOld = (int)Ts.TotalMinutes;

					if (DaysOld > 1)
					{
						return DaysOld.ToString() + " minutes ago";
					}
					else
					{
						return "1 minute ago";
					}

				}
				else if (DaysOld == 1)
				{
					return DaysOld.ToString() + " hour ago";
				}
				else
				{
					return DaysOld.ToString() + " hours ago";
				}
			}
			else
			{
				return DaysOld.ToString() + " days ago";
			}
		}

        /// <summary>
        /// Compresses a given stream.
        /// </summary>
        /// <param name="DataStream">The stream to be compressed.</param>
        /// <returns>A MemoryStream containing the compressed data on success, null on failure.</returns>
        public static MemoryStream Zip(Stream DataStream)
        {
            // Read all the data from the stream.
            byte[] Data = new byte[DataStream.Length];
            int Count = DataStream.Read(Data, 0, Data.Length);

            // Make sure the read succeeded.
            if (Count != Data.Length)
            {
                return null;
            }

            // Make a new MemoryStream.
            MemoryStream ZippedDataStream = new MemoryStream();

            // Create a GZipStream around the MemoryStream.
            GZipStream DataZipper = new GZipStream(ZippedDataStream, CompressionMode.Compress);

            // Compress the data into the MemoryStream using the GZipStream.
            DataZipper.Write(Data, 0, Data.Length);

            // Return the MemoryStream containing the compressed data.
            return ZippedDataStream;
        }

		/// <summary>
		/// Decompresses a given stream.
		/// </summary>
		/// <param name="DataStream">The stream to be decompressed.</param>
		/// <returns>A byte array containing the decompressed data on success, null on failure.</returns>
		public static byte[] Unzip(Stream zippedData, long unzippedByteSize)
		{
			// Make a new MemoryStream.
			byte[] unzippedData = new byte[unzippedByteSize];

			// Create a GZipStream around the MemoryStream.
			GZipStream DataUnzipper = new GZipStream(zippedData, CompressionMode.Decompress);

			// Decompress the data into the MemoryStream using the GZipStream.
			DataUnzipper.Read(unzippedData, 0, unzippedData.Length);

			// Return the MemoryStream containing the decompressed data.
			return unzippedData;
		}

        /// <summary>
        /// Uses a guid to obtain a unique number.
        /// </summary>
        /// <returns></returns>
        public static double GuidRandom()
        {
            double result;
            bool done;
            string bytestring = string.Empty;

            while(true)
            {
                // Get a Guid.
                Guid guid = Guid.NewGuid();

                // Convert it to a byte array.
                Byte[] bytes = guid.ToByteArray();

                // Make a string out of the bytes.
                for (int j = 0; j < bytes.Length; j++)
                {
                    bytestring = bytestring + bytes[j];
                }

                // Try and parse the string.
                done = double.TryParse(bytestring, out result);

                // Return if it succeeded, try again if not.
                if (done)
                {
                    return result;
                }
            }
        }

		/// <summary>
		/// Gets the QueryString from a Url
		/// </summary>
		/// <param name="Url">The Url you wish to get the querystring from</param>
		/// <returns>The Querystring in string format</returns>
		/// <example>
		/// 
		/// string QueryString = GetQueryString("http://www.apption.com/index.html?myparam=1&amp;myotherparam=2"); <br/>
		/// 
		/// Console.Write(QueryString);  //Value is myparam=1&amp;myotherparam=2
		/// 
		/// </example>
		public static string GetQueryString(string Url)
		{
			string[] UrlParts = Url.Split('/');
			string QueryString = UrlParts[UrlParts.Length - 1];

			int QueryStringFind = QueryString.IndexOf("?");

			if (QueryStringFind != -1)
			{
				QueryString = QueryString.Substring(QueryStringFind + 1);

				return QueryString;
			}

			return string.Empty;
		}

		/// <summary>
		/// Gets the value of a parameter name from a Url Querystring
		/// </summary>
		/// <param name="Url">The Url which contains the parameter you want to retrieve.</param>
		/// <param name="ParameterName">The name of the parameter you want to retrieve</param>
		/// <returns>The value of the parameter or null if the paramter does not exist.</returns>
		/// <example>
		/// 
		/// string Value = GetQueryParameter("http://www.apption.com/index.html?myparam=1&amp;myotherparam=2", "myparam"); <br/>
		/// 
		/// Console.Write(Value);  //Value is 1
		/// 
		/// </example>
		public static string GetQueryParameter(string Url, string ParameterName)
		{
			string QueryString = GetQueryString(Url);

			string[] Parts = QueryString.Split('&');

			for (int i = 0; i < Parts.Length; i++)
			{
				if (Parts[i].StartsWith(ParameterName + "="))
				{
					return Parts[i].Replace(ParameterName + "=", "");
				}
			}

			return null;
		}

		/// <summary>
		/// Normalizes the string for use in a title.
		/// </summary>
		/// <param name="value">The string whose words you want to normalize.</param>
		/// <returns>The string with its words normalized.</returns>
		/// <example>
		/// 
		/// string Value = NormalizeWordsForTitle("gazaro is awesome"); <br/>
		/// 
		/// Console.Write(Value); // Gazaro Is Awesome
		/// 
		/// </example>
		public static string NormalizeWordsForTitle(string value)
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < value.Length; ++i)
			{
				if (i == 0 || value[i-1] == ' ' || value[i-1] == '-' || value[i-1] == '\"')
				{
					sb.Append(Char.ToUpper(value[i]));
				}
				else
				{
					sb.Append(value[i]);
				}
			}

			return sb.ToString();
		}

	}  
}

