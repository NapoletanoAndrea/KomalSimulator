using System;
using UnityEngine;

[Serializable]
public struct KeyCodeResponse {
	public KeyCode keyCode;
	public AudioCueSO response;
}

public class FunController : MonoBehaviour {
	public KeyCodeResponse[] keyCodeResponses;

	private void Update() {
		foreach (var k in keyCodeResponses) {
			if (Input.GetKeyDown(k.keyCode)) {
				AudioManager.Instance.PlaySFX(k.response);
			}
		}
	}
}
