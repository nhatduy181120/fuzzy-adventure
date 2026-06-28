using UnityEngine;

public class Health : MonoBehaviour
{
    public int health;
    public int maxHealth;
    private Animator anim;
    private bool dead;
    private void Awake()
    {
        health = maxHealth;
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        
    }

    public void TakeDamage(int amount)
    { 
        health -= amount;
        
        if(health > 0)
        {
            anim.SetTrigger("Hurt");
        }
        else
        {
            if(!dead)
            {
                anim.SetTrigger("Die");
                GetComponent<PlayerMovement>().enabled = false;
                dead = true;
            }
        }
    }

    public void AddHealth(int value)
    {
        health += value;
    }
}
