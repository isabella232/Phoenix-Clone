using UnityEngine;

public class ScreenBounds : MonoBehaviour {
	public bool DrawCameraBounds;

	private static Vector2 _bottomleft;

	public static Vector2 BottomLeft {
		get {
			if (!isReady) GetReady();
			return _bottomleft;
		}
		private set {
			_bottomleft = value;
		}
	}

	private static Vector2 _topRight;

	public static Vector2 TopRight {
		get {
			if (!isReady) GetReady();
			return _topRight;
		}
		private set {
			_topRight = value;
		}
	}

	public static float Left { get { return BottomLeft.x; } }

	public static float Right { get { return TopRight.x; } }

	public static float Top { get { return TopRight.y; } }

	public static float Bottom { get { return BottomLeft.y; } }

	public static float HorizontalDistance { get { return Right - Left; } }

	public static float VerticalDistance { get { return Top - Bottom; } }

	private static bool isReady = false;

	// Use this for initialization
	public void Start() {
		GetReady();
	}

	private static void GetReady() {
		BottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
		TopRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
		isReady = true;
	}
	// Todo este baile es para asegurarme que los valores estén inicializados cuando alguien llama a la función.

	public void Update() {
		if (DrawCameraBounds) {
			DrawScreenWithMargins(0, 0, Color.gray);
		}
	}

	public static void DrawScreenWithMargins(float marginX, float marginY, Color color) {
		DrawBox(Left - marginX, Top + marginY, Right + marginX, Bottom - marginY, color);
	}

	public static void DrawBox(float left, float top, float right, float bottom, Color color) {
		Debug.DrawLine(new Vector3(left, top, 0), new Vector3(right, top, 0), color);
		Debug.DrawLine(new Vector3(left, bottom, 0), new Vector3(right, bottom, 0), color);
		Debug.DrawLine(new Vector3(left, top, 0), new Vector3(left, bottom, 0), color);
		Debug.DrawLine(new Vector3(right, top, 0), new Vector3(right, bottom, 0), color);
	}
}