using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バーチャルなプレイヤー
/// </summary>
public class Player : MonoBehaviour {

	static Player myPlayer;
	bool[] isAction = new bool[2];			//触手を操作しているか
	Vector2?[] pos;							//タッチした座標

	public static PieceContainer currentPieceContainer;		//はさんでいるオブジェクト

	bool firstTouch = false;								//ゲームで初めて触手をはやしたとき

	Tentacle[] currenTentacle = new Tentacle[2];			//操作している触手	


	Sprite failCreateSprite;

	bool[] isFailAnimPlay = new bool[2];

	void Start() {
		myPlayer = this;

		//生成失敗時の画像読み込み
		failCreateSprite = ResourceLoader.GetChips(R_MapChipType.MainChip)[15];
	}

	// Update is called once per frame
	void Update () {

		bool[] touchUp = InputManager.GetInputUpAll();
		for(int i = 0;i < 2;i++) {
			if(touchUp[i] && currenTentacle[i]) {
				Debug.Log("TouchUp + " + i);

				currenTentacle[i].Return();
				currenTentacle[i] = null;
				isAction[i] = false;
				if(currentPieceContainer) {
					//アニメーション変更
					if(currenTentacle[0]) currenTentacle[0].SetAnimatonState(TentacleAnimState.Move);
					if(currenTentacle[1]) currenTentacle[1].SetAnimatonState(TentacleAnimState.Move);

					DestroyCurrentContainer();
				}
			}
		}

		//pos = new Vector2[2];

		if(InputManager.GetInputAll(out pos)) {
			for(int i = 0;i < 2;i++) {
				if(pos[i] == null) continue;
				pos[i] = Camera.main.ScreenToWorldPoint((Vector2)pos[i]);

				if(!isAction[i]) {

					if(!SpawnTentacle(i)) continue;
					isAction[i] = true;

					//初めて生成した場合、ゲーム開始
					if(!firstTouch) {
						firstTouch = true;
						GameManager.GameStart();
					}
				}

			}

			//二つある場合限定
			if(currenTentacle[0] && currenTentacle[1]) {
				//はさむアクション
				TentacleAction();

				//はさみ続けられるかチェック
				if(!CheckRetentionContainer()) {

					//アニメーション変更
					currenTentacle[0].SetAnimatonState(TentacleAnimState.Move);
					currenTentacle[1].SetAnimatonState(TentacleAnimState.Move);

					//コンテナを破壊
					DestroyCurrentContainer();
				}

				//触手が何か挟んでいる場合は、横移動を平均値に
				if(currentPieceContainer) {

					//0番の位置を基点とする
					Vector2 tVec = (Vector2)pos[1] - (Vector2)pos[0];
					Vector2 v;

					//外積で左右判定
					float side = currenTentacle[0].angle.x * tVec.y - currenTentacle[0].angle.y * tVec.x;
					if(side < 0) {
						v = (Quaternion.Euler(0, 0, -90) * currenTentacle[0].angle).normalized;
					}
					else {
						v = (Quaternion.Euler(0, 0, 90) * currenTentacle[0].angle).normalized;
					}

					float r = Vector2.Angle(v, tVec) * Mathf.Deg2Rad;

					//合わせる座標
					pos[0] += v * Mathf.Cos(r) * tVec.magnitude * 0.5f;
					pos[1] += -v * Mathf.Cos(r) * tVec.magnitude * 0.5f;
				}

				//移動
				currenTentacle[0].Move((Vector2)pos[0]);
				currenTentacle[1].Move((Vector2)pos[1]);

				//はさんでいるものがあれば移動
				MoveContainer();
			}
			else {
				//移動
				if(currenTentacle[0]) currenTentacle[0].Move((Vector2)pos[0]);
				if(currenTentacle[1]) currenTentacle[1].Move((Vector2)pos[1]);
			}


		}
	}

