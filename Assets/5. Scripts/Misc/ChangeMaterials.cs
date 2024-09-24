using System;
using UnityEngine;

namespace Misc
{
    public class ChangeMaterials : MonoBehaviour
    {

        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material highlightMaterial;
        [SerializeField] private int materialIndex;


        private Renderer _renderer;

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
        }


        //Create a function with a bool parameter to change the material with index materialIndex to the defaultMaterial or highlightMaterial based on the bool parameter value
        public void ChangeMaterial(bool isHighlighted)
        {
            //Check if the renderer is not null
            if (!_renderer) return;
        
            //Get the materials array from the renderer
            var materials = _renderer.materials;
            //Check if the materialIndex is within the bounds of the materials array
            if (materialIndex >= 0 && materialIndex < materials.Length)
            {
                //Set the material at the materialIndex to the defaultMaterial or highlightMaterial based on the isHighlighted parameter
                materials[materialIndex] = isHighlighted ? highlightMaterial : defaultMaterial;
                //Assign the modified materials array back to the renderer
                _renderer.materials = materials;
            }
        }
    

    }
}
