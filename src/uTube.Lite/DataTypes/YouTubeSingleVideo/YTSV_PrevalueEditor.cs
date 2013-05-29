using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using uTube.Lite.Extensions;
using umbraco.editorControls;
using umbraco.cms.businesslogic.datatype;

namespace uTube.Lite.DataTypes.YouTubeSingleVideo
{
	public class YTSV_PrevalueEditor : AbstractJsonPrevalueEditor
	{
		private CheckBox cbEnablePreview;

		private CheckBox cbSaveVideoData;

		private TextBox txtPreviewHeight;

		private TextBox txtPreviewWidth;

		public YTSV_PrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
			: base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Ntext)
		{
		}

		public override void Save()
		{
			// prep the option values
			int previewHeight = 240;
			int previewWidth = 320;
			int.TryParse(this.txtPreviewHeight.Text, out previewHeight);
			int.TryParse(this.txtPreviewWidth.Text, out previewWidth);

			// set the options
			var options = new YTSV_Options()
			{
				EnablePreview = this.cbEnablePreview.Checked,
				PreviewHeight = previewHeight,
				PreviewWidth = previewWidth,
				SaveVideoData = this.cbSaveVideoData.Checked
			};

			// save the options as JSON
			this.SaveAsJson(options);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			// set-up child controls
			this.cbEnablePreview = new CheckBox() { ID = "cbEnablePreview" };
			this.txtPreviewHeight = new TextBox() { ID = "txtPreviewHeight", CssClass = "guiInputText", Width = 45 };
			this.txtPreviewWidth = new TextBox() { ID = "txtPreviewWidth", CssClass = "guiInputText", Width = 45 };
			this.cbSaveVideoData = new CheckBox() { ID = "cbSaveVideoData" };

			// populate option controls

			// add the child controls
			this.Controls.AddPrevalueControls(this.cbEnablePreview, this.txtPreviewHeight, this.txtPreviewWidth, this.cbSaveVideoData);
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			// Adds the client dependencies.
			this.RegisterEmbeddedClientResource("uTube.Lite.Resources.Styles.PrevalueEditor.css", ClientDependencyType.Css);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// get PreValues, load them into the controls.
			var options = this.GetPreValueOptions<YTSV_Options>();

			// if the options are null, then load the defaults
			if (options == null)
			{
				options = new YTSV_Options(true);
			}

			// set the values
			this.cbEnablePreview.Checked = options.EnablePreview;
			this.txtPreviewHeight.Text = options.PreviewHeight.ToString();
			this.txtPreviewWidth.Text = options.PreviewWidth.ToString();
			this.cbSaveVideoData.Checked = options.SaveVideoData;
		}

		public override void RenderBeginTag(HtmlTextWriter writer)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "uTube");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			base.RenderBeginTag(writer);
		}

		protected override void RenderContents(HtmlTextWriter writer)
		{
			// add data fields
			writer.AddPrevalueRow("Save Video Data:", "Enabling this option, the data-type to store the raw XML video data from YouTube. Disabling it will only store the video id.", this.cbSaveVideoData);

			// add preview fields
			writer.AddPrevalueHeading("Preview Options");
			writer.AddPrevalueRow("Enable preview:", this.cbEnablePreview);
			writer.AddPrevalueRow("Preview height:", this.txtPreviewHeight);
			writer.AddPrevalueRow("Preview width:", this.txtPreviewWidth);
		}

		public override void RenderEndTag(HtmlTextWriter writer)
		{
			base.RenderEndTag(writer);

			writer.RenderEndTag();
		}
	}
}
