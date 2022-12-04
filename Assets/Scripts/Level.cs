using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPosition : MonoBehaviour {
    public Tower tower;
    public Vector3 position;
}

public class PowerUpPosition : MonoBehaviour {
    public PowerUp powerup;
    public Vector3 position;
}

[CreateAssetMenu(fileName = "New Level", menuName = "Level", order = 0)]
public class Level : ScriptableObject {

    public int id;
    public MusicTrack track;
    public List<TowerPosition> towers = new List<TowerPosition>();
    public List<PowerUpPosition> powerUps = new List<PowerUpPosition>();
    public Vector3 startPosition;
    public Vector3 exitPosition;
    public Transform mazePrefab = null;
    public int time = 60;

}