using System.Collections;
using System.Linq;
using UnityEngine;

public abstract class EnemyMovementBase : MonoBehaviour {
	protected float hMargin, vMargin;
	protected ReferenceFrame referenceFrame;
	protected bool isFrozen;

	private bool previousFireState;
	private float previousAnimationSpeed;
	private Animator animator;
	private EnemyGun gun;

	public virtual void Freeze() { // Notar la potencial herencia en esta y las otras funciones.
		animator = this.GetComponentInChildren<Animator>();
		gun = this.GetComponentInChildren<EnemyGun>();
		previousFireState = gun.holdFire;
		previousAnimationSpeed = animator.speed;

		animator.speed = 0;
		gun.holdFire = true;
		isFrozen = true;
	}

	public virtual void Unfreeze() {
		if (!isFrozen) return;

		isFrozen = false;
		gun.holdFire = previousFireState;
		animator.speed = previousAnimationSpeed;
	}

	public virtual void Freeze(float duration) {
		StartCoroutine(FreezeCoroutine(duration));
	}

	protected virtual IEnumerator FreezeCoroutine(float duration) { // Notar el virtual.
		Freeze();
		yield return new WaitForSeconds(duration);
		Unfreeze();
	}

	public abstract void ForceMovement(Vector2 targetPosition, float movementSpeed);

	protected bool IsOffscreen() {
		if (this.rigidbody2D.position.y < ScreenBounds.Bottom - vMargin ||
			this.rigidbody2D.position.y > ScreenBounds.Top + vMargin ||
			this.rigidbody2D.position.x < ScreenBounds.Left - hMargin || 
			this.rigidbody2D.position.x > ScreenBounds.Right + hMargin) {
				return true;
		}
		return false;
	}

	protected Vector2 GetOffscreenDisplacement() {
		var displacement = Vector2.zero;

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

	protected virtual void Start() {
		var spriteBounds =
			this.GetComponentsInChildren<Renderer>()
			.Aggregate(new Bounds(), (bounds, renderer) => { bounds.Encapsulate(renderer.bounds); return bounds; });
		// Es como un foreach que acumula pero prefiero usar linq. :V

		hMargin = spriteBounds.size.x / 2;
		vMargin = spriteBounds.size.y / 2;

		referenceFrame = GetComponentInParent<ReferenceFrame>();
		if (referenceFrame == null) {
			Debug.LogError("There's an enemy not parented to the scene controller!", this);
			Debug.Break();
		}
	}
}