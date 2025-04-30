using System;
using UnityEngine;
using UnityEngine.UI;

public class AspectRatioFromTexture : MonoBehaviour
{
    public RawImage rawImage;
    public AspectRatioFitter aspectRatioFitter;
    
    [ContextMenu("Update Layout")]
    public void UpdateLayout()
    {
        aspectRatioFitter.aspectRatio = (float)rawImage.texture.width / (float)rawImage.texture.height;
    }

    private void Start()
    {
        UpdateLayout();
    }
}
