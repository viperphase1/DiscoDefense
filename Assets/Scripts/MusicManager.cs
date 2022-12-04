using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public MusicTrack[] tracks;
    private GameObject musicPlayer;
    private AudioSource audioSource;
    private AudioClip currentClip;
    private float[] samples = new float[512];

    public MusicTrack[] getTracks() {
        tracks = SOManager.GetAllInstances<MusicTrack>();
        return tracks;
    }
    
    public void addMusicPlayer(GameObject target)
    {
        if (target == null) {
            target = gameObject;
        }
        if (musicPlayer != target) {
            if (musicPlayer != null) {
                Destroy(audioSource);
            }
            musicPlayer = target;
            audioSource = musicPlayer.AddComponent(typeof(AudioSource)) as AudioSource;
            if (currentClip) {
                setClip(currentClip);
            }
        }
    }

    public void setClip(AudioClip clip) {
        audioSource.Stop();
        currentClip = clip;
        audioSource.clip = currentClip;
    }

    public void play() {
        audioSource.Play();
    }

    public void pause() {
        audioSource.Pause();
    }

    public void randomTrack(MusicTrack[] exclude) {

    }

    public MusicTrack defaultTrack(int round) {
        int trackIndex = round % tracks.Length;
        MusicTrack track = tracks[trackIndex];
        setClip(track.audio);
        return track;
    }

    public void addVisualizer() {

    }

    public void updateVisualizer() {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }

}
