using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    // [SerializeField] private AudioMixer audioMixer;
    // [SerializeField, Range(0.001f, 1)] private float masterVolume = 1;
    // [SerializeField, Range(0.001f, 1)] private float musicVolume = 1;
    // [SerializeField, Range(0.001f, 1)] private float sfxVolume = 1;
    
    public enum FadeType {
        Cross,
        Full
    }
    
    public static AudioManager Instance { get; private set; }

    private Dictionary<AudioCueSO, SoundEmitter> soundEmitters = new();

    private SoundEmitter musicSoundEmitter;

    private SoundEmitter bufferedSoundEmitter;
    private AudioCueSO bufferedCue;
    private float bufferedFadeSeconds;

    private void Awake() {
        if (!Instance) {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else {
            Destroy(this);
        }
    }

    // private void OnValidate() {
    //     if (Application.isPlaying) {
    //         SetGroupVolume("MasterVolume", masterVolume);
    //         SetGroupVolume("MusicVolume", musicVolume);
    //         SetGroupVolume("SFXVolume", sfxVolume);
    //     }
    // }
    //
    // public void SetGroupVolume(string parameterName, float normalizedVolume) {
    //     bool volumeSet = audioMixer.SetFloat(parameterName, NormalizedToMixerValue(normalizedVolume));
    //     if (!volumeSet)
    //         Debug.LogError("The AudioMixer parameter was not found");
    // }

    private float MixerValueToNormalized(float mixerValue) {
        return 1f + (mixerValue / 80f);
    }
    
    private float NormalizedToMixerValue(float normalizedValue) {
        return Mathf.Log(normalizedValue) * 20;
        return (normalizedValue - 1f) * 80f;
    }

    public void StopMusic(float fadeSeconds = 0) {
        if (musicSoundEmitter) {
            if (musicSoundEmitter.IsPlaying()) {
                musicSoundEmitter.FadeMusicOut(fadeSeconds);
            }
        }
        musicSoundEmitter = null;
    }

    public void PlayMusic(AudioCueSO audioCue, float fadeSeconds, FadeType fadeType = FadeType.Cross, Vector3 position = default) {
        if (!audioCue) {
            StopMusic();
            return;
        }
        
        SoundEmitter soundEmitter;
        if (!soundEmitters.TryGetValue(audioCue, out soundEmitter)) {
            var audioGameObject = Instantiate(new GameObject(), transform);
            audioGameObject.AddComponent<AudioSource>();
            soundEmitter = audioGameObject.AddComponent<SoundEmitter>();
            soundEmitters.Add(audioCue, soundEmitter);
        }
        if (musicSoundEmitter) {
            if (musicSoundEmitter.IsPlaying()) {
                if (musicSoundEmitter != soundEmitter) {
                    musicSoundEmitter.FadeMusicOut(fadeSeconds);
                    if (fadeType == FadeType.Full) {
                        bufferedSoundEmitter = soundEmitter;
                        bufferedCue = audioCue;
                        bufferedFadeSeconds = fadeSeconds;
                        musicSoundEmitter.OnFinishPlaying += PlayBufferedCue;
                        return;
                    }
                }
                else {
                    return;
                }
            }
        }
        musicSoundEmitter = soundEmitter;
        musicSoundEmitter.FadeMusicIn(audioCue, fadeSeconds);
    }
    
    private void PlayBufferedCue(SoundEmitter soundEmitter) {
        soundEmitter.OnFinishPlaying -= PlayBufferedCue;
        bufferedSoundEmitter.FadeMusicIn(bufferedCue, bufferedFadeSeconds);
        musicSoundEmitter = bufferedSoundEmitter;
    }

    public void PlaySFX(AudioCueSO audioCue, Vector3 position = default, bool waitForFinish = false) {
        if (!audioCue) {
            return;
        }
        
        SoundEmitter soundEmitter;
        if (!soundEmitters.TryGetValue(audioCue, out soundEmitter)) {
            var audioGameObject = Instantiate(new GameObject(), transform);
            audioGameObject.AddComponent<AudioSource>();
            soundEmitter = audioGameObject.AddComponent<SoundEmitter>();
            soundEmitters.Add(audioCue, soundEmitter);
        }
        if (waitForFinish && soundEmitter.IsPlaying()) {
            return;
        }
        soundEmitter.PlayAudioCue(audioCue, position);
    }
}
