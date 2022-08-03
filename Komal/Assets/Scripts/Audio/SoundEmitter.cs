using System;
using System.Collections;
using UnityEngine;

public class SoundEmitter : MonoBehaviour {
    private AudioCueSO audioCue;
    private AudioSource audioSource;

    public event Action<SoundEmitter> OnFinishPlaying;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void PlayAudioCue(AudioCueSO audioCue, Vector3 position = default) {
        this.audioCue = audioCue;
        audioSource.transform.position = position;
        audioSource.clip = audioCue.audioClip;
        audioSource.loop = audioCue.loop;
        audioSource.volume = audioCue.volume;
        audioSource.pitch = audioCue.pitch;

        if (!audioCue.loop) {
            StartCoroutine(FinishedPlaying(audioSource.clip.length));
        }
        
        audioSource.Play();
    }

    public void FadeMusicIn(AudioCueSO audioCue, float fadeSeconds) {
        PlayAudioCue(audioCue);
        StartCoroutine(FadeMusic(0, audioSource.volume, fadeSeconds));
    }

    public void FadeMusicOut(float fadeSeconds) {
        StartCoroutine(FadeMusic(audioSource.volume, 0, fadeSeconds, OnFadeOutComplete));
    }

    private IEnumerator FadeMusic(float startValue, float endValue, float fadeSeconds, Action OnComplete = null) {
        float count = 0;
        float t = 0;

        audioSource.volume = startValue;
        while (count <= fadeSeconds) {
            count += Time.deltaTime;
            t += Time.deltaTime / fadeSeconds;
            audioSource.volume = Mathf.Lerp(startValue, endValue, t);
            yield return null;
        }

        audioSource.volume = endValue;
        OnComplete?.Invoke();
    }

    private IEnumerator FinishedPlaying(float clipLenght) {
        yield return new WaitForSeconds(clipLenght);
        NotifyBeingDone();   
    }

    private void OnFadeOutComplete() {
        audioSource.Stop();
        NotifyBeingDone();
    }

    private void NotifyBeingDone() {
        OnFinishPlaying?.Invoke(this);
    }

    public bool IsPlaying() {
        return audioSource.isPlaying;
    }

    public bool IsLooping() {
        return audioSource.loop;
    }
}
