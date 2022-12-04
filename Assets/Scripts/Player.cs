using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float health = 100f;

    void reset() {
        health = 100f;
        if (PlayerPrefs.HasKey("AddHealth")) {
            health += (float) PlayerPrefs.GetInt("AddHealth");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
