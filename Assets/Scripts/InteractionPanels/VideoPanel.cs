using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPanel : MonoBehaviour
{
	public RenderTexture videoRenderTexture;
	public Button controlButton;
	public Button bigButton;
	public RawImage bigButtonIcon;
	public Slider progressBar;
	public Texture iconPlay;
	public Texture iconPause;
	public Text title;
	public Text timeDisplay;
	public RawImage videoSurface;
	public VideoPlayer videoPlayer;
	public AudioSource audioSource;
	public AudioSlider audioSlider;
	public RectTransform volumeImagesWrapper;
	public Button lowerVolumeButton;
	public Button increaseVolumeButton;

	private Image[] volumeImages;

	public void Update()
	{
		float time = (float)videoPlayer.time;
		float length = videoPlayer.frameCount / videoPlayer.frameRate;
		progressBar.value = time;
		progressBar.maxValue = length;
		timeDisplay.text = $"{MathHelper.FormatSeconds(time)} / {MathHelper.FormatSeconds(length)}";
	}

	public void Init(string newTitle, string fullPath)
	{
		videoPlayer.source = VideoSource.Url;
		videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
		videoPlayer.controlledAudioTrackCount = 1;
		videoPlayer.EnableAudioTrack(0, true);

		audioSource = videoPlayer.gameObject.GetOrAddComponent<AudioSource>();
		audioSource.playOnAwake = false;

		videoPlayer.SetTargetAudioSource(0, audioSource);

		videoPlayer.url = fullPath;
		videoPlayer.playOnAwake = false;

		title.text = newTitle;

		//NOTE(Jitse): Check if in Player
		if (volumeImagesWrapper != null)
		{
			volumeImages = volumeImagesWrapper.GetComponentsInChildren<Image>();
			lowerVolumeButton.onClick.AddListener(LowerVolume);
			increaseVolumeButton.onClick.AddListener(IncreaseVolume);
			//TODO(Jitse): Read value from file
		}

		//NOTE(Jitse): Check if in Editor
		if (audioSlider != null)
		{
			audioSlider.onValueChanged.AddListener( _ => AudioValueChanged());
			//TODO(Jitse): Read value from file
		}

		//NOTE(Simon): Make sure we have added the events
		controlButton.onClick.RemoveListener(TogglePlay);
		controlButton.onClick.AddListener(TogglePlay);
		bigButton.onClick.RemoveListener(TogglePlay);
		bigButton.onClick.AddListener(TogglePlay);
	}

	private void OnPrepareComplete(VideoPlayer source)
	{
		var heightFactor = source.texture.height / videoSurface.rectTransform.rect.height;
		var widthFactor = source.texture.width / videoSurface.rectTransform.rect.width;
		var largestFactor = Mathf.Max(heightFactor, widthFactor);

		var desiredWidth = videoSurface.rectTransform.rect.width * largestFactor;
		var desiredHeight = videoSurface.rectTransform.rect.height * largestFactor;
		videoRenderTexture = new RenderTexture((int)desiredWidth, (int)desiredHeight, 0, RenderTextureFormat.ARGB32);

		videoPlayer.targetTexture = videoRenderTexture;
		videoSurface.texture = videoRenderTexture;
		videoSurface.color = Color.white;
	}

	private void OnEnable()
	{
		videoPlayer.prepareCompleted += OnPrepareComplete;
		videoPlayer.Prepare();

		//NOTE(Simon): Make sure we have added the events
		controlButton.onClick.RemoveListener(TogglePlay);
		controlButton.onClick.AddListener(TogglePlay);
		bigButton.onClick.RemoveListener(TogglePlay);
		bigButton.onClick.AddListener(TogglePlay);
	}

	public void OnSeek(float value)
	{
		if (Math.Abs(value - videoPlayer.time) > 0.1f)
		{
			Debug.Log("Value Changed to " + value);
		}
	}

	public void TogglePlay()
	{
		if (videoPlayer.isPlaying)
		{
			videoPlayer.Pause();
		}
		else
		{
			videoPlayer.Play();
		}

		controlButton.GetComponent<RawImage>().texture = videoPlayer.isPlaying ? iconPause : iconPlay;
		bigButtonIcon.color = videoPlayer.isPlaying ? Color.clear : Color.white;
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
		audioSource.volume = audioSlider.value;
	}
}
