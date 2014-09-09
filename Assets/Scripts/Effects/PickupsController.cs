using UnityEngine;

public class PickupsController : MonoBehaviour {
	public int pickupAmount;
	public GameObject specialPrefab;
	public AudioClip pickupSound;
	public float fallSpeed;
	private ReferenceFrame referenceFrame;

	private float topBound, bottomBound, leftBound, rightBound;
	private Vector3 margin;

	// Use this for initialization
	private void Start() {
		referenceFrame = GetComponentInParent<ReferenceFrame>();

		margin = this.renderer.bounds.size;
		topBound = ScreenBounds.Top + margin.y;
		bottomBound = ScreenBounds.Bottom - margin.y;
		leftBound = ScreenBounds.Left - margin.x;
		rightBound = ScreenBounds.Right + margin.x;
	}

	// Update is called once per frame
	private void Update() {
		this.rigidbody2D.velocity = -referenceFrame.up * fallSpeed;

		if (this.rigidbody2D.position.y > topBound || this.rigidbody2D.position.y < bottomBound
			||
			this.rigidbody2D.position.x > rightBound || this.rigidbody2D.position.x < leftBound
			) {
			Destroy(this.gameObject);
		}
	}

	public void OnHit() {
		var playerSpecial = FindObjectOfType<PlayerSpecial>();
		playerSpecial.Give(specialPrefab, pickupAmount);

		AudioSource.PlayClipAtPoint(pickupSound, this.transform.position);
		Destroy(this.gameObject);
	}
}