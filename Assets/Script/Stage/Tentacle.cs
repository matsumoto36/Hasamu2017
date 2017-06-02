using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 触手本体
/// </summary>
public class Tentacle : MonoBehaviour {

	public Vector2 angle;
	public Vector2 initialPosition;

	int length = 0;
	const int CHIPOFFSET = 5;

	Sprite[] tentacleSpr = new Sprite[2];

	List<SpriteRenderer> tentacleBody;

	// Use this for initialization
	void Start () {
		tentacleBody = new List<SpriteRenderer>();
		initialPosition = transform.position;

		Sprite[] bff = ResourceLoader.GetChips(MapChipType.Sub1_Tentacle);
		tentacleSpr[0] = bff[CHIPOFFSET];
		tentacleSpr[1] = bff[CHIPOFFSET + 1];
		SetLength(5);
	}

	public static Tentacle CreateTentacle(Vector2 position) {
		Tentacle t = new GameObject("[Tentacle]").AddComponent<Tentacle>();
		return t;
	}

	// Update is called once per frame
	void Update () {
		
	}

	void Move() {

	}

	/// <summary>
	/// 触手の長さを変更する
	/// </summary>
	/// <param name="count"></param>
	void SetLength(int count) {

		if(length == count) return;

		tentacleBody = new List<SpriteRenderer>();
		for(int i = 0;i < count;i++) {
			int c = 0;
			if(i != 0) c++;

			SpriteRenderer spr = new GameObject("[Child " + i + "]").AddComponent<SpriteRenderer>();
			spr.transform.SetParent(transform);
			spr.sprite = tentacleSpr[c];
			spr.sortingOrder = 2;
			transform.position = angle * -1 * (i + 1);

			tentacleBody.Add(spr);
			
		}

		length = count;
	}
}
