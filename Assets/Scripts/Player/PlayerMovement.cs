using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	public float movementSpeed;
	public float turnSpeed;
	public AudioClip frozenSound;

	private bool isFrozen;
	private Animator shipAnimator;
	private float leftBound, rightBound;
	private float shipAngle;
	private float vRadius, hRadius;
	private ReferenceFrame referenceFrame;

	// Use this for initialization
	private void Start() {
		shipAnimator = this.GetComponentInChildren<Animator>();

		var margin = this.GetComponentInChildren<Renderer>().bounds.size;

		leftBound = ScreenBounds.Left + margin.x;
		rightBound = ScreenBounds.Right - margin.x;

		vRadius = ScreenBounds.Bottom + margin.y;
		hRadius = ScreenBounds.Left + margin.x;
		shipAngle = GetOriginalAngle();
		
		referenceFrame = FindObjectOfType<ReferenceFrame>();
	}

	private float GetOriginalAngle() { // Este lo saque con algebra en papel y testeo al ojo.
		return Mathf.Repeat(Mathf.Atan2(this.transform.position.y / vRadius, this.transform.position.x / hRadius) * Mathf.Rad2Deg - 90, 359);
	}

	private float hAxis;

	// Input va en update, fisicas van en FixedUpdate.
	private void Update() {
		if (isFrozen) return;
		// Los controles están configurados en el input manager para que vayan a +-1 instantaneamente, para simular
		// controles digitales.
		hAxis = Input.GetAxis("Horizontal"); // +1 es entero a la derecha, -1 es entero a la izquierda, 0 es neutro;
	}

	private void FixedUpdate() {
		if (referenceFrame.radialMovement) RadialUpdate(hAxis); else RectagularUpdate(hAxis);
	}

	private void RectagularUpdate(float hAxis) {
		var xPosition = this.rigidbody2D.position.x;

		if ((hAxis < 0 && xPosition >= leftBound) || (hAxis > 0 && xPosition <= rightBound)) {
			shipAnimator.SetBool("IsMoving", true);

			this.rigidbody2D.velocity = new Vector2(movementSpeed * hAxis, 0);
		} else {
			shipAnimator.SetBool("IsMoving", false);

			this.rigidbody2D.velocity = Vector2.zero;
		}
	}

	private void RadialUpdate(float hAxis) {
		if (hAxis != 0) {
			shipAnimator.SetBool("IsMoving", true);

			shipAngle = Mathf.Repeat(shipAngle + hAxis * turnSpeed, 359);
			// 360 se convierte en 0 and so on.

			var positionAngle = Mathf.Repeat(shipAngle + 90, 359) * Mathf.Deg2Rad;
			/* Los detalles de porqué tengo que sumar 90º son magia negra (los calculos me dicen que deberia
			 * _restar_ 90 ya que el angulo 0 del mundo apunta a la derecha y el angulo 0 de la nave apunta
			 * hacia arriba) pero queda en la posicion correcta y eso es lo que importa. */
			// nvm ya cache. saco vRadius del border inferior, que es negativo. Eso deja el eje Y al reves.
			var x = hRadius * Mathf.Cos(positionAngle);
			var y = vRadius * Mathf.Sin(positionAngle);

			var shipAngleCorrected = Mathf.Atan2(y, x) * Mathf.Rad2Deg + 90;
			/* Esto parece redundante pero la rotacion original asume una trayectoria circular, no elíptica.
			 * Creí que habia una manera de transformarla directamente sin usar tangente y arcotangente pero al
			 * parecer no es así.
			 * No es necesario pero prefiero que siempre apunte hacia el centro, porque es lo que uno espera. */

			this.rigidbody2D.MoveRotation(shipAngleCorrected);
			this.rigidbody2D.MovePosition(new Vector2(x, y));
		} else {
			shipAnimator.SetBool("IsMoving", false);
		}
	}

	public void Freeze(float duration) {
		StartCoroutine(FreezeCoroutine(duration));
	}

	private IEnumerator FreezeCoroutine(float duration) {
		isFrozen = true;
		StartCoroutine(PlayFrozenEffect());
		yield return new WaitForSeconds(duration);
		isFrozen = false;
	}

	private IEnumerator PlayFrozenEffect() {
		while (isFrozen) {
			this.audio.PlayOneShot(frozenSound);
			yield return new WaitForSeconds(1);
		}
	}

	/*private void OnGUI() {
		GUI.skin.GetStyle("label").alignment = TextAnchor.MiddleLeft;
		GUILayout.BeginArea(new Rect(0, Screen.height / 2, Screen.width / 2, Screen.height / 2));
		GUILayout.Label((inputAngle).ToString());
		GUILayout.Label(UncorrectAngle(inputAngleCorrected).ToString());
		GUILayout.EndArea();
	}*/
}