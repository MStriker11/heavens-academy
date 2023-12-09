using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    // make singleton
    public static PlayerHealth instance;

    public float maxHealth = 100f;
    public float healRate = 4f;
    public Image healthBar;
    private float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + healRate * Time.deltaTime);
        updateHealthGraphic();
    }

    public bool takeDamage(float damage)
    {
        if (currentHealth >= damage)
        {
            currentHealth -= damage;
            updateHealthGraphic();
            return true;
        }
        return false;
    }

    void updateHealthGraphic()
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }
}
