using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tower", menuName = "Tower", order = 0)]
public class Tower : ScriptableObject {

    public GameObject towerPrefab;
    public int price = 300;
    public PointType supportedPointTypes = PointType.Vertex;
    public float towerHeight = 3.0f;
    // minimize risk of concentrations of a single tower type
    public float areaTowerCapRatio = (float) 1 / 3;

}
