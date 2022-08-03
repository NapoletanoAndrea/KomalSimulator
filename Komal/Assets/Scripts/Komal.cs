using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public enum Status {
	Sleeping,
	Idle,
	Talking
}

public enum SpriteType {
	Sleeping,
	Idle,
	Angry
}

public class Komal : MonoBehaviour {

	#region Fields

	public Status startStatus;
	[SerializeField, ReadOnly] private Status currentStatus;

	[SerializeField] private Sprite sleepingSprite;
	[SerializeField] private Sprite idleSprite;

	private SpriteRenderer spriteRenderer;
	private KomalAudio komalAudio;

	private bool hasChangedStatus;

	#endregion

	#region Properties
	
	public Status CurrentStatus {
		get => currentStatus;
		set {
			OnStatusChanged(value);
			currentStatus = value;
		}
	}

	#endregion

	private void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		komalAudio = GetComponent<KomalAudio>();
	}

	private void Start() {
		CurrentStatus = startStatus;
	}

	private void OnStatusChanged(Status nextStatus) {
		if (CurrentStatus == nextStatus && hasChangedStatus) {
			return;
		}

		hasChangedStatus = true;

		if (CurrentStatus == Status.Sleeping) {
			komalAudio.StopSnoring();
		}

		if (nextStatus == Status.Sleeping) {
			komalAudio.PlaySnoringCue();
		}

		if (nextStatus == Status.Idle) {
			
		}
	}

	private Sprite GetSprite(SpriteType spriteType) {
		switch (spriteType) {
			case SpriteType.Idle: return idleSprite;
			case SpriteType.Sleeping: return sleepingSprite;
		}
		return idleSprite;
	}

	public void SetSprite(SpriteType spriteType) {
		spriteRenderer.sprite = GetSprite(spriteType);
	}
}
