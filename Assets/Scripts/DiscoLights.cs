using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Global;

public class DiscoLights : MonoBehaviour
{
    [HideInInspector]
    public Theme theme;
    [HideInInspector]
    public MusicManager mm;
    // determines concentration of lights
    public int unitsPerLight = 9;
    // the object to put lights over, provides the bounding box of where lights can be placed
    public GameObject boundingObject;
    private Transform lightPrefab;

    void Awake() {
       lightPrefab = Resources.Load<Transform>("Prefabs/DiscoLight");
    }

    public void addLights() {
        gameObject.Clear();
        Bounds b = GetMaxBounds(boundingObject);
        int numLights = Mathf.FloorToInt(b.size.x * b.size.y / unitsPerLight);
        for (int i = 0; i < numLights; i++) {
            var newLight = Instantiate(lightPrefab, transform);
            DiscoLight discoLight = newLight.GetComponent<DiscoLight>();
            discoLight.theme = theme;
            discoLight.mm = mm;
            // change the xz coordinate to a random value within the boundingObject's bounds
            newLight.position = new Vector3(Random.Range(b.min.x, b.max.x), newLight.position.y, Random.Range(b.min.z, b.max.z));
        }
    }
}
