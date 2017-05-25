using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ上に配置される一つのマス
/// </summary>
public class Piece : MonoBehaviour {

	public int id;
	public int subId;

	SpriteRenderer _renderer;

	// Use this for initialization
	void Start () {
		_renderer = GetComponent<SpriteRenderer>();

		SpriteLoad();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SpriteLoad() {
		_renderer.sprite = ResourceLoader.GetChips(MapChipType.MainChip)[id];
	}
}
