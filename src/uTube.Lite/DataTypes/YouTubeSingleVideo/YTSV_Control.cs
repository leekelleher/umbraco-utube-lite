using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.cms.businesslogic.datatype;
using umbraco.editorControls;
using uTube.Lite.Controls;

[assembly: WebResource("uTube.Lite.DataTypes.YouTubeSingleVideo.YTSV_Styles.css", "text/css")]
[assembly: WebResource("uTube.Lite.DataTypes.YouTubeSingleVideo.YTSV_Scripts.js", "application/javascript")]

namespace uTube.Lite.DataTypes.YouTubeSingleVideo
{
	public class YTSV_Control : PlaceHolder
	{
		public YTSV_Options Options { get; set; }

		public Player VideoPlayer { get; set; }

		public string VideoId
		{
			get
			{
				return this.VideoIdTextBox.Text.Trim();
			}

			set
			{
				if (this.VideoIdTextBox == null)
				{
					this.VideoIdTextBox = new TextBox();
				}

				this.VideoIdTextBox.Text = value;
			}
		}

		public TextBox VideoIdTextBox { get; set; }

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.EnsureChildControls();

			// Adds the client dependencies.
			this.RegisterEmbeddedClientResource("uTube.Lite.DataTypes.YouTubeSingleVideo.YTSV_Styles.css", ClientDependencyType.Css);
			this.RegisterEmbeddedClientResource("uTube.Lite.DataTypes.YouTubeSingleVideo.YTSV_Scripts.js", ClientDependencyType.Javascript);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// enable video preview?
			if (this.Options.EnablePreview && this.VideoId != null && this.VideoId.Length == 11)
			{
				this.VideoPlayer.VideoId = this.VideoId;
				this.VideoPlayer.Height = Unit.Pixel(this.Options.PreviewHeight);
				this.VideoPlayer.Width = Unit.Pixel(this.Options.PreviewWidth);
				this.VideoPlayer.Visible = true;
			}
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			this.EnsureChildControls();

			// construct the child controls
			if (this.VideoIdTextBox == null)
			{
				this.VideoIdTextBox = new TextBox();
			}

			this.VideoIdTextBox.ID = this.VideoIdTextBox.ClientID;
			this.VideoIdTextBox.CssClass = "umbEditorTextField";

			this.VideoPlayer = new Player() { Visible = false };

			// add the child controls
			this.Controls.AddPrevalueControls(this.VideoIdTextBox, this.VideoPlayer);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "YTSV");
			writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			// render Video URL text-box
			this.VideoIdTextBox.RenderControl(writer);

			// render the Video Preview box
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "YouTubePreview");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			this.VideoPlayer.RenderControl(writer);
			writer.RenderEndTag(); // .YouTubePreview

			writer.RenderEndTag(); // .YTSV

			// enable preview?
			if (this.Options.EnablePreview)
			{
				// add jquery window load event to create the ToggleBox
				var javascriptMethod = string.Format("jQuery('#{0}').YouTubeVideoPreview({1}, {2});", this.ClientID, this.Options.PreviewHeight, this.Options.PreviewWidth);
				var javascript = string.Concat("<script type='text/javascript'>jQuery(window).load(function(){ ", javascriptMethod, " });</script>");
				writer.WriteLine(javascript);
			}
		}
	}
}