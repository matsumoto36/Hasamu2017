using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// はさまれているときにつくられる
/// まとめてピースをはさむシステム
/// </summary>
public class PieceContainer : MonoBehaviour {

	public Vector2 containerSize;   //当たり判定用

	Piece[] pieceArray;				//まとめられているピースの配列
	ParticleSystem createParticle;	//作られたときに発生するパーティクル
	ParticleSystem moveParticle;	//移動中常に発生するパーティクル

	void Start() {
		//コンテナ作成パーティクル再生
		createParticle = ParticleManager.Play(ParticleType.ContainerCreate, transform.position, Quaternion.identity);
		createParticle.transform.SetParent(transform);
		var s = createParticle.shape;
		s.scale = containerSize;

		//移動用パーティクル再生
		moveParticle = ParticleManager.Play(ParticleType.ContainerMove, transform.position, Quaternion.identity);
		moveParticle.transform.SetParent(transform);
		s = moveParticle.shape;
		s.scale = containerSize;
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
	/// ピースの配列の位置とサイズを取得する
	/// </summary>
	/// <param name="pieces"></param>
	/// <returns></returns>
	public static Rect GetContainerSize(Piece[] pieces) {
		//当たり判定用に大きさを取得しておく
		//x = minX, y = minY, width = maxX, height = maxY
		Rect bff = new Rect(99, 99, 0, 0);
		foreach (Piece p in pieces) {
			if (p.position.x < bff.x) bff.x = p.position.x;
			if (p.position.y < bff.y) bff.y = p.position.y;
			if (p.position.x > bff.width) bff.width = p.position.x;
			if (p.position.y > bff.height) bff.height = p.position.y;

		}

		return new Rect(
			new Vector2((bff.x + bff.width) * 0.5f, (bff.y + bff.height) * 0.5f),
			new Vector2(bff.width - bff.x + 1, bff.height - bff.y + 1)
			);
	}

	/// <summary>
	/// コンテナーに新しく追加する
	/// </summary>
	/// <param name="pieces">追加したいピースの配列</param>
	void SetPieces(Piece[] pieces) {
		pieceArray = pieces;

		//当たり判定用に大きさを取得しておく
		//x = minX, y = minY, width = maxX, height = maxY
		Rect bff = GetContainerSize(pieces);

		//中心に移動
		transform.position = bff.position;
		//判定のサイズを格納
		containerSize = bff.size;

		//ピースの情報を変更
		foreach(Piece p in pieces) {
			p.noCollision = true;
			p.transform.SetParent(transform);
			p.GetComponent<BoxCollider2D>().enabled = false;
		}
	}

	/// <summary>
	/// 指定された座標へ動かす
	/// </summary>
	/// <param name="newPosition">移動する場所</param>
	/// <param name="isXDir">はさんでいる方向</param>
	public void Move(Vector2 newPosition, bool isXDir) {

		//角の座標を定義
		Vector2[] checkPosition = new Vector2[4];
		checkPosition[0] = new Vector2(-containerSize.x, -containerSize.y);
		checkPosition[1] = new Vector2(-containerSize.x, containerSize.y - 0.1f);
		checkPosition[2] = new Vector2(containerSize.x - 0.1f, -containerSize.y);
		checkPosition[3] = new Vector2(containerSize.x - 0.1f, containerSize.y - 0.1f);

		for (int i = 0; i < 4; i++) {
			checkPosition[i] *= 0.5f;
			checkPosition[i] += newPosition + new Vector2(0.5f, 0.5f);
			checkPosition[i].x = (int)checkPosition[i].x;
			checkPosition[i].y = (int)checkPosition[i].y;
		}


		/* x方向の制限 */
		bool isXCollision = false;

		if(isXDir) {

			for(int i = 0; i < 2; i++) {

				//チェック用の位置を保存
				Vector2 startPosition = checkPosition[i];
				Vector2 endPosition = checkPosition[i + 2];

				//同じ座標も対応するので+1する
				endPosition.x += 1;

				//判定を連続でとる
				do {
					Piece p = StageGenerator.GetPiece(startPosition);
					if(p && !p.noCollision) {
						isXCollision = true;
						break;
					}

					startPosition.x += 1;
				} while(startPosition != endPosition);

				if(isXCollision) break;
			}
		}

		Vector2 hitPos = new Vector2();
		//方向がYの場合は+1まで検査
		if (!isXDir) {
			Vector2[] cy = new Vector2[4];
			cy[0] = newPosition + new Vector2(0.1f, -0.5f - containerSize.y * 0.5f);
			cy[1] = newPosition + new Vector2(0.9f, -0.5f - containerSize.y * 0.5f);
			cy[2] = newPosition + new Vector2(0.1f, 1.5f + containerSize.y * 0.5f);
			cy[3] = newPosition + new Vector2(0.9f, 1.5f + containerSize.y * 0.5f);

			foreach (var ccy in cy) {
				if (StageGenerator.GetPiece(ccy)) {
					hitPos = StageGenerator.GetPiece(ccy).position;
					isXCollision = true;
					break;
				}
			}
		}

		if(isXCollision) {
			newPosition.y = (int)((transform.position.y + 0.25) * 2) * 0.5f;
		}


		/* y方向の制限 */
		bool isYCollision = false;

		if(!isXDir) {
			for(int i = 0; i < 2; i++) {

				//チェック用の位置を保存
				Vector2 startPosition = checkPosition[i * 2];
				Vector2 endPosition = checkPosition[i * 2 + 1];

				//同じ座標も対応するので+1する
				endPosition.y += 1;

				//判定を連続でとる
				do {
					Piece p = StageGenerator.GetPiece(startPosition);
					if(p && !p.noCollision) {
						isYCollision = true;
						break;
					}

					startPosition.y += 1;
				} while(startPosition != endPosition);

				if(isYCollision) break;
			}
		}

		hitPos = new Vector2();
		//方向がXの場合は+1まで検査
		if (isXDir) {
			Vector2[] cx = new Vector2[4];
			cx[0] = newPosition + new Vector2(-0.5f - containerSize.x * 0.5f, 0.1f);
			cx[1] = newPosition + new Vector2(-0.5f - containerSize.x * 0.5f, 0.9f);
			cx[2] = newPosition + new Vector2(1.5f + containerSize.x * 0.5f, 0.1f);
			cx[3] = newPosition + new Vector2(1.5f + containerSize.x * 0.5f, 0.9f);

			foreach (var ccx in cx) {
				if (StageGenerator.GetPiece(ccx)) {
					hitPos = StageGenerator.GetPiece(ccx).position;
					isYCollision = true;
					break;
				}
			}
		}

		if(isYCollision) {
			newPosition.x = (int)((transform.position.x + 0.25) * 2) * 0.5f;
		}


		//移動
		transform.position = newPosition;

		//座標を反映
		foreach(Piece p in pieceArray) {
			StageGenerator.SetPiecePosition(p, p.transform.position);
		}
	}

	/// <summary>
	/// コンテナを破壊してピースを開放する
	/// </summary>
	public void DestroyContainer() {

		//設定した変更を元に戻す
		foreach(Piece p in pieceArray) {
			p.noCollision = false;
			p.transform.SetParent(null);
			p.GetComponent<Collider2D>().enabled = true;
			StageGenerator.SetPiecePosition(p, p.transform.position);
			p.transform.position = p.position;
		}

		//パーティクル開放
		if (createParticle) {
			createParticle.transform.SetParent(null);
			ParticleManager.ParticleDestroy(createParticle);
		}
		if (moveParticle) {
			moveParticle.transform.SetParent(null);
			ParticleManager.ParticleDestroy(moveParticle);
		}

		Debug.Log("DestroyContainer");
		Destroy(gameObject);
	}

	/// <summary>
	/// ピースを新しいものに入れ替える
	/// </summary>
	/// <param name="target"></param>
	/// <param name="id"></param>
	public void ReplacementPiece(Piece target, int id) {

		Debug.Log("StartReplace");

		//targetが配列中のどの要素なのか検索
		for(int i = 0;i < pieceArray.Length;i++) {

			Debug.Log(pieceArray[i].position + " " + target.position);

			if(pieceArray[i].position == target.position) {

				Vector2 position = target.transform.localPosition;

				pieceArray[i] = StageGenerator.EditPieceID(target, id);
				Destroy(target.gameObject);

				//設定
				pieceArray[i].noCollision = true;
				pieceArray[i].transform.SetParent(transform);
				pieceArray[i].GetComponent<BoxCollider2D>().enabled = false;
				pieceArray[i].transform.localPosition = position;

				Debug.Log("replace");

				return;
			}
		}
	}
}
