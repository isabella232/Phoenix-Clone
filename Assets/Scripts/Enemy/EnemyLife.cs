using System.Collections;
using UnityEngine;

public class EnemyLife : MonoBehaviour {
	public GameObject explosionPrefab;
	public GUIText scorePopupPrefab;
	public float regenerationTime;
	public int scoreValue;
	public bool doFriendlyFire;
	public LayerMask layerMaskOfEnemies;
	private GameObject explosionOrigin;

	// Use this for initialization
	private void Start() {
		explosionOrigin = transform.Find("Spaceship/Explosion Origin").gameObject;
	}

	// Update is called once per frame
	private void Update() {
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

	public void OnMagnetHit(float magnetDuration, float magnetForce) {
		var enemies = Physics2D.OverlapCircleAll(rigidbody2D.position, ScreenBounds.HorizontalDistance, layerMaskOfEnemies);
		GameObject bestTarget = null;
		var closestDistanceSqr = Mathf.Infinity;
		var currentPos = rigidbody2D.position;
		Vector2 vectorEnemyToSelf = Vector2.zero;

		foreach (var enemy in enemies) {
			var enemyParent = enemy.transform.parent.gameObject;
			var enemyPos = enemyParent.rigidbody2D.position;

			vectorEnemyToSelf = enemyPos - currentPos;
			var distanceToEnemySqr = vectorEnemyToSelf.sqrMagnitude;

			if (distanceToEnemySqr < closestDistanceSqr && enemyParent.GetInstanceID() != this.gameObject.GetInstanceID()) {
				bestTarget = enemyParent;
				closestDistanceSqr = distanceToEnemySqr;
			}
		}
		if (bestTarget != null) {
			doFriendlyFire = true;
			CrashShips(bestTarget, this.gameObject, magnetForce);
		}
	}

	public void CrashShips(GameObject ship1, GameObject ship2, float magnetForce) {
		var pos1 = ship1.rigidbody2D.position;
		var pos2 = ship2.rigidbody2D.position;

		ship1.GetComponent<EnemyMovement>().ForceMovement(pos2, magnetForce);
		ship2.GetComponent<EnemyMovement>().ForceMovement(pos1, magnetForce);
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			OnHit(false);
			other.gameObject.GetComponentInParent<PlayerLife>().OnHit();
		} else if (doFriendlyFire && ((other.tag == "Enemy") || (other.tag == "EnemyAttachment"))) {
			OnHit(true);
			other.gameObject.GetComponentInParent<EnemyLife>().OnHit(true);
		}
	}
}