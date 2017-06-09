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

			//長さを決める
			for(int i = 0;i < 2;i++) {
				int l = 0;

				Vector2 v = pos[i] - currenTentacle[i].position;
				float rad = Vector2.Angle(currenTentacle[i].angle, v) * Mathf.Deg2Rad;

				l = (int)(v.magnitude * Mathf.Cos(rad)) + 1;

				currenTentacle[i].SetLength(l < 0 ? 0 : l);
				
			}

			#region はさむアクション

			//Debug.Log("0pos " + (currenTentacle[0].position + (currenTentacle[0].angle * currenTentacle[0].length)));
			//Debug.Log("1pos " + (currenTentacle[1].position + (currenTentacle[1].angle * currenTentacle[1].length)));

			//同じポジションを差しているか判定
			if(currenTentacle[0].GetTargetPosition() == currenTentacle[1].GetTargetPosition()) {

				Piece p = StageGenerator.GetPiece(currenTentacle[0].GetTargetPosition());
				if(p && p.id != 1 && !currentPiece) {
					currentPiece = p;
					StageGenerator.RemovePiece(p);
				}

				//Debug.Log("hasami");

			}
			else {
				//currentPiece = null;
			}

			#endregion

			//移動
			currenTentacle[0].Move(pos[0]);
			currenTentacle[1].Move(pos[1]);

			//はさんでいるものがあれば移動
			if(currentPiece) {

				currentPiece.transform.position = (currenTentacle[0].transform.position + currenTentacle[1].transform.position) * 0.5f;

			}

		}
		else if(InputManager.GetInput(out pos[0])) {

		}

		if(InputManager.GetInputUpDouble()) {
			isAction = false;
		}

	}


}
