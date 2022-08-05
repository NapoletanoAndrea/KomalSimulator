using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using Random = UnityEngine.Random;

[Serializable]
public struct KeyResponse {
    public List<string> keywords;
    public AudioCueSO[] possibleResponses;

    public AudioCueSO GetPossibleResponse() {
        int random = Random.Range(0, possibleResponses.Length);
        return possibleResponses[random];
    }
}

[Serializable]
public struct KeyToSpeech {
    public Status[] workingStatus;
    public string preKeyword;
    public KeyResponse[] keyResponses;

    public List<string> GetFullKeywords() {
        List<string> fullKeywords = new();
        foreach (var keyResponse in keyResponses) {
            foreach (var keyword in keyResponse.keywords) {
                fullKeywords.Add(preKeyword + " " + keyword);
            }
        }
        return fullKeywords;
    }

    public bool PlayResponse(Status komalStatus, string text) {
        foreach (var s in workingStatus) {
            if (s == komalStatus) {
                foreach (var keyResponse in keyResponses) {
                    foreach (var keyword in keyResponse.keywords) {
                        string finalString = preKeyword + " " + keyword;
                        if (text == finalString) {
                            AudioManager.Instance.PlaySFX(keyResponse.GetPossibleResponse());
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
}

public class CommandReceiver : MonoBehaviour {
    [SerializeField] private Komal komal;

    [SerializeField] private List<string> sleepingKeywords;
    [SerializeField] private List<string> silenceKeywords;
    [SerializeField] private KeyResponse stayUpKeys;
    [SerializeField] private List<KeyToSpeech> talkingKeys;

    private KeywordRecognizer keywordRecognizer;
    private List<string> keywords;

    private void Awake() {
        Init();
    }

    private void OnDisable() {
        keywordRecognizer.Dispose();
    }

    private void Init() {
        keywords = new();
        keywords.AddRange(sleepingKeywords);
        keywords.AddRange(silenceKeywords);
        keywords.AddRange(stayUpKeys.keywords);
        foreach (var talkingKey in talkingKeys) {
            keywords.AddRange(talkingKey.GetFullKeywords());
        }
        keywordRecognizer = new KeywordRecognizer(keywords.ToArray());
        keywordRecognizer.OnPhraseRecognized += OnKeywordsRecognized;
        keywordRecognizer.Start();
    }

    private void OnKeywordsRecognized(PhraseRecognizedEventArgs args) {
        if (komal.CurrentStatus == Status.Sleeping) {
            if (sleepingKeywords.Contains(args.text)) {
                komal.CurrentStatus = Status.Idle;
                komal.SetSprite(SpriteType.Idle);
                return;
            }
            
            if (silenceKeywords.Contains(args.text)) {
                AudioManager.Instance.StopMusic(.5f);
                return;
            }
        }

        if (komal.CurrentStatus == Status.Idle) {
            if (stayUpKeys.keywords.Contains(args.text)) {
                AudioManager.Instance.PlaySFX(stayUpKeys.GetPossibleResponse());
                komal.canSleep = false;
            }
        }

        foreach (var talkingKey in talkingKeys) {
            if (talkingKey.PlayResponse(komal.CurrentStatus, args.text)) {
                komal.Tick();
                return;
            }
        }
    }
}
