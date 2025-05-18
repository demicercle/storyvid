using System;
using UnityEngine;
using UnityEngine.UI;

public class AspectRatioFromTexture : MonoBehaviour
{
    public RawImage rawImage;
    public Texture texture;
    public AspectRatioFitter aspectRatioFitter;
    public bool autoUpdate;
    
    [ContextMenu("Update Layout")]
    public void UpdateLayout()
    {
        if (rawImage != null)
            texture = rawImage.texture;
        
        if (texture != null)
            aspectRatioFitter.aspectRatio = (float)texture.width / (float)texture.height;
    }

    private void Update()
    {
        if (autoUpdate)
            UpdateLayout();
    }
}