	/// <summary>
	/// はさむように生える触手のスポーン
	/// </summary>
	/// <param name="id">生える触手のID</param>
	/// <returns>できたらtrue</returns>
	bool SpawnTentacle(int id) {

		//Debug.Log("SpawnTentacle Start");

		Vector2 spawnPos = (Vector2)pos[id];

		//Debug.Log("SpawnTentacle SpawnPos " + spawnPos);

		RaycastHit2D hit;
		hit = Physics2D.Raycast(spawnPos, Vector2.zero);
		//取得できなければキャンセル
		if(!hit) return false;

		//Debug.Log("SpawnTentacle Get");


		Piece p = hit.collider.GetComponent<Piece>();
		//IDが同じでなければ、触手でなければキャンセル
		if(!p || p.id != 1) return false;

		//向き情報を格納
		Vector2 angle;
		int revId = id == 0 ? 1 : 0;
		if(currenTentacle[revId]) {

			//Debug.Log("SpawnTentacle Check");


			//長いほうから予測
			Vector2 checkAngle = p.position - currenTentacle[revId].position;
			if(Mathf.Abs(checkAngle.x) > Mathf.Abs(checkAngle.y)) {
				if(checkAngle.x < 0) checkAngle = new Vector2(-1, 0);
				else				 checkAngle = new Vector2(1, 0);
			}
			else {
				if(checkAngle.y < 0) checkAngle = new Vector2(0, -1);
				else				 checkAngle = new Vector2(0, 1);
			}

			Debug.Log("SpawnTentacle Check Angle:" + checkAngle);
			Debug.Log("SpawnTentacle Check Angle:" + currenTentacle[revId].angle);
			//触手は生えているが、向きが正しくない場合
			if(checkAngle != currenTentacle[revId].angle) {
				Debug.Log("SpawnTentacle Retry Angle:" + checkAngle);

				SpawnTentacle(revId, currenTentacle[revId].position, checkAngle);
			}

			angle = -currenTentacle[id == 0 ? 1 : 0].angle;
		}
		else {
			//順番に検証
			//優先度
			//1.空いているところ
			//2.触手ブロック以外のところ
			//3.触手ブロックのところ

			Vector2? checkResult = null;

			//取得用位置を格納
			Vector2[] checkPos = new Vector2[] {
				new Vector2(p.position.x, p.position.y + 1),
				new Vector2(p.position.x - 1, p.position.y),
				new Vector2(p.position.x, p.position.y - 1),
				new Vector2(p.position.x + 1, p.position.y),
			};
			Piece[] checkPiece = new Piece[4];
			for(int i = 0;i < 4;i++) {
				checkPiece[i] = StageGenerator.GetPiece(checkPos[i]);
			}

			//1
			for(int j = 0;j < 4;j++) {
				if(!checkPiece[j] && !StageGenerator.CheckStageBound(checkPos[j])) {
					checkResult = checkPos[j];
					break;
				}
			}
			if(checkResult == null) {
				//2
				for(int j = 0;j < 4;j++) {
					if(checkPiece[j] && checkPiece[j].id != 1) {
						checkResult = checkPiece[j].position;
						break;
					}
				}

				if(checkResult == null) checkResult = checkPiece[0].position;
			}

			angle = ((Vector2)checkResult - p.position).normalized;
		}

		//Debug.Log("SpawnTentacle Angle " + angle);

		return SpawnTentacle(id, p.position, angle);
	}

