using System;
using UnityEngine;
using UnityEngine.UI;

public class AspectRatioFromTexture : MonoBehaviour
{
    public RawImage rawImage;
    public AspectRatioFitter aspectRatioFitter;
    public bool autoUpdate;
    
    [ContextMenu("Update Layout")]
    public void UpdateLayout()
    {
        if (rawImage.texture != null)
            aspectRatioFitter.aspectRatio = (float)rawImage.texture.width / (float)rawImage.texture.height;
    }

    private void Update()
    {
        if (autoUpdate)
            UpdateLayout();
    }
}
