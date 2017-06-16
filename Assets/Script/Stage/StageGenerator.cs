using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour {

	public static Vector2 stageSize;

	static Piece[,] generatedStage;
	static bool isGanerate;

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

		Vector3 stagePosition = position;
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

		RemovePiece(piece);
		Destroy(piece.gameObject);

		//作成して返却
		return CreatePiece(position, newID);
	}

	/// <summary>
	/// ステージから削除(オブジェクトは削除されない)
	/// </summary>
	/// <param name="piece">ピース</param>
	public static void RemovePiece(Piece piece) {
		generatedStage[(int)piece.position.y, (int)piece.position.x] = null;
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
		int arrY = (int)newPos.y;

		//存在してたらキャンセル
		if(generatedStage[arrY, arrX]) return false;

		generatedStage[arrY, arrX] = piece;
		generatedStage[(int)piece.position.y, (int)piece.position.x] = null;

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
		stageSize = new Vector2(width, height);

		generatedStage = new Piece[height, width];
		for(int i = 0;i < height;i++) {
			for(int j = 0;j < width;j++) {

				int id = map[height - (i + 1), j];
				Vector2 position = new Vector2(j, i);
				//idが0以上のときはピースを作成
				if(id >= 0) {
					CreatePiece(position, id);
				}
				else {
					//それ以外はクリア場所を作成
					Hole.CreateHole(position);
				}
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


		Vector2 center = new Vector2((width - 1f) * 0.5f, (height - 1f) * 0.5f);

		//背景画像を設置
		SpriteRenderer backGround = new GameObject("[BackGround]").AddComponent<SpriteRenderer>();
		backGround.sprite = ResourceLoader.GetOtherSprite(OtherSpriteType.Mask);
		backGround.color = new Color32(101, 173, 174, 255);
		backGround.sortingOrder = 0;
		backGround.transform.position = center;
		backGround.transform.localScale = center * 3f;

		Vector3 cameraPos = center;
		cameraPos.x += 0.6f;
		cameraPos.z = -1;

		Camera.main.transform.position = cameraPos;
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
