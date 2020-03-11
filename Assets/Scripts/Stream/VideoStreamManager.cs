using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VideoQuality
{
	x2160p=2160,
	x1440p=1440,
	x1080p=1080
}

public class VideoSettings
{
	public string url;
	public string videoId;
	public VideoQuality quality = VideoQuality.x2160p;
}

public class VideoStreamManager
{
	public VideoSettings settings;

	public string GetUrl(string videoId, VideoQuality quality)
	{
		int value = (int) quality;
		return $"https://localhost:5001/stream?videoid={videoId}&quality={value}";
	}

}
