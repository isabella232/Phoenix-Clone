using UnityEngine;

public class EMPController : SpecialPowerController {
	public float growthRate;
	public float duration;
	private float timeCounter;

	private CircleCollider2D circleCollider;
	private float colliderRadius;

	// Use this for initialization
	private void Start() {
		timeCounter = 0;
		circleCollider = GetComponent<CircleCollider2D>();
		colliderRadius = circleCollider.radius;
	}

	// Update is called once per frame
	private void Update() {
		timeCounter += Time.deltaTime * growthRate;
		transform.localScale = Vector3.one * timeCounter;
		circleCollider.radius = colliderRadius * timeCounter;

		if (timeCounter >= 2) {
			if (OnSpecialEnd != null) OnSpecialEnd();
			Destroy(this.gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Enemy") {
			other.GetComponentInParent<EnemyMovement>().OnEMP(duration);
		}
	}

	public override string SpecialName {
		get { return "EMP"; }
	}
}