using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class CommandReceiver : MonoBehaviour {
    [SerializeField] private Komal komal;

    [SerializeField] private List<string> sleepingKeywords;
    
    private KeywordRecognizer keywordRecognizer;
    private string[] keywords;

    private void Awake() {
        keywordRecognizer = new KeywordRecognizer(sleepingKeywords.ToArray());
        keywordRecognizer.OnPhraseRecognized += OnKeywordsRecognized;
        keywordRecognizer.Start();
    }

    private void OnKeywordsRecognized(PhraseRecognizedEventArgs args) {
        if (komal.CurrentStatus == Status.Sleeping) {
            if (sleepingKeywords.Contains(args.text)) {
                komal.CurrentStatus = Status.Idle;
                komal.SetSprite(SpriteType.Idle);
            }
        }
    }
}
