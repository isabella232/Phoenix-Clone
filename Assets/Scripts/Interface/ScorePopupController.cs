using UnityEngine;

public class ScorePopupController : MonoBehaviour {
	private ScoreController scoreMaster;

	[HideInInspector]
	public int scoreValue;

	public float popupDuration;
	public float popupMovement;

	private float accumulatedTime;

	// Use this for initialization
	private void Start() {
		scoreMaster = FindObjectOfType<ScoreController>();
		scoreMaster.Award(scoreValue);

		this.guiText.text = scoreValue.ToString();

		accumulatedTime = 0;
	}

	// Update is called once per frame
	private void Update() {
		accumulatedTime += Time.deltaTime;

		if (accumulatedTime >= popupDuration) {
			Destroy(this.gameObject);
		} else {
			this.transform.position = new Vector3(this.transform.position.x,
				this.transform.position.y + Mathf.Lerp(0, popupMovement, accumulatedTime),
				this.transform.position.z);
		}
	}
}