﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TentacleAnimState {
	Move,
	Hold_Normal,
	Hold_Hot,
	Hold_Cold,
	Return,
}

/// <summary>
/// 触手本体
/// </summary>
public class Tentacle : MonoBehaviour {

	const float BODY_OFFSET = 7f;//
	static int idCounter = 1;		//idを割り当てる用


	public Vector2 angle;			//生える向き
	public Vector2 position;        //本体のいる位置
	float length = 0;				//本体の長さ

	int id;                         //触手のid
	public TentacleAnimState state
	{
		get; private set;
	}
	Animator bodyAnimation;
	Coroutine execCol;
	SpriteRenderer maskRenderer;

	AudioSource moveAudio;          //移動中の音
	float volBound = 0.03f;			//ボリュームを上げるかどうかの閾値
	float volAdd = 0.05f;           //上がる量

	Vector2 oldTouchPos;			

	void Awake() {
		id = GetNextMaskID();
		bodyAnimation = Instantiate(ResourceLoader.GetPrefab(R_PrefabType.TentacleBody)).GetComponent<Animator>();

		//移動中の音を再生
		moveAudio = AudioManager.Play(SEType.TentacleMove);
		moveAudio.volume = 0;

		state = TentacleAnimState.Move;
	}

	void Start () {

		//ボディ部分を作成
		bodyAnimation.transform.SetParent(transform);
		bodyAnimation.transform.localPosition = -angle * (BODY_OFFSET + 1);
		bodyAnimation.transform.localScale *= 0.75f;

		var meshInstance = bodyAnimation.GetComponent<Anima2D.SpriteMeshInstance>();
		meshInstance.sortingOrder = 2;
		meshInstance.sharedMaterial = ResourceLoader.GetMaterial(R_MaterialType.MaskableSprite);
		meshInstance.sharedMaterial.SetInt("_ID", id);

		//マスク用レンダラーの作成
		maskRenderer = new GameObject("[Mask]").AddComponent<SpriteRenderer>();
		maskRenderer.material = ResourceLoader.GetMaterial(R_MaterialType.MaskingSprite);
		maskRenderer.material.SetInt("_ID", id);
		maskRenderer.sprite = ResourceLoader.GetOtherSprite(R_OtherSpriteType.Mask);
		maskRenderer.transform.SetParent(transform);
		maskRenderer.transform.localPosition = angle * 0.5f;
		maskRenderer.sortingOrder = 2;

		//画像の回転
		float rot = 0;
		if(angle.y != 0) rot -= 90;
		if(angle.x == 1 || angle.y == -1) rot += 180;
		bodyAnimation.transform.rotation = Quaternion.AngleAxis(rot, Vector3.forward);

		//生えるアニメーション
		execCol = StartCoroutine(CreateAnim());

		//生えるパーティクル
		ParticleManager.PlayOneShot(
			ParticleType.TentacleSpawn,
			position + angle / 2,
			Quaternion.Euler(0, 0, Mathf.Atan2(angle.y, angle.x) * Mathf.Rad2Deg - 90),
			5);

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
		t.oldTouchPos = position;
		return t;
	}

	public void Move(Vector2 touchPosition) {

		//touchPosition 制限
		Vector2 tvec = touchPosition - ((Vector2)transform.position - angle);
		touchPosition.x = transform.position.x - angle.x + Mathf.Clamp(tvec.x, -1.2f, 1.2f);
		touchPosition.y = transform.position.y - angle.y + Mathf.Clamp(tvec.y, -1.2f, 1.2f);

		bool isHorizonCancel = false;				//横移動キャンセル用
		Vector2 OVec = touchPosition - position;    //ベースからのベクトル

		Vector2 v;									//垂直なベクトル
		float r = Vector3.Angle(angle, OVec);		//角度(deg)

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

		//向きの方向の長さ(angle分盛る)
		Vector2 angleVec = angle * OVec.magnitude * Mathf.Sin(Vector2.Angle(OVec, v) * Mathf.Deg2Rad) + angle;
		//向きに垂直な長さ
		Vector2 vVec = v * OVec.magnitude * Mathf.Cos(Vector2.Angle(OVec, v) * Mathf.Deg2Rad);

		//デバッグ表示
		Debug.DrawLine(transform.position, touchPosition, Color.red);
		//Debug.DrawLine(position, touchPosition, Color.red);
		//Debug.DrawLine(position, position + angleVec, Color.blue);
		//Debug.DrawLine(position, position + vVec, Color.blue);

		#region 縦方向の制限

		//自分の生成している向きより下か？
		if(r > 90) {

			angleVec = angle;

			//角度等更新
			OVec = touchPosition - position;
			r = Vector3.Angle(angle, OVec);
		}

		//ブロックに埋まっているか
		int checkCount = (int)angleVec.magnitude + 1;
		for(int i = 1;i <= checkCount;i++) {
			cPos = position + (angle * i);
			p = StageGenerator.GetPiece(cPos);

			//埋まっているか
			if(p && !p.noCollision) {
				//Debug.Log("ume");
				Vector2 cPosV = cPos - position;
				float d = (int)(cPosV.magnitude * Mathf.Sin(Vector2.Angle(cPosV, v) * Mathf.Deg2Rad)) - 1;

				angleVec = angle * d;
				break;
			}
		}

		#endregion

		#region 横方向の制限

		//i=0のとき、横に触手ブロックがなければ移動不可
		//i!=0のとき、横にブロックがあれば移動不可
		checkCount = (int)(angleVec.magnitude + 0.9f);
		checkCount = checkCount == 0 ? 1 : checkCount;
		//Debug.Log("cnt:" + checkCount);
		for(int i = 0;i <= checkCount;i++) {
			cPos = position + v + (angle * i);
			p = StageGenerator.GetPiece(cPos);
			if((i == 0 && (!p || p.id != 1)) || (i != 0 && p && !p.noCollision)) {
				isHorizonCancel = true;
				break;
			}
		}

		//横移動がキャンセルされた場合
		if(isHorizonCancel) {
			//Debug.Log("c");
			vVec = Vector2.zero;
		}

		#endregion

		//デバッグ表示
		//Debug.DrawLine(position, position + vVec + angleVec);

		float moveLength = (position + vVec + angleVec - (Vector2)transform.position).magnitude;

		//音量調整
		float newVolume = moveAudio.volume;
		if (moveLength > volBound) {
			newVolume += volAdd;
		} else {
			newVolume -= volAdd;
		}

		moveAudio.volume = Mathf.Clamp(newVolume, 0, 0.7f);

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

		//表示範囲を決める
		SetVisibleArea();
	}

