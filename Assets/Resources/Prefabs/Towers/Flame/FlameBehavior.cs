using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameBehavior : FollowBehavior
{
    private ParticleSystem flamePS;

    public override void Start() {
        base.Start();
        flamePS = spawnPoint.GetComponent<ParticleSystem>();
    }

    public override void tracking() {
        base.tracking();
        flamePS.Play();
    }

    public override void notTracking() {
        base.notTracking();
        flamePS.Stop();
    }
}
