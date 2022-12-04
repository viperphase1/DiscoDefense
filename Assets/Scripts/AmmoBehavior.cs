using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBehavior : MonoBehaviour
{
    public float damage = 1f;
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
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
            Destroy(gameObject);
            var player = collision.collider.gameObject.GetComponent<Player>();
            player.health -= damage;
            // TODO: hit animation
            // TODO: hit sfx
        }
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Structure")) {
            if (ricochet == 0 || bounces > 2) {
                Destroy(gameObject);
            } else {
                bounces++;
            }
        }
    }
}
