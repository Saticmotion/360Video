﻿using System;
using System.Collections;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AudioControl : MonoBehaviour
{
	public Button playButton;
	public Slider audioTimeSlider;
	public Text clipTimeText;

	private RawImage playButtonImage;
	public Texture iconPlay;
	public Texture iconPause;

	public Slider audioSlider;
	public Button lowerVolumeButton;
	public Button increaseVolumeButton;

	public AudioMixer mixer;

	private AudioSource audioSource;
	private AudioClip clip;

	private float savedAudioVolumePanel;

	private string url;
	private float fullClipLength;
	private float currentClipTime;

	void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.playOnAwake = false;
		playButtonImage = playButton.GetComponentInChildren<RawImage>();
	}

	public void Init(string url)
	{
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}

		audioSource.Stop();
		clip = null;
		this.url = url;
		StartCoroutine(GetAudioClip(url));

		if (lowerVolumeButton != null)
		{
			lowerVolumeButton.onClick.AddListener(LowerVolume);
			increaseVolumeButton.onClick.AddListener(IncreaseVolume);
		}
		if (audioSlider != null)
		{
			LoadVolume();
			audioSlider.onValueChanged.AddListener(_ => AudioValueChanged());
			mixer.SetFloat("AudioVolumePanel", CorrectVolume(savedAudioVolumePanel));
			audioSlider.value = savedAudioVolumePanel;
		}
	}

	void Update()
	{
		playButtonImage.texture = audioSource.isPlaying ? iconPause : iconPlay;
		ShowAudioPlayTime();
	}

	private void OnEnable()
	{
		//NOTE(Jitse): Update slider value
		LoadVolume();
		audioSlider.value = savedAudioVolumePanel;
	}

	public void TogglePlay()
	{
		if (clip == null)
		{
			StartCoroutine(GetAudioClip(url));
		}

		if (audioSource.isPlaying)
		{
			audioSource.Pause();
		}
		else
		{
			audioSource.Play();
		}
	}

	public void Restart()
	{
		if (clip != null)
		{
			audioSource.Stop();
			audioSource.Play();
		}
	}

	IEnumerator GetAudioClip(string urlToLoad)
	{
		var audioType = AudioHelper.AudioTypeFromFilename(urlToLoad);

		using (var www = UnityWebRequestMultimedia.GetAudioClip("file://" + urlToLoad, audioType))
		{
			www.timeout = 2;
			www.SendWebRequest();

			while (!www.isDone)
			{
				yield return null;
			}

			if (www.isDone)
			{
				clip = DownloadHandlerAudioClip.GetContent(www);
				clip.LoadAudioData();
				audioSource.clip = clip;
				ShowAudioPlayTime();

				fullClipLength = clip.length;
			}
			else if (www.isNetworkError)
			{
				Debug.Log(www.error);
			}
			else if (www.isHttpError)
			{
				Debug.Log(www.error);
			}
			else
			{
				Debug.Log("Something went wrong while downloading audio file. No errors, but not done either.");
			}
		}
	}

	private void ShowAudioPlayTime()
	{
		currentClipTime = audioSource.time;
		clipTimeText.text = $"{MathHelper.FormatSeconds(currentClipTime)} / {MathHelper.FormatSeconds(fullClipLength)}";
		audioTimeSlider.maxValue = fullClipLength;
		audioTimeSlider.value = currentClipTime;
	}

	public void LowerVolume()
	{
		audioSlider.value -= 0.1f;
	}

	public void IncreaseVolume()
	{
		audioSlider.value += 0.1f;
	}

	public void AudioValueChanged()
	{
		mixer.SetFloat("AudioVolumePanel", CorrectVolume(audioSlider.value));
		SaveVolume();
	}

	private float CorrectVolume(float value)
	{
		float temp = Mathf.Log10(value) * 20;
		return temp;
	}

	private void LoadVolume()
	{
		string cookiePath = Path.Combine(Application.persistentDataPath, ".volume");

		if (!File.Exists(cookiePath))
		{
			using (StreamWriter writer = File.AppendText(cookiePath))
			{
				writer.WriteLine("1.0");
				writer.WriteLine("1.0");
			}
		}
		StreamReader reader = new StreamReader(cookiePath);
		var fileContents = reader.ReadToEnd();
		reader.Close();

		var lines = fileContents.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
		savedAudioVolumePanel = float.Parse(lines[1], CultureInfo.InvariantCulture.NumberFormat);
	}

	private void SaveVolume()
	{
		string cookiePath = Path.Combine(Application.persistentDataPath, ".volume");

		if (File.Exists(cookiePath))
		{
			StreamReader reader = new StreamReader(cookiePath);
			var fileContents = reader.ReadToEnd();
			reader.Close();

			var lines = fileContents.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
			StreamWriter writer = new StreamWriter(cookiePath);
			writer.WriteLine(lines[0]);
			writer.WriteLine(audioSlider.value.ToString("F6", new CultureInfo("en-US").NumberFormat));
			writer.Close();
		}
	}
}
