using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using Random = UnityEngine.Random;

[Serializable]
public struct KeyToSpeech {
    public Status[] workingStatus;
    public string preKeyword;
    public List<string> keywords;
    public AudioCueSO[] possibleResponses;

    public List<string> GetFullKeywords() {
        List<string> fullKeywords = new();
        foreach (var keyword in keywords) {
            fullKeywords.Add(preKeyword + " " + keyword);
        }
        return fullKeywords;
    }

    public bool IsValid(Status komalStatus, string text) {
        foreach (var s in workingStatus) {
            if (s == komalStatus) {
                foreach (var keyword in keywords) {
                    string finalString = preKeyword + " " + keyword;
                    if (text == finalString) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void PlayResponse() {
        int random = Random.Range(0, possibleResponses.Length);
        AudioManager.Instance.PlaySFX(possibleResponses[random]);
    }
}

public class CommandReceiver : MonoBehaviour {
    [SerializeField] private Komal komal;

    [SerializeField] private List<string> sleepingKeywords;
    [SerializeField] private List<KeyToSpeech> talkingKeys;

    private KeywordRecognizer keywordRecognizer;
    private List<string> keywords;

    private void Awake() {
        keywords = new();
        keywords.AddRange(sleepingKeywords);
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
        }

        foreach (var talkingKey in talkingKeys) {
            if (talkingKey.IsValid(komal.CurrentStatus, args.text)) {
                talkingKey.PlayResponse();
            }
        }
    }
}
