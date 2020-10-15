using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioSlider : Slider
{
	private RectTransform background;
	private RectTransform fillArea;
	private RectTransform handle;
	private RectTransform image;

	private bool muted;
	private bool isDragging;
	private float oldAudioValue;

    // Start is called before the first frame update
    void Start()
    {
		var components = this.GetComponentsInChildren<RectTransform>(true);
		background = components[1];
		fillArea = components[2];
		handle = components[4];

		var parent = GetComponentsInParent<RectTransform>()[1];
		image = parent.gameObject.GetComponentsInChildren<RectTransform>()[1];

		muted = false;
		isDragging = false;

		background.gameObject.SetActive(false);
		fillArea.gameObject.SetActive(false);
		handle.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update()
    {
		if (RectTransformUtility.RectangleContainsScreenPoint(image, Input.mousePosition) || 
			RectTransformUtility.RectangleContainsScreenPoint(this.GetComponent<RectTransform>(), Input.mousePosition))
		{
			background.gameObject.SetActive(true);
			fillArea.gameObject.SetActive(true);
			handle.gameObject.SetActive(true);
		}
		else
		{
			if (!isDragging)
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

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		isDragging = false;
	}

	public void Mute()
	{
		if (muted)
		{
			this.value = oldAudioValue;
		}
		else
		{
			oldAudioValue = this.value;
			this.value = 0;
		}

		muted = !muted;
	}
}
