using UnityEngine;
using UnityEngine.UI;

public class npcA : MonoBehaviour
{
    public int maxHealth = 100;
    public int health;

    public Text npcHPText; 

    void Start()
    {
        health = maxHealth;
        UpdateHPUI(); 
    }

    public void TakeDamage (int damage)
    {
        health -= damage;

        if (health < 0)
            health = 0;

        UpdateHPUI();

        if (health <= 0)
            Die(); 
    }
    
    void UpdateHPUI()
    {
        if (npcHPText != null)
        {
            npcHPText.text = "HP: " + health;
        }
    }

    void Die()
    {
        gameObject.SetActive(false);
    }
}
