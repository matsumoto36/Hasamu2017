﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バーチャルなプレイヤー
/// </summary>
public class Player : MonoBehaviour {

	public static bool isAction = false;			//触手を操作しているか
	public static Vector2[] pos = new Vector2[2];   //タッチした座標

	bool firstTouch = false;						//ゲームで初めて触手をはやしたとき

	Tentacle[] currenTentacle = new Tentacle[2];	//操作している触手	

	static PieceContainer currentPieceContainer;	//はさんでいるオブジェクト

	// Update is called once per frame
	void Update () {

		pos = new Vector2[2];

		if(InputManager.GetInputDouble(out pos)) {

			pos[0] = Camera.main.ScreenToWorldPoint(pos[0]);
			pos[1] = Camera.main.ScreenToWorldPoint(pos[1]);

			//押された瞬間は取得も兼ねる
			if(!isAction) {

				//触手生成
				if(!SpawnTentacle()) return;
				isAction = true;
	
				//初めて生成した場合、ゲーム開始
				if(!firstTouch) {
					firstTouch = true;
					GameManager.GameStart();
				}
			}

			//はさむアクション
			TentacleAction();

			//移動
			currenTentacle[0].Move(pos[0]);
			currenTentacle[1].Move(pos[1]);

			//はさんでいるものがあれば移動
			if(currentPieceContainer)
				currentPieceContainer.Move((currenTentacle[0].transform.position + currenTentacle[1].transform.position) * 0.5f);

		}

		if(InputManager.GetInputUpDouble()) {
			isAction = false;

			//はさんでいるものがあれば解除
			if(currentPieceContainer) {
				currentPieceContainer.DestroyContainer();
				currentPieceContainer = null;
			}
		}

	}

	/// <summary>
	/// はさむように生える触手のスポーン
	/// </summary>
	/// <returns>できたらtrue</returns>
	bool SpawnTentacle() {

		RaycastHit2D[] hits = new RaycastHit2D[2];
		hits[0] = Physics2D.Raycast(pos[0], Vector2.zero);
		hits[1] = Physics2D.Raycast(pos[1], Vector2.zero);

		//取得できなければキャンセル
		if(!hits[0] || !hits[1]) return false;

		Piece[] p = new Piece[2];
		p[0] = hits[0].collider.GetComponent<Piece>();
		p[1] = hits[1].collider.GetComponent<Piece>();

		//位置が縦、横のどちらかが同じでなければキャンセル
		if(p[0].position.x == p[1].position.x && p[0].position.y == p[1].position.y) return false;
		if(p[0].position.x != p[1].position.x && p[0].position.y != p[1].position.y) return false;

		//IDが同じでなければ、触手でなければキャンセル
		if(p[0].id != p[1].id || p[0].id != 1) return false;


		//向き情報を格納
		Vector2[] angles = new Vector2[2];
		angles[0] = (p[1].position - p[0].position).normalized;
		angles[1] = (p[0].position - p[1].position).normalized;

		//邪魔していたらキャンセル
		if(StageGenerator.GetPiece(angles[0] + p[0].position)) return false;
		if(StageGenerator.GetPiece(angles[1] + p[1].position)) return false;

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

		return true;
	}

	void TentacleAction() {
		//触手の間のピースを取得
		Piece[] btwp = GetPiecesBetweenTentacle();
		if(btwp != null && !currentPieceContainer) {
			Debug.Log("はさめたよ");

			//はさむ
			currentPieceContainer = PieceContainer.CreateContainer(btwp);
		}
	}

	/// <summary>
	/// 触手の間にあるピースを取得(完全)
	/// </summary>
	/// <returns>すべて取得できれば、できたピースの配列</returns>
	Piece[] GetPiecesBetweenTentacle() {

		Vector2[] targetPositions = new Vector2[2];
		targetPositions[0] = currenTentacle[0].GetTargetPosition();
		targetPositions[1] = currenTentacle[1].GetTargetPosition();

		//位置が縦、横のどちらかが同じでなければキャンセル
		if(targetPositions[0].x != targetPositions[1].x && targetPositions[0].y != targetPositions[1].y) return null;

		//大きさを求める
		int count = (int)(targetPositions[0] - targetPositions[1]).magnitude + 1;

		//取得してみる
		List<Piece> pieces = new List<Piece>();
		for(int i = 0;i < count;i++) {
			Piece p = StageGenerator.GetPiece(targetPositions[0] + currenTentacle[0].angle * i);
			//一つでも取得できなければキャンセル
			if(!p || !(p is IExecutable)) return null;

			pieces.Add(p);
		}

		return pieces.ToArray();
	}

	/// <summary>
	/// コンテナを削除
	/// </summary>
	public static void DestroyCurrentContainer() {
		if(!currentPieceContainer) return;

			currentPieceContainer.DestroyContainer();
			currentPieceContainer = null;
	}
}
