﻿using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;
using System.IO;

public class DetailPanel : MonoBehaviour
{
	public bool shouldClose;

	public Text videoLength;
	public Image thumb;
	public Text title;
	public Text description;
	public Text author;
	public Text timestamp;
	public Text downloadSize;
	public VideoSerialize video;
	public Button playButton;
	public Button downloadButton;
	public Button deleteButton;
	
	private WWW imageDownload;
	private float time;
	private const float refreshTime = 1.0f;

	void Update()
	{
		if (imageDownload != null)
		{
			if (imageDownload.error != null)
			{
				Debug.Log("Failed to download thumbnail: " + imageDownload.error);
				imageDownload.Dispose();
				imageDownload = null;
			}
			else if (imageDownload.isDone)
			{
				thumb.sprite = Sprite.Create(imageDownload.texture, new Rect(0, 0, imageDownload.texture.width, imageDownload.texture.height), new Vector2(0.5f, 0.5f));
				imageDownload.Dispose();
				imageDownload = null;
				thumb.color = Color.white;
			}
		}

		time += Time.deltaTime;
		if (time > refreshTime)
		{
			Refresh();
			time = 0;
		}
	}

	public void Init(VideoSerialize videoToDownload)
	{
		video = videoToDownload;

		videoLength.text = MathHelper.FormatSeconds(video.length);
		title.text = video.title;
		description.text = video.description;
		author.text = video.username;
		timestamp.text = MathHelper.FormatTimestampToTimeAgo(video.realTimestamp);
		downloadSize.text = MathHelper.FormatBytes(video.downloadsize);

		imageDownload = new WWW(Web.thumbnailUrl + "/" + Encoding.UTF8.GetString(Convert.FromBase64String(video.uuid)) + ".jpg");
		Refresh();
	}
	
	public void Refresh()
	{
		bool downloaded = Directory.Exists(Path.Combine(Application.persistentDataPath, video.uuid));
		downloadButton.gameObject.SetActive(!downloaded);
		playButton.gameObject.SetActive(downloaded);
		deleteButton.gameObject.SetActive(downloaded);
	}

	public void Back()
	{
		shouldClose = true;
	}

	public void Play()
	{
	}

	public void Delete()
	{
		string path = Path.Combine(Application.persistentDataPath, video.uuid);
		bool downloaded = Directory.Exists(path);
		if (downloaded)
		{
			Directory.Delete(path, true);
		}
		Refresh();
	}

	public void Download()
	{
		VideoDownloadManager.Main.AddDownload(video);
	}
}