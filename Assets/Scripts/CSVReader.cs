using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CSVReader
{
    private const char SEPARATOR = '\t';
    
    public TextAsset textAsset;
    
    
    
    private List<string> colIDs;
    private List<string> rowIDs;
    private List<List<string>> csv;

    public string GetCellContent(string rowID, string columnID)
    {
        return GetCellContent(rowIDs.IndexOf(rowID), colIDs.IndexOf(columnID));
    }
    
    public string GetCellContent(int row, int col)
    {
        return csv[row][col];
    }

    public void ParseTextAsset()
    {
        Parse(textAsset.text);
    }

    public void Parse(string text)
    {
        csv = new List<List<string>>();
        colIDs = new List<string>();
        rowIDs = new List<string>();

        var rows = text.Split('\n');
        for (int r = 0; r < rows.Length; r++)
        {
            var row = rows[r];
            var cols = row.Split(SEPARATOR);
            csv.Add(new List<string>(cols));
        }

        for (int r = 0; r < csv.Count; r++)
        {
            for (int c = 0; c < csv[r].Count; c++)
            {
                if (r == 0)
                {
                    colIDs.Add(csv[r][c]);
                }
                
                else if (c == 0)
                {
                    rowIDs.Add(csv[r][c]);
                }
            }
        }
        
        Debug.Log("CSVReader Parsed");
        Debug.Log("cols: " + string.Join(',', colIDs));
        Debug.Log("rows: " + string.Join(',', rowIDs));
    }

    public CSVReader(TextAsset textAsset)
    {
        if (textAsset != null)
            Parse(textAsset.text);
    }

    public CSVReader(string content = null)
    {
        if (!string.IsNullOrEmpty(content))
            Parse(content);
    }
}