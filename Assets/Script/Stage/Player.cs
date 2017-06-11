using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バーチャルなプレイヤー
/// </summary>
public class Player : MonoBehaviour {

	public static bool isAction = false;
	public static Vector2[] pos = new Vector2[2];

	Tentacle[] currenTentacle = new Tentacle[2];

	Piece currentPiece;
	Rigidbody2D cpRig;

	// Update is called once per frame
	void Update () {

		pos = new Vector2[2];

		if(InputManager.GetInputDouble(out pos)) {

			pos[0] = Camera.main.ScreenToWorldPoint(pos[0]);
			pos[1] = Camera.main.ScreenToWorldPoint(pos[1]);

			//押された瞬間は取得も兼ねる
			if(!isAction) {

				#region 触手生成

				RaycastHit2D[] hits = new RaycastHit2D[2];
				hits[0] = Physics2D.Raycast(pos[0], Vector2.zero);
				hits[1] = Physics2D.Raycast(pos[1], Vector2.zero);

				//取得できなければキャンセル
				if(!hits[0] || !hits[1]) return;

				Piece[] p = new Piece[2];
				p[0] = hits[0].collider.GetComponent<Piece>();
				p[1] = hits[1].collider.GetComponent<Piece>();

				//位置が縦、横のどちらかが同じでなければキャンセル
				if(p[0].position.x == p[1].position.x && p[0].position.y == p[1].position.y) return;
				if(p[0].position.x != p[1].position.x && p[0].position.y != p[1].position.y) return;

				//IDが同じでなければ、触手でなければキャンセル
				if(p[0].id != p[1].id || p[0].id != 1) return;
				

				//向き情報を格納
				Vector2[] angles = new Vector2[2];
				angles[0] = (p[1].position - p[0].position).normalized;
				angles[1] = (p[0].position - p[1].position).normalized;

				//邪魔していたらキャンセル
				if(StageGenerator.GetPiece(angles[0] + p[0].position)) return;
				if(StageGenerator.GetPiece(angles[1] + p[1].position)) return;

				//デバッグ用でSEを鳴らす
				AudioManager.Play(SEType.Tap, 1);

				//触手の生成開始
				for(int i = 0;i < 2;i++) {
					if(currenTentacle[i]) currenTentacle[i].Death();

					//触手を生成
					currenTentacle[i] = Tentacle.CreateTentacle(p[i].position);
					currenTentacle[i].angle = angles[i];
					currenTentacle[i].transform.position = pos[i];
				}

				#endregion

				isAction = true;
			}

			#region はさむアクション

			//同じポジションを差しているか判定
			if(currenTentacle[0].GetTargetPosition() == currenTentacle[1].GetTargetPosition()) {

				Piece p = StageGenerator.GetPiece(currenTentacle[0].GetTargetPosition());
				if(p && p.id != 1 && !currentPiece) {
					//はさむ
					SetCurrentPiece(p);
				}

				//Debug.Log("hasami");

			}
			else {
				//RemoveCurrentPiece();
			}

			#endregion

			//移動
			currenTentacle[0].Move(pos[0]);
			currenTentacle[1].Move(pos[1]);

			//はさんでいるものがあれば移動
			MoveCurrentPiece();

		}
		else if(InputManager.GetInput(out pos[0])) {

		}

		if(InputManager.GetInputUpDouble()) {
			isAction = false;
		}

	}

	/// <summary>
	/// ピースをはさむ
	/// </summary>
	/// <param name="piece"></param>
	void SetCurrentPiece(Piece piece) {

		if(currentPiece) RemoveCurrentPiece();

		piece.isMoved = true;
		currentPiece = piece;

	}

	/// <summary>
	/// はさんでいるピースを動かす
	/// </summary>
	void MoveCurrentPiece() {

		if(!currentPiece) return;

		Vector2 movePos = (currenTentacle[0].transform.position + currenTentacle[1].transform.position) * 0.5f;
		Vector2 moveVec = movePos - currentPiece.position;
		//x方向のチェック
		if(moveVec.x != 0 && StageGenerator.GetPiece(currentPiece.position + new Vector2(moveVec.x / Mathf.Abs(moveVec.x), 0))) {
			movePos.x = currentPiece.position.x;
		}
		//y方向のチェック
		else if(moveVec.y != 0 && StageGenerator.GetPiece(currentPiece.position + new Vector2(0, moveVec.y / Mathf.Abs(moveVec.y)))) {
			movePos.y = currentPiece.position.y;
		}

		//移動
		currentPiece.transform.position = movePos;

		//ステージ座標を移動させる
		StageGenerator.SetPiecePosition(currentPiece, movePos);
	}

	/// <summary>
	/// はさんでいるヒースを離す
	/// </summary>
	void RemoveCurrentPiece() {

		if(!currentPiece) return;

		Destroy(cpRig);
		currentPiece.transform.position = currentPiece.position;
		currentPiece = null;
	}


}
