using System;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using umbraco;
using uTube.Lite.Extensions;

namespace uTube.Lite.XsltExtensions
{
	[XsltExtension("utube.lite")]
	public class Library
	{
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
			return Common.GetVideoId(url);
		}

		/// <summary>
		/// Gets data about the YouTube video.
		/// </summary>
		/// <param name="videoId">The YouTube video id.</param>
		/// <param name="cacheInSeconds">The cache in seconds.</param>
		/// <returns>Returns data about the video.</returns>
		public static XPathNodeIterator GetVideoData(string videoId, int cacheInSeconds)
		{
			if (!string.IsNullOrEmpty(videoId))
			{
				var cacheKey = string.Concat("uTube_GetVideoData_", videoId);

				// attempt to get from cache
				object obj = HttpContext.Current.Cache.Get(cacheKey);
				if (obj != null)
				{
					return (XPathNodeIterator)obj;
				}

				// if not in cache, get data from source.
				var data = Common.GetVideoData(videoId);
				if (data != null)
				{
					XPathNodeIterator output = data.CreateNavigator().Select("/");

					// add to cache
					HttpContext.Current.Cache.Insert(cacheKey, output, null, DateTime.Now.Add(new TimeSpan(0, 0, cacheInSeconds)), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Low, null);

					return output;
				}
			}

			// fallback - return an error message
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<error>No VideoId passed</error>");
			return doc.CreateNavigator().Select("/error");
		}

		/// <summary>
		/// Gets data about the YouTube video.
		/// </summary>
		/// <param name="videoId">The YouTube video id.</param>
		/// <returns>Returns data about the video.</returns>
		public static XPathNodeIterator GetVideoData(string videoId)
		{
			return GetVideoData(videoId, 600);
		}

		/// <summary>
		/// Gets YouTube embed code.
		/// </summary>
		/// <param name="videoId">The video id.</param>
		/// <returns></returns>
		public static string GetYouTubeEmbedCode(string videoId)
		{
			return GetYouTubeEmbedCode(videoId, 385, 480);
		}

		/// <summary>
		/// Gets YouTube embed code.
		/// </summary>
		/// <param name="videoId">The video id.</param>
		/// <param name="videoHeight">Height of the video.</param>
		/// <param name="videoWidth">Width of the video.</param>
		/// <returns></returns>
		public static string GetYouTubeEmbedCode(string videoId, int videoHeight, int videoWidth)
		{
			return Common.GetYouTubeEmbedCode(videoId, videoHeight, videoWidth, true);
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
		public static double GetVideoHeight(int videoWidth, string aspectRatio)
		{
			return Common.GetVideoHeight(videoWidth, aspectRatio);
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
		public static double GetVideoWidth(int videoHeight, string aspectRatio)
		{
			return Common.GetVideoWidth(videoHeight, aspectRatio);
		}

		/// <summary>
		/// Checks to see if the videos is allowed to be embedded.
		/// </summary>
		/// <param name="videoId">The YouTube video id.</param>
		/// <returns>
		/// Returns boolean (true/false) if the video can be embedded
		/// </returns>
		/// <example>
		/// Calling AllowEmbed("0wrsZog8qXg") will return "true".
		/// </example>
		public bool AllowEmbed(string videoId)
		{
			// get the video data
			var data = Common.GetVideoData(videoId);

			// check there is video data
			if (data != null)
			{
				// create a namespace manager
				var nsmgr = new XmlNamespaceManager(data.NameTable);

				// add the YouTube schema namespace
				nsmgr.AddNamespace("atom",  "http://www.w3.org/2005/Atom");
				nsmgr.AddNamespace("yt",    "http://gdata.youtube.com/schemas/2007");

				// select the 'accessControl' node that specifies the embed permission.
				var isEmbeddable = data.SelectSingleNode("/atom:entry/yt:accessControl[@action='embed' and @permission='allowed']", nsmgr);

				// check that the node isn't null.
				if (isEmbeddable != null)
				{
					// the node exists - return true!
					return true;
				}
			}

			// all else fails - return false!
			return false;
		}

		/// <summary>
		/// Gets the aspect ratio of the video
		/// </summary>
		/// <param name="videoId">The video id.</param>
		/// <returns>Returns the Aspect Ratio of the video</returns>
		/// <example>
		/// GetAspectRatio("66TuSJo4dZM") will return "widescreen".
		/// </example>
		public string GetAspectRatio(string videoId)
		{
			// get the video data
			var data = Common.GetVideoData(videoId);

			// check there is video data
			if (data != null)
			{
				// create a namespace manager
				var nsmgr = new XmlNamespaceManager(data.NameTable);

				// add the Media and YouTube schema namespaces
				nsmgr.AddNamespace("atom",  "http://www.w3.org/2005/Atom");
				nsmgr.AddNamespace("media", "http://search.yahoo.com/mrss/");
				nsmgr.AddNamespace("yt",    "http://gdata.youtube.com/schemas/2007");

				// select the 'aspectRatio' node.
				var aspectRatio = data.SelectSingleNode("/atom:entry/media:group/yt:aspectRatio", nsmgr);

				// The node only appears if the value is 'widescreen' ie 16:9 - Check node exists
				if (aspectRatio != null)
				{
					// the node exists - return the value!
					return aspectRatio.InnerText;
				}
			}

			// all else fails - return 'standard' ie 4:3
			return "standard";
		}
	}
}
