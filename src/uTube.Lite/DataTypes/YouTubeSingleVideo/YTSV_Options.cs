using System;
using System.ComponentModel;
using umbraco.editorControls;

namespace uTube.Lite.DataTypes.YouTubeSingleVideo
{
	/// <summary>
	/// Options for the YouTube Single Video.
	/// </summary>
	public class YTSV_Options : AbstractOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="YTSV_Options"/> class.
		/// </summary>
		public YTSV_Options()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="YTSV_Options"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
		public YTSV_Options(bool loadDefaults)
			: base(loadDefaults)
		{
		}

		/// <summary>
		/// Gets or sets a value indicating whether [enable preview].
		/// </summary>
		/// <value><c>true</c> if [enable preview]; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool EnablePreview { get; set; }

		/// <summary>
		/// Gets or sets the height of the preview.
		/// </summary>
		/// <value>The height of the preview.</value>
		[DefaultValue(240)]
		public int PreviewHeight { get; set; }

		/// <summary>
		/// Gets or sets the width of the preview.
		/// </summary>
		/// <value>The width of the preview.</value>
		[DefaultValue(320)]
		public int PreviewWidth { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [save video data].
		/// </summary>
		/// <value><c>true</c> if [save video data]; otherwise, <c>false</c>.</value>
		[DefaultValue(false)]
		public bool SaveVideoData { get; set; }
	}
}
