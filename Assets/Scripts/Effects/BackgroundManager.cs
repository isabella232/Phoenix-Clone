using System;
using System.Collections;
using UnityEngine;

public class BackgroundManager : MonoBehaviour {
	public Texture[] textures;
	public float scrollSpeed;
	public int currentBG;
	public AudioClip deathSong;
	private int currentLoadedBG;
	private bool isPaused;
	private float timeDelta;

	private Action onVolumeChangeReady;

	// Use this for initialization
	private void Start() {
		LoadBackground();
		timeDelta = 0;
	}

	// Update is called once per frame
	private void Update() {
		if (Input.GetKeyDown(KeyCode.F3)) {
			currentBG--;
		}
		if (Input.GetKeyDown(KeyCode.F4)) {
			currentBG++;
		}

		if (currentLoadedBG != currentBG) {
			LoadBackground();
		}

		if (!isPaused) {
			timeDelta += Time.deltaTime;
			renderer.material.mainTextureOffset = new Vector2(0, Mathf.Repeat(timeDelta * scrollSpeed, 1));
		}
	}

	private Texture fadeTexture;

	private void OnGUI() {
		if (fadeTexture != null) {
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture, ScaleMode.StretchToFill);
		}
	}

	public void FadeOut(Action callback) {
		StartCoroutine(DoFade(callback, this.audio.volume, 0, Color.clear, Color.black));
	}

	private IEnumerator DoFade(Action callback, float startAudioValue, float endAudioValue, Color startVideoValue, Color endVideoValue) {
		var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		texture.SetPixel(0, 0, startVideoValue);
		texture.Apply();
		fadeTexture = texture;
		var timeCounter = 0f;

		while (this.audio.volume != endAudioValue) {
			yield return new WaitForEndOfFrame(); // Asi se llama solo una vez por frame.
			this.audio.volume = Mathf.Lerp(startAudioValue, endAudioValue, timeCounter);
			texture.SetPixel(0, 0, Color.Lerp(startVideoValue, endVideoValue, timeCounter));
			texture.Apply();
			timeCounter += Time.deltaTime;
		}
		fadeTexture = null;
		callback();
	}

	private void LoadBackground() {
		currentBG = currentBG % textures.Length;
		currentLoadedBG = currentBG;
		renderer.material.mainTexture = textures[currentLoadedBG];
	}

	public void Pause() {
		isPaused = true;
		this.audio.Pause();
	}

	public void Resume() {
		isPaused = false;
		this.audio.Play();
	}

	public void PlayDeathSong() {
		this.audio.Stop();
		this.audio.PlayOneShot(deathSong);
	}
}