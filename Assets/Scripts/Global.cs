using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class Global
{
    public static Transform FindDeepChild(Transform aParent, string aName) {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == aName)
                return c;
            foreach(Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }

    public static void PointBurst(Vector3 epiCenter, Transform[] affectedObjects, float power = 10.0f, float radius = 2.0f) {
        foreach (Transform t in affectedObjects) {
            var rigidBodies = t.GetComponentsInChildren<Rigidbody>();
            // Debug.Log("PointBurst: Affected RigidBodies: " + rigidBodies.Length);
            foreach (Rigidbody rb in rigidBodies) {
                rb.velocity = new Vector3(0,0,0);
                rb.AddExplosionForce(power, epiCenter, radius, 0.0f, ForceMode.Impulse);
            }
        }
    }

    public static List<int> PrimeFactorization(int number) {
        var primes = new List<int>();

        for (int div = 2; div <= number; div++) {
            while (number % div == 0)
            {
                primes.Add(div);
                number = number / div;
            }
        }
        
        return primes;
    }

    public static int MultiplyIntegers(List<int> integers) {
        int result = 1;
        for (int i = 0; i < integers.Count; i++) {
            result *= integers[i];
        }
        return result;
    }

    public static Bounds GetMaxBounds(GameObject g) {
        var renderers = g.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(g.transform.position, Vector3.zero);
        var b = renderers[0].bounds;
        foreach (Renderer r in renderers) {
            b.Encapsulate(r.bounds);
        }
        return b;
    }

    public static void RouteAudioSourceToMixerGroup(AudioSource source, string mixerGroupName) {
        AudioMixerGroup[] mixerGroups = mixer.FindMatchingGroups(mixerGroupName);
        if (mixerGroups.Length > 0)
        {
            // Assign the first matching mixer group to the audio source
            source.outputAudioMixerGroup = mixerGroups[0];
        }
    }

    public static void NormalizeVolume(AudioSource source, bool useRMS = true)
    {
        if (source.clip != null)
        {
            float amplitude = useRMS
                ? AudioNormalizer.GetRMSAmplitude(source.clip)
                : AudioNormalizer.GetPeakAmplitude(source.clip);

            // Assuming a target amplitude of 0.1 for normalization
            float targetAmplitude = 0.1f;
            source.volume = targetAmplitude / amplitude;
        }
        else
        {
            Debug.LogError("AudioSource does not have an AudioClip assigned.");
        }
    }

    public static AudioMixer mixer = Resources.Load<AudioMixer>("Mixer");

    public static Theme theme;
    public static MusicManager mm;
}