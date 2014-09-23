using UnityEngine;

public class EnemyStunner : MonoBehaviour {
	public float duration;

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			var parentObject = other.transform.parent;
			parentObject.GetComponent<PlayerMovement>().Freeze(duration);
			parentObject.GetComponentInChildren<PlayerGun>().Disable(duration);
			parentObject.GetComponentInChildren<PlayerSpecial>().Disable(duration);
			parentObject.GetComponentInChildren<PlayerShield>().Disable(duration);
		}
	}
}