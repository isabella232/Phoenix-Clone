using UnityEngine;

public interface SpawnBehavior {

	Vector2 GetPosition(Spawner spawner);
}

public class OffscreenSpawnBehavior : SpawnBehavior {

	public Vector2 GetPosition(Spawner spawner) {
		var refFrame = spawner.referenceFrame;
		var playerOnUpperHalf = refFrame.player.rigidbody2D.position.y > 0;
		var playerOnLeftHalf = refFrame.player.rigidbody2D.position.x < 0;

		float x, y;
		if (playerOnLeftHalf) {
			if (playerOnUpperHalf) {

			}
		}

		return Vector2.zero;
	}
}