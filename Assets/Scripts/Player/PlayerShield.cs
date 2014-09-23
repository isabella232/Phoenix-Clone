using System.Collections;
using UnityEngine;

public class PlayerShield : MonoBehaviour {
	public GameObject shieldObject;
	public GameObject gunObject;
	public float shieldDurationSeconds;
	public float shieldWarningSeconds;
	public AudioClip shieldBlockSound;

	[HideInInspector]
	public bool ShieldIsUp, isDisabled;
	private Animator shieldAnimator;

	// Use this for initialization
	private void Start() {
		ShieldIsUp = false;
		shieldAnimator = shieldObject.GetComponent<Animator>();
	}

	// Update is called once per frame
	private void Update() {
		if (isDisabled) return;
		if (!ShieldIsUp && Input.GetButtonDown("Fire2")) {
			StartCoroutine(DoShield());
		}
	}

	private IEnumerator DoShield() {
		gunObject.SetActive(false);
		shieldObject.SetActive(true);
		ShieldIsUp = true;

		yield return new WaitForSeconds(shieldDurationSeconds - shieldWarningSeconds);
		shieldAnimator.SetTrigger("FlashShield");
		yield return new WaitForSeconds(shieldWarningSeconds);

		ShieldIsUp = false;
		shieldObject.SetActive(false);
		gunObject.SetActive(true);
	}

	public void OnHit() {
		this.audio.PlayOneShot(shieldBlockSound);
	}
	public void Disable(float duration) {
		StartCoroutine(DisableCoroutine(duration));
	}

	private IEnumerator DisableCoroutine(float duration) {
		isDisabled = true;
		yield return new WaitForSeconds(duration);
		isDisabled = false;
	}

}