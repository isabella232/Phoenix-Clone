using System.Collections;
using UnityEngine;

public abstract class EnemyMovementBase : MonoBehaviour {
	protected float visibleBoxSize;
	protected ReferenceFrame referenceFrame;
	protected bool isFrozen;
	protected Vector2 currentPos, velocity;

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

	/*public void LateUpdate() {
		ScreenBounds.DrawBox(this.rigidbody2D.position.x - visibleBoxSize / 2, this.rigidbody2D.position.y + visibleBoxSize / 2, this.rigidbody2D.position.x + visibleBoxSize / 2, this.rigidbody2D.position.y - visibleBoxSize / 2, Color.white);
	}*/

	public bool IsOffscreen() {
		return !ScreenBounds.Rect.Overlaps(
			new Rect(this.rigidbody2D.position.x - visibleBoxSize / 2, this.rigidbody2D.position.y + visibleBoxSize / 2, visibleBoxSize, visibleBoxSize)
			);
	}

	protected Vector2 GetOffscreenDisplacement() {
		if (!IsOffscreen()) return Vector2.zero;

		float x = 0, y = 0;

		if (this.rigidbody2D.position.y < ScreenBounds.Bottom && velocity.y < 0) {
			// Si salimos por el borde inferior y va abajo.
			y = (ScreenBounds.Height + visibleBoxSize);
			// Se mueve todo hacia arriba.
		} else if (this.rigidbody2D.position.y > ScreenBounds.Top && velocity.y > 0) {
			// Si salimos por el borde superior y va arriba
			y = -(ScreenBounds.Height + visibleBoxSize);
			// Se mueve todo hacia abajo.
		}

		if (this.rigidbody2D.position.x < ScreenBounds.Left && velocity.x < 0) {
			// Si salimos por el border izquierdo y va a la derecha.
			x = (ScreenBounds.Width + visibleBoxSize);
			// Se mueve todo hacia la derecha.
		} else if (this.rigidbody2D.position.x > ScreenBounds.Right && velocity.x > 0) {
			// Si salimos por el border derecho y va a la derecha.
			x = (ScreenBounds.Width + visibleBoxSize);
			// Se mueve todo hacia la izquierda.
		}
		return new Vector2(x, y);
	}

	protected virtual void Start() {
		var collider = this.GetComponentInChildren<Collider2D>();

		visibleBoxSize = Mathf.Max(collider.bounds.size.x, collider.bounds.size.y);

		referenceFrame = GetComponentInParent<ReferenceFrame>();
		if (referenceFrame == null) {
			Debug.LogError("There's an enemy not parented to the scene controller!", this);
			Debug.Break();
		}

		currentPos = this.rigidbody2D.position;
	}
}