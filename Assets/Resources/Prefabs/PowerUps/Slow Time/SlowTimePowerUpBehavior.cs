using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTimePowerUpBehavior : PowerUpBehavior
{
    private float minTimeScale = 0.75f;
    private float myDeltaTime;
    private float speed = .4f;
    private string action = "hold";
    private float startTime = Mathf.Infinity;
    private AudioSource audioSrc;

    void Start() {
        myDeltaTime = Time.deltaTime;
        audioSrc = GameObject.Find("Root").GetComponent<Endless>().mm.audioSource;
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
        if (audioSrc) {
            Debug.Log("SlowTimeParams: " + Time.timeScale + ", " + audioSrc.pitch);
        }
    }

    void slowDown() {
        Debug.Log("slowDown");
        Time.timeScale = Mathf.MoveTowards(Time.timeScale, minTimeScale, myDeltaTime * speed);
        audioSrc.pitch = Mathf.MoveTowards(audioSrc.pitch, minTimeScale, myDeltaTime * speed);
        if (Time.timeScale == minTimeScale) {
            Debug.Log("Hold");
            action = "hold";
            startTime = Time.time;
        }
    }

    void speedUp() {
        Debug.Log("speedUp");
        Time.timeScale = Mathf.MoveTowards(Time.timeScale, 1.0f, myDeltaTime * speed);
        audioSrc.pitch = Mathf.MoveTowards(audioSrc.pitch, 1.0f, myDeltaTime * speed);
        if (Time.timeScale == 1.0f) {
            resolve();
        }
    }

    protected override void ApplyPowerUp() {
        Debug.Log("ApplyPowerUp: Slow Time");
        action = "slowDown";
    }
}
