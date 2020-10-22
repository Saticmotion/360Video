using System;
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

	public Slider volumeSlider;
	public Button decreaseVolumeButton;
	public Button increaseVolumeButton;

	public AudioMixer mixer;

	private AudioSource audioSource;
	private AudioClip clip;

	private float savedAudioVolumePanel;

	private string url;
	private float fullClipLength;
	private float currentClipTime;

	private bool volumeChanging;
	private bool increaseButtonPressed;
	private bool decreaseButtonPressed;
	private float volumeButtonClickTime;

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

		if (decreaseVolumeButton != null)
		{
			decreaseVolumeButton.onClick.AddListener(LowerVolume);
			increaseVolumeButton.onClick.AddListener(IncreaseVolume);
		}
		if (volumeSlider != null)
		{
			LoadVolume();
			volumeSlider.onValueChanged.AddListener(_ => AudioValueChanged());
			mixer.SetFloat("AudioVolumePanel", CorrectVolume(savedAudioVolumePanel));
			volumeSlider.value = savedAudioVolumePanel;
		}
	}
	private void OnEnable()
	{
		//NOTE(Jitse): Update slider value
		LoadVolume();
		volumeSlider.value = savedAudioVolumePanel;
	}

	void Update()
	{
		CheckButtonStates();

		playButtonImage.texture = audioSource.isPlaying ? iconPause : iconPlay;
		ShowAudioPlayTime();
	}
	
	private void CheckButtonStates()
	{
		if (increaseButtonPressed)
		{
			if (!volumeChanging)
			{
				IncreaseVolume();
				volumeChanging = true;
			}
			if (Time.realtimeSinceStartup > volumeButtonClickTime + 0.15)
			{
				volumeChanging = false;
				volumeButtonClickTime = Time.realtimeSinceStartup;
			}
			if (Input.GetMouseButtonUp(0))
			{
				increaseButtonPressed = false;
			}
		}
		else if (decreaseButtonPressed)
		{
			if (!volumeChanging)
			{
				LowerVolume();
				volumeChanging = true;
			}
			if (Time.realtimeSinceStartup > volumeButtonClickTime + 0.15)
			{
				volumeChanging = false;
				volumeButtonClickTime = Time.realtimeSinceStartup;
			}
			if (Input.GetMouseButtonUp(0))
			{
				decreaseButtonPressed = false;
			}
		}
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
		volumeSlider.value -= 0.1f;
	}

	public void IncreaseVolume()
	{
		volumeSlider.value += 0.1f;
	}

	public void AudioValueChanged()
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

	public void OnPointerDownIncreaseButton()
	{
		increaseButtonPressed = true;
		volumeButtonClickTime = Time.realtimeSinceStartup;
	}

	public void OnPointerDownLowerButton()
	{
		decreaseButtonPressed = true;
		volumeButtonClickTime = Time.realtimeSinceStartup;
	}
}
