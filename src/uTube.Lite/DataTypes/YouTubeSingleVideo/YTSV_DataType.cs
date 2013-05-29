using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using umbraco.cms.businesslogic.datatype;
using umbraco.editorControls;
using umbraco.interfaces;
using Umbraco.Core;
using uTube.Lite.Extensions;

namespace uTube.Lite.DataTypes.YouTubeSingleVideo
{
	public class YTSV_DataType : AbstractDataEditor
	{
		public const string DataTypeGuid = "8032E957-5D03-4AC0-8C17-CC045928FB49";

		private YTSV_Control m_Control = new YTSV_Control();

		private XmlData m_Data;

		private YTSV_PrevalueEditor m_PreValueEditor;

		public YTSV_DataType()
			: base()
		{
			// set the render control as the placeholder
			this.RenderControl = this.m_Control;

			// assign the initialise event for the placeholder
			this.m_Control.Init += new EventHandler(this.m_Control_Init);

			// assign the save event for the data-type/editor
			this.DataEditorControl.OnSave += new AbstractDataEditorControl.SaveEventHandler(this.DataEditorControl_OnSave);
		}

		public override string DataTypeName
		{
			get
			{
				return "uTube: YouTube Single Video";
			}
		}

		public override Guid Id
		{
			get
			{
				return new Guid(DataTypeGuid);
			}
		}

		public override IData Data
		{
			get
			{
				if (this.m_Data == null)
				{
					this.m_Data = new XmlData(this);
				}

				return this.m_Data;
			}
		}

		public override IDataPrevalue PrevalueEditor
		{
			get
			{
				if (this.m_PreValueEditor == null)
				{
					this.m_PreValueEditor = new YTSV_PrevalueEditor(this);
				}

				return this.m_PreValueEditor;
			}
		}

		private void m_Control_Init(object sender, EventArgs e)
		{
			// get the render control options from the Prevalue Editor.
			var options = ((YTSV_PrevalueEditor)this.PrevalueEditor).GetPreValueOptions<YTSV_Options>();

			if (this.Data.Value != null)
			{
				var value = this.Data.Value.ToString();

				// check if the value is XML... load and select accordingly
				if (XmlHelper.CouldItBeXml(value))
				{
					using (var stream = new StringReader(value))
					{
						var doc = new XPathDocument(stream);
						var nav = doc.CreateNavigator();
						var nsmgr = new XmlNamespaceManager(nav.NameTable);

						nsmgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
						nsmgr.AddNamespace("media", "http://search.yahoo.com/mrss/");
						nsmgr.AddNamespace("yt", "http://gdata.youtube.com/schemas/2007");

						// [LK] the `/value` XPath adds support for anyone switching from UMP provider
						var node = nav.SelectSingleNode("/atom:entry/media:group/yt:videoid | /value", nsmgr);
						if (node != null)
						{
							value = node.Value;
						}
					}
				}

				// assign the video id value
				this.m_Control.VideoId = value;
			}

			// set the render control options
			this.m_Control.Options = options;
		}

		private void DataEditorControl_OnSave(EventArgs e)
		{
			if (this.m_Control.Options.SaveVideoData && !string.IsNullOrEmpty(this.m_Control.VideoId))
			{
				// save the raw video (XML) data
				var data = Common.GetVideoData(this.m_Control.VideoId);
				if (data != null)
				{
					this.Data.Value = data.OuterXml;
					return;
				}
			}

			// save the value of the control
			this.Data.Value = this.m_Control.VideoId;
		}
	}
}