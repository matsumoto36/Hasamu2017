using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// はさまれているときにつくられる
/// まとめてピースをはさむシステム
/// </summary>
public class PieceContainer : MonoBehaviour {

	Piece[] pieceArray;				//まとめられているピースの配列

	Rigidbody2D rig;				//移動阻止用
	public Vector2 containerSize;	//当たり判定用

	void Update() {
		
	}

	/// <summary>
	/// 新しくコンテナーを作成
	/// </summary>
	/// <param name="pieces">コンテナに入れるピース</param>
	public static PieceContainer CreateContainer(Piece[] pieces) {
		PieceContainer pc = new GameObject("[PieceContainer]").AddComponent<PieceContainer>();
		pc.SetPieces(pieces);
		return pc;
	}

	/// <summary>
	/// コンテナーに新しく追加する
	/// </summary>
	/// <param name="pieces">追加したいピースの配列</param>
	void SetPieces(Piece[] pieces) {
		pieceArray = pieces;

		//当たり判定用に大きさを取得しておく
		//x = minX, y = minY, width = maxX, height = maxY
		Rect bff = new Rect(99, 99, 0, 0);
		foreach(Piece p in pieces) {
			if(p.position.x < bff.x) bff.x = p.position.x;
			if(p.position.y < bff.y) bff.y = p.position.y;
			if(p.position.x > bff.width) bff.width = p.position.x;
			if(p.position.y > bff.height) bff.height = p.position.y;

		}

		//中心に移動
		transform.position = new Vector2((bff.x + bff.width) * 0.5f, (bff.y + bff.height) * 0.5f);
		//判定のサイズを格納
		containerSize = new Vector2(bff.width - bff.x + 1, bff.height - bff.y + 1);

		//ピースの情報を変更
		foreach(Piece p in pieces) {
			p.isMoved = true;
			p.transform.SetParent(transform);
			p.GetComponent<BoxCollider2D>().enabled = false;
		}

		//各種判定を取り付ける
		gameObject.AddComponent<BoxCollider2D>().size = containerSize - new Vector2(0.1f, 0.1f);
		rig = gameObject.AddComponent<Rigidbody2D>();
		rig.gravityScale = 0;
		rig.freezeRotation = true;

		rig.sharedMaterial = new PhysicsMaterial2D();
		rig.sharedMaterial.friction = 1;
		rig.sharedMaterial.bounciness = 0;
	}

	/// <summary>
	/// 指定された座標へ動かす
	/// </summary>
	/// <param name="newPosition">移動する場所</param>
	public void Move(Vector2 newPosition) {

		//移動
		rig.MovePosition(newPosition);

		//座標を反映
		foreach(Piece p in pieceArray) {
			StageGenerator.SetPiecePosition(p, p.transform.position);
		}
	}
}
