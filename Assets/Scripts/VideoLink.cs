[System.Serializable]
public struct VideoLink
{
    public int episode;
    public string videoFrom;
    public string videoTo;
    public string text;
    public int points;
    public int id;

    public bool EpisodeComplete() => string.IsNullOrEmpty(videoTo);

    public override string ToString()
    {
        return "VideoLink (" + episode + ", " + videoFrom + ">" + videoTo + ")";
    }
}