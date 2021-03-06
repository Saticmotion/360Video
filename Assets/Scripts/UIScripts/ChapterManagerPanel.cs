﻿using UnityEngine;
using UnityEngine.UI;

public class ChapterManagerPanel : MonoBehaviour
{
	public GameObject chapterItemPrefab;

	public GameObject chapterItemHolder;

	public InputField newName;
	public InputField newDescription;

	private static Color errorColor = new Color(1, 0.8f, 0.8f, 1f);

	void Start()
	{
		newName.onValueChanged.AddListener(OnEditName);
		newDescription.onValueChanged.AddListener(OnEditDescription);

		var chapters = ChapterManager.Instance.chapters;

		for (int i = 0; i < chapters.Count; i++)
		{
			AddChapterItem(chapters[i]);
		}
	}

	public void OnAdd()
	{
		var name = newName.text;
		var description = newDescription.text;

		var success = ChapterManager.Instance.AddChapter(name, description, out var chapter);

		if (success)
		{
			newName.text = "";
			newDescription.text = "";

			AddChapterItem(chapter);
		}
		else
		{
			newName.image.color = errorColor;
		}
	}

	public void AddChapterItem(Chapter chapter)
	{
		var chapterGo = Instantiate(chapterItemPrefab);
		var chapterItem = chapterGo.GetComponent<ChapterItemEditable>();
		chapterGo.transform.SetParent(chapterItemHolder.transform);
		chapterItem.Init(chapter);
		chapterItem.deleteButton.onClick.AddListener(() => OnRemove(chapterGo, name));
	}

	public void OnRemove(GameObject chapterGo, string name)
	{
		ChapterManager.Instance.RemoveChapter(name);
		Destroy(chapterGo);
	}

	public void OnEditName(string _)
	{
		newName.image.color = Color.white;
	}

	public void OnEditDescription(string _)
	{
		newDescription.image.color = Color.white;
	}

	public void Close()
	{
		Destroy(gameObject);
	}
}
