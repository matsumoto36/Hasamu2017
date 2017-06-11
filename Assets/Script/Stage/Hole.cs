﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 爆弾を落とすための穴
/// </summary>
public class Hole : MonoBehaviour {

	Vector2 checkPosition;

	// Use this for initialization
	void Start () {
		checkPosition = transform.position;
	}
	
	/// <summary>
	/// クリア用の判定を作成する
	/// </summary>
	/// <param name="position">判定する位置</param>
	/// <returns>本体</returns>
	public static Hole CreateHole(Vector2 position) {
		Hole hole = new GameObject("[Hole]").AddComponent<Hole>();
		hole.transform.position = position;

		SpriteRenderer r = hole.gameObject.AddComponent<SpriteRenderer>();
		r.sortingOrder = 1;
		r.sprite = ResourceLoader.GetChips(MapChipType.Hole)[0];

		return hole;
	}

	// Update is called once per frame
	void Update () {

		Piece p = StageGenerator.GetPiece(checkPosition);
		if(p && p.id == 3) {
			GameManager.GameClear();
		}
	}
}
