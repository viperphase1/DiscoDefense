using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class DiscoLight : MonoBehaviour
{
    [HideInInspector]
    public Theme theme;
    [HideInInspector]
    public MusicManager mm;
    Random rng = new Random();
    protected int colorChangeInterval;
    protected int behaviorChangeInterval;
    public float colorChangeRate = 8.0f;
    public float behaviorChangeRate = 4.0f;
    public float bounceRate = 0.5f;
    public float maxSwingAngle = 60.0f;
    public float movementSpeed = 100f;
    public string[] behaviors = {
        "sweep",
        "swing",
        "still",
        // "glitch",
        // "roam"
    };
    public string behavior = "swing";
    private int direction = 1;
    private float initialRotationX;
    private Vector3 euler;

    // Start is called before the first frame update
    void Start()
    {
        // assign a random color from the theme
        changeColor();
        randomDirection();
        euler = transform.eulerAngles;
        initialRotationX = euler.x;
    }

    void randomDirection() {
        direction = 1;
        int coinFlip = rng.Next(0, 2);
        if (coinFlip == 1) {
            direction = -1;
        }
    }

    void rotate(float x, float y, float z) {
        euler = new Vector3(euler.x + x, euler.y + y, euler.z + z);
    }

    // Update is called once per frame
    void Update()
    {
        changeColorOnInterval();
        changeBehaviorOnInterval();

        if (behavior == "sweep") {
            rotate(0, direction * movementSpeed * Time.deltaTime, 0);
        }
        if (behavior == "swing") {
            if (Mathf.Abs(initialRotationX - euler.x) > maxSwingAngle) {
                Debug.Log("overswing: " + euler.x);
                euler.x = initialRotationX + direction * maxSwingAngle;
                Debug.Log("manually set rotation x to " + euler.x);
                direction = -1 * direction;
            }
            rotate(direction * Time.deltaTime * movementSpeed, 0, 0);
        }
        transform.eulerAngles = euler;

    }

    void changeBehaviorOnInterval() {
        int interval = mm.getInterval(behaviorChangeRate);
        if (interval != behaviorChangeInterval) {
            behaviorChangeInterval = interval;
            if (interval > 0) {
                changeBehavior();
                Debug.Log("rotation x at behavior change " + transform.eulerAngles.x);
            }
        }
    }

    void changeBehavior() {
        behavior = behaviors[rng.Next(0, behaviors.Length)];
        randomDirection();
    }

    void changeColorOnInterval() {
        int interval = mm.getInterval(colorChangeRate);
        if (interval != colorChangeInterval) {
            colorChangeInterval = interval;
            if (interval > 0) {
                changeColor();
            }
        }
    }

    void changeColor() {
        Light light = gameObject.GetComponent<Light>();
        light.color = theme.colors[rng.Next(0, theme.colors.Count)];
    }
}
