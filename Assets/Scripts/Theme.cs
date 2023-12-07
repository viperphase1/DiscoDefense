using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Theme", menuName = "Theme", order = 0)]
public class Theme : ScriptableObject {

    public List<Material> wallMaterials = new List<Material>();
    public List<Material> floorMaterials = new List<Material>();
    public List<MusicTrack> tracks = new List<MusicTrack>();
    public List<Color> colors = new List<Color>();
    public GameObject towerDecoration;
    public GameObject headDecoration;
    
}
