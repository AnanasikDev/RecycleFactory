using UnityEngine;

/// <summary>
/// Class for audio random manipulations
/// </summary>
public static class HexAudioRandom
{
    public static float pitch_min = 0.94f;
    public static float pitch_max = 1.04f;

    public static float volume_min = 0.97f;
    public static float volume_max = 1.03f;

    /// <summary>
    /// Sets and returns random pitch for the audio source.
    /// </summary>
    public static float RandomizePitch(this AudioSource source)
    {
        source.pitch = Random.Range(pitch_min, pitch_max);
        return source.pitch;
    }

    /// <summary>
    /// Sets and returns random volume for the audio source.
    /// </summary>
    public static float RandomizeVolume(this AudioSource source)
    {
        source.volume = Random.Range(volume_min, volume_max);
        return source.volume;
    }
}