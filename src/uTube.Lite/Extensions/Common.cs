using System;
using System.Net;
using System.Web;
using System.Xml;

namespace uTube.Lite.Extensions
{
	/// <summary>
	/// A class of commonly used methods to handle YouTube videos.
	/// </summary>
	public class Common
	{
		/// <summary>
		/// Gets YouTube feed API URL.
		/// </summary>
		/// <value>YouTube feed API URL.</value>
		public static string YouTubeFeedApiUrl
		{
			get
			{
				return "http://gdata.youtube.com/feeds/api/videos/{0}?v=2";
			}
		}

		/// <summary>
		/// Gets the YouTube video URL.
		/// </summary>
		/// <value>The YouTube video URL.</value>
		public static string YouTubeVideoUrl
		{
			get
			{
				return "http://www.youtube.com/watch?v={0}";
			}
		}

		/// <summary>
		/// Gets the video data.
		/// </summary>
		/// <param name="videoId">The video id.</param>
		/// <returns>
		/// Returns an XML document of the video data.
		/// </returns>
		public static XmlDocument GetVideoData(string videoId)
		{
			// create new XML document.
			var xd = new XmlDocument();

			// declare XML string - with default error message
			string xml = "<error>No VideoId passed</error>";

			// check the video Id isn't empty.
			if (!string.IsNullOrEmpty(videoId))
			{
				// format the YouTube feed URL - with the video Id.
				string url = string.Format(YouTubeFeedApiUrl, videoId);

				// create a new WebClient instance.
				using (var webClient = new WebClient())
				{
					// download the string from the URL (i.e. the XML data)
					xml = webClient.DownloadString(url);

					// quick check that the XML string is valid
					if (!xml.StartsWith("<?xml"))
					{
						xml = string.Concat("<error>", xml, " (", videoId, ")</error>");
					}
				}
			}

			// load the XML string
			xd.LoadXml(xml);

			// return the XML document.
			return xd;
		}

		/// <summary>
		/// Gets YouTube embed code.
		/// </summary>
		/// <param name="videoId">The video id.</param>
		/// <param name="videoHeight">Height of the video.</param>
		/// <param name="videoWidth">Width of the video.</param>
		/// <param name="allowFullScreen">if set to <c>true</c> [allow full screen].</param>
		/// <returns>Returns the HTML embed code for the YouTube video.</returns>
		public static string GetYouTubeEmbedCode(string videoId, double videoHeight, double videoWidth, bool allowFullScreen)
		{
			string embedCode = string.Concat(@"
<object width=""{2}"" height=""{1}"">
	<param name=""movie"" value=""http://www.youtube.com/v/{0}?fs={4}""></param>
	<param name=""allowFullScreen"" value=""{3}""></param>
	<param name=""allowscriptaccess"" value=""always""></param>
	<embed src=""http://www.youtube.com/v/{0}?fs={4}"" type=""application/x-shockwave-flash"" allowscriptaccess=""always"" allowfullscreen=""{3}"" width=""{2}"" height=""{1}""></embed>
</object>");

			return string.Format(embedCode, videoId, videoHeight, videoWidth, allowFullScreen ? "true" : "false", allowFullScreen ? 1 : 0);
		}

		/// <summary>
		/// Gets the video id from a YouTube URL.
		/// </summary>
		/// <param name="url">The YouTube video URL.</param>
		/// <returns>
		/// Returns the video ID from the YouTube URL.
		/// </returns>
		/// <example>
		/// Calling GetVideoId("http://www.youtube.com/watch?v=0wrsZog8qXg") will return "0wrsZog8qXg".
		/// </example>
		public static string GetVideoId(string url)
		{
			// check the string isn't empty.
			if (!string.IsNullOrEmpty(url))
			{
				// declare the URI variable
				Uri uri;

				// try create the URI from the URL
				if (Uri.TryCreate(url, UriKind.Absolute, out uri))
				{
					// parse the querystring segement
					var qs = HttpUtility.ParseQueryString(uri.Query);

					// check that the querystring has keys and a video Id parameter
					if (qs.HasKeys() && qs["v"] != null)
					{
						// return the video Id
						return qs["v"];
					}
				}

				// if the length of the URL is 11 characters, we assume its a video Id
				if (url.Length == 11)
				{
					// return the video Id
					return url;
				}
			}

			// if all else fails - return an empty string
			return string.Empty;
		}

		/// <summary>
		/// Gets the height of the video.
		/// </summary>
		/// <param name="videoWidth">Width of the video.</param>
		/// <param name="aspectRatio">The aspect ratio.</param>
		/// <returns>
		/// Returns the height of the video, according to the aspect ratio.
		/// </returns>
		/// <example>
		/// GetVideoHeight(1024, "widescreen") will return "576".
		/// GetVideoHeight(1024, "standard") will return "768".
		/// </example>
		public static double GetVideoHeight(double videoWidth, string aspectRatio)
		{
			double ratio = 0.75; // 4:3

			if (aspectRatio == "widescreen")
			{
				ratio = 0.5625; // 16:9
			}

			return videoWidth * ratio;
		}

		/// <summary>
		/// Gets the width of the video.
		/// </summary>
		/// <param name="videoHeight">Height of the video.</param>
		/// <param name="aspectRatio">The aspect ratio.</param>
		/// <returns>
		/// Returns the width of the video, according to the aspect ratio.
		/// </returns>
		/// <example>
		/// GetVideoWidth(576, "widescreen") will return "1024".
		/// GetVideoWidth(768, "standard") will return "1024".
		/// </example>
		public static double GetVideoWidth(double videoHeight, string aspectRatio)
		{
			double ratio = GetVideoHeight(1, aspectRatio); // get the base ratio

			return videoHeight / ratio;
		}
	}
}