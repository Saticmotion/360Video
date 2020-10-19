using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioSlider : Slider
{
	private RectTransform background;
	private RectTransform fillArea;
	private RectTransform handle;
	private RectTransform icon;

	private bool muted;
	private bool isDragging;
	private float oldAudioValue;

	new void Start()
	{
		var components = GetComponentsInChildren<RectTransform>(true);
		background = components[1];
		fillArea = components[2];
		handle = components[4];

		var parent = GetComponentsInParent<RectTransform>()[1];
		icon = parent.gameObject.GetComponentsInChildren<RectTransform>()[1];

		muted = false;
		isDragging = false;

		background.gameObject.SetActive(false);
		fillArea.gameObject.SetActive(false);
		handle.gameObject.SetActive(false);
	}

	new void Update()
	{
		if (RectTransformUtility.RectangleContainsScreenPoint(icon, Input.mousePosition))
		{
			background.gameObject.SetActive(true);
			fillArea.gameObject.SetActive(true);
			handle.gameObject.SetActive(true);
		}
		else
		{
			if (!isDragging && !RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition))
			{
				background.gameObject.SetActive(false);
				fillArea.gameObject.SetActive(false);
				handle.gameObject.SetActive(false);
			}
		}
	}

	public override void OnDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);
		isDragging = true;
		muted = false;
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		muted = false;
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		isDragging = false;
	}

	public void Mute()
	{
		if (muted)
		{
			value = oldAudioValue;
		}
		else
		{
			oldAudioValue = value;
			value = 0;
		}

		muted = !muted;
	}
}