	bool SpawnTentacle(int id, Vector2 spawnPos, Vector2 angle) {

		//邪魔していたらキャンセルアニメーションを再生
		Piece anglePiece = StageGenerator.GetPiece(angle + spawnPos);
		if(anglePiece) {
			if(!isFailAnimPlay[id]) StartCoroutine(FailCreateAnimation(id, anglePiece.position));
			return false;
		}

		//触手の生成開始
		if(currenTentacle[id]) currenTentacle[id].Return();
		currenTentacle[id] = Tentacle.CreateTentacle(spawnPos);
		currenTentacle[id].angle = angle;
		currenTentacle[id].transform.position = spawnPos;

		//デバッグ用でSEを鳴らす
		//AudioManager.Play(SEType.Tap, 1);

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

			//アニメーション変更
			int id;
			id = StageGenerator.GetPiece(currenTentacle[0].GetTargetPosition()).id;
			currenTentacle[0].SetAnimatonState(Tentacle.GetHoldState(id));
			if (id == 5) AudioManager.Play(SEType.Hot, 0.5f);
			if (id == 6) AudioManager.Play(SEType.Cold, 0.5f);

			id = StageGenerator.GetPiece(currenTentacle[1].GetTargetPosition()).id;
			currenTentacle[1].SetAnimatonState(Tentacle.GetHoldState(id));
			if (id == 5) AudioManager.Play(SEType.Hot, 0.5f);
			if (id == 6) AudioManager.Play(SEType.Cold, 0.5f);

			//音再生
			AudioManager.Play(SEType.HasamuNormal);
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

		Vector2 maxLength = new Vector2(0.4f, 0.2f);
		bool ans = true;

		for(int i = 0;i < 2;i++) {

			Vector2 bound = new Vector2(currenTentacle[i].angle.x * currentPieceContainer.containerSize.x,
										currenTentacle[i].angle.y * currentPieceContainer.containerSize.y) * 0.5f;

			Vector2 checkVec = ((Vector2)currenTentacle[i].transform.position + (currenTentacle[i].angle * 0.5f))
							 - ((Vector2)currentPieceContainer.transform.position - bound);

			//Debug.DrawLine((Vector2)currentPieceContainer.transform.position, (Vector2)currenTentacle[i].transform.position, Color.black);

			//Debug.DrawLine(((Vector2)currenTentacle[i].transform.position + (currenTentacle[i].angle * 0.5f)),
			//	 ((Vector2)currentPieceContainer.transform.position - bound));

			float r = Vector2.Angle(checkVec, currenTentacle[i].angle);
			//if(r < 60) continue;

			r *= Mathf.Deg2Rad;

			Vector2 v = new Vector2(Mathf.Abs(Mathf.Sin(r)), Mathf.Cos(r) * -1) * checkVec.magnitude;
			//Debug.Log("v:" + v);


			//一定距離あれば解除
			if(v.x > maxLength.x || v.y > maxLength.y) ans = false;
			//if(containerDistance[i] > 0.1f) ans = false;
		}

		return ans;
	}

	/// <summary>
	/// はさんでいるピースの移動
	/// </summary>
	void MoveContainer() {
		if(!currentPieceContainer) return;

		//中点に移動
		Vector2 movePos = (currenTentacle[0].transform.position + currenTentacle[1].transform.position) * 0.5f;
		bool isXDir = currenTentacle[0].angle.x != 0;
		currentPieceContainer.Move(movePos, isXDir);

	}

	/// <summary>
	/// コンテナを削除
	/// </summary>
	public static void DestroyCurrentContainer() {
		if(!currentPieceContainer) return;

		currentPieceContainer.DestroyContainer();
		currentPieceContainer = null;

		for(int i = 0; i < 2; i++) {
			Tentacle t = myPlayer.currenTentacle[i];
			if(t && t.state != TentacleAnimState.Return) {
				myPlayer.currenTentacle[i].SetAnimatonState(TentacleAnimState.Move);
			}
		}
	}

	/// <summary>
	/// 生成が失敗したときに再生されるアニメーション
	/// </summary>
	/// <param name="animNum">占有する番号</param>
	/// <param name="position">再生する場所</param>
	/// <returns></returns>
	IEnumerator FailCreateAnimation(int animNum, Vector2 position) {

		Debug.Log("FailAnim");

		float playTime = 1.0f;  //再生し続ける時間
		float freq = 4;         //秒間に何回点滅するか
		float maxAlpha = 0.5f;	//アルファの最大値

		//再生中
		isFailAnimPlay[animNum] = true;

		SpriteRenderer spr = new GameObject("[FailObject]").AddComponent<SpriteRenderer>();
		spr.material = ResourceLoader.GetMaterial(R_MaterialType.AdditiveSprite);
		spr.sprite = failCreateSprite;
		spr.sortingOrder = 10;
		spr.transform.position = position;
		Color c = spr.color;

		float t = 0;
		while(t < playTime) {
			t += Time.deltaTime;

			//色を計算
			c.a = 0.5f * maxAlpha + Mathf.Sin(t * 2 * Mathf.PI * freq) * 0.5f * maxAlpha;
			//反映
			spr.color = c;

			yield return null;
		}

		//再生が終わったら削除
		Destroy(spr.gameObject);

		//再生終了
		isFailAnimPlay[animNum] = false;
	}
}
