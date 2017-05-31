using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour {

	static Transform myTrans;
	static Piece[,] generatedStage;

	// Use this for initialization
	void Start () {

		//後にCSVから読み込む
		int[,] map = new int[,] {
			{0, 0, 0, 0, 0, 0, 2, 2, 2, 1, 0, 1},
			{0, 1, 0, 1, 1, 1, 0, 0, 0, 1, 0, 1},
			{0, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 1},
			{0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
			{0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1},
			{1, 1, 1, 0, 3, 4, 5, 6, 0, 1, 0, 1},
			{1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1},
			{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
		};

		

		//マップを生成
		GenerateMap(map);


	}

	/// <summary>
	/// ある地点のピースを取得する
	/// </summary>
	/// <param name="pos">座標</param>
	/// <returns>ピース</returns>
	public static Piece GetPiece(Vector2 pos) {
		
		//範囲外ならキャンセル
		if(CheckStageBound(pos)) return null;
		return generatedStage[(int)pos.y, (int)pos.x];

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

		System.DateTime dt = System.DateTime.Now;

		int width = map.GetLength(1);
		int height = map.GetLength(0);

		float chipSize = ResourceLoader.GetChipSize(MapChipType.MainChip);

		generatedStage = new Piece[height, width];
		for(int i = 0;i < height;i++) {
			for(int j = 0;j < width;j++) {

				GameObject g = new GameObject("[stage " + j + " " + (height - (i + 1)) + " ]");
				Piece p = null;

				int id = map[height - (i + 1), j];

				//idで取り付けクラスを分ける
				switch(id) {
					case 0:
						Destroy(g);
						break;
					case 1:
						p = g.AddComponent<PieceTentacle>();
						break;
					case 2:
						p = g.AddComponent<Piece>();
						break;
					case 3:
						p = g.AddComponent<PieceBomb>();
						break;
					case 4:
						p = g.AddComponent<PieceBlockNormal>();
						break;
					case 5:
						p = g.AddComponent<PieceBlockFire>();
						break;
					case 6:
						p = g.AddComponent<PieceBlockCold>();
						break;
					default:
						p = g.AddComponent<Piece>();
						break;
				}

				generatedStage[(height - (i + 1)), j] = p;

				if(!p) continue;
				
				SpriteRenderer spr = g.AddComponent<SpriteRenderer>();
				BoxCollider2D col = g.AddComponent<BoxCollider2D>();

				p.tag = "Piece";

				p.transform.SetParent(myTrans);
				p.transform.position = new Vector3(j * chipSize, i * chipSize, 0.0f);

				p.id = id;
				p.position = new Vector2(j, (height - (i + 1)));
				p._renderer = spr;
				spr.sortingOrder = p.id;
				col.size = new Vector2(1, 1) * chipSize;
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

		Camera.main.transform.position = new Vector3((width - 1) * chipSize / 2, (height - 1) * chipSize / 2, -1);
	}
}
