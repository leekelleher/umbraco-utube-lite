(function ($) {
	// jquery plugin for the YouTube Video Preview
	$.fn.YouTubeVideoPreview = function (height, width) {
		var $this = $(this);
		var embed_code = '<object width="' + width + '" height="' + height + '"><param name="movie" value="http://www.youtube.com/v/{videoId}"></param><param name="allowFullScreen" value="false"></param><param name="allowscriptaccess" value="always"></param><embed src="http://www.youtube.com/v/{videoId}" type="application/x-shockwave-flash" allowscriptaccess="always" allowfullscreen="false" width="' + width + '" height="' + height + '"></embed></object>';

		$this.find('.umbEditorTextField').blur(function () {
			var value = $(this).val().trim();

			if (value != '') {
				var videoId = extractVideoID(value);

				if (videoId.length == 11) {
					var embed = embed_code.replace(/{videoId}/g, videoId);
					$this.find('.YouTubePreview').html(embed);
					$(this).val(videoId);
				}
			}
		});

		// extract the videoId from the URL
		function extractVideoID(url) {
			var youtube_id;
			youtube_id = url.replace(/^[^v]+v.(.{11}).*/, "$1");
			return youtube_id;
		}
	}
})(jQuery);