using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// 爆弾
/// </summary>
public class PieceBomb : Piece, IExecutable
{

	Piece[] p = new Piece[4];

	Text timeViewer;
	RectTransform canvasRect;

	void Start() {

		canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();

		timeViewer = new GameObject("[Time]").AddComponent<Text>();
		timeViewer.transform.SetParent(canvasRect.transform);
		timeViewer.rectTransform.localScale = new Vector2(1, 1);
		timeViewer.font = Resources.Load<Font>("Font/Makinas");
		timeViewer.rectTransform.sizeDelta = new Vector2(200, 60);
		timeViewer.fontSize = 50;
		timeViewer.alignment = TextAnchor.MiddleCenter;
		//canvas = FindObjectOfType<Canvas>();
	}

	public void Update() {
		//取得用位置を格納
		Vector2[] checkPos = new Vector2[] {
			new Vector2(position.x, position.y + 1),
			new Vector2(position.x - 1, position.y),
			new Vector2(position.x, position.y - 1),
			new Vector2(position.x + 1, position.y),
		};

		//挟まれ検知
		for(int i = 0; i < checkPos.Length; i++) {
			p[i] = StageGenerator.GetPiece(checkPos[i]);

			//p[i]がnullであれば判定しない
			if(!(p[0] == null || p[2] == null)) {
				if(p[0].id == p[2].id) {
					Sandwiched(p[0].id);
				}
			}

			if(!(p[1] == null || p[3] == null)) {
				if(p[1].id == p[3].id) {
					Sandwiched(p[1].id);
				}
			}
		}

		Vector2 timePos;

		//表示時間の移動
		Vector2 pos = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, pos, Camera.main, out timePos);
		timeViewer.rectTransform.anchoredPosition = timePos;

		//時間の表示
		timeViewer.text = string.Format("{0:000.0}", Timebar.time);

	}

	/// <summary>
	/// 触手に掴まれているときに毎フレーム実行される
	/// </summary>
	public void Action() {

	}

	///<summary>
	///挟まれると呼び出される
	///</summary>
	public void Sandwiched(int id) {
		switch(id) {
			case 4:
				Timebar.Decpersec = 1;
				break;
			case 5:
				Timebar.Decpersec = 2;
				break;
			case 6:
				Timebar.Decpersec = 0.5f;
				break;
		}
	}

	public IEnumerator DestroyBombAnim() {

		//落ちる音再生
		AudioManager.Play(SEType.Hole, 1.0f);

		float rotSpeed = 5;
		float timeSpeed = 1;
		float t = 0;
		while(t < 1.0f) {
			t += Time.deltaTime * timeSpeed;

			Vector3 scale = new Vector3(1, 1, 1) * (1 - t);
			Quaternion rot = Quaternion.AngleAxis(rotSpeed, Vector3.forward);

			transform.localScale = scale;
			timeViewer.transform.localScale = scale;

			transform.rotation *= rot;
			timeViewer.transform.rotation *= rot;

			yield return null;
		}

		//パーティクル再生
		ParticleManager.PlayOneShot(ParticleType.BombDestrtoy, transform.position, Quaternion.identity, 5);

		//カウントダウンストップ
		Timebar.StopTimer();

		Destroy(gameObject);

		//ステージクリア
		GameManager.GameClear();
	}

	void OnDestroy() {
		Destroy(timeViewer.gameObject);
	}
}
