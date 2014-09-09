using System.Collections;
using System.Linq;
using UnityEngine;

public abstract class EnemyMovement : MonoBehaviour {
	protected bool isFrozen;

	public virtual void OnEMP(float duration) {
		StartCoroutine(Freeze(duration));
	}

	protected virtual IEnumerator Freeze(float duration) { // Notar la potencial herencia.
		var animator = this.GetComponentInChildren<Animator>();
		var gun = this.GetComponentInChildren<EnemyGun>();
		var previousFireState = gun.holdFire;
		var previousAnimationSpeed = animator.speed;

		animator.speed = 0;
		gun.holdFire = true;
		isFrozen = true;

		yield return new WaitForSeconds(duration);

		isFrozen = false;
		gun.holdFire = previousFireState;
		animator.speed = previousAnimationSpeed;
	}

	public abstract void ForceMovement(Vector2 targetPosition, float movementSpeed);
}

public class EnemyMovementGeneric : EnemyMovement {

	public enum AIMode {
		Undecided,
		Hovering,
		Dashing,
		DashRest,
		ForcedMovement
	}

	private AIMode currentMode;

	private Vector2 startPos, endPos, currentPos;
	private float timeCounter;

	#region AI values

	public float DashChance, DashSpeed, DashMinDistance, DashMaxDistance, DashForward, DashBack, DashWait, DashBacktrackChance;
	public int DashesInCycle;

	public float HoverTime, HoverSpeed;

	private float _dashChance, _dashSpeed, _dashMinDistance, _dashMaxDistance, _dashForward, _dashBack, _dashWait, _dashBacktrackChance;
	private float _hoverTime, _hoverSpeed;
	private float _forcedSpeed;
	private int _dashesInCycle;

	private int sweepCounter;
	private bool goingRight;

	#endregion AI values

	private float hMargin, vMargin;
	private ReferenceFrame referenceFrame;
	private Vector3 enemyToPlayerVector;

	// Use this for initialization
	private void Start() {
		var spriteBounds =
			this.GetComponentsInChildren<Renderer>()
			.Aggregate(new Bounds(), (bounds, renderer) => { bounds.Encapsulate(renderer.bounds); return bounds; });
		// Es como un foreach que acumula pero prefiero usar linq. :V

		hMargin = spriteBounds.size.x / 2;
		vMargin = spriteBounds.size.y / 2;

		startPos = endPos = Vector2.zero;
		currentPos = rigidbody2D.position;
		currentMode = AIMode.Undecided;

		referenceFrame = GetComponentInParent<ReferenceFrame>();

		UpdateAIVariables();
	}

	protected override IEnumerator Freeze(float duration) {
		yield return StartCoroutine(base.Freeze(duration));
		currentMode = AIMode.Undecided;
	}

