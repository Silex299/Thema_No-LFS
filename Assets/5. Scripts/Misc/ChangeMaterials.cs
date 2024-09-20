using UnityEngine;

namespace Misc
{
    public class ChangeMaterials : MonoBehaviour
    {

        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material highlightMaterial;
        [SerializeField] private int materialIndex;
    
        //Create a function with a bool parameter to change the material with index materialIndex to the defaultMaterial or highlightMaterial based on the bool parameter value
        public void ChangeMaterial(bool isHighlighted)
        {
            //Get the renderer component of the object
            var renderer = GetComponent<Renderer>();
            //Check if the renderer is not null
            if (renderer == null) return;
        
            //Get the materials array from the renderer
            var materials = renderer.materials;
            //Check if the materialIndex is within the bounds of the materials array
            if (materialIndex >= 0 && materialIndex < materials.Length)
            {
                //Set the material at the materialIndex to the defaultMaterial or highlightMaterial based on the isHighlighted parameter
                materials[materialIndex] = isHighlighted ? highlightMaterial : defaultMaterial;
                //Assign the modified materials array back to the renderer
                renderer.materials = materials;
            }
        }
    

    }
}
