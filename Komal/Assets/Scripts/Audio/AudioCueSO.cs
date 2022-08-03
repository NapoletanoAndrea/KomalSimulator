using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Audio Cue")]
public class AudioCueSO : ScriptableObject {
    public AudioClip audioClip;
    [Range(0f, 1f)] public float volume = 1;
    [Range(-3f, 3f)] public float pitch = 1;
    public bool loop;
}
