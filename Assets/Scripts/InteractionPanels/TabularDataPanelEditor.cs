using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabularDataPanelEditor : MonoBehaviour
{
	public InputField title;
	public RectTransform tabularDataWrapper;
	public RectTransform tabularDataCellPrefab;

	public Button addColumnButton;
	public Button removeColumnButton;
	public Button addRowButton;
	public Button removeRowButton;

	public bool answered;
	public string answerTitle;
	public List<string> answerTabularData;

	public int answerRows = 1;
	public int answerColumns = 1;

	private const int MAXCOLUMNS = 5;
	private const int MAXROWS = 20;

	private static Color errorColor = new Color(1, 0.8f, 0.8f, 1f);

	public void Init(string initialTitle, int rows, int columns, List<string> initialTabularData = null)
	{
		if (initialTabularData == null || initialTabularData.Count < 1 || initialTabularData.Count > MAXROWS * MAXCOLUMNS)
		{
			Instantiate(tabularDataCellPrefab, tabularDataWrapper);
		}
		else
		{
			for (int i = 0; i < initialTabularData.Count; i++)
			{
				var dataCell = Instantiate(tabularDataCellPrefab, tabularDataWrapper);
				var cellText = dataCell.transform.GetComponentInChildren<InputField>();
				string newText = initialTabularData[i];

				cellText.text = newText;
			}

			answerRows = rows;
			answerColumns = columns;

			tabularDataWrapper.GetComponent<GridLayoutGroup>().cellSize = EnsureMinSize(answerColumns, answerRows);
		}

		SetButtonStates();

		title.text = initialTitle;

		title.onValueChanged.AddListener(_ => OnInputChangeColor(title));
	}

	public void AddRow()
	{
		answerRows++;

		int dataCountOld = tabularDataWrapper.childCount;
		for (int i = 0; i < answerColumns * answerRows - dataCountOld; i++)
		{
			Instantiate(tabularDataCellPrefab, tabularDataWrapper);
		}

		SetButtonStates();

		tabularDataWrapper.GetComponent<GridLayoutGroup>().cellSize = EnsureMinSize(answerColumns, answerRows);
	}

	public void RemoveRow()
	{
		int dataCountOld = tabularDataWrapper.childCount;
		for (int i = dataCountOld - 1; i >= answerColumns * (answerRows - 1); i--)
		{
			Destroy(tabularDataWrapper.GetChild(i).gameObject);
		}

		answerRows--;

		SetButtonStates();

		tabularDataWrapper.GetComponent<GridLayoutGroup>().cellSize = EnsureMinSize(answerColumns, answerRows);
	}

	public void AddColumn()
	{
		for (int i = answerColumns; i <= tabularDataWrapper.childCount; i += answerColumns)
		{
			var dataCell = Instantiate(tabularDataCellPrefab, tabularDataWrapper);

			dataCell.transform.SetSiblingIndex(i);
			i++;
		}

		answerColumns++;

		SetButtonStates();

		tabularDataWrapper.GetComponent<GridLayoutGroup>().cellSize = EnsureMinSize(answerColumns, answerRows);
	}

	public void RemoveColumn()
	{
		int dataCountOld = tabularDataWrapper.childCount;
		for (int i = dataCountOld - 1; i >= 0; i--)
		{
			//NOTE(Jitse): Remove last cell per row
			if ((i + 1) % answerColumns == 0)
			{
				Destroy(tabularDataWrapper.GetChild(i).gameObject);
			}
		}

		answerColumns--;

		SetButtonStates();

		tabularDataWrapper.GetComponent<GridLayoutGroup>().cellSize = EnsureMinSize(answerColumns, answerRows);
	}

	public void Answer()
	{
		bool errors = false;

		if (String.IsNullOrEmpty(title.text))
		{
			title.image.color = errorColor;
			errors = true;
		}

		answerTabularData = new List<string>(tabularDataWrapper.childCount);

		//TODO(Jitse): Encoding stuff
		for (int i = 0; i < tabularDataWrapper.childCount; i++)
		{
			var input = tabularDataWrapper.GetChild(i).gameObject;
			answerTabularData.Add(input.GetComponentInChildren<InputField>().text);
		}

		if (!errors)
		{
			answered = true;
			answerTitle = title.text;
		}
	}

	public Vector2 EnsureMinSize(int columns, int rows)
	{
		float minGridCellSizeX = 50;
		float minGridCellSizeY = 50;
		float availableSpaceX = 430;
		float availableSpaceY = 220;

		return new Vector2(Mathf.Max(availableSpaceX / columns, minGridCellSizeX),
							Mathf.Max(availableSpaceY / rows, minGridCellSizeY));
	}

	public void SetButtonStates()
	{
		removeRowButton.interactable = answerRows > 1;
		addRowButton.interactable = answerRows < MAXROWS;
		removeColumnButton.interactable = answerColumns > 1;
		addColumnButton.interactable = answerColumns < MAXCOLUMNS;
	}

	public void OnInputChangeColor(InputField input)
	{
		input.image.color = Color.white;
	}
}
