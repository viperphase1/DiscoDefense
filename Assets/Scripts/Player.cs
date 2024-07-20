using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public float maxHealth = 100f;
    [HideInInspector]
    public Dictionary<string, GameObject> activePowerUps = new Dictionary<string, GameObject>();
    public Dictionary<string, StatusEffect> statusEffects = (new StatusEffectsManager()).statusEffects;
    private float health;
    private float maxScale;
    private Slider healthBar;
    private TextMeshProUGUI healthCount;
    public AudioSource audioSource;
    private Dictionary<string, Image> icons = new Dictionary<string, Image>();
    public Transform shield;
    public int bullets = 0;

    void reset() {
        health = maxHealth;
        if (PlayerPrefs.HasKey("AddHealth")) {
            health += (float) PlayerPrefs.GetInt("AddHealth");
        }
        UpdateHealthBar();
    }

    // Start is called before the first frame update
    void Start()
    {
        shield = Global.FindDeepChild(transform, "Shield");
        healthBar = GetComponentInChildren<Slider>();
        healthCount = GetComponentInChildren<TextMeshProUGUI>();
        audioSource = gameObject.AddComponent<AudioSource>();
        icons["fire"] = Global.FindDeepChild(transform, "Fire Icon").GetComponent<Image>();
        icons["poison"] = Global.FindDeepChild(transform, "Poison Icon").GetComponent<Image>();
        reset();
    }

    void Update() {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        Debug.Log("Magnitude: " + rb.velocity.magnitude);
        audioSource.pitch = Mathf.Min(0.8f, rb.velocity.magnitude / 3f);
        foreach (StatusEffect statusEffect in statusEffects.Values) {
            takeDamage(statusEffect.Update(Time.deltaTime));
            icons[statusEffect.EffectType].fillAmount = statusEffect.Level / statusEffect.Threshold;
        }
    }

    public void setSwingClip(AudioClip swingClip) {
        audioSource.clip = swingClip;
        audioSource.loop = true;
        audioSource.Play(0);
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
        if (health > maxHealth) health = maxHealth;
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        healthBar.value = health / 100f;
        healthCount.text = Mathf.RoundToInt(health).ToString();
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Exit")) {
            EventManager.TriggerEvent("reachedExit");
        }
    }

    private void OnParticleCollision(GameObject psHost)
    {
        Debug.Log("Particle collision");
        if (psHost.CompareTag("Fire"))
        {
            Debug.Log("Fire particle collision");
            statusEffects["fire"].Apply();
        }
        if (psHost.CompareTag("Poison"))
        {
            Debug.Log("Poison particle collision");
            statusEffects["poison"].Apply();
        }
    }
}
