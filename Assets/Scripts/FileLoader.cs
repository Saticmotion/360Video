﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class FileLoader : MonoBehaviour 
{
	public GameObject video360;
	public GameObject video180;
	public GameObject video;
	public GameObject image360;
	public GameObject image180;
	public GameObject imageFlat;

	public GameObject camera360;
	public GameObject camera180;
	public GameObject cameraFlat;

	public GameObject playerInfoGUI;

	public FileType fileType = FileType.Video360;


	public enum FileType {
		Video360,
		Video180,
		Video,
		Image360,
		Image180,
		Image
	}

	void Start () 
	{
		if (!video360)	{ Debug.LogError(string.Format("Hey you forgot to hook up Video360 to the FileLoader script at {0}",	name)); }
		if (!video180)	{ Debug.LogError(string.Format("Hey you forgot to hook up Video180 to the FileLoader script at {0}",	name)); }
		if (!video)		{ Debug.LogError(string.Format("Hey you forgot to hook up Video to the FileLoader script at {0}",		name)); }
		if (!image360)	{ Debug.LogError(string.Format("Hey you forgot to hook up Image360 to the FileLoader script at {0}",	name)); }
		if (!image180)	{ Debug.LogError(string.Format("Hey you forgot to hook up Image180 to the FileLoader script at {0}",	name)); }
		if (!imageFlat)	{ Debug.LogError(string.Format("Hey you forgot to hook up Image to the FileLoader script at {0}",		name)); }
		if (!cameraFlat){ Debug.LogError(string.Format("Hey you forgot to hook up a Camera to the FileLoader script at {0}",	name)); }

		var fileName = @"C:\Users\20003613\Documents\Git\360video\Assets\Resources\video2.mp4";
		//var fileName = @"http://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_640x360.m4v";

		GameObject newCamera = null;
		GameObject videoPlayer = null;

		switch (fileType)
		{
			case FileType.Image360:
			{
				Instantiate(camera360);
				break;
			}
			case FileType.Image180:
			{
				Instantiate(camera180);
				break;
			}
			case FileType.Image:
			{
				Instantiate(cameraFlat);
				break;
			}



			case FileType.Video360:
			{
				Instantiate(camera360);
				videoPlayer = Instantiate(video360);
				break;
			}
			case FileType.Video180:
			{
				Instantiate(camera180);
				videoPlayer = Instantiate(video180);
				break;
			}
			case FileType.Video:
			{
				Instantiate(cameraFlat);
				videoPlayer = Instantiate(video);
				break;
			}



			default:
				throw new ArgumentOutOfRangeException();
		}

		if (fileType == FileType.Video || fileType == FileType.Video180 || fileType == FileType.Video360)
		{
			var player = videoPlayer.GetComponent<VideoPlayer>();
			player.url = fileName;
			player.waitForFirstFrame = true;
			player.Play();

			var playerInfo = Instantiate(playerInfoGUI);
			playerInfo.transform.SetParent(Canvass.main.transform, false);

			var seekbar = playerInfo.GetComponentInChildren<Seekbar>();
			var controller = videoPlayer.GetComponent<VideoController>();
			seekbar.controller = controller;
			controller.seekbar = seekbar.transform.GetChild(0).GetComponent<RectTransform>();
			controller.timeText = seekbar.transform.parent.GetComponentInChildren<Text>();
		}
	}
	
	void Update () 
	{
		
	}
}
