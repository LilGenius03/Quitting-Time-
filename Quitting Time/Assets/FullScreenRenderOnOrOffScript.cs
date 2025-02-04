using UnityEngine;
using UnityEngine.Rendering.Universal; 

public class FullScreenRenderOnOrOffScript : MonoBehaviour
{
    public FullScreenPassRendererFeature FullScreenPassRendererFeature;
    public bool FullScreenPassEnabled;

    void Start()
    {
        
        if (FullScreenPassRendererFeature == null)
        {
            Debug.LogError("FullScreenPassRendererFeature is not assigned!");
            return;
        }

       
        FullScreenPassRendererFeature.SetActive(FullScreenPassEnabled);
    }

    void Update()
    {
        
        if (FullScreenPassRendererFeature != null)
        {
            FullScreenPassRendererFeature.SetActive(FullScreenPassEnabled);
        }
    }
}

