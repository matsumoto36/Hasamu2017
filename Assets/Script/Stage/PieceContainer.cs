using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// はさまれているときにつくられる
/// まとめてピースをはさむシステム
/// </summary>
public class PieceContainer : MonoBehaviour {

	Piece[] pieceArray;				//まとめられているピースの配列

	public Vector2 containerSize;   //当たり判定用

	ParticleSystem createParticle;
	ParticleSystem moveParticle;

	//デバッグ用
	List<Vector2> cy;
	List<Vector2> cx;

	private void Start() {
		//コンテナ作成パーティクル再生
		createParticle = ParticleManager.Play(ParticleType.ContainerCreate, transform.position, Quaternion.identity);
		createParticle.transform.SetParent(transform);
		var s = createParticle.shape;
		s.box = containerSize;

		//移動用パーティクル再生
		moveParticle = ParticleManager.Play(ParticleType.ContainerMove, transform.position, Quaternion.identity);
		moveParticle.transform.SetParent(transform);
		s = moveParticle.shape;
		s.box = containerSize;
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
		//Vector2[] checkPosition = new Vector2[4];
		//checkPosition[0] = new Vector2(-containerSize.x, -containerSize.y);
		//checkPosition[1] = new Vector2(-containerSize.x, containerSize.y - 0.1f);
		//checkPosition[2] = new Vector2(containerSize.x - 0.1f, -containerSize.y);
		//checkPosition[3] = new Vector2(containerSize.x - 0.1f, containerSize.y - 0.1f);

		//for(int i = 0; i < 4; i++) {
		//	checkPosition[i] *= 0.5f;
		//	checkPosition[i] += newPosition + new Vector2(0.5f, 0.5f);
		//	checkPosition[i].x = (int)checkPosition[i].x;
		//	checkPosition[i].y = (int)checkPosition[i].y;
		//}


		///* x方向の制限 */
		////bool isXCollision = false;
		//for(int i = 0; i < 2; i++) {

		//	cx = new List<Vector2>();

		//	//チェック用の位置を保存
		//	Vector2 startPosition = checkPosition[i];
		//	Vector2 endPosition = checkPosition[i + 2];

		//	//同じ座標も対応するので+1する
		//	endPosition.x += 1;

		//	do {
		//		cx.Add(startPosition);
		//		Piece p = StageGenerator.GetPiece(startPosition);
		//		if(p && !p.noCollision) {
		//			//isXCollision = true;
		//			break;
		//		}

		//		startPosition.x += 1;
		//	} while(startPosition != endPosition);

		//	//if(isXCollision) break;
		//}




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

	void OnDrawGizmos() {

		Vector2[] checkPosition = new Vector2[4];
		Gizmos.color = Color.red;

		checkPosition[0] = new Vector2(-containerSize.x, -containerSize.y);
		checkPosition[1] = new Vector2(-containerSize.x, containerSize.y - 0.1f);
		checkPosition[2] = new Vector2(containerSize.x - 0.1f, -containerSize.y);
		checkPosition[3] = new Vector2(containerSize.x - 0.1f, containerSize.y - 0.1f);

		for(int i = 0;i < 4;i++) {

			checkPosition[i] *= 0.5f;
			checkPosition[i] += (Vector2)transform.position + new Vector2(0.5f, 0.5f);
			checkPosition[i].x = (int)checkPosition[i].x;
			checkPosition[i].y = (int)checkPosition[i].y;

			//Gizmos.DrawWireCube(checkPosition[i], Vector3.one);
		}

		Gizmos.color = Color.blue;

		//if(isX) {
		if(cx != null) {
			for(int i = 0; i < cx.Count; i++) {
				Vector2 ccx = new Vector2((int)cx[i].x, (int)cx[i].y);
				Gizmos.DrawWireCube(ccx, Vector3.one);
			}
		}

		//} else {
		//	if(cy != null) {
		//		for(int i = 0; i < 4; i++) {
		//			cy[i].x = (int)cy[i].x;
		//			cy[i].y = (int)cy[i].y;
		//			Gizmos.DrawWireCube(cy[i], Vector3.one);
		//		}
		//	}
		//}
	}
}
