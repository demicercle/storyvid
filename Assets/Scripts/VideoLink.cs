[System.Serializable]
public struct VideoLink
{
    public string name;
    public int episode;
    public string videoFrom;
    public string videoTo;
    public string text;
    public int points;
    public int id;
    public bool completeEpisode;

    public override string ToString()
    {
        return "VideoLink(" + episode + ", " + videoFrom + ">" + videoTo + ")";
    }
}