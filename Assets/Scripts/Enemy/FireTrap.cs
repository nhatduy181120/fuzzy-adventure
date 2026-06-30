using System.Collections;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    [SerializeField] private int damage;
    [Header("Firetrap Timers")]
    [SerializeField] private float activationDelay;
    [SerializeField] private float activeTime;
    private Animator anim;
    private SpriteRenderer spriteRend;

    private bool triggered;
    private bool active;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if(!triggered)
            {
                StartCoroutine(ActiveFiretrap());
            }
            if (active)
                collision.GetComponent<Health>().TakeDamage(damage);
        }
    }
    
    private IEnumerator ActiveFiretrap()
    {
        triggered = true;
        spriteRend.color = Color.red;

        yield return new WaitForSeconds(activationDelay);
        spriteRend.color = Color.white;
        active = true;
        anim.SetBool("Activated", true);

        yield return new WaitForSeconds(activeTime);
        active = false;
        triggered = false;
        anim.SetBool("Activated", false);
    }
}
