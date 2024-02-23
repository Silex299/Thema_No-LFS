using System;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SimpleSwitchTrigger : MonoBehaviour
{
    [SerializeField, FoldoutGroup("Function")]
    protected string inputString;

    [SerializeField, FoldoutGroup("Function"), Space(20)]
    protected ConnectedObjectAction action;

    protected bool PlayerIsInTrigger;

    [SerializeField, FoldoutGroup("Input"), InfoBox("Status toggles everytime you call it")]
    protected bool status = true;

    [SerializeField, FoldoutGroup("Input")]
    protected float floatInput;


    [SerializeField, FoldoutGroup("Visual Representation")]
    private MeshRenderer meshRenderer;

    [SerializeField, FoldoutGroup("Visual Representation")]
    private Material onMaterial;

    [SerializeField, FoldoutGroup("Visual Representation")]
    private Material offMaterial;

    // ReSharper disable once InconsistentNaming
    protected bool _switchVisual;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player_Main"))
        {
            PlayerIsInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player_Main"))
        {
            PlayerIsInTrigger = false;
        }
    }

    private void Start()
    {
        _switchVisual = status;
        SwitchVisual();
    }

    protected virtual void Update()
    {
        if (status != _switchVisual)
        {
            SwitchVisual();
        }

        if (!PlayerIsInTrigger) return;

        if (Input.GetButtonDown(inputString))
        {
            TriggerEvents();
        }
    }


    protected virtual void TriggerEvents()
    {
        if(action.Execute(new object[] { status, floatInput }))
        {
            status = !status;
        }
        
    }

    protected void SwitchVisual()
    {
        _switchVisual = status;
        var materials = meshRenderer.materials;
        materials[1] = status ? onMaterial : offMaterial;
        meshRenderer.materials = materials;
    }

    [Serializable]
    public class ConnectedObjectAction
    {
        public string actionName;
        public Component connectedComponent;


        public bool Execute(object[] parameters)
        {
            var type = connectedComponent.GetType();

            Type[] parmaterType = new Type[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                parmaterType[i] = parameters[i].GetType();
            }

            var method = type.GetMethod(actionName, parmaterType);


            if (method == null)
            {
                Debug.LogError("Method doesn't exist");
                return false;
            }

            var result = (bool)method.Invoke(connectedComponent, parameters);
            
            return result;
            
        }
    }
}