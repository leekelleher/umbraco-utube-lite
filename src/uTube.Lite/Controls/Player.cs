using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using uTube.Lite.Extensions;

namespace uTube.Lite.Controls
{
	/// <summary>
	/// A control to embed a YouTube video into a HTML page.
	/// </summary>
	[DefaultProperty("VideoId")]
	[ToolboxData("<{0}:Player runat=server></{0}:Player>")]
	public class Player : WebControl
	{
		/// <summary>
		/// Gets or sets a value indicating whether [allow full screen].
		/// </summary>
		/// <value><c>true</c> if [allow full screen]; otherwise, <c>false</c>.</value>
		[Bindable(true)]
		[Category("Apperance")]
		[DefaultValue(true)]
		[Description("Allows embedded video player to be full-screen")]
		public bool AllowFullScreen { get; set; }

		/// <summary>
		/// Gets or sets the aspect ratio.
		/// </summary>
		/// <value>The aspect ratio.</value>
		/// <remarks>
		/// The values are "standard" (4:3) and "widescreen" (16:9).
		/// </remarks>
		[Bindable(true)]
		[Category("Apperance")]
		[DefaultValue("widescreen")]
		[Description("Sets the aspect ratio of the video")]
		public string AspectRatio { get; set; }

		/// <summary>
		/// Gets or sets the height of the Web server control.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// A <see cref="T:System.Web.UI.WebControls.Unit"/> that represents the height of the control. The default is <see cref="F:System.Web.UI.WebControls.Unit.Empty"/>.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">
		/// The height was set to a negative value.
		/// </exception>
		[Bindable(true)]
		[Category("Apperance")]
		[DefaultValue("385")]
		[Description("The height of the embedded video player")]
		public string VideoHeight
		{
			get
			{
				return this.Height.Value.ToString();
			}

			set
			{
				int height;
				if (int.TryParse(value, out height))
				{
					this.Height = Unit.Pixel(height);
				}
			}
		}

		/// <summary>
		/// Gets or sets the video id.
		/// </summary>
		/// <value>The video id.</value>
		[Bindable(true)]
		[Category("Data")]
		[DefaultValue("")]
		[Description("The Id of the YouTube video")]
		public string VideoId { get; set; }

		/// <summary>
		/// Gets or sets the video URL.
		/// </summary>
		/// <value>The video URL.</value>
		[Bindable(true)]
		[Category("Data")]
		[DefaultValue("")]
		[Description("The Url of YouTube video")]
		public string VideoUrl
		{
			get
			{
				return string.Format(Common.YouTubeVideoUrl, this.VideoId);
			}

			set
			{
				this.VideoId = Common.GetVideoId(value);
			}
		}

		/// <summary>
		/// Gets or sets the width of the Web server control.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// A <see cref="T:System.Web.UI.WebControls.Unit"/> that represents the width of the control. The default is <see cref="F:System.Web.UI.WebControls.Unit.Empty"/>.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">
		/// The width of the Web server control was set to a negative value.
		/// </exception>
		[Bindable(true)]
		[Category("Apperance")]
		[DefaultValue("480")]
		[Description("The width of the embedded video player")]
		public string VideoWidth
		{
			get
			{
				return this.Width.Value.ToString();
			}

			set
			{
				int width;
				if (int.TryParse(value, out width))
				{
					this.Width = Unit.Pixel(width);
				}
			}
		}

		/// <summary>
		/// Renders the HTML opening tag of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		public override void RenderBeginTag(HtmlTextWriter writer)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "uTube uTubeClassicPlayer");
			writer.RenderBeginTag(HtmlTextWriterTag.Div); //// start 'uTube uTubeClassicPlayer'

			////base.RenderBeginTag(writer);
		}

		/// <summary>
		/// Renders the HTML closing tag of the control into the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		public override void RenderEndTag(HtmlTextWriter writer)
		{
			////base.RenderEndTag(writer);

			writer.RenderEndTag(); //// end 'uTube uTubeClassicPlayer'
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// set the default video height & width
			if ((string.IsNullOrEmpty(this.VideoHeight) || this.VideoHeight == "0") && (string.IsNullOrEmpty(this.VideoWidth) || this.VideoWidth == "0"))
			{
				this.VideoHeight = "385";
				this.VideoWidth = "480";
			}

			// set the default aspect ratio
			if (string.IsNullOrEmpty(this.AspectRatio))
			{
				this.AspectRatio = "standard";
			}

			// if the width it available, but the height is not, then calculate the height.
			if ((!this.Width.IsEmpty && this.Width.Value > 0) && (this.Height.IsEmpty && this.Height.Value < 1))
			{
				int height = (int)Common.GetVideoHeight(this.Width.Value, this.AspectRatio.ToLower());
				this.Height = Unit.Pixel(height);
			}

			// if the height it available, but the width is not, then calculate the width.
			if ((!this.Height.IsEmpty && this.Height.Value > 0) && (this.Width.IsEmpty && this.Width.Value < 1))
			{
				int width = (int)Common.GetVideoWidth(this.Height.Value, this.AspectRatio.ToLower());
				this.Width = Unit.Pixel(width);
			}
		}

		/// <summary>
		/// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			if (DesignMode)
			{
				writer.Write("uTube: YouTube Player");
			}
			else
			{
				string embedCode = Common.GetYouTubeEmbedCode(this.VideoId, this.Height.Value, this.Width.Value, this.AllowFullScreen);

				writer.Write(embedCode);
			}
		}
	}
}