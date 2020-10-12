using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TabularDataPanelEditor : MonoBehaviour
{
	public Transform layoutPanelTransform;
	public InputField title;
	public RectTransform tabularDataWrapper;
	public RectTransform tabularDataCellPrefab;
	public ScrollRect scrollRect;

	public Button addColumnButton;
	public Button removeColumnButton;
	public Button addRowButton;
	public Button removeRowButton;
	public Button doneButton;

	public bool answered;
	public string answerTitle;
	public List<string> answerTabularData;

	//NOTE(Jitse): use two dimensional or one dimensional list?
	//private List<List<string>> tabularData;
	private List<string> tabularData;
	private int currentRows = 1;
	private int currentColumns = 1;

	//NOTE(Jitse): MAXCOLUMNS == 8 due to (current) width limitations.
	private const int MAXCOLUMNS = 5;
	private const int MAXROWS = 20;
	private const float GRIDCELLSIZEX = 430;
	private const float GRIDCELLSIZEY = 220;

	private static Color errorColor = new Color(1, 0.8f, 0.8f, 1f);


	public void Init(string initialTitle, List<string> initialTabularData = null)
	{
		if (initialTabularData == null || initialTabularData.Count < 1 || initialTabularData.Count > MAXROWS * MAXCOLUMNS)
		{
			initialTabularData = new List<string>() { "" };
			var dataCell = Instantiate(tabularDataCellPrefab, tabularDataWrapper);
			var cellText = dataCell.transform.GetComponentInChildren<InputField>();

			dataCell.transform.SetAsLastSibling();
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

				dataCell.transform.SetAsLastSibling();
			}
			currentRows = Convert.ToInt32(initialTabularData[initialTabularData.Count - 1].Split(':')[0]);
			currentColumns = Convert.ToInt32(initialTabularData[initialTabularData.Count - 1].Split(':')[1]);

			if (currentRows >= 1)
			{
				removeRowButton.interactable = true;
			}
			if (currentRows == MAXROWS)
			{
				addRowButton.interactable = false;
			}
			if (currentColumns >= 1)
			{
				removeColumnButton.interactable = true;
			}
			if (currentColumns == MAXCOLUMNS)
			{
				addColumnButton.interactable = false;
			}

			initialTabularData.RemoveAt(initialTabularData.Count - 1);

			float cellSizeX = GRIDCELLSIZEX / currentColumns;
			float cellSizeY = GRIDCELLSIZEY / currentRows;

			if (cellSizeY < 50)
			{
				cellSizeY = 50;
			}
			if (cellSizeX < 50)
			{
				cellSizeX = 50;
			}
			tabularDataWrapper.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSizeX, cellSizeY);
		}

		tabularData = initialTabularData;

		title.text = initialTitle;

		title.onValueChanged.AddListener(_ => OnInputChangeColor(title));
	}

	public void AddRow()
	{
		if (currentRows < MAXROWS)
		{
			if (currentRows == 1)
			{
				removeRowButton.interactable = true;
			}

			currentRows++;

			int dataCountOld = tabularData.Count;
			for (int i = 0; i < currentColumns*currentRows - dataCountOld; i++)
			{
				tabularData.Add("");
				var dataCell = Instantiate(tabularDataCellPrefab, tabularDataWrapper);
				var cellText = dataCell.transform.GetComponentInChildren<InputField>();

				dataCell.transform.SetAsLastSibling();
			}

			if (currentRows == MAXROWS)
			{
				addRowButton.interactable = false;
			}

			float cellSizeX = GRIDCELLSIZEX / currentColumns;
			float cellSizeY = GRIDCELLSIZEY / currentRows;

			if (cellSizeY < 50)
			{
				cellSizeY = 50;
			}
			if (cellSizeX < 50)
			{
				cellSizeX = 50;
			}
			tabularDataWrapper.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSizeX, cellSizeY);
		}
	}

	public void RemoveRow()
	{
		if (currentRows > 1)
		{
			var dataCells = tabularDataWrapper.GetComponentsInChildren<RectTransform>();

			if (currentRows == MAXROWS)
			{
				addRowButton.interactable = true;
			}

			int dataCountOld = tabularData.Count;
			for (int i = dataCountOld - 1; i >= currentColumns * (currentRows - 1); i--)
			{
				tabularData.RemoveAt(i);
				Destroy(tabularDataWrapper.GetChild(i).gameObject);
			}

			currentRows--;

			if (currentRows == 1)
			{
				removeRowButton.interactable = false;
			}

			float cellSizeX = GRIDCELLSIZEX / currentColumns;
			float cellSizeY = GRIDCELLSIZEY / currentRows;

			if (cellSizeY < 50)
			{
				cellSizeY = 50;
			}
			if (cellSizeX < 50)
			{
				cellSizeX = 50;
			}
			tabularDataWrapper.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSizeX, cellSizeY);
		}
	}

	public void AddColumn()
	{
		if (currentColumns < MAXCOLUMNS)
		{
			if (currentColumns == 1)
			{
				removeColumnButton.interactable = true;
			}

			int dataCountOld = tabularData.Count;
			for (int i = currentColumns; i <= tabularData.Count; i = i + currentColumns)
			{
				tabularData.Insert(i, "");
				var dataCell = Instantiate(tabularDataCellPrefab, tabularDataWrapper);
				var cellText = dataCell.transform.GetComponentInChildren<InputField>();

				dataCell.transform.SetSiblingIndex(i);
				i++;
			}

			currentColumns++;

			if (currentColumns == MAXCOLUMNS)
			{
				addColumnButton.interactable = false;
			}

			float cellSizeX = GRIDCELLSIZEX / currentColumns;
			float cellSizeY = GRIDCELLSIZEY / currentRows;

			if (cellSizeY < 50)
			{
				cellSizeY = 50;
			}
			if (cellSizeX < 50)
			{
				cellSizeX = 50;
			}
			tabularDataWrapper.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSizeX, cellSizeY);
		}
	}

	public void RemoveColumn()
	{
		if (currentColumns > 1)
		{
			var dataCells = tabularDataWrapper.GetComponentsInChildren<RectTransform>();
			if (currentColumns == MAXCOLUMNS)
			{
				addColumnButton.interactable = true;
			}

			int dataCountOld = tabularData.Count;
			for (int i = dataCountOld - 1; i >= 0; i--)
			{
				 if ((i + 1) % currentColumns == 0)
				{
					tabularData.RemoveAt(i);
					Destroy(tabularDataWrapper.GetChild(i).gameObject);
				}
			}

			currentColumns--;

			if (currentColumns == 1)
			{
				removeColumnButton.interactable = false;
			}

			float cellSizeX = GRIDCELLSIZEX / currentColumns;
			float cellSizeY = GRIDCELLSIZEY / currentRows;

			if (cellSizeY < 50)
            {
				cellSizeY = 50;
            }
			if (cellSizeX < 50)
            {
				cellSizeX = 50;
            }
			tabularDataWrapper.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSizeX, cellSizeY);
		}
	}

	public void Answer()
	{
		bool errors = false;

		if (String.IsNullOrEmpty(title.text))
		{
			title.image.color = errorColor;
			errors = true;
		}

		Encoding asciiEncoding = Encoding.ASCII;
		for (int i = 0; i < tabularData.Count; i++)
		{
			var input = tabularDataWrapper.GetChild(i).gameObject;
			tabularData[i] = input.GetComponentInChildren<InputField>().text;
			if (tabularData[i].Contains(','))
			{
				tabularData[i] = tabularData[i].Replace(",", "//comma//");
			}
		}

		if (!errors)
		{
			answered = true;
			answerTitle = title.text;
			answerTabularData = tabularData;
			answerTabularData.Add(currentRows.ToString() + ":" + currentColumns.ToString());
		}
	}

	public void OnInputChangeColor(InputField input)
	{
		input.image.color = Color.white;
	}
}
