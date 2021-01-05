﻿using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MenuBar : MonoBehaviour
{
	public List<Button> menuBarItems;
	public List<GameObject> menuBarPanels;
	public Text versionText;

	public void Start()
	{
		Debug.Assert(menuBarItems.Count == menuBarPanels.Count, "Make sure you have a menu bar panel for each menu bar button. Also make sure their ordering is the same");

		for (var i = 0; i < menuBarItems.Count; i++)
		{
			var index = i;
			menuBarItems[i].onClick.AddListener(delegate { OnMenuBarItemClick(index); });

			menuBarPanels[i].SetActive(false);
		}

		//NOTE(Jitse): Get the version number from a file and display it
		TextAsset versionAsset = Resources.Load("BuildVersion") as TextAsset;
		versionText.text = "Version: " + versionAsset.text;
	}

	void OnMenuBarItemClick(int index)
	{
		for (int i = 0; i < menuBarPanels.Count; i++)
		{
			if (i == index)
			{
				menuBarPanels[i].SetActive(!menuBarPanels[index].activeSelf);
			}
			else
			{
				menuBarPanels[i].SetActive(false);
			}
		}
	}

	public void ClosePanels()
	{
		foreach (var p in menuBarPanels)
		{
			p.SetActive(false);
		}
	}
}
