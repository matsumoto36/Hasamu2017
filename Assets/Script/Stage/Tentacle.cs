using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 触手本体
/// </summary>
public class Tentacle : MonoBehaviour {

	const int CHIPOFFSET = 5;		//使う画像のオフセット

	public Vector2 angle;			//生える向き
	public Vector2 position;        //本体のいる位置

	public int length = 0;
	Sprite[] tentacleSpr = new Sprite[2];

	List<SpriteRenderer> tentacleBody = new List<SpriteRenderer>();

	void Awake () {
		Sprite[] bff = ResourceLoader.GetChips(MapChipType.Sub1_Tentacle);
		tentacleSpr[0] = bff[CHIPOFFSET];
		tentacleSpr[1] = bff[CHIPOFFSET + 1];
	}

	/// <summary>
	/// 触手を生成する
	/// </summary>
	/// <param name="position">場所</param>
	/// <returns>生成した触手</returns>
	public static Tentacle CreateTentacle(Vector2 position) {
		Tentacle t = new GameObject("[Tentacle]").AddComponent<Tentacle>();
		t.position = position;
		t.transform.position = position;
		return t;
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void Move(Vector2 newPosition) {

		bool isHorizonCancel = false;			//横移動キャンセル用
		bool isVerticCancel = false;           //縦移動キャンセル用
		Vector2 OVec = newPosition - position;	//ベースからのベクトル
		Vector2 v;								//垂直なベクトル
		float r = Vector3.Angle(angle, OVec);   //角度(deg)

		Vector2 cPos;
		Piece p;

		//外積で左右判定
		float side = angle.x * OVec.y - angle.y * OVec.x;
		if(side < 0) {
			v = (Quaternion.Euler(0, 0, -90) * angle).normalized;
		}
		else {
			v = (Quaternion.Euler(0, 0, 90) * angle).normalized;
		}


		//向きの方向の長さ
		Vector2 angleVec = angle * OVec.magnitude * Mathf.Sin(Vector2.Angle(OVec, v) * Mathf.Deg2Rad);
		//向きに垂直な長さ
		Vector2 vVec = v * OVec.magnitude * Mathf.Cos(Vector2.Angle(OVec, v) * Mathf.Deg2Rad);

		//デバッグ表示
		Debug.DrawLine(position, newPosition, Color.red);
		Debug.DrawLine(position, position + angleVec, Color.blue);
		Debug.DrawLine(position, position + vVec, Color.blue);


		#region 縦方向の制限

		//自分の生成している向きより下か？
		if(r > 90) {

			angleVec = Vector2.zero;

			//角度等更新
			OVec = newPosition - position;
			r = Vector3.Angle(angle, OVec);
		}

		//デバッグ表示
		Debug.DrawLine(position, newPosition, Color.green);

		//ブロックに埋まっているか
		int checkCount = (int)angleVec.magnitude + 1;
		for(int i = 1;i <= checkCount;i++) {
			cPos = position + (angle * i);
			p = StageGenerator.GetPiece(cPos);

			//埋まっているか
			if(p && !p.isMoved) {
				//Debug.Log("ume");
				Vector2 cPosV = cPos - position;

				float a = OVec.magnitude * Mathf.Cos(Vector2.Angle(OVec, v) * Mathf.Deg2Rad);
				float d = (int)(cPosV.magnitude * Mathf.Sin(Vector2.Angle(cPosV, v) * Mathf.Deg2Rad)) - 1;

				angleVec = angle * d;
				break;
			}
		}

		#endregion

		//デバッグ表示
		Debug.DrawLine(position, newPosition, Color.yellow);

		#region 横方向の制限

		//i=0のとき、横に触手ブロックがなければ移動不可
		//i!=0のとき、横にブロックがあれば移動不可
		checkCount = (int)(angleVec.magnitude + 0.9f);
		checkCount = checkCount == 0 ? 1 : checkCount;
		//Debug.Log("cnt:" + checkCount);
		for(int i = 0;i <= checkCount;i++) {
			cPos = position + v + (angle * i);
			p = StageGenerator.GetPiece(cPos);
			if((i == 0 && (!p || p.id != 1)) || (i != 0 && p && !p.isMoved)) {
				isHorizonCancel = true;
				break;
			}
		}

		//横移動がキャンセルされた場合
		if(isHorizonCancel) {
			//Debug.Log("c");
			//newPosition = position + angleVec;
			vVec = Vector2.zero;
		}

		#endregion

		//デバッグ表示
		Debug.DrawLine(position, position + vVec + angleVec);

		#region 移動

		transform.position = position + vVec + angleVec;

		#endregion

		#region ベース座標移動

		//ベースの座標を移動するか
		Vector2 vPosInt = new Vector2((int)(position.x + vVec.x + 0.5), (int)(position.y + vVec.y + 0.5));
		if(!isHorizonCancel && vPosInt != position) {
			p = StageGenerator.GetPiece(vPosInt);
			position = p && p.id == 1 ? vPosInt : position;
		}

		#endregion

		//長さを決める
		SetLength();
	}

	public Vector2 GetTargetPosition() {
		return position + angle * length;
	}

	/// <summary>
	/// 触手の長さを変更する
	/// </summary>
	void SetLength() {

		int l = 0;

		Vector2 v = (Vector2)transform.position - position;
		float rad = Vector2.Angle(angle, v) * Mathf.Deg2Rad;

		l = (int)(v.magnitude * Mathf.Cos(rad)) + 1;

		if(l < 0 || l == length) return;

		length = l;

		foreach(var g in tentacleBody) {
			Destroy(g.gameObject);
		}

		tentacleBody = new List<SpriteRenderer>();
		for(int i = 0;i < length;i++) {
			int c = 0;
			if(i != 0) c++;

			SpriteRenderer spr = new GameObject("[Child " + i + "]").AddComponent<SpriteRenderer>();
			spr.transform.SetParent(transform);
			spr.sprite = tentacleSpr[c];
			spr.sortingOrder = 2;
			spr.transform.localPosition = angle * i * -1;

			//回転
			float rot = 0;
			if(angle.y != 0)					rot -= 90;
			if(angle.x == 1 || angle.y == -1)	rot += 180;
			spr.transform.rotation = Quaternion.AngleAxis(rot, Vector3.forward);

			tentacleBody.Add(spr);
		}
	}

	/// <summary>
	/// 触手が死ぬ時
	/// </summary>
	public void Death() {
		Destroy(gameObject);
	}

	void OnDrawGizmos() {
		Gizmos.DrawWireCube(GetTargetPosition(), Vector3.one);
	}
}
