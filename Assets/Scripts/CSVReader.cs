using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CSVReader
{
    private const char SEPARATOR = '\t';
    
    public TextAsset textAsset;
    
    public List<List<string>> csv;

    public string GetCellContent(string rowID, string columnID)
    {
        var colIndex = csv[0].IndexOf(columnID);
        if (colIndex < 0)
        {
            Debug.LogError("Cannot find column with ID: " + columnID);
            return string.Empty;
        }

        foreach (List<string> row in csv)
        {
            if (row[0] == rowID)
            {
                return row[colIndex];
            }
        }
        
        Debug.LogError("Cannot find row with ID: " + rowID);
        return string.Empty;
    }

    public string[] GetRowIDs(int startIndex = 1)
    {
        List<string> ids = new List<string>();
        for (int i = startIndex; i < csv.Count; i++)
        {
            ids.Add(csv[i][0]);
        }

        return ids.ToArray();
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

        var rows = text.Split('\n');
        for (int r = 0; r < rows.Length; r++)
        {
            csv.Add(new List<string>(rows[r].Split(SEPARATOR)));
        }
    }

    private void PrintCSV()
    {
        int r = 0;
        string output = "";
        csv.ForEach(row =>
        {
            output += r.ToString() + "\t";
            row.ForEach(cell =>
            {
                output += cell + "\t";
            });
            output += "\n";
            r++;
        });
        Debug.Log(output);
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