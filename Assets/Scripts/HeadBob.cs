using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour{

    public float bobRate = 2f;
    public float bobSpeed = 0.5f;
    [HideInInspector]
    public MusicManager mm;
    private Quaternion upState;
    private Quaternion downState;
    private float phaseStart = 0f;
    private string phase = null;

    void Start() {
        upState = transform.rotation;
        Vector3 euler = upState.eulerAngles;
        downState = Quaternion.Euler(euler.x, euler.y, euler.z + 10);
    }

    void Update() {
        float secondsToNextInterval = mm.getSecondsToNextInterval(bobRate);
        string old_phase = phase;
        if (secondsToNextInterval <= bobSpeed) {
            phase = "down";
        } else {
            phase = "up";
        }
        if (phase != old_phase) {
            phaseStart = Time.time;
        }
        if (phase == "down") {
            bobDown(bobSpeed);
        }
        if (phase == "up") {
            float resetSpeed = Mathf.Min(bobSpeed * 1.6f, mm.getIntervalLengthInSeconds(bobRate) - bobSpeed);
            bobUp(resetSpeed);
        }
    }

    void bobDown(float duration) {
        transform.rotation = Quaternion.Lerp(upState, downState, (Time.time - phaseStart) / duration);
    }

    void bobUp(float duration) {
        transform.rotation =  Quaternion.Lerp(downState, upState, (Time.time - phaseStart) / duration);
    }
}
