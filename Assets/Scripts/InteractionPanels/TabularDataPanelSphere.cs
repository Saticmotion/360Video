using System;
using UnityEngine;
using UnityEngine.UI;

public class TabularDataPanelSphere : MonoBehaviour
{
	public Text title;
	public string[] tabularData;
	public RectTransform tabularDataWrapper;
	public RectTransform tabularDataCellPrefab;
	public RectTransform scrollPanel;
	public Button backButton;
	public Button nextButton;

	private int currentColumns;
	private int currentRows;
	private int currentPage;
	private int maxPages;

	private const int MAXROWSPAGE = 7;
	private const float MIN_GRID_SIZE_X = 50;
	private const float MIN_GRID_SIZE_Y = 50;

	public void Init(string newTitle, int rows, int columns, string[] newTabularData)
	{
		backButton.onClick.AddListener(BackButtonClick);
		nextButton.onClick.AddListener(NextButtonClick);

		ClearTable();

		title.text = newTitle;
		tabularData = newTabularData;

		if (newTabularData != null && newTabularData.Length > 0)
		{
			currentRows = rows;
			currentColumns = columns;
			maxPages = Mathf.CeilToInt((float)currentRows / MAXROWSPAGE);
		}
		else
		{
			Toasts.AddToast(5, "File is corrupt");
			return;
		}

		SetButtonStates();
		PopulateTable();

		tabularDataWrapper.GetComponent<GridLayoutGroup2>().constraintCount = currentColumns;
		float cellSizeX = Mathf.Max(scrollPanel.rect.width / currentColumns, MIN_GRID_SIZE_X);
		float cellSizeY = Mathf.Max(scrollPanel.rect.height / currentRows, MIN_GRID_SIZE_Y);

		tabularDataWrapper.GetComponent<GridLayoutGroup2>().cellSize = new Vector2(cellSizeX, cellSizeY);
	}

	//TODO(Jitse): Choose between destroying objects or pooling.
	//TODO(cont.): Uncomment the parts from here on out to destroy objects (and then also delete the uncommented code.)
	private void PopulateTable()
	{
		for (int row = 0; row < currentRows; row++)
		{
			for (int column = 0; column < currentColumns; column++)
			{
				var dataCell = Instantiate(tabularDataCellPrefab, tabularDataWrapper);
				if (row >= MAXROWSPAGE)
				{
					dataCell.gameObject.SetActive(false);
				}
				var cellText = dataCell.transform.GetComponentInChildren<InputField>();
				cellText.interactable = false;

				cellText.text = tabularData[row * currentColumns + column];
				cellText.textComponent.fontSize = 16;
				cellText.textComponent.color = Color.black;

				dataCell.transform.SetAsLastSibling();
			}
		}
	}

	private void ClearTable()
	{
		int rowLimit = currentPage * MAXROWSPAGE + MAXROWSPAGE;
		if (rowLimit > currentRows)
		{
			rowLimit = currentRows;
		}
		for (int row = currentPage * MAXROWSPAGE; row < rowLimit; row++)
		{
			for (int column = 0; column < currentColumns; column++)
			{
				tabularDataWrapper.GetChild(row * currentColumns + column).gameObject.SetActive(false);
			}
		}
	}

	private void NextButtonClick()
	{
		ClearTable();

		currentPage++;
		SetButtonStates();
		ActivateTableChildren();
	}

	private void BackButtonClick()
	{
		ClearTable();

		currentPage--;
		SetButtonStates();
		ActivateTableChildren();
	}	

	private void ActivateTableChildren()
	{
		int rowLimit = currentPage * MAXROWSPAGE + MAXROWSPAGE;
		if (rowLimit > currentRows)
		{
			rowLimit = currentRows;
		}
		for (int row = currentPage * MAXROWSPAGE; row < rowLimit; row++)
		{
			for (int column = 0; column < currentColumns; column++)
			{
				tabularDataWrapper.GetChild(row * currentColumns + column).gameObject.SetActive(true);
			}
		}
	}

	private void SetButtonStates()
	{
		backButton.interactable = currentPage > 0;
		nextButton.interactable = currentPage < maxPages - 1;
	}
}
