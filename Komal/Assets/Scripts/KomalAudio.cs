using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KomalAudio : MonoBehaviour {
    [SerializeField] private AudioCueSO snoringCue;

    public void PlaySnoringCue() {
        AudioManager.Instance.PlayMusic(snoringCue, 0);
    }

    public void StopSnoring() {
        AudioManager.Instance.StopMusic();
    }
}
