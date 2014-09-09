using System.Collections;
using UnityEngine;

public class EnemyGun : MonoBehaviour {
	public GameObject shotPrefab;
	public float fireChance;
	public float fireInterval;
	public bool holdFire;

	// Use this for initialization
	private void Start() {
		StartCoroutine(FireGun());
	}

	// Update is called once per frame
	private void Update() {
	}

	private IEnumerator FireGun() {
		while (!holdFire) {
			yield return new WaitForSeconds(fireInterval);
			if (Random.value <= fireChance) {
				Instantiate(shotPrefab, this.transform.position, this.transform.rotation);
			}
		}
	}
}