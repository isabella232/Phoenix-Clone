using System.Collections;
using UnityEngine;

public class MagnetShotController : SpecialPowerController {
	public float magnetDuration, magnetForce;
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
			other.GetComponentInParent<EnemyLife>().OnMagnetHit(magnetDuration, magnetForce);
			controller.Remove();
		}
	}

	public override string SpecialName {
		get { return "Magnet Shot"; }
	}
}