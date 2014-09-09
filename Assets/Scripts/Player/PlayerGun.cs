using System.Collections;
using UnityEngine;

public class PlayerGun : MonoBehaviour {
	public GameObject shotPrefab;

	[HideInInspector]
	public int CurrentShotsOnScreen;

	public int MaxShotsOnScreen;
	public float ShotInterval;

	private bool readyToFire;

	private void Start() {
		readyToFire = true;
		CurrentShotsOnScreen = 0;
	}

	private void Update() {
		if (readyToFire && CurrentShotsOnScreen < MaxShotsOnScreen && Input.GetButtonDown("Fire1")) {
			Fire();
		}
	}

	public void Fire() {
		var shot = Instantiate(shotPrefab, this.transform.position, this.transform.rotation) as GameObject;
		var script = shot.GetComponent<ShotController>();
		script.gun = this;

		readyToFire = false;
		++CurrentShotsOnScreen;
		StartCoroutine(ResetGun());
	}

	private IEnumerator ResetGun() {
		yield return new WaitForSeconds(ShotInterval);
		readyToFire = true;
	}
}