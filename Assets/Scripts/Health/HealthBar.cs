using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] public int health;
    [SerializeField] public int maxHealth;

    [SerializeField] public Sprite emptyHeart;
    [SerializeField] public Sprite fullHeart;
    [SerializeField] public Image[] hearts;

    public Health playerHealth;

    private void Update()
    {
        health = playerHealth.health;
        maxHealth = playerHealth.maxHealth;

        for(int i = 0; i < hearts.Length; i++)
        {
            if(i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }

}
