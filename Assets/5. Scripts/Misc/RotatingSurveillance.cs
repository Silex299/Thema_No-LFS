using UnityEngine;
using Sirenix.OdinInspector;
using Health;
using VLB;
using UnityEngine.Events;

public class RotatingSurveillance : MonoBehaviour
{
    #region variables


    [SerializeField, BoxGroup("Movement Params")] private float minAngle;
    [SerializeField, BoxGroup("Movement Params")] private float maxAngle;
    [SerializeField, BoxGroup("Movement Params")] private float angularSpeed;
    [SerializeField, BoxGroup("Movement Params")] private float focusSpeed;


    [SerializeField, BoxGroup("Surveillance Params")] private LayerMask raycastMask;
    [SerializeField, BoxGroup("Surveillance Params")] private float restartDelay;

    [SerializeField, BoxGroup("Visual Params")] private MeshRenderer visualRenderer;
    [SerializeField, BoxGroup("Visual Params")] private Light surveillanceLight;
    [SerializeField, BoxGroup("Visual Params")] private float defaultLightIntensity;
    [SerializeField, BoxGroup("Visual Params")] private float powerUpIntensity;

    [SerializeField, BoxGroup("Visual Params"), Space(10)] private VolumetricLightBeam beam;
    [SerializeField, BoxGroup("Visual Params")] private float defaultBeamIntensity;
    [SerializeField, BoxGroup("Visual Params")] private float powerUpBeamIntensity;

    [SerializeField, BoxGroup("Visual Params"), Space(10)] private float powerUpSpeed;
    [SerializeField, BoxGroup("Visual Params")] private float powerDownSpeed;

    [SerializeField, BoxGroup("Visual Params"), Space(10)] private Material onMaterial;
    [SerializeField, BoxGroup("Visual Params")] private Material offMaterial;
    [SerializeField, BoxGroup("Visual Params")] private int materialIndex;


    [SerializeField, BoxGroup("Sound")] private AudioSource machineSource;
    [SerializeField, BoxGroup("Sound")] private AudioSource rumbleSoundSource;

    [SerializeField, BoxGroup("Sound"), Space(10)] private AudioSource machineSfxSoruce;
    [SerializeField, BoxGroup("Sound")] private AudioClip machineStartSound;
    [SerializeField, BoxGroup("Sound")] private AudioClip machineStopSound;
    [SerializeField, BoxGroup("Sound")] private AudioClip machinePowerUpSound;

    [SerializeField, BoxGroup("Misc")] private bool surveil = true;
    [SerializeField, BoxGroup("Misc")] private float machineTransitionTime = 2f;

    [SerializeField, BoxGroup("Exteral Actions")] private UnityEvent onItemDetected;
    [SerializeField, BoxGroup("Exteral Actions")] private UnityEvent onMachineRestart;


    private bool _powerUp;
    private bool _normalPower;
    private bool _powerDown;

    private bool _focus;
    private Quaternion _focusRotation;
    private float _currentAngularSpeed;

    #endregion



    private void OnTriggerStay(Collider other)
    {
        if (!surveil) return;

        //REMOVE DEBUG
        if (other.CompareTag("Player_Main") || other.CompareTag("Interactable"))
        {
            //var position = transform.position;
            var position = surveillanceLight.transform.position;
            var direction = (other.transform.position + new Vector3(0, 0.5f, 0) - surveillanceLight.transform.position).normalized;
            var ray = new Ray(position, direction);


            if (Physics.Raycast(ray, out RaycastHit info, Mathf.Infinity, raycastMask))
            {
                Debug.DrawLine(ray.origin, info.point, Color.green, 1f);
                print(info.collider.name);

                if (info.collider.CompareTag("Player_Main") || info.collider.CompareTag("Interactable"))
                {
                    Debug.LogWarning(info.collider.tag);
                    if (other.TryGetComponent<HealthBaseClass>(out HealthBaseClass health))
                    {
                        health.Kill("RAY");
                        ItemDetected(other.transform.position);
                    }
                }

            }
            else
            {
                Debug.DrawLine(ray.origin, ray.direction * 100f, Color.red, 1f);
            }

        }
        else if (other.CompareTag("Reactive_Interactable"))
        {
            if (other.TryGetComponent<Misc.Items.ReactiveInteractable>(out var interactable))
            {
                interactable.Interact(true);
            }
        }
    }



    private void Start()
    {
        _currentAngularSpeed = angularSpeed;
    }

