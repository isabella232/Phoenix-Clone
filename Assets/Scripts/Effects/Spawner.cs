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
		var threshold = Random.Range(0, weightSum);
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

public class Spawner : MonoBehaviour {
	[HideInInspector]
	public ReferenceFrame referenceFrame;
	public WeightedEntry[] spawnables;
	private RandomChoice pickupChooser;

	public float minDelayBetweenSpawns, maxDelayBetweenSpawns;
	public bool spawn;
	public Rect spawnArea; // Relative to screen size.
	public SpawnBehavior specialBehavior;

	// Use this for initialization
	private void Start() {
		referenceFrame = GetComponent<ReferenceFrame>();
		pickupChooser = new RandomChoice(spawnables);
		StartCoroutine(WaitAndSpawn());
	}

	private IEnumerator WaitAndSpawn() {
		while (spawn) {
			yield return new WaitForSeconds(Random.Range(minDelayBetweenSpawns, maxDelayBetweenSpawns));

			Vector2 position = Vector2.zero;
			if (specialBehavior == null) {
				var x = (spawnArea.x + Random.Range(-spawnArea.width / 2, spawnArea.width / 2)) * ScreenBounds.HorizontalDistance;
				var y = (spawnArea.y + Random.Range(-spawnArea.height / 2, spawnArea.height / 2)) * ScreenBounds.VerticalDistance;

				position = new Vector2(x, y);
			} else {
				position = specialBehavior.GetPosition(this);
			}

			var pickup = Instantiate(pickupChooser.Choose(), position, Quaternion.identity) as GameObject;
			pickup.transform.parent = this.transform;
		}
	}
}