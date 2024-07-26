using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Global;

public class SlowTimePowerUpBehavior : PowerUpBehavior
{
    private float minTimeScale = 0.75f;
    private float myDeltaTime;
    private float speed = .2f;
    private string action = "hold";
    private float startTime = Mathf.Infinity;
    private AudioSource audioSrc;

    void Start() {
        myDeltaTime = Time.deltaTime;
        audioSrc = mm.audioSource;
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
        Time.timeScale = Mathf.MoveTowards(Time.timeScale, minTimeScale, myDeltaTime * speed);
        audioSrc.pitch = Mathf.MoveTowards(audioSrc.pitch, minTimeScale, myDeltaTime * speed);
        if (Time.timeScale == minTimeScale) {
            action = "hold";
            startTime = Time.time;
        }
    }

    void speedUp() {
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
