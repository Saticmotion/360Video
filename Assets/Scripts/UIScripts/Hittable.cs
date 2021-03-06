﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Hittable : MonoBehaviour
{
	public UnityEvent onHit;
	public UnityEvent onHitDown;
	public UnityEvent onHitUp;
	public UnityEvent onHoverStart;
	public UnityEvent onHoverStay;
	public UnityEvent onHoverEnd;

	public RawImage image;
	public Color hoverColor;
	private Color originalColor;

	public bool hitting;
	public bool hovering;

	private bool oldHitting;
	private bool oldHovering;

	// Use this for initialization
	void Start()
	{
		if (!SceneManager.GetActiveScene().name.Equals("Editor"))
		{
			Player.hittables.Add(this);
		}
	}

	void Update()
	{
		if (oldHitting && hitting)
		{
			onHit.Invoke();
		}

		if (!oldHitting && hitting)
		{
			onHitDown.Invoke();
		}

		if (oldHitting & !hitting)
		{
			onHitUp.Invoke();
		}

		if (!oldHovering && hovering)
		{
			if (image)
			{
				originalColor = image.color;
				image.color = hoverColor;
			}

			onHoverStart.Invoke();
		}

		if (oldHovering && hovering)
		{
			onHoverStay.Invoke();
		}

		if (oldHovering && !hovering)
		{
			if (image)
			{
				image.color = originalColor;
			}
			onHoverEnd.Invoke();
		}

		oldHitting = hitting;
		oldHovering = hovering;
	}

	void OnDestroy()
	{
		// NOTE(Lander): Remove the items only when used in Player 
		if (Player.hittables != null)
		{
			Player.hittables.Remove(this);
		}
	}
}
