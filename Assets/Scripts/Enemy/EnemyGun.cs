using System.Collections;
using UnityEngine;

public class EnemyGun : MonoBehaviour {
	public GameObject shotPrefab;
	public float fireChance;
	public float fireInterval;
	public bool holdFire;
	private EnemyMovementBase movementController;

	// Use this for initialization
	private void Start() {
		movementController = GetComponentInParent<EnemyMovementBase>();
		StartCoroutine(FireGun());
	}

	// Update is called once per frame
	private void Update() {
	}

	private IEnumerator FireGun() {
		while (!holdFire || movementController.IsOffscreen()) {
			yield return new WaitForSeconds(fireInterval);
			if (Random.value <= fireChance) {
				Instantiate(shotPrefab, this.transform.position, this.transform.rotation);
			}
		}
	}
}