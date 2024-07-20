using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerUpBehavior : PowerUpBehavior
{
    private float startTime = Mathf.Infinity;
    // Start is called before the first frame update
    protected override void ApplyPowerUp()
    {
        Debug.Log("Apply Shield Power Up");
        startTime = Time.time;
        player.shield.gameObject.SetActive(true);
    }

    void Update() {
        if (Time.time - startTime > duration) {
            Debug.Log("Shield expired");
            player.shield.gameObject.SetActive(false);
            resolve();
        }
    }
}
