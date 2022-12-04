using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Track", menuName = "Track", order = 0)]
public class MusicTrack : ScriptableObject {
    public int bpm = 120;
    public AudioClip audio;
}
