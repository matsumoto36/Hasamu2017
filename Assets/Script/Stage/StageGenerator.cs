using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour {

	public Piece[,] generatedStage;

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

				Piece p = new GameObject("[stage " + (height - (i + 1)) + " " + j + " ]").AddComponent<Piece>();
				SpriteRenderer spr = p.gameObject.AddComponent<SpriteRenderer>();
				BoxCollider2D col = p.gameObject.AddComponent<BoxCollider2D>();

				p.tag = "Piece";

				p.transform.SetParent(transform);
				p.transform.position = new Vector3(j * chipSize, i * chipSize, 0.0f);
				
				p.id = map[height - (i + 1), j];
				spr.sortingOrder = p.id;
				col.size = new Vector2(1, 1) * chipSize;

				generatedStage[height - (i + 1), j] = p;
			}
		}

		Debug.Log("Map Generated.");

		Camera.main.transform.position = new Vector3((width-1) * chipSize / 2, (height-1) * chipSize / 2, -1);
	}
}