	/// <summary>
	/// 触手が差し示している場所を取得する
	/// </summary>
	/// <returns>場所</returns>
	public Vector2 GetTargetPosition() {
		Vector2 ans = position + angle * (length - 0.3f);
		ans.x = (int)(ans.x + 0.5);
		ans.y = (int)(ans.y + 0.5);

		return ans;
	}

	/// <summary>
	/// 触手の長さを変更する
	/// </summary>
	void SetLength() {

		float l = 0;

		Vector2 v = (Vector2)transform.position - position;
		float rad = Vector2.Angle(angle, v) * Mathf.Deg2Rad;

		l = (v.magnitude * Mathf.Cos(rad)) + 1;

		if(l < 0) return;

		length = l;
	}

	/// <summary>
	/// 触手の見える範囲を決める
	/// </summary>
	void SetVisibleArea() {
		if(!maskRenderer) return;

		Vector3 vec = (Vector2)transform.position - position;

		//Debug.DrawLine((Vector2)transform.position + angle * 0.5f, position + angle * 0.5f, Color.black);

		float r = Vector2.Angle(vec, angle) * Mathf.Deg2Rad;
		float vSize = Mathf.Cos(r) * vec.magnitude + 0.2f;

		//Debug.DrawLine((Vector2)transform.position + angle * 0.5f, (Vector2)transform.position + angle * 0.5f - angle * vSize, Color.black);

		Vector2 size = new Vector2(Mathf.Abs(angle.x), Mathf.Abs(angle.y)) * (vSize - 5) + new Vector2(5, 5);

		maskRenderer.transform.localScale = size;
		maskRenderer.transform.localPosition = -(angle * 0.5f * (vSize - 1));
	}

	/// <summary>
	/// アニメーションを変更する
	/// </summary>
	/// <param name="state"></param>
	public void SetAnimatonState(TentacleAnimState state) {
		bodyAnimation.speed = 1;
		bodyAnimation.SetInteger("animState", (int)state);
		this.state = state;
	}

	/// <summary>
	/// はさんでいるときのブロックで変わる、アニメーションステートを取得する
	/// </summary>
	/// <param name="blockID">はさむブロック</param>
	/// <returns></returns>
	public static TentacleAnimState GetHoldState(int blockID) {
		switch(blockID) {
			case 5:
				return TentacleAnimState.Hold_Hot;
			case 6:
				return TentacleAnimState.Hold_Cold;
			default:
				return TentacleAnimState.Hold_Normal;
		}
	}

	/// <summary>
	/// 触手が戻る時に実行する
	/// </summary>
	public void Return() {

		SetAnimatonState(TentacleAnimState.Return);

		//実行中なら止める
		if(execCol != null) StopCoroutine(execCol);
		StartCoroutine(ReturnAnim());
	}

	/// <summary>
	/// 作られるときのアニメーション
	/// </summary>
	/// <returns></returns>
	IEnumerator CreateAnim() {

		//SEを再生
		AudioManager.Play(SEType.TentacleSpawn, 1);

		//自分の位置からオフセットまで移動
		float time = 0.2f;
		float t = 0;
		Vector2 startPosition = bodyAnimation.transform.localPosition;
		Vector2 endPosition = -angle * BODY_OFFSET;
		while(t < 1.0f) {
			t += Time.deltaTime / time;

			bodyAnimation.transform.localPosition = 
				Vector2.Lerp(startPosition, endPosition, t);

			yield return null;
		}
		bodyAnimation.transform.localPosition = endPosition;
		execCol = null;
	}

	/// <summary>
	/// 戻るときのアニメーション
	/// </summary>
	/// <returns></returns>
	IEnumerator ReturnAnim() {

		//移動中の音を削除
		Destroy(moveAudio.gameObject);

		//SEを再生
		AudioManager.Play(SEType.TentacleSpawn, 1);

		float time = 1f;
		float t = 0;
		Vector2 startPosition = bodyAnimation.transform.localPosition;
		Vector2 endPosition = -angle * (BODY_OFFSET + ((Vector2)transform.position - position).magnitude);
		while(t < 1.0f) {
			t += Time.deltaTime / time;

			bodyAnimation.transform.localPosition =
				Vector2.Lerp(startPosition, endPosition, t);

			yield return null;
		}

		Destroy(gameObject);

	}

	void OnDrawGizmos() {
		//Gizmos.DrawWireCube(GetTargetPosition(), Vector3.one);
	}

	public static int GetNextMaskID() {
		return idCounter >= 128 ? idCounter = 1 : idCounter = idCounter * 2;
	}
}
