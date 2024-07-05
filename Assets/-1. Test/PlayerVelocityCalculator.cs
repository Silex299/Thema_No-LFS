using UnityEngine;

public class PlayerVelocityCalculator : MonoBehaviour
{
    public float calculationInterval = 0.2f;
    public Vector3 velocity;
    
    public static PlayerVelocityCalculator Instance;
    
    
    private float _lastCalculationTime;
    private Vector3 _lastPosition;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    private void Update()
    {
        CalculateVelocity();
    }


    private void CalculateVelocity()
    {
        
        //Return if the time since the last calculation is less than the interval
        if (Time.time - _lastCalculationTime < calculationInterval) return;
        
        velocity = (transform.position - _lastPosition) / (Time.time - _lastCalculationTime);
        _lastPosition = transform.position;
        _lastCalculationTime = Time.time;
    }
    
    
}
