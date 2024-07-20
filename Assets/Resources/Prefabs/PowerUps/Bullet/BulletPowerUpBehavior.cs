using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPowerUpBehavior : PowerUpBehavior
{
    protected override void ApplyPowerUp() {
        player.bullets += 1;
    }
}
