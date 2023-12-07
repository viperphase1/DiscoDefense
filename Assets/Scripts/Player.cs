using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float maxHealth = 100f;
    private float health;
    public Slider healthBar;
    public Dictionary<string, GameObject> activePowerUps = new Dictionary<string, GameObject>();

    void reset() {
        health = maxHealth;
        if (PlayerPrefs.HasKey("AddHealth")) {
            health += (float) PlayerPrefs.GetInt("AddHealth");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        reset();
    }

    public void takeDamage(float damage) {
        Debug.Log("Hit");
        health -= damage;
        UpdateHealthBar();
        if (health <= 0) {
            EventManager.TriggerEvent("healthDepleted");
        }
    }

    public void addHealth(float amount) {
        health += amount;
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        healthBar.value = health / maxHealth;
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Exit")) {
            EventManager.TriggerEvent("reachedExit");
        }
    }
}
