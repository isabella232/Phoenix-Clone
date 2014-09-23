using UnityEngine;

public class ShotController : MonoBehaviour {
	public float shotSpeed;
	public bool isPlayerShot;
	public bool disableDefaultBehavior;

	[HideInInspector]
	public PlayerGun gun;

	private float topBound, bottomBound, leftBound, rightBound;

	// Use this for initialization
	private void Start() {
		var margin = this.renderer.bounds.size.y;
		topBound = ScreenBounds.Top + 2 * margin;
		bottomBound = ScreenBounds.Bottom - 2 * margin;
		leftBound = ScreenBounds.Left - 2 * margin;
		rightBound = ScreenBounds.Right + 2 * margin;

		this.rigidbody2D.velocity = transform.up * shotSpeed;
	}

	// Update is called once per frame
	private void Update() {
		if (this.rigidbody2D.position.y > topBound || this.rigidbody2D.position.y < bottomBound
			||
			this.rigidbody2D.position.x > rightBound || this.rigidbody2D.position.x < leftBound
			) {
			Remove();
		}
	}

	public void Remove() {
		if (gun != null) {
			--gun.CurrentShotsOnScreen;
		}
		Destroy(this.gameObject);
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (disableDefaultBehavior) return;
		if (isPlayerShot) {
			if (other.tag == "Enemy") {
				var enemyLife = other.GetComponentInParent<EnemyLife>();
				if (!enemyLife.immuneToNormalFire) enemyLife.OnHit(true);
				Remove();
			} else if (other.tag == "EnemyAttachment") {
				other.GetComponentInParent<EnemyLife>().OnAttachmentHit(other.gameObject);
				Remove();
			} else if (other.tag == "Pickup") {
				other.GetComponent<PickupsController>().OnHit();
				Remove();
			}
		} else if (other.tag == "Player") {
			other.GetComponentInParent<PlayerLife>().OnHit();
			Remove();
		} else if (other.tag == "PlayerShield") {
			other.transform.parent.GetComponentInChildren<PlayerShield>().OnHit();
			Remove();
		}
	}
}