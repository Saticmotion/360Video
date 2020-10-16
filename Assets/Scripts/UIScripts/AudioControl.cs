using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioControl : MonoBehaviour
{
	public Button playButton;
	public Slider audioTimeSlider;
	public Text clipTimeText;

	private RawImage playButtonImage;
	public Texture iconPlay;
	public Texture iconPause;

	public AudioSlider audioSlider;
	public RectTransform volumeImagesWrapper;
	public Button lowerVolumeButton;
	public Button increaseVolumeButton;

	private AudioSource audioSource;
	private AudioClip clip;
	private Image[] volumeImages;

	private string url;
	private float fullClipLength;
	private float currentClipTime;

	// 70 & 185 left
	void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.playOnAwake = false;
		playButtonImage = playButton.GetComponentInChildren<RawImage>();
	}

	public void Init(string url)
	{
		if (SceneManager.GetActiveScene().name == "Player")
		{
			volumeImages = volumeImagesWrapper.GetComponentsInChildren<Image>();
			lowerVolumeButton.onClick.AddListener(LowerVolume);
			increaseVolumeButton.onClick.AddListener(IncreaseVolume);
		}
		else if (SceneManager.GetActiveScene().name == "Editor")
		{
			audioSlider.onValueChanged.AddListener(
				   delegate { AudioValueChanged(); }
			   );
		}

		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}
		audioSource.Stop();
		clip = null;
		this.url = url;
		StartCoroutine(GetAudioClip(url));
	}

	void Update()
	{
		playButtonImage.texture = audioSource.isPlaying ? iconPause : iconPlay;
		ShowAudioPlayTime();
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
		if (audioSource.volume >= 0.1f)
		{
			audioSource.volume -= 0.1f;
		}
		else
		{
			audioSource.volume = 0f;
		}

		int index = Convert.ToInt32(audioSource.volume * 10);
		var tempColor = volumeImages[index].color;
		tempColor.a = 0f;
		volumeImages[index].color = tempColor;
	}

	public void IncreaseVolume()
	{
		if (audioSource.volume < 1f)
		{
			int index = Convert.ToInt32(audioSource.volume * 10);
			var tempColor = volumeImages[index].color;
			tempColor.a = 1f;
			volumeImages[index].color = tempColor;

			audioSource.volume += 0.1f;
		}
		else
		{
			audioSource.volume = 1f;
		}
	}

	public void AudioValueChanged()
	{
		audioSource.volume = this.GetComponentInChildren<AudioSlider>().value;
	}
}
