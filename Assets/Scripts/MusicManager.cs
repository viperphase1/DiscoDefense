using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private MusicTrack[] tracks;
    private float[] samples = new float[512];
    public GameObject musicPlayer;
    public AudioSource audioSource;
    public MusicTrack currentTrack;

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
            audioSource.loop = true;
            if (currentTrack) {
                setClip(currentTrack.audio);
            }
        }
    }

    public float getIntervalsSinceTrackStart(float rate) {
        float intervalLength = 60f * rate / currentTrack.bpm;
        float intervals = audioSource.timeSamples / (currentTrack.audio.frequency * intervalLength);
        return intervals;
    }

    public float getPositionBetweenIntervals(float rate) {
        float intervals = getIntervalsSinceTrackStart(rate);
        return Mathf.Repeat(intervals, 1.0f);
    }

    public int getInterval(float rate) {
        float intervals = getIntervalsSinceTrackStart(rate);
        int interval = Mathf.FloorToInt(intervals);
        return interval;
    }

    public float getIntervalLengthInSeconds(float rate) {
        return 60f * rate / currentTrack.bpm;
    }

    public float getSecondsToNextInterval(float rate) {
        float position = getPositionBetweenIntervals(rate);
        float toNextInterval = 1 - position;
        float secondsToNextInterval = toNextInterval * getIntervalLengthInSeconds(rate);
        return secondsToNextInterval;
    }

    public void setClip(AudioClip clip) {
        audioSource.Stop();
        audioSource.clip = clip;
    }

    public void play() {
        audioSource.Play();
    }

    public void pause() {
        audioSource.Pause();
    }

    public void randomTrack(MusicTrack[] exclude) {

    }

    public void setTrack(MusicTrack track) {
        currentTrack = track;
        setClip(track.audio);
    }

    public MusicTrack defaultTrack(int round) {
        int trackIndex = round % tracks.Length;
        currentTrack = tracks[trackIndex];
        setClip(currentTrack.audio);
        return currentTrack;
    }

    public void addVisualizer() {

    }

    public void updateVisualizer() {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }

}
