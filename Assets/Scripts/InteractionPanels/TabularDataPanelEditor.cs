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
		//NOTE(Jitse): Check to see if data not corrupt.
		if (!(initialTabularData == null || initialTabularData.Count < 1 || initialTabularData.Count > MAXROWS * MAXCOLUMNS))
		{
			answerRows = rows;
			answerColumns = columns;
		}

		PopulateTable(initialTabularData);
		tabularDataWrapper.GetComponent<GridLayoutGroup>().cellSize = EnsureMinSize(answerColumns, answerRows);

		SetButtonStates();

		title.text = initialTitle;

		title.onValueChanged.AddListener(_ => OnInputChangeColor(title));
	}

	private void PopulateTable(List<string> tabularData)
	{
		int counter = 0;
		for (int row = 0; row < MAXROWS; row++)
		{
			for (int column = 0; column < MAXCOLUMNS; column++)
			{
				var dataCell = Instantiate(tabularDataCellPrefab, tabularDataWrapper);
				if (row < answerRows && column < answerColumns)
				{
					var cellText = dataCell.transform.GetComponentInChildren<InputField>();

					if (tabularData.Count != 0)
					{
						cellText.text = tabularData[counter];
						counter++;
					}
				}
				else
				{
					dataCell.gameObject.SetActive(false);
				}
			}
		}
	}
	public void AddRow()
	{
		for (int i = answerRows * MAXCOLUMNS; i < answerRows * MAXCOLUMNS + answerColumns; i++)
		{
			tabularDataWrapper.GetChild(i).gameObject.SetActive(true);
		}

		answerRows++;

		SetButtonStates();

		tabularDataWrapper.GetComponent<GridLayoutGroup>().cellSize = EnsureMinSize(answerColumns, answerRows);
	}

	public void RemoveRow()
	{
		answerRows--;

		for (int i = answerRows * MAXCOLUMNS; i < answerRows * MAXCOLUMNS + answerColumns; i++)
		{
			tabularDataWrapper.GetChild(i).gameObject.SetActive(false);
		}

		SetButtonStates();

		tabularDataWrapper.GetComponent<GridLayoutGroup>().cellSize = EnsureMinSize(answerColumns, answerRows);
	}

	public void AddColumn()
	{
		for (int i = 0; i < answerRows; i++)
		{
			int columnIndex = i * MAXCOLUMNS + answerColumns;
			tabularDataWrapper.GetChild(columnIndex).gameObject.SetActive(true);
		}

		answerColumns++;

		SetButtonStates();

		tabularDataWrapper.GetComponent<GridLayoutGroup>().cellSize = EnsureMinSize(answerColumns, answerRows);
	}

	public void RemoveColumn()
	{
		answerColumns--;

		for (int i = 0; i < answerRows; i++)
		{
			int columnIndex = i * MAXCOLUMNS + answerColumns;
			tabularDataWrapper.GetChild(columnIndex).gameObject.SetActive(false);
		}

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
			if (input.activeInHierarchy)
			{
				answerTabularData.Add(input.GetComponentInChildren<InputField>().text);
			}
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
