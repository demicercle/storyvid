using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CSVReader
{
    private const char SEPARATOR = '\t';
    
    public TextAsset textAsset;
    
    public List<string> colIDs;
    public List<string> rowIDs;
    public List<List<string>> csv;

    public string GetCellContent(string rowID, string columnID)
    {
        var r = rowIDs.IndexOf(rowID);
        if (r < 0) Debug.LogError("Cannot find cell with rowID=" + rowID);
        var c = colIDs.IndexOf(columnID);
        if (c < 0) Debug.LogError("Cannot find cell with columnID=" + columnID);
        Debug.Log("GetCellContent " + rowID + "," + columnID + " = " + c + "/" + r);
        return GetCellContent(r,c);
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
                    colIDs.Add(csv[r][c].Trim());
                }
                
                else if (c == 0)
                {
                    rowIDs.Add(csv[r][c].Trim());
                }
            }
        }
        
        Debug.Log("CSVReader Parsed");
        Debug.Log("cols: " + string.Join(',', colIDs));
        Debug.Log("rows: " + string.Join(',', rowIDs));
        PrintCSV();
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