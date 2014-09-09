using UnityEngine;
using System.Collections;

public class ShieldSoundController : MonoBehaviour {
	public AudioClip shieldFlash;

	void PlayFlashSound() {
		this.audio.PlayOneShot(shieldFlash);
	}
}
