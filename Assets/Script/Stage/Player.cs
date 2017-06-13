using System.Collections;
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

	static PieceContainer currentPieceContainer;    //はさんでいるオブジェクト

	public float[] containerDistance = new float[2];


	public bool isSafe;
	public Vector2[] v;

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

			//はさみ続けられるかチェック
			if(!CheckRetentionContainer()) DestroyCurrentContainer();

			//はさんでいるものがあれば移動
			MoveContainer();
		}

		if(InputManager.GetInputUpDouble()) {
			isAction = false;

			//触手があれば削除
			for(int i = 0;i < 2;i++) {
				if(currenTentacle[i]) currenTentacle[i].Death();
			}

			//はさんでいるものがあれば解除
			DestroyCurrentContainer();
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

			//触手を生成
			currenTentacle[i] = Tentacle.CreateTentacle(p[i].position);
			currenTentacle[i].angle = angles[i];
			currenTentacle[i].transform.position = pos[i];
		}

		return true;
	}

	/// <summary>
	/// 触手のはさむアクション
	/// </summary>
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
	/// はさみ続けられるかチェック
	/// </summary>
	/// <returns>はさめる = true</returns>
	bool CheckRetentionContainer() {

		if(!currentPieceContainer) return false;

		bool ans = true;

		for(int i = 0;i < 2;i++) {

			Vector2 bound = new Vector2(currenTentacle[i].angle.x * currentPieceContainer.containerSize.x,
										currenTentacle[i].angle.y * currentPieceContainer.containerSize.y) * 0.5f;

			Vector2 checkVec = (Vector2)currentPieceContainer.transform.position - (Vector2)currenTentacle[i].transform.position;

			//Debug.DrawLine((Vector2)currentPieceContainer.transform.position, (Vector2)currenTentacle[i].transform.position, Color.black);


			containerDistance[i] = checkVec.magnitude - bound.magnitude - 0.5f;

			//一定距離あれば解除
			if(containerDistance[i] > 0.3f) ans = false;
		}

		return ans;
	}

	/// <summary>
	/// はさんでいるピースの移動
	/// </summary>
	void MoveContainer() {
		if(!currentPieceContainer) return;

		Vector2 movePos = (currenTentacle[0].transform.position + currenTentacle[1].transform.position) * 0.5f;

		v = new Vector2[2];
		v[0] = new Vector2(currenTentacle[0].angle.x * currentPieceContainer.containerSize.x,
									 currenTentacle[0].angle.y * currentPieceContainer.containerSize.y);

		v[1] = new Vector2(currenTentacle[1].angle.x * currentPieceContainer.containerSize.x,
									 currenTentacle[1].angle.y * currentPieceContainer.containerSize.y);

		v[0].x += v[0].x != 0 ? 0.5f : 0;
		v[0].y += v[0].y != 0 ? 0.5f : 0;
		v[0] += movePos;

		v[1].x += v[1].x != 0 ? 0.5f : 0;
		v[1].y += v[1].y != 0 ? 0.5f : 0;
		v[1] += movePos;

		Debug.DrawLine(movePos, v[0], Color.black);
		Debug.DrawLine(movePos, v[1], Color.black);

		Piece[] p = new Piece[2];
		p[0] = StageGenerator.GetPiece(v[0]);
		p[1] = StageGenerator.GetPiece(v[1]);

		if(p[0] && p[0].noCollision) p[0] = null;
		if(p[1] && p[1].noCollision) p[1] = null;

		isSafe = !(p[0] || p[1]);

		currentPieceContainer.Move(movePos, isSafe);

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
