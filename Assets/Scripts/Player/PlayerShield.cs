using System.Collections;
using UnityEngine;

public class PlayerShield : MonoBehaviour {
	public GameObject shieldObject;
	public GameObject gunObject;
	public float shieldDurationSeconds;
	public float shieldWarningSeconds;
	public float shieldCooldownSeconds;
	public AudioClip shieldBlockSound;
	public float shotIntervalWithShield;
	private float originalShotInterval;
	private PlayerGun gun;

	[HideInInspector]
	public bool ShieldIsUp, isDisabled;

	private Animator shieldAnimator;

	// Use this for initialization
	private void Start() {
		ShieldIsUp = false;
		shieldAnimator = shieldObject.GetComponent<Animator>();
		gun = gunObject.GetComponent<PlayerGun>();
		originalShotInterval = gun.ShotInterval;
	}

	// Update is called once per frame
	private void Update() {
		if (isDisabled) return;
		if (!ShieldIsUp && Input.GetButtonDown("Fire2")) {
			StartCoroutine(DoShield());
		}
	}

	private IEnumerator DoShield() {
		gun.ShotInterval = shotIntervalWithShield;
		shieldObject.SetActive(true);
		ShieldIsUp = true;

		yield return new WaitForSeconds(shieldDurationSeconds - shieldWarningSeconds);
		shieldAnimator.SetTrigger("FlashShield");
		yield return new WaitForSeconds(shieldWarningSeconds);

		ShieldIsUp = false;
		shieldObject.SetActive(false);
		gun.ShotInterval = originalShotInterval;

		Disable(shieldCooldownSeconds);
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