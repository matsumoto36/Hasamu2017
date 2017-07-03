using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 偽のタッチ情報を送るためのクラス
/// </summary>
public class FalseTouch : MonoBehaviour {

	public Transform fulcrum;

	// Use this for initialization
	void Start () {

		//エディタ外なら表示しない
		if(!Application.isEditor) {
			fulcrum.gameObject.SetActive(false);
			gameObject.SetActive(false);
		}

	}
	
	// Update is called once per frame
	void Update () {

		//自分を支点にして、マウスと点対象にfulcrumを移動
		Vector3 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		fulcrum.position += move * 5 * Time.deltaTime;

		Vector3 def = Camera.main.ScreenToWorldPoint(Input.mousePosition) - fulcrum.position;
		Vector3 pos = fulcrum.position - def;
		pos.z = 0f;
		transform.position = pos;

	}
}
