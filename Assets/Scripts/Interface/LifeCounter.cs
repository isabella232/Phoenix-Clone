using System.Collections;
using UnityEngine;

public class LifeCounter : MonoBehaviour {
	public GameObject playerPrefab;
	public int startingLives;
	public float respawnTime;
	public float invincibleTime;

	private BackgroundManager bgManager;
	private ScoreController scoreController;
	private int currentLives;
	private bool showHighScoreBox;
	private UITransitionState transition;

	private PlayerSpecial playerSpecial;

	public void Start() {
		currentLives = startingLives;
		bgManager = FindObjectOfType<BackgroundManager>();
		scoreController = FindObjectOfType<ScoreController>();
		showHighScoreBox = false;
		timeCounter = 0;
		transition = UITransitionState.Steady;
		playerSpecial = FindObjectOfType<PlayerSpecial>();
	}

	private float timeCounter;

	public void Update() {
		if (showHighScoreBox) {
			switch (transition) {
				case UITransitionState.EasingIn:
					timeCounter = Mathf.Clamp01(timeCounter + Time.deltaTime);

					if (timeCounter >= 1) {
						transition = UITransitionState.Steady;
					}
					break;

				case UITransitionState.EasingOut:
					timeCounter = Mathf.Clamp01(timeCounter - Time.deltaTime);

					if (timeCounter <= 0) {
						transition = UITransitionState.Steady;
					}
					break;

				default:
					break;
			}
		}

		if (Input.GetKeyDown(KeyCode.F8)) {
			scoreController.Award(1000);
			FindObjectOfType<LifeCounter>().currentLives = 0;
			FindObjectOfType<PlayerLife>().OnHit();
		}

		
	}

	public void OnGUI() {
		var style = CustomStyle.GetLabelStyle();
		style.alignment = TextAnchor.UpperLeft;

		GUILayout.BeginArea(new Rect(5, 5, 300, 100));
		{
			GUILayout.BeginVertical();
			{
				GUILayout.Label(string.Format("Lives: {0}", currentLives));
				if (playerSpecial.powerController != null) {
					GUILayout.Label(string.Format("{0} x{1}", playerSpecial.powerController.SpecialName, playerSpecial.powerCount));
				} else {
					GUILayout.Label("No Powerup");
				}
			}
		}
		GUILayout.EndArea();

		if (showHighScoreBox) {
			var boxStyle = GUI.skin.GetStyle("box");
			boxStyle.alignment = TextAnchor.MiddleCenter;

			CustomStyle.SetStyleData(GUI.skin.GetStyle("button"));

			var size = new Vector2(0.3f, 0.3f);
			var center = new Vector2(0.5f, 0.5f);

			var halfSize = size / 2;
			var topLeft = center - size / 2;
			var menuRect = new Rect(
				Screen.width * (topLeft.x + halfSize.x * (timeCounter - 1) * -1),
				Screen.height * (topLeft.y + halfSize.y * (timeCounter - 1) * -1),
				Screen.width * size.x * timeCounter,
				Screen.height * size.y * timeCounter);

			style.alignment = TextAnchor.MiddleCenter;
			GUILayout.BeginArea(menuRect, boxStyle);
			/*GUILayout.BeginVertical();
			{
				GUI.enabled = transition == UITransitionState.Steady;

				GUILayout.FlexibleSpace();

				style.alignment = TextAnchor.MiddleCenter;
				GUILayout.Label("Nuevo High Score!");

				GUILayout.FlexibleSpace();

				GUILayout.BeginHorizontal();
				{
					var name = GUILayout.TextField("AAA", 3);
					if (GUILayout.Button("Guardar")) {
					}
				}
			}
			GUILayout.EndVertical();*/
			GUILayout.EndArea();
		}
	}

	public void LostLife(Vector3 position, Quaternion rotation) {
		bgManager.Pause();

		if (currentLives == 0) {
			StartCoroutine(PlayerLost());
		} else {
			StartCoroutine(WaitAndRespawn(position, rotation));
		}
	}

	public IEnumerator WaitAndRespawn(Vector3 position, Quaternion rotation) {
		yield return new WaitForSeconds(respawnTime);

		SpawnNewPlayer(position, rotation);

		bgManager.Resume();
		currentLives--;
	}

	private void SpawnNewPlayer(Vector3 position, Quaternion rotation) {
		var player = Instantiate(playerPrefab, position, rotation) as GameObject;
		player.GetComponent<PlayerLife>().MakeInvincible(invincibleTime);
		FindObjectOfType<ReferenceFrame>().player = player;
		playerSpecial = player.GetComponentInChildren<PlayerSpecial>();
	}

	public IEnumerator PlayerLost() {
		if (scoreController.IsNewHighScore) {
			showHighScoreBox = true;
			timeCounter = 0;
			transition = UITransitionState.EasingIn;
		}

		foreach (var shot in GameObject.FindGameObjectsWithTag("Shot")) {
			Destroy(shot);
		}
		var enemies = GameObject.FindGameObjectsWithTag("EnemyController");
		foreach (var enemy in enemies) {
			enemy.GetComponentInChildren<EnemyGun>().holdFire = true;
			enemy.GetComponentInChildren<EnemyMovementBase>().enabled = false;
		}
		yield return new WaitForSeconds(respawnTime);
		foreach (var enemy in enemies) {
			enemy.GetComponentInChildren<Animator>().SetTrigger("Celebrate");
		}
		bgManager.PlayDeathSong();
	}
}