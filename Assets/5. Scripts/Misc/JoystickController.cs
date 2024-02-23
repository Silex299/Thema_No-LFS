using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickController : MonoBehaviour
{
    //TODO check for controller first

    public bool shakeController;
    public float controllerShakeIntensity;


    private void Update()
    {
        if (shakeController)
        {
            ControllerShakeUpdate();
        }
    }




    public void StartControllerShake(bool shake)
    {
        shakeController = shake;
        if (!shake) ShakeController(0);
    }

    private void ControllerShakeUpdate()
    {
        Gamepad.current.SetMotorSpeeds(controllerShakeIntensity, controllerShakeIntensity);
    }

    public void ShakeController(float intensity = 1)
    {
        Gamepad.current.SetMotorSpeeds(intensity, intensity);
    }


}
