using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
	public Button increaseVolumeButton;
	public Button decreaseVolumeButton;
	public RectTransform background;
	public RectTransform icon;
	public Slider slider;

	private bool muted;
	private bool isDragging;
	private float oldAudioValue;

	void Start()
	{
		muted = false;
		isDragging = false;

		slider.OnDrag(OnDrag());
		slider.OnPointerDown(OnPointerDown());
		slider.OnPointerUp(OnPointerUp());

		slider.handleRect.gameObject.SetActive(false);
		slider.fillRect.gameObject.SetActive(false);
		background.gameObject.SetActive(false);
	}

	void Update()
	{
		if (RectTransformUtility.RectangleContainsScreenPoint(icon, Input.mousePosition))
		{
			background.gameObject.SetActive(true);
			slider.handleRect.gameObject.SetActive(true);
			slider.fillRect.gameObject.SetActive(true);
		}
		else
		{
			if (!isDragging && !RectTransformUtility.RectangleContainsScreenPoint(slider.GetComponent<RectTransform>(), Input.mousePosition))
			{
				background.gameObject.SetActive(false);
				slider.handleRect.gameObject.SetActive(false);
				slider.fillRect.gameObject.SetActive(false);
			}
		}
	}

	public PointerEventData OnDrag()
	{
		isDragging = true;
		muted = false;
		return null;
	}

	public PointerEventData OnPointerDown()
	{
		muted = false;
		return null;
	}

	public PointerEventData OnPointerUp()
	{
		isDragging = false;
		return null;
	}

	public void Mute()
	{
		if (muted)
		{
			slider.value = oldAudioValue;
		}
		else
		{
			oldAudioValue = slider.value;
			slider.value = 0;
		}

		muted = !muted;
	}
}
