using System.Collections;
using UnityEngine;

public class EnemyLife : MonoBehaviour {
	public GameObject explosionPrefab;
	public GUIText scorePopupPrefab;
	public float regenerationTime;
	public int scoreValue;
	public bool immuneToNormalFire, immuneToShipCollision, doesntKill;
	public bool doFriendlyFire;
	private GameObject explosionOrigin;

	// Use this for initialization
	private void Start() {
		explosionOrigin = transform.Find("Spaceship/Explosion Origin").gameObject;
	}

	public void OnHit(bool hitCameFromPlayer) {
		Instantiate(explosionPrefab, explosionOrigin.transform.position, explosionOrigin.transform.rotation);
		if (hitCameFromPlayer) {
			var pop = Instantiate(scorePopupPrefab, Camera.main.WorldToViewportPoint(this.transform.position), Quaternion.identity) as GUIText;
			pop.GetComponent<ScorePopupController>().scoreValue = scoreValue;
		}

		Destroy(this.gameObject);
	}

	public void OnAttachmentHit(GameObject attachment) {
		var explosion = Instantiate(explosionPrefab, attachment.transform.position, attachment.transform.rotation) as GameObject;
		explosion.transform.localScale /= 2;
		attachment.SetActive(false);
		StartCoroutine(RegenerateAttachment(attachment));
	}

	private IEnumerator RegenerateAttachment(GameObject attachment) {
		yield return new WaitForSeconds(regenerationTime);
		attachment.SetActive(true);
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			if (!immuneToShipCollision) OnHit(false);
			if (!doesntKill) other.GetComponentInParent<PlayerLife>().OnHit();
		} else if (other.tag == "PlayerShield") {
			OnHit(true);
			other.GetComponentInParent<PlayerShield>().OnHit();
		} else if (doFriendlyFire && ((other.tag == "Enemy") || (other.tag == "EnemyAttachment"))) {
			if (!immuneToShipCollision) OnHit(true);
			other.GetComponentInParent<EnemyLife>().OnHit(true);
		}
	}

	public void attachElectricEffect(GameObject otherShip) {
		_otherShip = otherShip;
		showElectricEffect = true;
	}

	private bool showElectricEffect;
	private GameObject _otherShip;

	// Update is called once per frame
	private void Update() {
		if (showElectricEffect) {
			Debug.DrawLine(this.rigidbody2D.position, _otherShip.rigidbody2D.position, Color.white);
			// TODO: A real effect here.
		}
	}
}