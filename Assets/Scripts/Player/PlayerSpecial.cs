using System;
using System.Collections;
using UnityEngine;

public abstract class SpecialPowerController : MonoBehaviour {

	public Action OnSpecialEnd { get; set; }
	public abstract string SpecialName { get; }
}

public class PlayerSpecial : MonoBehaviour {
	public GameObject powerTemplate;
	[HideInInspector]
	public SpecialPowerController powerController;
	public int powerCount;

	private bool specialInAction;

	// Use this for initialization
	private void Start() {
		specialInAction = false;
	}

	// Update is called once per frame
	private void Update() {
		if (powerTemplate != null && powerCount > 0 && !specialInAction && Input.GetButtonDown("Fire3")) {
			StartCoroutine(FireSpecial());
		}
	}

	private IEnumerator FireSpecial() {
		var special = Instantiate(powerTemplate, this.transform.position, this.transform.rotation) as GameObject;
		var controller = special.GetComponent<SpecialPowerController>();
		if (controller != null) {
			controller.OnSpecialEnd = SpecialEnded;
		}
		specialInAction = true;
		powerCount--;

		if (powerCount <= 0) {
			powerTemplate = null;
			this.powerController = null;
		}

		yield break;
	}

	private void SpecialEnded() {
		specialInAction = false;
	}

	public void Give(GameObject specialPrefab, int pickupAmount) {
		if (this.powerTemplate == specialPrefab) {
			this.powerCount += pickupAmount;
		} else {
			this.powerTemplate = specialPrefab;
			this.powerCount = pickupAmount;
			this.powerController = this.powerTemplate.GetComponent<SpecialPowerController>();
		}
	}
}