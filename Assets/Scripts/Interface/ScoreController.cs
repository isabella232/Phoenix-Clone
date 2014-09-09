using UnityEngine;

public class ScoreController : MonoBehaviour {

	[HideInInspector]
	public int Score;

	[HideInInspector]
	public int HighScore;

	public bool IsNewHighScore { get; private set; }

	private int savedHighScore;

	// Use this for initialization
	private void Start() {
		savedHighScore = PlayerPrefs.GetInt("HighScore", 0);
		Score = HighScore = 0;

		HighScore = savedHighScore;
	}

	private void OnGUI() {
		if (Score > HighScore) {
			HighScore = Score;

			if (HighScore > savedHighScore) {
				IsNewHighScore = true;
			}
		}

		var labelStyle = CustomStyle.GetLabelStyle();
		labelStyle.alignment = TextAnchor.UpperRight;

		var contentScore = new GUIContent(string.Format("Score: {0:000000}", Score));
		var contentHigh = new GUIContent(string.Format("High Score: {0:000000}", HighScore));

		var sizeScore = labelStyle.CalcSize(contentScore);
		var sizeHigh = labelStyle.CalcSize(contentHigh);
		var height = sizeScore.y + sizeHigh.y;

		GUILayout.BeginArea(new Rect(Screen.width / 2 - 5, 5, Screen.width / 2, height));
		{
			GUILayout.BeginVertical();
			{
				GUILayout.Label(contentScore);
				GUILayout.Label(contentHigh);
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndArea();
	}

	public void SaveHighScore(string name) {
		PlayerPrefs.SetInt("HighScore", HighScore);
		PlayerPrefs.SetString("HighScorer", name);
		savedHighScore = HighScore;
		IsNewHighScore = false;
	}

	public void Award(int scoreValue) {
		Score += scoreValue;
	}
}