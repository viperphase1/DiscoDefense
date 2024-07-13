using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBehavior : TowerBehavior
{
    public float rotationSpeed = 30f;

    public override void Update() {
        Vector3 distance = playerCamera.position - transform.position;
        withinRange = distance.magnitude < radius * scale;
        if (withinRange) {
            tracking();
        } else {
            notTracking();
        }
    }

    public virtual void tracking() {
        // this tower should not always face the player but slowly rotate towards the player
        // if the player stays in one spot too long then the laser will eventually line up with them and they will take damage
        // ChatGPT code for rotating to face the player at a fixed speed
        Vector3 directionToTarget = playerCamera.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
        targetRotation *= Quaternion.Euler(0f, 90f, 0f);
        float rotationStep = rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationStep);

        if (!audioSource.isPlaying) {
            audioSource.Play();
        }
    }

    public virtual void notTracking() {
        if (audioSource.isPlaying) {
            audioSource.Pause();
        }
    }
}
