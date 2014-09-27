using System.Collections;
using System.Linq;
using UnityEngine;

public class RandomChoice {
	private WeightedEntry[] entries;
	private int weightSum;

	public RandomChoice(WeightedEntry[] entries) {
		this.entries = entries;
		weightSum = entries.Sum(e => e.weight);
	}

	/* http://snippets.luacode.org/snippets/Weighted_random_choice_104 */

	public GameObject Choose() {
		var threshold = Random.Range(0f, weightSum);
		for (var i = 0; i < entries.Length; i++) {
			threshold -= entries[i].weight;
			if (threshold <= 0) return entries[i].element;
		}
		return entries[entries.Length - 1].element;
	}
}

[System.Serializable]
public class WeightedEntry {
	public int weight;
	public GameObject element;
}

[System.Serializable]
public class Entry {
	public int quantity;
	public GameObject element;
}

public class Spawner : MonoBehaviour {
	public WeightedEntry[] spawnables;
	public Entry[] spawnAtWakeUp;

	public bool spawn;
	public float minDelayBetweenSpawns, maxDelayBetweenSpawns;
	public Rect spawnArea; // Relative to screen size.
	public bool searchForSpecialBehavior;
	private SpawnBehavior specialBehavior;

	[HideInInspector]
	public ReferenceFrame referenceFrame;

	private RandomChoice randomChoice;

	// Use this for initialization
	private void Start() {
		if (searchForSpecialBehavior) specialBehavior = GetComponent<SpawnBehavior>();

		referenceFrame = GetComponentInParent<ReferenceFrame>();

		randomChoice = new RandomChoice(spawnables);

		if (spawnAtWakeUp.Length != 0) {
			for (var i = 0; i < spawnAtWakeUp.Length; i++) {
				for (var j = 0; j < spawnAtWakeUp[i].quantity; j++) {
					Spawn(spawnAtWakeUp[i].element);
				}
			}
		}

		StartCoroutine(WaitAndSpawn());
	}

	private IEnumerator WaitAndSpawn() {
		while (spawn) {
			yield return new WaitForSeconds(Random.Range(minDelayBetweenSpawns, maxDelayBetweenSpawns));
			Spawn(randomChoice.Choose());
		}
	}

	private void Spawn(GameObject enemy) {
		if (referenceFrame.player == null) return; // No spawns while player's dead.

		var position = specialBehavior == null ? GetPosition() : specialBehavior.GetPosition();

		var gameObject = Instantiate(enemy, position, Quaternion.identity) as GameObject;
		gameObject.transform.parent = this.transform;
	}

	public Vector2 GetPosition() {
		var x = (spawnArea.x + Random.Range(-spawnArea.width / 2, spawnArea.width / 2)) * ScreenBounds.Width;
		var y = (spawnArea.y + Random.Range(-spawnArea.height / 2, spawnArea.height / 2)) * ScreenBounds.Height;

		return new Vector2(x, y);
	}
}