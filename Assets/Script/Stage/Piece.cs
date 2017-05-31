using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ上に配置される一つのマス
/// </summary>
public class Piece : MonoBehaviour {

	public int id;						//自分のID
	public int subId;					//アニメーション用のサブID

	public Vector2 position;			//ステージ上の位置

	public SpriteRenderer _renderer;	//自分の画像

	// Update is called once per frame
	void Update () {           

	}

	/// <summary>
	/// 画像を持ってくる
	/// </summary>
	public virtual void SpriteLoad() {
		_renderer.sprite = ResourceLoader.GetChips(MapChipType.MainChip)[id];
	}

	/// <summary>
	/// サブIDと回転を設定する
	/// </summary>
	/// <returns>サブID</returns>
	public void SetSubIDAndRotation() {

		#region subIDの指定方法
		/*
		 1
		208
		 4
		合計 =	0	0
				1	1
				2	1 + 90
				3	2
				4	1 + 180
				5	3
				6	2 + 90
				7	4
				8	1 + 270
				9	2 + 270
				10	3 + 90
				11	4 + 270
				12	2 + 180
				13	4 + 180
				14	4 + 90
				15	5
		このように、各隣接場所に重みを付け一つのint(4bit)で定義できるようにする。
		あとは回転なしのパターンを回転させ、自分の数値に一致したときの回数が回転数なので
		それを適用させる
		*/
		#endregion

		//座標を表示
		//Debug.Log(position);

		//取得用位置を格納
		Vector2[] checkPos = new Vector2[] {
			new Vector2(position.x, position.y - 1),
			new Vector2(position.x - 1, position.y),
			new Vector2(position.x, position.y + 1),
			new Vector2(position.x + 1, position.y),
		};

		//接続数c, 重み合計値sumを得る
		int connectCount = 0; int weightSum = 0;
		for(int i = 0;i < 4;i++) {
			Piece p = StageGenerator.GetPiece(checkPos[i]);
			if(p && p.id == id) {
				connectCount++;
				weightSum += (int)Mathf.Pow(2, i);
			}
		}

		//接続数と合計値表示
		//Debug.Log("c:" + c + " sum:" + sum);

		//0ならsubID = 0, 4なら5が確定する(回転もなし)
		if(connectCount == 0) { subId = 0; return; }
		if(connectCount == 4) { subId = 5; return; }

		//subIDを決定する
		subId = connectCount;

		/* ふっしぎー！ */
		if(connectCount == 3 || weightSum % 5 == 0) subId++;

		/* ---ここから回転--- */

		//回転なしの重み合計値を定義(要素-1がsubID)
		int[] connectDefaultWeight = new int[] { 1, 3, 5, 7 };
		int weight = connectDefaultWeight[subId - 1];

		//回転しない場合は終了
		if(weight == weightSum) return;

		int rotationCount = 0;
		do {
			//4bitとして1bitの左rotate
			weight *= 2;
			if(weight > 15) {
				weight = weight - 15;
			}

			rotationCount++;
		} while(weight != weightSum); //合うまで継続

		//回転数表示
		//Debug.Log("rotationCount:" + rotationCount);

		//回転
		transform.rotation = Quaternion.AngleAxis(rotationCount * 90, Vector3.forward);
	}

}