    private void Update()
    {
        if (surveil)
        {
            Rotate();
        }
        else if (_focus)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _focusRotation, Time.deltaTime * focusSpeed);
        }

        if (_powerUp)
        {
            PowerUp();
        }
        else if (_normalPower)
        {
            NormalPower();
        }
        else if (_powerDown)
        {
            PowerDown();
        }
    }


    private void Rotate()
    {
        var angle = transform.localEulerAngles.y;

        if (angle > 180)
        {
            angle -= 360;
        }

        if (angle > maxAngle)
        {
            _currentAngularSpeed = -angularSpeed;
        }
        else if (angle < minAngle)
        {
            _currentAngularSpeed = angularSpeed;
        }


        transform.Rotate(Vector3.up, _currentAngularSpeed, Space.World);

    }



    private void ItemDetected(Vector3 itemPosition)
    {
        surveil = false;

        onItemDetected?.Invoke();
        Focus(itemPosition);//FOCUS
        PowerUp();//POWER 

        Invoke("RestartMachine", restartDelay);
    }

    //TODO: merge RestartMachine and TurnOffMachine if possible
    private void RestartMachine()
    {
        print("Restart MACHINE");
        surveil = true;
        onMachineRestart?.Invoke();
        _focus = false;
        _powerUp = false;
        NormalPower();
    }

    private void Focus(Vector3 itemPosition)
    {
        if (!_focus)
        {

            //UP is looking at the object
            Vector3 v1 = -transform.up;
            v1.y = 0;
            Vector3 v2 = itemPosition - transform.position;
            v2.y = 0;

            var focusAngle = Vector3.SignedAngle(v1, v2, Vector3.up);

            Vector3 euler = transform.eulerAngles;
            euler.y += focusAngle;
            _focusRotation = Quaternion.Euler(euler);

            _focus = true;
        }
    }



    private float _machineTransitionTimeElapsed;
    private void PowerUp()
    {
        if (!_powerUp)
        {
            _powerUp = true;
            _machineTransitionTimeElapsed = 0;
            if (machineSfxSoruce)
            {
                machineSfxSoruce.PlayOneShot(machinePowerUpSound);
            }
        }
        else
        {
            _machineTransitionTimeElapsed += Time.deltaTime;
            float fraction = _machineTransitionTimeElapsed / machineTransitionTime;

            surveillanceLight.intensity = Mathf.Lerp(defaultLightIntensity, powerUpIntensity, fraction);
            beam.intensityGlobal = Mathf.Lerp(defaultBeamIntensity, powerUpBeamIntensity, fraction);

            if (fraction >= 1)
            {
                _powerUp = false;
            }
        }

    }

    private void NormalPower(bool fromZero = false)
    {

        if (!_normalPower)
        {
            surveillanceLight.gameObject.SetActive(true);
            _normalPower = true;
            _powerDown = false;
            _machineTransitionTimeElapsed = 0;

            if (machineSfxSoruce && fromZero)
            {
                machineSfxSoruce.PlayOneShot(machineStartSound);
                machineSource.Play();
            }
        }
        else
        {
            _machineTransitionTimeElapsed += Time.deltaTime;
            float fraction = _machineTransitionTimeElapsed / machineTransitionTime;

            surveillanceLight.intensity = Mathf.Lerp(fromZero ? 0 : surveillanceLight.intensity, defaultLightIntensity, fraction);
            beam.intensityGlobal = Mathf.Lerp(fromZero ? 0 : beam.intensityGlobal, defaultBeamIntensity, fraction);

            
            if(machineSource) machineSource.volume = 1;
            if(rumbleSoundSource) rumbleSoundSource.volume = 1;

            if (fraction >= 1)
            {
                _normalPower = false;
            }
        }
    }


    private void PowerDown()
    {
        if (!_powerDown)
        {
            _powerDown = true;
            _normalPower = false;
            _machineTransitionTimeElapsed = 0;

            if (machineSfxSoruce)
            {
                machineSfxSoruce.PlayOneShot(machineStopSound);
            }

        }
        else
        {
            _machineTransitionTimeElapsed += Time.deltaTime;

            float fraction = _machineTransitionTimeElapsed / machineTransitionTime;

            surveillanceLight.intensity = Mathf.Lerp(defaultLightIntensity, 0, fraction);
            beam.intensityGlobal = Mathf.Lerp(defaultBeamIntensity, 0, fraction);
            machineSource.volume = Mathf.Lerp(1, 0, fraction);
            rumbleSoundSource.volume = Mathf.Lerp(1, 0, fraction);

            if (fraction >= 1)
            {
                _powerDown = false;
                surveillanceLight.gameObject.SetActive(false);
                print(fraction + "DISABLING");
                machineSource.Stop();
            }
        }

    }

    public void TurnOffMachine(bool turnOff)
    {
        if (turnOff == !surveil) return;

        surveil = !turnOff;
        MeshRenderer Renderer;

        if (visualRenderer == null)
        {
            Renderer = GetComponent<MeshRenderer>();
        }
        else
        {
            Renderer = visualRenderer;
        }


        if(Renderer != null)
        {
            var materials = Renderer.materials;

            if (turnOff)
            {
                PowerDown();
                materials[materialIndex] = offMaterial;
            }
            else
            {
                surveillanceLight.gameObject.SetActive(true);
                NormalPower(true);
                materials[materialIndex] = onMaterial;
            }
            Renderer.materials = materials;
        }
        else
        {
            if (turnOff)
            {
                PowerDown();
            }
            else
            {
                surveillanceLight.gameObject.SetActive(true);
                NormalPower(true);
            }
        }

        
    }


}



