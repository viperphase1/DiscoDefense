using UnityEngine;

public static class AudioNormalizer
{
    public static float GetRMSAmplitude(AudioClip audioClip)
    {
        float[] samples = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(samples, 0);

        float sumOfSquares = 0f;
        for (int i = 0; i < samples.Length; i++)
        {
            sumOfSquares += samples[i] * samples[i];
        }

        return Mathf.Sqrt(sumOfSquares / samples.Length);
    }

    public static float GetPeakAmplitude(AudioClip audioClip)
    {
        float[] samples = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(samples, 0);

        float peak = 0f;
        for (int i = 0; i < samples.Length; i++)
        {
            float absSample = Mathf.Abs(samples[i]);
            if (absSample > peak)
            {
                peak = absSample;
            }
        }

        return peak;
    }
}