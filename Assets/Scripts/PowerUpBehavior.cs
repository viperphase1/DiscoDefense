using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBehavior : MonoBehaviour
{
    protected Player player;
    public float duration = 0f;
    public string slug = "PowerUp";
    public bool claimed = false;
    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
            Debug.Log("PowerUp/Player Collision");
            claimed = true;
            player = collider.gameObject.GetComponent<Player>();
            if (player.activePowerUps.ContainsKey(slug) && duration > 0f) { 
                player.activePowerUps[slug].GetComponent<PowerUpBehavior>().duration += duration;
                Destroy(gameObject);
            } else {
                player.activePowerUps[slug] = gameObject;
                ApplyPowerUp();
            }
            // make it so the powerup cannot be triggered again but keep it around for powerups that have a duration
            collider.enabled = false;
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers) {
                renderer.enabled = false;
            }
        }
    }

    protected void resolve() {
        if (player.activePowerUps.ContainsKey(slug)) {
            player.activePowerUps.Remove(slug);
        }
        Destroy(gameObject);
    }

    protected virtual void ApplyPowerUp() {}
}
