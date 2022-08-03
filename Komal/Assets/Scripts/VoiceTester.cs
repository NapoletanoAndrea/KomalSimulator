using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceTester : MonoBehaviour {
    private KeywordRecognizer keywordRecognizer;
    [SerializeField] private string[] keywords;

    private void Awake() {
        keywordRecognizer = new KeywordRecognizer(keywords);
        keywordRecognizer.OnPhraseRecognized += OnKeywordsRecognized;
        keywordRecognizer.Start();
    }

    private void OnKeywordsRecognized(PhraseRecognizedEventArgs args) {
        Debug.Log(args.text);
    }
}
