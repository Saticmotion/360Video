using System;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
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
	public Slider volumeSlider;
	public Button decreaseVolumeButton;
	public Button increaseVolumeButton;
	public AudioMixer mixer;
	public AudioMixerGroup mixerGroup;

	private bool volumeChanging;
	private bool increaseButtonPressed;
	private bool decreaseButtonPressed;
	private float volumeButtonClickTime;
	private float savedAudioVolumePanel;

	public void Update()
	{
		CheckButtonStates();

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
		audioSource.outputAudioMixerGroup = mixerGroup;

		videoPlayer.SetTargetAudioSource(0, audioSource);

		videoPlayer.url = fullPath;
		videoPlayer.playOnAwake = false;

		title.text = newTitle;

		//NOTE(Jitse): The volume buttons are only used in the Player.
		//NOTE(cont.): This check prevents null reference errors.
		if (decreaseVolumeButton != null && increaseVolumeButton != null)
		{
			decreaseVolumeButton.onClick.AddListener(DecreaseVolume);
			increaseVolumeButton.onClick.AddListener(IncreaseVolume);
		}

		LoadVolume();
		volumeSlider.onValueChanged.AddListener( _ => VolumeValueChanged());
		mixer.SetFloat("AudioVolumePanel", CorrectVolume(savedAudioVolumePanel));
		volumeSlider.value = savedAudioVolumePanel;

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

		//NOTE(Jitse): Update slider value
		LoadVolume();
		volumeSlider.value = savedAudioVolumePanel;
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

	public void DecreaseVolume()
	{
		volumeSlider.value -= 0.1f;
	}

	public void IncreaseVolume()
	{
		volumeSlider.value += 0.1f;
	}

	public void VolumeValueChanged()
	{
		mixer.SetFloat("AudioVolumePanel", CorrectVolume(volumeSlider.value));
		SaveVolume();
	}

	private float CorrectVolume(float value)
	{
		return Mathf.Log10(value) * 20;
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
			writer.WriteLine(volumeSlider.value.ToString("F6", new CultureInfo("en-US").NumberFormat));
			writer.Close();
		}
	}

	private void CheckButtonStates()
	{
		//NOTE(Jitse): If volume up button is being held down
		if (increaseButtonPressed)
		{
			//NOTE(Jitse): If volume hasn't changed in the last x seconds
			if (!volumeChanging)
			{
				IncreaseVolume();
				volumeChanging = true;
			}
			//NOTE(Jitse): If x seconds have passed since last volume increase
			if (Time.realtimeSinceStartup > volumeButtonClickTime + 0.15)
			{
				volumeChanging = false;
				volumeButtonClickTime = Time.realtimeSinceStartup;
			}
			//NOTE(Jitse): If volume up button no longer being held down
			if (Input.GetMouseButtonUp(0))
			{
				increaseButtonPressed = false;
			}
		}
		//NOTE(Jitse): If volume down button is being held down
		else if (decreaseButtonPressed)
		{
			//NOTE(Jitse): If volume hasn't changed in the last x seconds
			if (!volumeChanging)
			{
				DecreaseVolume();
				volumeChanging = true;
			}
			//NOTE(Jitse): If x seconds have passed since last volume decrease
			if (Time.realtimeSinceStartup > volumeButtonClickTime + 0.15)
			{
				volumeChanging = false;
				volumeButtonClickTime = Time.realtimeSinceStartup;
			}
			//NOTE(Jitse): If volume down button no longer being held down
			if (Input.GetMouseButtonUp(0))
			{
				decreaseButtonPressed = false;
			}
		}
	}

	public void OnPointerDownIncreaseButton()
	{
		increaseButtonPressed = true;
		volumeButtonClickTime = Time.realtimeSinceStartup;
	}

	public void OnPointerDownDecreaseButton()
	{
		decreaseButtonPressed = true;
		volumeButtonClickTime = Time.realtimeSinceStartup;
	}
}
