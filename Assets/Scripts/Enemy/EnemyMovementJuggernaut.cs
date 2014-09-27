using System.Collections;
using UnityEngine;

public sealed class EnemyMovementJuggernaut : EnemyMovementBase {

	public enum AIMode {
		PreAim,
		Aiming,
		Windup,
		Charge,
		ForcedMovement
	}

	private AIMode currentMode;

	private Vector2 startPos, endPos;
	private float startRotation, endRotation, currentRotation;
	private float timeCounter;
	private float _forcedSpeed;

	#region AI values

	public float ChargeSpeed, ChargeTime;
	public float WindupSpeed, WindupTime;
	public float AimingSpeed;

	private float _chargeSpeed, _chargeTime;
	private float _windupSpeed, _windupTime;
	private float _aimingSpeed;

	#endregion AI values

	private Vector3 enemyToPlayerVector;

	// Use this for initialization
	protected override void Start() {
		base.Start();

		startPos = endPos = Vector2.zero;
		currentMode = AIMode.PreAim;

		UpdateAIVariables();
	}

	protected override IEnumerator FreezeCoroutine(float duration) {
		yield return StartCoroutine(base.FreezeCoroutine(duration));
		currentMode = AIMode.PreAim;
	}

	private void UpdateAIVariables() {
		_chargeTime = ChargeTime;
		_chargeSpeed = ChargeSpeed;
		_windupTime = WindupTime;
		_windupSpeed = WindupSpeed;
		_aimingSpeed = AimingSpeed;
	}

	public override void ForceMovement(Vector2 targetPosition, float movementSpeed) {
		timeCounter = 0;
		currentMode = AIMode.ForcedMovement;
		startPos = this.rigidbody2D.position;
		endPos = targetPosition;
		_forcedSpeed = movementSpeed;
	}

	// Update is called once per frame
	private void Update() {
		if (isFrozen) return;

		var displacement = GetOffscreenDisplacement();

		if (displacement != Vector2.zero) {
			currentPos += displacement;
			startPos += displacement;
			endPos += displacement;
			this.rigidbody2D.position = currentPos;
			// Warp enemy so it can reappear.

			return; // Continue movement next frame.
		}

		this.velocity = -currentPos; // velocidad(t2) = posicion(t2) - posicion(t1)

		switch (currentMode) {
			case AIMode.PreAim:
				if (referenceFrame.player != null) {
					enemyToPlayerVector = this.transform.position - referenceFrame.player.transform.position;
					var enemyToPlayerAngle = Mathf.Atan2(enemyToPlayerVector.y, enemyToPlayerVector.x) * Mathf.Rad2Deg - 90;
					// -90 se saco al ojo.

					startRotation = this.rigidbody2D.rotation;
					endRotation = enemyToPlayerAngle;

					timeCounter = 0;
					currentMode = AIMode.Aiming;
				}
				break;

			case AIMode.Aiming:
				if (timeCounter <= 1) {
					currentRotation = Mathf.LerpAngle(startRotation, endRotation, timeCounter);
					this.rigidbody2D.MoveRotation(currentRotation);
					timeCounter += Time.deltaTime * _aimingSpeed;
				} else {
					timeCounter = 0;
					currentMode = AIMode.Windup;
					return;
				}
				break;

			case AIMode.Windup:
				if (timeCounter >= _windupTime) {
					timeCounter = 0;
					currentMode = AIMode.Charge;
					return; // Start movement on next frame
				} else {
					timeCounter += Time.deltaTime;

					currentPos += (Vector2)this.rigidbody2D.transform.up * _windupSpeed;
					// El "up" de la nave es hacia 'adelante'.
				}
				break;

			case AIMode.Charge:
				if (timeCounter >= _chargeTime) {
					if (IsOffscreen()) {
						timeCounter -= 1; // Continue movement if it ends while offscreen.
					} else {
						currentMode = AIMode.PreAim;
						return; // Start movement on next frame
					}
				} else {
					timeCounter += Time.deltaTime;

					var scaler = (_chargeTime - timeCounter) / _chargeTime;
					currentPos -= (Vector2)this.rigidbody2D.transform.up * _chargeSpeed * scaler;
				}

				break;

			case AIMode.ForcedMovement:
				if (timeCounter <= 1) {
					timeCounter += Time.deltaTime * _forcedSpeed;
					currentPos = Vector2Ex.Hermite(startPos, endPos, timeCounter);
				} else {
					currentMode = AIMode.PreAim;
					return; // Restart next frame.
				}
				break;

			default:
				return;	// Do nothing.
		}

		this.velocity += currentPos; // velocidad(t2) = posicion(t2) - posicion(t1)

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