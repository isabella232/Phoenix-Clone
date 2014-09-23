using System.Collections;
using UnityEngine;

public class PlayerLife : MonoBehaviour {
	public GameObject explosionPrefab;

	private PlayerShield shield;
	private LifeCounter lifeCounter;

	// Use this for initialization
	private void Start() {
		shield = GetComponentInChildren<PlayerShield>();
		lifeCounter = FindObjectOfType<LifeCounter>();
	}

	// Update is called once per frame
	private void Update() {
	}

	public void OnHit() {
		if (shield.ShieldIsUp) {
			audio.PlayOneShot(shield.shieldBlockSound);
		} else {
			Instantiate(explosionPrefab, this.transform.position, this.transform.rotation);
			lifeCounter.LostLife(this.transform.position, this.transform.rotation);
			Destroy(this.gameObject);
		}
	}

	public void MakeInvincible(float time) {
		StartCoroutine(DoInvincibility(time));
	}

	private IEnumerator DoInvincibility(float time) {
		var ship = transform.Find("Spaceship");
		var animator = ship.GetComponent<Animator>();

		ship.collider2D.enabled = false;
		animator.SetBool("Invincible", true);
		yield return new WaitForSeconds(time);
		animator.SetBool("Invincible", false);
		ship.collider2D.enabled = true;
	}
}