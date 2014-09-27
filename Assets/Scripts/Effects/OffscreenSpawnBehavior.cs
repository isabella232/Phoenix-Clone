using UnityEngine;

[RequireComponent(typeof(Spawner))]
public abstract class SpawnBehavior : MonoBehaviour {

	public abstract Vector2 GetPosition();
}

public class OffscreenSpawnBehavior : SpawnBehavior {
	private bool isSetUp;

	private float margin;
	private Vector2 middleBoxTopLeft, middleBoxBottomRight;
	private Spawner spawner;

	/* Digamos que esta es la pantalla, dividida en 9 partes:
	 *            1           |           2
	 *       ___________________________________
	 *      |           |           |           |
	 *   8  |     A     |     E     |     B     |  3
	 *      |___________|___________|___________|
	 *      |           |           |           |
	 *   -  |     H     |     I     |     F     |  -
	 *      |___________|___________|___________|
	 *      |           |           |           |
	 *   7  |     D     |     G     |     C     |  4
	 *      |___________|___________|___________|
	 *
	 *            6           |           5
	 *
	 * El enemigo aparece en los lados opuestos al sector que ocupa.
	 * A: 4+5          E: 5+6
	 * B: 6+7          F: 7+8
	 * C: 8+1          G: 1+2
	 * D: 2+3          H: 3+4
	 * */

	private enum Vertical { Undef, Top, Middle, Bottom }

	private enum Horizontal { Undef, Left, Middle, Right }

	private Vertical vArea;
	private Horizontal hArea;

	public void Start() {
		spawner = GetComponent<Spawner>();
		margin = GetComponentInParent<ReferenceFrame>().player.GetComponentInChildren<Renderer>().bounds.size.magnitude;

		// La diagonal de la nave del jugador da una medida decente.

		middleBoxBottomRight = new Vector2(ScreenBounds.Width / 6, -ScreenBounds.Height / 6);
		middleBoxTopLeft = -middleBoxBottomRight;
	}

	public override Vector2 GetPosition() {
		var playerPos = spawner.referenceFrame.player.rigidbody2D.position;

		hArea =
			playerPos.x < middleBoxTopLeft.x ? Horizontal.Left :
			playerPos.x > middleBoxBottomRight.x ? Horizontal.Right :
			Horizontal.Middle;
		vArea =
			playerPos.y > middleBoxTopLeft.y ? Vertical.Top :
			playerPos.y < middleBoxBottomRight.y ? Vertical.Bottom :
			Vertical.Middle;
		var useVerticalSpawnAreas = Random.value < 0.5;
		// Las areas verticales son 3 4 7 y 8.
		// Las areas horizontales son 1 2 5 y 6.

		float x, y;

		switch (hArea) {
			case Horizontal.Left: // A, H o D
				if (useVerticalSpawnAreas) {
					x = ScreenBounds.Right + margin; // 3 o 4
				} else {
					x = Random.Range(0, ScreenBounds.Right + margin); // 2 o 5
				}
				break;

			case Horizontal.Right: // B, F o C
				if (useVerticalSpawnAreas) {
					x = ScreenBounds.Left - margin; // 7 o 8
				} else {
					x = Random.Range(ScreenBounds.Left - margin, 0); // 1 o 6
				}
				break;

			case Horizontal.Middle: // E o G
			default:
				x = Random.Range(ScreenBounds.Left - margin, ScreenBounds.Right + margin); // 1+2 o 5+6
				break;
		}

		switch (vArea) {
			case Vertical.Top: // A, E o B
				if (hArea == Horizontal.Middle || !useVerticalSpawnAreas) {
					y = ScreenBounds.Bottom - margin; // 5 o 6
				} else {
					y = Random.Range(0, ScreenBounds.Bottom - margin); // 4 o 7
				}
				break;

			case Vertical.Bottom: // C, G o D
				if (hArea == Horizontal.Middle || !useVerticalSpawnAreas) {
					y = ScreenBounds.Top + margin; // 1 o 2
				} else {
					y = Random.Range(ScreenBounds.Top + margin, 0); // 3 o 8
				} break;

			case Vertical.Middle: // H o F
			default:
				y = Random.Range(ScreenBounds.Top + margin, ScreenBounds.Bottom - margin); // 3+4 o 7+8
				break;
		} // Hay un caso fallado en la seccion I, que no es posible acceder normalmente.

		return new Vector2(x, y);
	}

	/*private void OnGUI() {
		GUI.skin.GetStyle("label").alignment = TextAnchor.MiddleLeft;
		GUILayout.BeginArea(new Rect(0, Screen.height / 2, Screen.width / 2, Screen.height / 2));
		GUILayout.Label((hPos).ToString());
		GUILayout.Label((vPos).ToString());
		GUILayout.Label((middleBoxTopLeft.y).ToString());
		GUILayout.Label((middleBoxBottomRight.y).ToString());
		GUILayout.Label((spawner.referenceFrame.player.rigidbody2D.position.y).ToString());
		GUILayout.EndArea();
	}*/
}