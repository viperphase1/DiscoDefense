using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPowerUpBehavior : PowerUpBehavior
{
    protected override void ApplyPowerUp() {
        player.addHealth(25f);
        resolve();
    }
}
