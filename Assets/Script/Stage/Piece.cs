using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ上に配置される一つのマス
/// </summary>
public class Piece : MonoBehaviour {

	public int id;				//自分のID
	public int subId;			//アニメーション用のサブID

	public Vector2 position;	//ステージ上の位置

	SpriteRenderer _renderer;	//自分の画像

	// Use this for initialization
	public void Start () {
		_renderer = GetComponent<SpriteRenderer>();
		SpriteLoad();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// 画像を持ってくる
	/// </summary>
	void SpriteLoad() {
		_renderer.sprite = ResourceLoader.GetChips(MapChipType.MainChip)[id];
	}
}
