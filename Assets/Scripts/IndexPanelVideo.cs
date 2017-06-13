﻿using UnityEngine;
using UnityEngine.UI;

public class IndexPanelVideo : MonoBehaviour 
{
	public Text titleText;
	public Text authorText;
	public Text sizeText;
	public Text lengthText;
	public Image thumbnailImage;

	private WWW imageDownload;

	public void SetData(string title = null, string author = null, string thumbnailURL = null, int sizeBytes = 0, int lengthSeconds = 0)
	{

		if (title != null)
		{
			titleText.text = title;
		}
		if (author != null)
		{
			authorText.text = author;
		}
		if (thumbnailURL != null)
		{
			imageDownload = new WWW(Web.thumbnailUrl);
		}
		if (sizeBytes > 0)
		{
			titleText.text = title;
		}
		if (lengthSeconds > 0)
		{
			titleText.text = title;
		}
	}

	public void Update()
	{
		if (imageDownload != null)
		{
			if (imageDownload.isDone)
			{
				thumbnailImage.sprite = Sprite.Create(imageDownload.texture, new Rect(0, 0, imageDownload.texture.width, imageDownload.texture.height), new Vector2(0.5f, 0.5f));
				imageDownload.Dispose();
				imageDownload = null;
			}
		}
	}
}
