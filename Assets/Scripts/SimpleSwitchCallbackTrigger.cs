using System;
using UnityEngine;

public class SimpleSwitchCallbackTrigger : SimpleSwitchTrigger   
{
    
    [SerializeField] private bool isActive;
    
    
    protected override void TriggerEvents()
    {
        if(!isActive) return;
        
        status = !status; 
        isActive = false;
        action.Execute(new object[] {status, floatInput, new Action(MachineReset)});
    }
    
    
    private void MachineReset()
    {
        status = true;
        isActive = true;
        SwitchVisual();
    }
    
    /// <summary>
    /// <param name="offSwitch"> bool, if call param is true, it swicthes off the trigger </param>
    ///</summary>
    public void OffSwitch(bool offSwitch)
    {
        status = offSwitch;
        isActive = offSwitch;
        SwitchVisual(); 
    }
    
    
}
