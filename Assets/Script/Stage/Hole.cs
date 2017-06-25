using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 爆弾を落とすための穴
/// </summary>
public class Hole : MonoBehaviour {

	Vector2 checkPosition;

	// Use this for initialization
	void Start () {
		checkPosition = transform.position;
	}
	
	/// <summary>
	/// クリア用の判定を作成する
	/// </summary>
	/// <param name="position">判定する位置</param>
	/// <returns>本体</returns>
	public static Hole CreateHole(Vector2 position) {
		Hole hole = new GameObject("[Hole]").AddComponent<Hole>();
		hole.transform.position = position;

		SpriteRenderer r = hole.gameObject.AddComponent<SpriteRenderer>();
		r.sortingOrder = 1;
		r.sprite = ResourceLoader.GetChips(R_MapChipType.Hole)[0];

		return hole;
	}

	// Update is called once per frame
	void Update () {

		Piece p = StageGenerator.GetPiece(checkPosition);
		if(p && p.id == 3) {
			DestroyBomb(p);
		}
	}

	/// <summary>
	/// 爆弾を落とした時の処理
	/// </summary>
	void DestroyBomb(Piece piece) {

		//コンテナを破壊
		Player.DestroyCurrentContainer();

		//ステージから削除
		StageGenerator.RemovePiece(piece);

		//演出等
		StartCoroutine(DestroyBombAnim(piece));

		//ステージクリア
		GameManager.GameClear();
	}

	IEnumerator DestroyBombAnim(Piece piece) {

		//落ちる音再生
		AudioManager.Play(SEType.Hole, 1.0f);

		float rotSpeed = 5;
		float timeSpeed = 1;
		float t = 0;
		while(t < 1.0f) {
			t += Time.deltaTime * timeSpeed;

			piece.transform.localScale = new Vector3(1, 1, 1) * (1 - t);
			piece.transform.rotation *= Quaternion.AngleAxis(rotSpeed, Vector3.forward);

			yield return null;
		}


		//カウントダウンストップ
		Timebar.StopTimer();

		Destroy(piece.gameObject);
	}
}
