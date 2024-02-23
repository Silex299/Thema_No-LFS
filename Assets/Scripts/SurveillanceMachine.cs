using System;
using System.Collections;
using Health;
using MyCamera;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using VLB;
using JetBrains.Annotations;

[RequireComponent(typeof(Collider))]
public class SurveillanceMachine : MonoBehaviour
{
    #region Variables

    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private bool surveillanceStatus = true;
    [SerializeField] private MeshRenderer meshRenderer;


    [BoxGroup("Surveillance components"), SerializeField]
    private Light surveillanceLight;

    [BoxGroup("Surveillance components"), SerializeField]
    private VolumetricLightBeam vlb;

    [BoxGroup("Surveillance components"), SerializeField]
    private Color activeColor;

    [BoxGroup("Surveillance components"), SerializeField]
    private Color deactivatedColor;

    [BoxGroup("Status components"), SerializeField]
    private Light statusLight;

    [BoxGroup("Status components"), SerializeField]
    private Material surveillanceOnMaterial;

    [BoxGroup("Status components"), SerializeField]
    private Material surveillanceOffMaterial;

    [SerializeField, BoxGroup("Flicker")] private bool flickerDisable;

    [SerializeField, BoxGroup("Flicker"), HideIf("flickerDisable")]
    private float activeDuration = 2f;

    [SerializeField, BoxGroup("Flicker"), HideIf("flickerDisable")]
    private float inactiveDuration = 5f;


    private float _timer;

    #endregion

    #region Unity Methods

    private void Update()
    {
        if (!flickerDisable) FlickerSurveillanceMachine();
    }

    private void OnTriggerStay(Collider other)
    {
        CheckForPlayerOrInteractable(other);
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// Flicker the surveillance machine according to active and inactive duration
    /// </summary>
    private void FlickerSurveillanceMachine()
    {
        _timer += Time.deltaTime;

        switch (surveillanceStatus)
        {
            case true when _timer >= activeDuration:
                ChangeSurveillanceStatus(false);
                _timer = 0;
                break;
            case false when _timer >= inactiveDuration:
                ChangeSurveillanceStatus(true);
                _timer = 0;
                break;
        }
    }


    /// <summary>
    /// Changes the status of the surveillance machine
    /// </summary>
    /// <param name="status"> status of the surveillance machine </param>
    /// <param name="time"> time after which machine should restart (if status is false)</param>
    /// <param name="onMachineRestart"> calls back on completing actions </param>
    public bool ChangeSurveillanceStatus(bool status, float time, [CanBeNull] Action onMachineRestart)
    {
        if(surveillanceStatus == status) return  false;
        surveillanceStatus = status;
        SurveillanceStatus();

        //Restart the surveillance machine if it is turned off and time is not zero
        if (!status && time != 0) StartCoroutine(RestartSurveillance(time, onMachineRestart));
        
        return true;
    }
  

    public void ChangeSurveillanceStatus(bool status)
    {
        ChangeSurveillanceStatus(status, 0, null);
    }


    /// <summary>
    /// Changes if the surveillance machine is flickering
    /// </summary>
    /// <param name="enable"></param>
    public void ChangeFlickerStatus(bool enable)
    {
        flickerDisable = !enable;
    }


    /// <summary>
    /// Restarts the surveillance machine after certain time
    /// </summary>
    /// <param name="time"> time in seconds after which the machine should restart</param>
    /// <param name="onMachineRestart"> callback on machine restart </param>
    /// <returns></returns>
    private IEnumerator RestartSurveillance(float time, [CanBeNull] Action onMachineRestart)
    {
        yield return new WaitForSeconds(time);

        if (surveillanceStatus) yield return null;
        surveillanceStatus = true;

        onMachineRestart?.Invoke();
        SurveillanceStatus();
    }

    private void CheckForPlayerOrInteractable(Collider other)
    {
        if (!surveillanceStatus) return;

        if (!other.CompareTag("Player_Main") && !other.CompareTag("Interactable")) return;

        if (Physics.Linecast(transform.position, other.transform.position + 0.5f * Vector3.up, out var hit,
                raycastMask))
        {
            Debug.DrawLine(transform.position, hit.point, Color.green);

            if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Player_Main"))
            {
                ChangeSurveillanceStatus(true);
                ChangeFlickerStatus(false);
                StartCoroutine(PowerUpMachine(2f, 4f));

                //TODO: Check for convenience if this can be put under DeathByGodRay function
                CameraManager.Instance.ShakeCamera(true);
                CameraManager.Instance.Focus(PlayerController.Instance.transform);

                PlayerController.Instance.Player.Health.DeathByGodRay();
            }

            else if (hit.collider.CompareTag("Interactable"))
            {
                if (!hit.collider.TryGetComponent<Health.HealthBase>(out HealthBase health)) return;

                health.DeathByGodRay();

                CameraManager.Instance.ShakeCamera(true);
                CameraManager.Instance.Focus(hit.collider.transform);

                ChangeSurveillanceStatus(true);
                ChangeFlickerStatus(false);
                StartCoroutine(PowerUpMachine(2f, 4f));
            }
        }
        else
        {
            Debug.DrawLine(transform.position, other.transform.position + 0.5f * Vector3.up, Color.red);
        }
    }


    private IEnumerator PowerUpMachine(float duration, float targetIntensity)
    {
        float startIntensity = vlb.intensityGlobal;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float newIntensity = Mathf.Lerp(startIntensity, targetIntensity, t);
            vlb.intensityGlobal = newIntensity;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // yield return new WaitForSeconds(2.5f);


        // vlb.intensityGlobal = 0.57f;
    }

    /// <summary>
    /// Updates visual queues for the surveillance machine according to @surveillanceStatus
    /// </summary>
    private void SurveillanceStatus()
    {
        if (surveillanceStatus)
        {
            UpdateSurveillanceVisuals(surveillanceOnMaterial, activeColor, 1000, true, Color.green);
        }
        else
        {
            UpdateSurveillanceVisuals(surveillanceOffMaterial, deactivatedColor, 100, false, Color.red);
        }
    }
    
    
    
    /// <summary>
    /// Called to update surveillance machine visuals
    /// </summary>
    /// <param name="statusMaterial"> emission material responsible for surveillance status </param>
    /// <param name="lightColor"> surveillance light color </param>
    /// <param name="lightIntensity"> surveillance light intensity </param>
    /// <param name="vlbEnabled"> if volumetric light is enabled or not </param>
    /// <param name="statusLightColor"> color of the light that is responsible for surveillance status </param>
    private void UpdateSurveillanceVisuals(Material statusMaterial, Color lightColor, float lightIntensity,
        bool vlbEnabled, Color statusLightColor)
    {
        if (meshRenderer && statusMaterial)
        {
            var materials = meshRenderer.materials;
            materials[1] = statusMaterial;
            meshRenderer.materials = materials;
        }

        surveillanceLight.color = lightColor;
        surveillanceLight.intensity = lightIntensity;
        vlb.enabled = vlbEnabled;
        statusLight.color = statusLightColor;
    }

    #endregion
}
