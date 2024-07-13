using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerSoundEffect {
    public string towerSlug;
    public AudioClip clip;
}

[CreateAssetMenu(fileName = "New Theme", menuName = "Theme", order = 0)]
public class Theme : ScriptableObject {

    public List<Material> wallMaterials = new List<Material>();
    public List<Material> floorMaterials = new List<Material>();
    public List<MusicTrack> tracks = new List<MusicTrack>();
    public List<TowerSoundEffect> towerSoundEffects = new List<TowerSoundEffect>();
    public AudioClip swingClip;
    public List<Color> colors = new List<Color>();
    public GameObject towerDecoration;
    public GameObject headDecoration;

}