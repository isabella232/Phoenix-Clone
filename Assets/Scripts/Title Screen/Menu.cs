using UnityEngine;

public enum UITransitionState {
	EasingIn,
	EasingOut,
	Steady
}

public class Menu : MonoBehaviour {
	private float timeCounter;
	private MainMenuState state;
	private MainMenuState nextState;
	private UITransitionState transition;

	public float transitionSpeed;

	public enum MainMenuState {
		NothingToDraw,
		Title,
		Credits,
		HighScore,
	}

	// Use this for initialization
	private void Start() {
		timeCounter = 0;
		state = MainMenuState.Title;
		transition = UITransitionState.EasingIn;
	}

	private void Update() {
		switch (transition) {
			case UITransitionState.EasingIn:
				timeCounter = Mathf.Clamp01(timeCounter + Time.deltaTime * transitionSpeed);
				if (timeCounter >= 1) {
					transition = UITransitionState.Steady;
				}
				break;

			case UITransitionState.EasingOut:
				timeCounter = Mathf.Clamp01(timeCounter - Time.deltaTime * transitionSpeed);
				if (timeCounter <= 0) {
					state = nextState;
					transition = UITransitionState.EasingIn;
					return;
				}
				break;

			default:
				break;
		}

		if (Input.GetKeyDown(KeyCode.F5)) {
			state = MainMenuState.Title;
			transition = UITransitionState.EasingIn;
			timeCounter = 0;
		}
	}

	private void OnGUI() {
		var size = new Vector2(0.3f, 0.3f);
		var center = new Vector2(0.5f, 0.7f);

		var halfSize = size / 2;
		var topLeft = center - size / 2;

		// Dibuja ese fondo negroso del menu.
		GUI.Box(new Rect(Screen.width * topLeft.x, 0, Screen.width * size.x, Screen.height), "");

		var boxStyle = new GUIStyle();
		boxStyle.alignment = TextAnchor.MiddleCenter;

		var labelStyle = CustomStyle.GetLabelStyle();
		labelStyle.alignment = TextAnchor.MiddleCenter;

		CustomStyle.SetStyleData(GUI.skin.GetStyle("button"));
		
		var menuRect = new Rect(
						Screen.width * (topLeft.x + halfSize.x * (timeCounter - 1) * -1),
						Screen.height * (topLeft.y + halfSize.y * (timeCounter - 1) * -1),
						Screen.width * size.x * timeCounter,
						Screen.height * size.y * timeCounter);

		GUI.enabled = transition == UITransitionState.Steady;

		switch (state) {
			case MainMenuState.HighScore:
				DrawHighScore(boxStyle, labelStyle, menuRect);
				break;

			case MainMenuState.Credits:
				DrawCredits(boxStyle, labelStyle, menuRect);
				break;

			case MainMenuState.Title:
			default:
				DrawTitle(boxStyle, menuRect);
				break;

			case MainMenuState.NothingToDraw:
				break;
		}
	}

	private void DrawTitle(GUIStyle boxStyle, Rect menuRect) {
		GUILayout.BeginArea(menuRect, boxStyle);
		GUILayout.BeginVertical();
		{
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Nuevo Juego")) {
				FindObjectOfType<BackgroundManager>().FadeOut(StartGame);
				GoToState(MainMenuState.NothingToDraw);
			}
			if (GUILayout.Button("Ver High Scores")) {
				GoToState(MainMenuState.HighScore);
			}
			if (GUILayout.Button("Creditos")) {
				GoToState(MainMenuState.Credits);
			}
			if (GUILayout.Button("Salir")) {
				FindObjectOfType<BackgroundManager>().FadeOut(QuitGame);
				GoToState(MainMenuState.NothingToDraw);
			}
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	private void DrawCredits(GUIStyle boxStyle, GUIStyle labelStyle, Rect menuRect) {
		GUILayout.BeginArea(menuRect, boxStyle);
		GUILayout.BeginVertical();
		{
			GUILayout.FlexibleSpace();
			GUILayout.Label("Programado por Kyte");
			GUILayout.Label("Assets sacados de internet");
			GUILayout.Label("Para mayor información, ver descripción");
			if (GUILayout.Button("Volver") || Input.GetKeyDown(KeyCode.Escape)) {
				GoToState(MainMenuState.Title);
			}
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	private void DrawHighScore(GUIStyle boxStyle, GUIStyle labelStyle, Rect menuRect) {
		var highScore = PlayerPrefs.GetInt("HighScore", 1000);
		var highScorer = PlayerPrefs.GetString("HighScorer", "AAA");

		GUILayout.BeginArea(menuRect, boxStyle);
		GUILayout.BeginVertical();
		{
			GUILayout.FlexibleSpace();

			GUILayout.Label("High Score");
			GUILayout.BeginHorizontal();
			{
				labelStyle.alignment = TextAnchor.MiddleLeft;
				GUILayout.Label(string.Format("{0}", highScorer, highScore));
				labelStyle.alignment = TextAnchor.MiddleRight;
				GUILayout.Label(string.Format("{1}", highScorer, highScore));
			}
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Volver") || Input.GetKeyDown(KeyCode.Escape)) {
				GoToState(MainMenuState.Title);
			}
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	private void StartGame() {
		Application.LoadLevel("Main Scene");
	}

	private void QuitGame() {
		Application.Quit();
	}

	private void GoToState(MainMenuState state) {
		nextState = state;
		transition = UITransitionState.EasingOut;
		timeCounter = 1;
	}
}