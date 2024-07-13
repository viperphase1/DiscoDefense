using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBehavior : MonoBehaviour
{
    [HideInInspector]
    public float damage = 1f;
    [HideInInspector]
    public Player player;
    protected bool destroyInProgress = false;
    private int ricochet = 0;
    private int bounces = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Modifiers_Ricochet")) {
            ricochet = PlayerPrefs.GetInt("Modifiers_Ricochet");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision) {
        if (destroyInProgress) return;
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player Camera")) {
            Debug.Log("Ammo/Player Collision");
            OnDestroy();
            player.takeDamage(damage);
            // TODO: hit animation
            // TODO: hit sfx
        }
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Structures")) {
            if (ricochet == 0 || bounces > ricochet) {
                Debug.Log("Ammo/Wall Collision");
                OnDestroy();
            } else {
                bounces++;
            }
        }
    }

    public virtual void OnDestroy() {
        destroyInProgress = true;
        Destroy(gameObject);
    }
}
