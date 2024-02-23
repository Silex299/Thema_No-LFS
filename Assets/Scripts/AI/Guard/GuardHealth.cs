using Health;
using UnityEngine;

public class GuardHealth : HealthBase
{


    private void OnCollisionEnter(Collision collision)
    {
        print(collision.impulse.magnitude);
    }


}
