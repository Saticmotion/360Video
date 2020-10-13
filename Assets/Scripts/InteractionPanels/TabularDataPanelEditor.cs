using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

	private int currentRows = 1;
	private int currentColumns = 1;

	private const int MAXCOLUMNS = 5;
	private const int MAXROWS = 20;

	private static Color errorColor = new Color(1, 0.8f, 0.8f, 1f);

	public void Init(string initialTitle, List<string> initialTabularData = null)
	{
		if (initialTabularData == null || initialTabularData.Count < 1 || initialTabularData.Count > MAXROWS * MAXCOLUMNS)
		{
			initialTabularData = new List<string> { "" };
			Instantiate(tabularDataCellPrefab, tabularDataWrapper);
		}
		else
		{
			for (int i = 0; i < initialTabularData.Count - 1; i++)
			{
				var dataCell = Instantiate(tabularDataCellPrefab, tabularDataWrapper);
				var cellText = dataCell.transform.GetComponentInChildren<InputField>();
				string newText = initialTabularData[i];

				if (newText.Contains("//comma//"))
				{
					newText = newText.Replace("//comma//", ",");
				}

				cellText.text = newText;
			}


			var size = initialTabularData[initialTabularData.Count - 1].Split(':');
			currentRows = Convert.ToInt32(size[0]);
			currentColumns = Convert.ToInt32(size[1]);

			initialTabularData.RemoveAt(initialTabularData.Count - 1);

			tabularDataWrapper.GetComponent<GridLayoutGroup>().cellSize = EnsureMinSize(currentColumns, currentRows);
		}

		SetButtonStates();

		title.text = initialTitle;

		title.onValueChanged.AddListener(_ => OnInputChangeColor(title));
	}

	public void AddRow()
	{
		currentRows++;

		int dataCountOld = tabularDataWrapper.childCount;
		for (int i = 0; i < currentColumns * currentRows - dataCountOld; i++)
		{
			Instantiate(tabularDataCellPrefab, tabularDataWrapper);
		}

		SetButtonStates();

		tabularDataWrapper.GetComponent<GridLayoutGroup>().cellSize = EnsureMinSize(currentColumns, currentRows);
	}

	public void RemoveRow()
	{
		int dataCountOld = tabularDataWrapper.childCount;
		for (int i = dataCountOld - 1; i >= currentColumns * (currentRows - 1); i--)
		{
			Destroy(tabularDataWrapper.GetChild(i).gameObject);
		}

		currentRows--;

		SetButtonStates();

		tabularDataWrapper.GetComponent<GridLayoutGroup>().cellSize = EnsureMinSize(currentColumns, currentRows);
	}

	public void AddColumn()
	{
		for (int i = currentColumns; i <= tabularDataWrapper.childCount; i += currentColumns)
		{
			var dataCell = Instantiate(tabularDataCellPrefab, tabularDataWrapper);

			dataCell.transform.SetSiblingIndex(i);
			i++;
		}

		currentColumns++;

		SetButtonStates();

		tabularDataWrapper.GetComponent<GridLayoutGroup>().cellSize = EnsureMinSize(currentColumns, currentRows);
	}

	public void RemoveColumn()
	{
		int dataCountOld = tabularDataWrapper.childCount;
		for (int i = dataCountOld - 1; i >= 0; i--)
		{
			//NOTE(Jitse): Remove last cell per row
			if ((i + 1) % currentColumns == 0)
			{
				Destroy(tabularDataWrapper.GetChild(i).gameObject);
			}
		}

		currentColumns--;

		SetButtonStates();

		tabularDataWrapper.GetComponent<GridLayoutGroup>().cellSize = EnsureMinSize(currentColumns, currentRows);
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
			if (answerTabularData[i].Contains(','))
			{
				answerTabularData[i] = answerTabularData[i].Replace(",", "//comma//");
			}
		}

		if (!errors)
		{
			answered = true;
			answerTitle = title.text;
			answerTabularData.Add(currentRows + ":" + currentColumns);
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
		removeRowButton.interactable = currentRows > 1;
		addRowButton.interactable = currentRows < MAXROWS;
		removeColumnButton.interactable = currentColumns > 1;
		addColumnButton.interactable = currentColumns < MAXCOLUMNS;
	}

	public void OnInputChangeColor(InputField input)
	{
		input.image.color = Color.white;
	}
}
