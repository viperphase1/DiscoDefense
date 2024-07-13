using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonBehavior : TowerBehavior {
    private ParticleSystem[] gasStreams;

    public override void Start() {
        base.Start();
        gasStreams = GetComponents<ParticleSystem>();
        foreach (ParticleSystem stream in gasStreams) {
            stream.Play();
        }
        audioSource.loop = true;
        audioSource.Play();
    }

    public override void Update(){
        
    }

}