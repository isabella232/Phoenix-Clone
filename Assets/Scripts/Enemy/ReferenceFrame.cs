using System.Collections;
using UnityEngine;

public class ReferenceFrame : MonoBehaviour {
	public GameObject player;
	public bool radialMovement;

	[HideInInspector]
	public Vector2 up, right;

	// Use this for initialization
	private void Start() {
		if (!radialMovement) {
			up = Vector2.up;
			right = Vector2.right;
		}
	}

	// Update is called once per frame
	private void Update() {
		if (!radialMovement) return;

		if (player != null) {
			/* En el sistema rectangular, el vector "abajo" (0, -1) acerca la nave enemiga al jugador, y todo lo demás
			 * es en torno a eso.
			 * En el sistema radial, la idea es crear un marco de referencia que siempre pone al jugador 'abajo' respecto
			 * a la nave enemiga, redondeado el ángulo a multiplos de 45 grados. */
			//up = -player.rigidbody2D.position.SnapTo(45);
			up = ((Vector2)player.transform.up).SnapTo(45);
			/* Acabo de notar que quiero que el punto de referencia sea siempre (mas o menos) igual a la direccion en que
			 * la nave apunta (el vector arriba). Esto permite que el marco de referencia se ajuste al sistema 
			 * rectangular o radial sin ifs. (Usar la posicion del jugador rotaría el vector cuando el jugador se va hacia
			 * los lados) */
			right = Quaternion.Euler(0, 0, -90) * up;
			// Con esto rotamos el vector 'arriba' a la derecha.
		}
	}
}