	private void UpdateAIVariables() {
		_dashChance = DashChance;
		_dashSpeed = DashSpeed;
		_dashMinDistance = DashMinDistance;
		_dashMaxDistance = DashMaxDistance;
		_dashesInCycle = DashesInCycle;
		_dashForward = DashForward;
		_dashBack = DashBack;
		_dashWait = DashWait;
		_dashBacktrackChance = DashBacktrackChance;

		_hoverTime = HoverTime;
		_hoverSpeed = HoverSpeed;
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

		Vector2 displacement = Vector2.zero;

		displacement = CheckIfOffscreen(displacement);

		if (displacement != Vector2.zero) {
			currentPos += displacement;
			startPos += displacement;
			endPos += displacement;
			this.rigidbody2D.position = currentPos;
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
			case AIMode.Undecided:
				var r = Random.value;
				if (r <= _dashChance) {
					sweepCounter = 0;

					var horizontalDistance = Vector2.Dot(enemyToPlayerVector, referenceFrame.right);
					if (horizontalDistance > _dashMaxDistance) {
						// Positivo => A la derecha del jugador => Queremos ir a la izquierda.
						goingRight = true; // DashRest lo va a dar vuelta.
					} else if (horizontalDistance < -_dashMaxDistance) {
						// Negativo => A la izquierda del jugador => Queremos ir a la derecha.
						goingRight = false;
					} else {
						goingRight = Random.value <= 0.5f; // Pick direction randomly.
					}

					currentMode = AIMode.DashRest;
					timeCounter = _dashWait;
				} else {
					currentMode = AIMode.Hovering;
					timeCounter = 0;
				}

				UpdateAIVariables();

				return;	// Start new movement on next frame.

			case AIMode.Dashing:
				if (timeCounter >= 1) {
					if (sweepCounter == _dashesInCycle) {
						// Reset AI
						currentMode = AIMode.Undecided;
						return;
					} else {
						timeCounter = 0;
						currentMode = AIMode.DashRest;
						return;	// Start next movement on next frame;
					}
				}

				timeCounter += Time.deltaTime * _dashSpeed;
				currentPos = Vector2.Lerp(startPos, endPos, timeCounter);
				break;

			case AIMode.DashRest:
				if (timeCounter > _dashWait) {
					// Choose direction
					sweepCounter++;

					goingRight = !goingRight;

					startPos = transform.position;

					var backtrack = Random.value <= _dashBacktrackChance;

					var hVector = referenceFrame.right * (Random.Range(_dashMinDistance, _dashMaxDistance) * (goingRight ? 1 : -1));
					var vVector = referenceFrame.up;
					if (backtrack) {
						vVector *= Random.Range(0, _dashBack);
					} else {
						vVector *= -Random.Range(0, _dashForward);
					}

					endPos = startPos + hVector + vVector;
					timeCounter = 0;
					currentMode = AIMode.Dashing;
				} else {
					timeCounter += Time.deltaTime;
				}
				break;

			case AIMode.Hovering:
				if (timeCounter >= _hoverTime) {
					currentMode = AIMode.Undecided;
					return; // Start movement on next frame
				} else {
					timeCounter += Time.deltaTime;

					currentPos -= referenceFrame.up * _hoverSpeed;
				}
				break;

			case AIMode.ForcedMovement:
				if (timeCounter <= 1) {
					timeCounter += Time.deltaTime * _forcedSpeed;
					currentPos = Vector2Ex.Hermite(startPos, endPos, timeCounter);
				} else {
					currentMode = AIMode.Undecided;
					return; // Restart next frame.
				}
				break;

			default:
				return;	// Do nothing.
		}

		this.rigidbody2D.MovePosition(currentPos);
	}

	private Vector2 CheckIfOffscreen(Vector2 displacement) {
		if (this.rigidbody2D.position.y < ScreenBounds.Bottom - vMargin && referenceFrame.up.y > 0) {
			// Si salimos por el borde inferior y el marco de referencia apunta hacia arriba.
			displacement += Vector2.up * (ScreenBounds.VerticalDistance + 2 * vMargin);
			// Se mueve todo hacia arriba.
		} else if (this.rigidbody2D.position.y > ScreenBounds.Top + vMargin && referenceFrame.up.y < 0) {
			// Si salimos por el borde superior y el marco de referencia apunta hacia abajo.
			displacement -= Vector2.up * (ScreenBounds.VerticalDistance + 2 * vMargin);
			// Se mueve todo hacia abajo.
		}
		if (this.rigidbody2D.position.x < ScreenBounds.Left - hMargin && referenceFrame.up.x > 0) {
			// Si salimos por el border izquierdo y el marco de referencia apunta a la derecha.
			displacement += Vector2.right * (ScreenBounds.HorizontalDistance + 2 * hMargin);
			// Se mueve todo hacia la derecha.
		} else if (this.rigidbody2D.position.x > ScreenBounds.Right + hMargin && referenceFrame.up.x < 0) {
			// Si salimos por el border derecho y el marco de referencia apunta a la izquierda.
			displacement -= Vector2.right * (ScreenBounds.HorizontalDistance + 2 * hMargin);
			// Se mueve todo hacia la izquierda.
		}
		return displacement;
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