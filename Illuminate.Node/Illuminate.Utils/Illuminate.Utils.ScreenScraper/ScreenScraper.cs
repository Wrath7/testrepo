using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO.Compression;

namespace Illuminate.Utils.ScreenScraper
{
	/// <summary>
	/// Class used to crawl a web page
	/// </summary>
	public class HttpCrawler : Illuminate.Core.IlluminateObject
	{
		#region Protected Properties

		protected int _timeout = 20000;

		/// <summary>
		/// The encoding page type for the Url you are crawling.  
		/// </summary>
		protected Encoding _crawlerEncoder = Encoding.UTF8;

		#endregion

		#region Public Properties

		public int Timeout
		{
			get
			{
				return _timeout;
			}
			set
			{
				_timeout = value;
			}
		}

		#endregion

		#region Constructor

		/// <summary>
		/// HttpCrawler Constructor
		/// </summary>
		public HttpCrawler(Encoding CrawlerEncoder)
		{
			_crawlerEncoder = CrawlerEncoder;
		}

		public HttpCrawler()
		{
		}

		#endregion

		#region Main Methods

		/// <summary>
		/// Gets the HTML content of a web page
		/// </summary>
		/// <param name="Url">The Url you wish to retrieve the HTML from</param>
		/// <returns>The HTML of the web page</returns>
		/// <example>
		/// 
		/// HttpCrawler crawler = new HttpCrawler(); <br/>
		/// string Content = crawler.Scrape("http://www.gazaro.com");
		/// 
		/// </example>
		public Response Scrape(string Url)
		{
			Response returnRsp = new Response();
			HttpWebRequest request;
			HttpWebResponse response;

			try
			{
				request = (HttpWebRequest)WebRequest.Create(Url);

				#region Set Header Details

				request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
				request.Accept = "*/*";
				request.KeepAlive = false;
				request.Timeout = _timeout;
				request.UserAgent = "Gazaro-Crawler";
				request.AllowAutoRedirect = true;

				#endregion

				response = (HttpWebResponse)request.GetResponse();
				MemoryStream ms = GetDataStream(response);

				returnRsp.Content = System.Text.Encoding.UTF8.GetString(ms.ToArray());
				returnRsp.BinaryData = ms.ToArray();
				returnRsp.ReturnCode = (int)response.StatusCode;
				returnRsp.StatusMessage = response.StatusDescription;

				foreach (string header in response.Headers.Keys)
				{
					returnRsp.Headers.Add(header, response.Headers[header]);
				}
			}
			catch (WebException e)
			{
				response = (HttpWebResponse)e.Response;

				if (e.Response != null)
				{
					foreach (string header in response.Headers.Keys)
					{
						returnRsp.Headers.Add(header, response.Headers[header]);
					}
				}

				if (e.Status.ToString() == "ProtocolError")
				{
					returnRsp.StatusMessage = response.StatusDescription;
					returnRsp.ReturnCode = (int)response.StatusCode;
					
					response.Close();
					response = null;
				}
				else
				{
					int status = (int)e.Status;
					switch (status)
					{
						case 1:
							returnRsp.ReturnCode = 501;
							break;
						case 2:
							returnRsp.ReturnCode = 501;
							break;
						case 14:
							returnRsp.ReturnCode = 408;
							break;
						default:
							returnRsp.ReturnCode = 500;
							break;
					}
				}
			}
			catch (Exception e)
			{
				returnRsp.StatusMessage = e.Message;
			}

			return returnRsp;
		}

		private MemoryStream GetDataStream(HttpWebResponse response)
		{
			Stream stream = response.GetResponseStream();

			#region Deflate the Stream

			if (response.Headers["Content-Encoding"] != null)
			{
				if (response.Headers["Content-Encoding"].ToLower().Contains("gzip"))
				{
					stream = new GZipStream(stream, CompressionMode.Decompress);
				}
				else if (response.Headers["Content-Encoding"].ToLower().Contains("deflate"))
				{
					stream = new DeflateStream(stream, CompressionMode.Decompress);
				}
			}

			#endregion

			byte[] buffer = new byte[1024];
			MemoryStream ms = new MemoryStream();
			int bytesRead = 0;

			while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
			{
				ms.Write(buffer, 0, bytesRead);
			}

			return ms;
		}

		#endregion
	}
}
