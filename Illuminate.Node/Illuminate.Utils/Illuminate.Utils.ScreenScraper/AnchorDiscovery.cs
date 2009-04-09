using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Illuminate.Utils.ScreenScraper
{
	public class AnchorParser
	{
		Regex anchorRegex = new Regex("<a(.+?)href=(\"|')(.+?)(\"|')(>|(.+?)>)", RegexOptions.IgnoreCase);

		public List<string> Parse(string url, string htmlContent)
		{
			List<string> urlList = new List<string>();

			string rootUrl = Regex.Match(url, "http://(.+?)/", RegexOptions.IgnoreCase).Value;

			if (rootUrl.Length == 0) rootUrl = url;
			if (!rootUrl.EndsWith("/")) rootUrl = rootUrl + "/";

			if (!url.EndsWith("/"))
			{
				if (url.LastIndexOf("/") > 6)
				{
					string lastSection = url.Substring(url.LastIndexOf("/"));
					if (lastSection.Contains("."))
					{
						url = url.Substring(0, url.LastIndexOf("/") + 1);
					}
					else
					{
						url = url + "/";
					}
				}
				else
				{
					url = url + "/";
				}				
			}

			MatchCollection anchorMatches = anchorRegex.Matches(htmlContent);

			foreach (Match m in anchorMatches)
			{
				string anchorText = m.Value;
				anchorText = Regex.Replace(anchorText, "<a(.+?)href=(\"|')", "", RegexOptions.IgnoreCase);
				anchorText = Regex.Replace(anchorText, "(\"|')(>|(.+?)>)", "", RegexOptions.IgnoreCase);

				string link = anchorText;
				link = System.Web.HttpUtility.HtmlDecode(link);

				if (!link.StartsWith("http://"))
				{
					if (link.StartsWith("/"))
					{
						link = link.Substring(1);

						link = rootUrl + link;
					}
					else if (link.StartsWith(".."))
					{
						string urlPath = url;

						if (!urlPath.EndsWith("/"))
							urlPath = Regex.Match(urlPath, "http://(.+)/", RegexOptions.IgnoreCase).Value;

						int relativeIndex = link.IndexOf("../");

						while (relativeIndex != -1)
						{
							string[] urlParts = urlPath.Split('/');

							urlPath = string.Empty;

							for (int i = 0; i < urlParts.Length - 2; i++)
							{
								urlPath = urlPath + urlParts[i] + "/";
							}

							link = link.Substring(3);

							relativeIndex = link.IndexOf("../");
						}

						if (url.StartsWith("http://"))
							link = urlPath + link;
						else
							continue;
					}
					else
					{
						link = url + link;
					}

				}

				if (!urlList.Contains(link)) urlList.Add(link);
			}

			return urlList;
		}
	}
}
