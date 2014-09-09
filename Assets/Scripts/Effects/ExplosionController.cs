using System.Collections;
using UnityEngine;

public class ExplosionController : MonoBehaviour {

	// Use this for initialization
	private void Start() {
	}

	// Update is called once per frame
	private void Update() {
	}

	public void AnimationDone() {
		Destroy(this.gameObject);
	}
}