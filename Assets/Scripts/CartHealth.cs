using System.Collections;
using Health;
using Managers;
using MyCamera;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

public class CartHealth : HealthBase
{
    [SerializeField, BoxGroup("References")]
    private Animator animator;

    [SerializeField, BoxGroup("References")]
    private PlayableDirector director;


    [SerializeField, BoxGroup("Cart Explode")]
    private GameObject explodedCart;

    public override void DeathByGodRay()
    {
        director.Pause();
        animator.Play("Cart Explode");
        
        StartCoroutine(ExplodeCart());
    }

    private IEnumerator ExplodeCart()
    {
        yield return new WaitForSeconds(5f);

        if (explodedCart)
        {
            PlayerController.Instance.StartCoroutine(PlayerController.Instance.Player.Health.DeathByAnimation("Flying back death", 1f));
            var obj = Instantiate(explodedCart, GameManager.instance.transform);

            var transform1 = transform;
            obj.transform.position = transform1.position;
            obj.transform.rotation = transform1.rotation;

            Destroy(gameObject);
        }
    }
}