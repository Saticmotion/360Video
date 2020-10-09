using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TabularDataPanel : MonoBehaviour
{
    public Text title;
    public List<string> tabularData;
    public RectTransform tabularDataWrapper;
    public RectTransform tabularDataCellPrefab;

    private int currentColumns = 0;
    private int currentRows = 0;
    private const float GRIDCELLSIZEX = 430;
    private const float GRIDCELLSIZEY = 220;

    public void Init(string newTitle, List<string> newTabularData)
    {
		for (int i = tabularData.Count - 2; i >= 0; i--)
		{
			Destroy(tabularDataWrapper.GetChild(i).gameObject);
		}
		title.text = newTitle;
        tabularData = newTabularData;

		if (newTabularData != null && newTabularData.Count > 0)
		{
			if (tabularData[tabularData.Count - 1].Contains(":"))
			{
				var rowsColumns = tabularData[tabularData.Count - 1].Split(':');
				currentRows = Convert.ToInt32(rowsColumns[0]);
				currentColumns = Convert.ToInt32(rowsColumns[1]);
			}
		}

        for (int row = 0; row < currentRows; row++)
        {
            for (int column = 0; column < currentColumns; column++)
            {
                var dataCell = Instantiate(tabularDataCellPrefab, tabularDataWrapper);
                var cellText = dataCell.transform.GetComponentInChildren<InputField>();
                cellText.interactable = false;
				string newText = tabularData[row * currentColumns + column];

				if (newText.Contains("//comma//"))
				{
					newText = newText.Replace("//comma//", ",");
				}

				cellText.text = newText;

                dataCell.transform.SetAsLastSibling();
            }
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
