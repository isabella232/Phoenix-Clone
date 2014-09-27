using UnityEngine;

public class ScreenBounds : MonoBehaviour {
	public bool DrawCameraBounds;

	private static Vector2 _bottomLeft, _topRight;

	public static float Left { get { if (!isReady) GetReady(); return _bottomLeft.x; } }

	public static float Right { get { if (!isReady) GetReady(); return _topRight.x; } }

	public static float Top { get { if (!isReady) GetReady(); return _topRight.y; } }

	public static float Bottom { get { if (!isReady) GetReady(); return _bottomLeft.y; } }

	public static float Width { get { if (!isReady) GetReady(); return _screenRect.width; } }

	public static float Height { get { if (!isReady) GetReady(); return _screenRect.height; } }

	public static Rect Rect { get { if (!isReady) GetReady(); return _screenRect; } }

	private static bool isReady = false;
	private static Rect _screenRect;

	// Use this for initialization
	public void Start() {
		GetReady();
	}

	private static void GetReady() {
		_bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
		_topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
		_screenRect = new Rect(_bottomLeft.x, _topRight.y, _topRight.x - _bottomLeft.x, _topRight.y - _bottomLeft.y);
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