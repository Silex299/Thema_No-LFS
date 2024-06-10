using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.Rendering.Universal;

public class RendererController : MonoBehaviour
{
    public UniversalRendererData[] rendererData;

    public void EnableFullScreenRenderer(bool active)
    {

        print(active);
        
        foreach (var data in rendererData)
        {
            //find feature with name "FullScreenRenderer"
            data.rendererFeatures.OfType<FullScreenPassRendererFeature>().FirstOrDefault()?.SetActive(active);
        }
        
    }
}