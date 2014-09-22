using UnityEngine;

public class MagnetShotController : SpecialPowerController {
	public float magnetDuration, magnetForce;
	public LayerMask layerMaskOfEnemies;
	public AudioClip magnetSound;
	private ShotController controller;

	// Use this for initialization
	private void Start() {
		controller = GetComponent<ShotController>();
		OnSpecialEnd();
	}

	// Update is called once per frame
	private void Update() {
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Enemy" || other.tag == "EnemyAttachment") {
			var victimPrimaryLife = other.GetComponentInParent<EnemyLife>();
			var victimSecondary = FindClosestEnemy(victimPrimaryLife.gameObject);

			if (victimSecondary != null) {
				victimPrimaryLife.doFriendlyFire = true;
				victimPrimaryLife.attachElectricEffect(victimSecondary);

				var posPrimary = victimPrimaryLife.rigidbody2D.position;

				victimPrimaryLife.GetComponent<EnemyMovementBase>().Freeze();
				victimSecondary.GetComponent<EnemyMovementBase>().ForceMovement(posPrimary, magnetForce);
				
				AudioSource.PlayClipAtPoint(magnetSound, this.rigidbody2D.position);

				controller.Remove();
			} else {
				controller.Remove();
			}
		}
	}

	private GameObject FindClosestEnemy(GameObject victimPrimary) {
		var enemies = Physics2D.OverlapCircleAll(victimPrimary.rigidbody2D.position, ScreenBounds.HorizontalDistance, layerMaskOfEnemies);
		GameObject victimSecondary = null;
		var closestDistanceSqr = Mathf.Infinity;
		var currentPos = rigidbody2D.position;
		var vectorEnemyToSelf = Vector2.zero;

		foreach (var enemy in enemies) {
			var enemyParent = enemy.transform.parent.gameObject;
			var enemyPos = enemyParent.rigidbody2D.position;

			if (enemyPos == victimPrimary.rigidbody2D.position) continue;

			vectorEnemyToSelf = enemyPos - currentPos;
			var distanceToEnemySqr = vectorEnemyToSelf.sqrMagnitude;

			if (distanceToEnemySqr < closestDistanceSqr) {
				victimSecondary = enemyParent;
				closestDistanceSqr = distanceToEnemySqr;
			}
		}
		return victimSecondary;
	}

	public override string SpecialName {
		get { return "Magnet Shot"; }
	}
}