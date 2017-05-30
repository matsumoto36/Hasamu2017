using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour {

	static Piece[,] generatedStage;

	// Use this for initialization
	void Start () {

		//後にCSVから読み込む
		int[,] map = new int[,] {
			{0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},
			{0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},
			{0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},
			{0, 0, 0, 0, 0, 0, 0, 2, 0, 1, 0},
			{0, 0, 0, 0, 0, 3, 0, 2, 2, 1, 0},
			{0, 0, 0, 0, 0, 4, 0, 0, 0, 1, 0},
			{0, 0, 0, 0, 0, 5, 0, 0, 0, 1, 0},
			{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
		};

		//マップを生成
		GenerateMap(map);
	}

	/// <summary>
	/// ピースを移動する
	/// </summary>
	/// <returns>移動できたか</returns>
	public static bool SetPiecePosition(Piece piece, Vector2 newPos) {

		Debug.Log("newPos" + newPos);
		newPos = new Vector2((int)(newPos.x + 0.5), (int)(newPos.y + 0.5));

		//範囲外ならキャンセル
		if(0 > newPos.x) return false;
		if(0 > newPos.y) return false;
		if(generatedStage.GetLength(1) <= newPos.x) return false;
		if(generatedStage.GetLength(0) <= newPos.y) return false;

		int arrX = (int)newPos.x;
		int arrY = generatedStage.GetLength(0) - (int)newPos.y - 1;

		Debug.Log("arr(" + arrX + "," + arrY);

		//存在してたらキャンセル
		if(generatedStage[arrY, arrX]) return false;

		generatedStage[arrY, arrX] = piece;
		generatedStage[(int)piece.position.y, (int)piece.position.x] = null;

		piece.transform.position = new Vector3(newPos.x, newPos.y, 0.0f);
		piece.position = new Vector2(arrX, arrY);

		

		Debug.Log("Moved" + newPos);
		return true;
	}

	/// <summary>
	/// マップを生成
	/// </summary>
	/// <param name="map">マップのデータ</param>
	void GenerateMap(int[,] map) {

		int width = map.GetLength(1);
		int height = map.GetLength(0);

		float chipSize = ResourceLoader.GetChipSize(MapChipType.MainChip);

		generatedStage = new Piece[height, width];
		for(int i = 0;i < height;i++) {
			for(int j = 0;j < width;j++) {

				GameObject g = new GameObject("[stage " + j + " " + (height - (i + 1)) + " ]");
				Piece p = null;

				int id = map[height - (i + 1), j];

				switch(id) {
					case 0:
						Destroy(g);
						break;
					case 1:
					case 2:
						p = g.AddComponent<Piece>();
						break;
					case 3:
						p = g.AddComponent<PieceBomb>();
						break;
					default:
						p = g.AddComponent<Piece>();
						break;
				}

				generatedStage[(height - (i + 1)), j] = p;

				if(!p) continue;
				
				SpriteRenderer spr = p.gameObject.AddComponent<SpriteRenderer>();
				BoxCollider2D col = p.gameObject.AddComponent<BoxCollider2D>();

				p.tag = "Piece";

				p.transform.SetParent(transform);
				p.transform.position = new Vector3(j * chipSize, i * chipSize, 0.0f);

				p.id = id;
				p.position = new Vector2(j, (height - (i + 1)));
				spr.sortingOrder = p.id;
				col.size = new Vector2(1, 1) * chipSize;
			}
		}

		Debug.Log("Map Generated.");

		Camera.main.transform.position = new Vector3((width-1) * chipSize / 2, (height-1) * chipSize / 2, -1);
	}
}
