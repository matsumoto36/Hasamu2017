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

	// Update is called once per frame
	void Update () {

		pos = new Vector2[2];

		if(InputManager.GetInputDouble(out pos)) {

			pos[0] = Camera.main.ScreenToWorldPoint(pos[0]);
			pos[1] = Camera.main.ScreenToWorldPoint(pos[1]);

			//押された瞬間は取得も兼ねる
			if(!isAction) {

				RaycastHit2D[] hits = new RaycastHit2D[2];
				hits[0] = Physics2D.Raycast(pos[0], Vector2.zero);
				hits[1] = Physics2D.Raycast(pos[1], Vector2.zero);

				//取得できなければキャンセル
				if(!hits[0] || !hits[1]) return;

				Piece p1 = hits[0].collider.GetComponent<Piece>();
				Piece p2 = hits[1].collider.GetComponent<Piece>();

				//位置が縦、横のどちらかが同じでなければキャンセル
				if(p1.position.x == p2.position.x && p1.position.y == p2.position.y) return;
				if(p1.position.x != p2.position.x && p1.position.y != p2.position.y) return;

				//IDが同じでなければ、触手でなければキャンセル
				if(p1.id != p2.id || p1.id != 1) return;
				isAction = true;

				//デバッグ用でSEを鳴らす
				AudioManager.Play(SEType.Tap, 1);

				//触手を生成
				currenTentacle[0] = Tentacle.CreateTentacle(p1.position);
				currenTentacle[1] = Tentacle.CreateTentacle(p2.position);

			}

			//移動
			currenTentacle[0].transform.position = pos[0];
			currenTentacle[1].transform.position = pos[1];

		}
		else if(InputManager.GetInput(out pos[0])) {

		}

		if(InputManager.GetInputUpDouble()) {
			isAction = false;
		}

	}


}
