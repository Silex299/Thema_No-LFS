using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PlayerHang : MonoBehaviour
{
    [SerializeField] private Transform movePlayerTo;

    private bool _actOnPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player_Main"))
        {
            _actOnPlayer = true;
        }
    }

    private void Update()
    {
        if (!_actOnPlayer) return;
        Player_Scripts.PlayerController
    }


}
