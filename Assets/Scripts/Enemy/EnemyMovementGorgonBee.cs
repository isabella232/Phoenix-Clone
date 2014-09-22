using UnityEngine;

public sealed class EnemyMovementGorgonBee : EnemyMovementBase {

	public enum AIMode {
		Moving,
		ForcedMovement
	}

	private AIMode currentMode;

	private Vector2 startPos, endPos, currentPos;
	private float timeCounter;
	private float _forcedSpeed;

	private Vector2 enemyToPlayerVector;

	public float movementSpeed;

	// Use this for initialization
	protected override void Start() {
		base.Start();

		startPos = endPos = Vector2.zero;
		currentPos = rigidbody2D.position;
		currentMode = AIMode.Moving;
	}

	public override void ForceMovement(Vector2 targetPosition, float forcedSpeed) {
		timeCounter = 0;
		currentMode = AIMode.ForcedMovement;
		startPos = this.rigidbody2D.position;
		endPos = targetPosition;
		_forcedSpeed = forcedSpeed;
	}

	// Update is called once per frame
	private void Update() {
		if (isFrozen) return;

		var displacement = GetOffscreenDisplacement();

		if (displacement != Vector2.zero) {
			this.rigidbody2D.position = this.rigidbody2D.position + displacement;
			// Warp enemy so it can reappear.

			return; // Continue movement next frame.
		}

		if (referenceFrame.player != null) {
			enemyToPlayerVector = this.transform.position - referenceFrame.player.transform.position;
			var enemyToPlayerAngle = Mathf.Atan2(enemyToPlayerVector.y, enemyToPlayerVector.x) * Mathf.Rad2Deg - 90;
			// -90 se saco al ojo.

			this.rigidbody2D.MoveRotation(enemyToPlayerAngle);
		}

		switch (currentMode) {
			case AIMode.Moving:
				if (referenceFrame.player == null) return;
				currentPos -= (Vector2)referenceFrame.player.transform.up * movementSpeed;
				break;

			case AIMode.ForcedMovement:
				if (timeCounter <= 1) {
					timeCounter += Time.deltaTime * _forcedSpeed;
					currentPos = Vector2Ex.Hermite(startPos, endPos, timeCounter);
				} else {
					currentMode = AIMode.Moving;
					return; // Restart next frame.
				}
				break;

			default:
				return;	// Do nothing.
		}

		this.rigidbody2D.MovePosition(currentPos);
	}

	/*private void OnGUI() {
		GUI.skin.GetStyle("label").alignment = TextAnchor.MiddleLeft;
		GUILayout.BeginArea(new Rect(0, Screen.height / 2, Screen.width / 2, Screen.height / 2));
		GUILayout.Label((goingRight ? "Right" : "Left").ToString());
		var horizontalDistance = Vector2.Dot(enemyToPlayerVector, referenceFrame.right);
		GUILayout.Label(((horizontalDistance > _dashMaxDistance ? "Too Right" : (horizontalDistance < -_dashMaxDistance ? "Too Left" : "Random"))).ToString());
		GUILayout.Label((currentMode).ToString());
		GUILayout.EndArea();
	}*/
}