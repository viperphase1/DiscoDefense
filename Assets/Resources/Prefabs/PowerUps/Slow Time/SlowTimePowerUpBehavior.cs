using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTimePowerUpBehavior : PowerUpBehavior
{
    private float minTimeScale = 0.5f;
    private float minPitch = 0.5f;
    private float myDeltaTime;
    private float speed = .4f;
    private string action = "hold";
    private float startTime = Mathf.Infinity;

    void Start() {
        myDeltaTime = Time.deltaTime; 
    }

    void Update() {
        if (Time.time - startTime > duration) {
            action = "speedUp";
        } 
        // the slow time powerup is extended by increasing the duration
        // this may happen when the powerup is on it's final transition
        // in this case we just need to set the action back to "slowDown"
        else if (action == "speedUp") {
            action = "slowDown";
        }
        if (action == "slowDown") {
            slowDown();
        }
        if (action == "speedUp") {
            speedUp();
        }
    }

    void slowDown() {
        AudioSource audioSrc = player.GetComponent<AudioSource>();
        Time.timeScale = Mathf.MoveTowards(Time.timeScale, minTimeScale, myDeltaTime * speed);
        audioSrc.pitch = Mathf.MoveTowards(audioSrc.pitch, minPitch, myDeltaTime * speed);
        if (Time.timeScale == minTimeScale) {
            action = "hold";
            startTime = Time.time;
        }
    }

    void speedUp() {
        AudioSource audioSrc = player.GetComponent<AudioSource>();
        Time.timeScale = Mathf.MoveTowards(Time.timeScale, 1.0f, myDeltaTime * speed);
        audioSrc.pitch = Mathf.MoveTowards(audioSrc.pitch, 1.0f, myDeltaTime * speed);
        if (Time.timeScale == 1.0f) {
            resolve();
        }
    }

    protected override void ApplyPowerUp() {
        action = "slowDown";
    }
}
