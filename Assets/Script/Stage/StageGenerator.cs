﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour {

	static Transform myTrans;
	static Piece[,] generatedStage;
	static bool isGanerate;

	// Use this for initialization
	void Start () {

		//後にCSVから読み込む
		int[,] map = new int[,] {
			{0, 0, 0, 0, 0, 0, 2, 2, 2},
			{0, 0, 1, 1, 1, 1, 1, 1, 0},
			{0, 0, 0, 1, 0, 0, 0, 0, 0},
			{1, 0, 0, 0, 3, 4, 5, 6, 0},
			{1, 0, 0, 0, 0, 0, 0, 0, 0},
			{1, 1, 1, 1, 1, 1, 1, 1, 1},
		};

		myTrans = gameObject.transform;

		//マップを生成
		GenerateMap(map);


	}

	/// <summary>
	/// ある地点のピースを取得する
	/// </summary>
	/// <param name="position">座標</param>
	/// <returns>ピース</returns>
	public static Piece GetPiece(Vector2 position) {
		
		//範囲外ならキャンセル
		if(CheckStageBound(position)) return null;
		return generatedStage[(int)position.y, (int)position.x];

	}

	/// <summary>
	/// 新しくステージにピースを作成
	/// </summary>
	/// <param name="position">作る配列上の位置</param>
	/// <returns>作成されたピース</returns>
	public static Piece CreatePiece(Vector2 position, int id) {

		//あったらキャンセル
		if(generatedStage[(int)position.y, (int)position.x]) return null;

		GameObject g = new GameObject("[stage " + position.x + " " + position.y + " ]");

		//指定のクラスを取り付ける
		Piece p = AttachPiece(g, id);
		//空気なら終了
		if(!p) return null;

		generatedStage[(int)position.y, (int)position.x] = p;

		SpriteRenderer spr = g.AddComponent<SpriteRenderer>();
		BoxCollider2D col = g.AddComponent<BoxCollider2D>();

		p.tag = "Piece";

		p.transform.SetParent(myTrans);

		Vector3 stagePosition = new Vector3(position.x, generatedStage.GetLength(0) - position.y - 1, 0.0f);
		p.transform.position = stagePosition;

		p.id = id;
		p.position = position;
		p._renderer = spr;
		spr.sortingOrder = GetOrderInLayer(p.id);
		col.size = new Vector2(1, 1);

		//画像をロード
		if(isGanerate) p.SpriteLoad();

		return p;
	}

	/// <summary>
	/// 指定のピースのIDを変更(新規作成)
	/// !アタッチするクラスも変更されます
	/// </summary>
	/// <param name="piece">元のピース</param>
	/// <param name="newID">新しいID</param>
	/// <returns>作成されたピース</returns>
	public static Piece EditPieceID(Piece piece, int newID) {

		//空だったらキャンセル
		if(!piece) return null;
		//場所を保存
		Vector2 position = piece.position;

		Destroy(piece.gameObject);
		//作成して返却
		return CreatePiece(position, newID);
	}

	/// <summary>
	/// 配列の要素数を超えるかチェック
	/// </summary>
	/// <returns>超える = true</returns>
	public static bool CheckStageBound(Vector2 pos) {

		if(0 > pos.x) return true;
		if(0 > pos.y) return true;
		if(generatedStage.GetLength(1) <= pos.x) return true;
		if(generatedStage.GetLength(0) <= pos.y) return true;

		return false;
	}

	/// <summary>
	/// ピースを移動する
	/// </summary>
	/// <returns>移動できたか</returns>
	public static bool SetPiecePosition(Piece piece, Vector2 newPos) {

		newPos = new Vector2((int)(newPos.x + 0.5), (int)(newPos.y + 0.5));

		//範囲外ならキャンセル
		if(CheckStageBound(newPos)) return false;

		int arrX = (int)newPos.x;
		int arrY = generatedStage.GetLength(0) - (int)newPos.y - 1;

		//存在してたらキャンセル
		if(generatedStage[arrY, arrX]) return false;

		generatedStage[arrY, arrX] = piece;
		generatedStage[(int)piece.position.y, (int)piece.position.x] = null;

		piece.transform.position = new Vector3(newPos.x, newPos.y, 0.0f);
		piece.position = new Vector2(arrX, arrY);

		//Debug.Log("Moved" + newPos);
		return true;
	}

	/// <summary>
	/// マップを生成
	/// </summary>
	/// <param name="map">マップのデータ</param>
	public static void GenerateMap(int[,] map) {

		isGanerate = false;
		System.DateTime dt = System.DateTime.Now;

		int width = map.GetLength(1);
		int height = map.GetLength(0);

		generatedStage = new Piece[height, width];
		for(int i = 0;i < height;i++) {
			for(int j = 0;j < width;j++) {

				int id = map[height - (i + 1), j];
				Vector2 position = new Vector2(j, (height - (i + 1)));
				//ピースを作成
				CreatePiece(position, id);
			}
		}

		//画像ロード用
		for(int i = 0;i < height;i++) {
			for(int j = 0;j < width;j++) {

				Piece p = generatedStage[(height - (i + 1)), j];
				if(p) p.SpriteLoad();
			}
		}

		System.TimeSpan ts = System.DateTime.Now - dt;
		Debug.Log("Map Generated. time: " + ts.Milliseconds + "ms");
		isGanerate = true;


		Camera.main.transform.position = new Vector3((width - 1f) / 2, (height - 1f) / 2, -1);
	}

	/// <summary>
	/// 指定のPieceクラスをアタッチ
	/// </summary>
	/// <param name="stageObj">アタッチするオブジェクト</param>
	/// <param name="id">id</param>
	/// <returns></returns>
	static Piece AttachPiece(GameObject stageObj, int id) {

		Piece piece = null;

		//idで取り付けクラスを分ける
		switch(id) {
			case 0:
				Destroy(stageObj);
				break;
			case 1:
				piece = stageObj.AddComponent<PieceTentacle>();
				break;
			case 2:
				piece = stageObj.AddComponent<Piece>();
				break;
			case 3:
				piece = stageObj.AddComponent<PieceBomb>();
				break;
			case 4:
				piece = stageObj.AddComponent<PieceBlockNormal>();
				break;
			case 5:
				piece = stageObj.AddComponent<PieceBlockFire>();
				break;
			case 6:
				piece = stageObj.AddComponent<PieceBlockCold>();
				break;
			default:
				piece = stageObj.AddComponent<Piece>();
				break;
		}

		return piece;
	}


	static int GetOrderInLayer(int id) {

		switch(id) {
			case 0:
				return id;
			case 1:
				return 3;
			case 2:
				return 1;
			case 3:
			case 4:
			case 5:
			case 6:
				return 4;
			default:
				return -1;
		}
	}
}
