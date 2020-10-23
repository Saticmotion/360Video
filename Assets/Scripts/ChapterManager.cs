using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Chapter
{
	public int id;
	public string name;
	public string description;
	public float time;
}

public class ChapterManager : MonoBehaviour
{
	public static ChapterManager Instance { get; private set; }
	public List<Chapter> chapters;

	//NOTE(Simon): Always explicitly start at 1
	private int indexCounter = 1;

	void Start()
	{
		chapters = new List<Chapter>();
		Instance = this;
	}

	public bool AddChapter(string name, string description)
	{
		bool error = false;
		for (int i = 0; i < chapters.Count; i++)
		{
			if (chapters[i].name == name)
			{
				error = true;
				break;
			}
		}

		if (string.IsNullOrEmpty(name))
		{
			error = true;
		}

		if (error)
		{
			return false;
		}

		indexCounter++;
		chapters.Add(new Chapter
		{
			name = name,
			description = description,
			time = 0f,
			id = indexCounter
		});

		UnsavedChangesTracker.Instance.unsavedChanges = true;

		return true;
	}

	public void RemoveChapter(string name)
	{
		for (int i = chapters.Count - 1; i >= 0; i--)
		{
			if (chapters[i].name == name)
			{
				chapters.RemoveAt(i);
				break;
			}
		}

		UnsavedChangesTracker.Instance.unsavedChanges = true;
	}

	public void SetChapters(List<Chapter> newChapters)
	{
		chapters = newChapters;

		for (int i = 0; i < chapters.Count; i++)
		{
			if (chapters[i].id > indexCounter)
			{
				indexCounter = chapters[i].id;
			}
		}
	}
}
