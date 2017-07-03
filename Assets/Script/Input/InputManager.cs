using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スマホの入力を管理する
/// </summary>
public class InputManager : MonoBehaviour {

	public static bool isFreeze = false;	//入力をフリーズするか

	static InputManager myManager;			//自分のリファレンス
	static Transform debug_FalseTouch;			//エディタ用マルチタップ位置
	static bool[] isTouchArray = new bool[10];  //タッチしているか保持

	// Use this for initialization
	void Start () {
		debug_FalseTouch = GameObject.Find("FalseTouch").transform;
		myManager = FindObjectOfType<InputManager>();
	}

	/// <summary>
	/// すべてのタッチ入力を取る
	/// </summary>
	/// <param name="pos">out 位置(null許容)</param>
	/// <returns>一つでもあればtrue</returns>
	public static bool GetInputAll(out Vector2?[] pos) {

		//初期化
		pos = new Vector2?[10];

		if(isFreeze) return false;

		//エディタ上かどうか判定
		if(Application.isEditor) {
			if(Input.GetMouseButton(0)) {
				pos[0] = Input.mousePosition;
				isTouchArray[0] = true;
			}

			if(Input.GetKey(KeyCode.F)) {
				pos[1] = Camera.main.WorldToScreenPoint(debug_FalseTouch.position);
				isTouchArray[1] = true;
			}

		}
		else {
			//入力されているTouchをすべて回す
			foreach(var t in Input.touches) {
				pos[t.fingerId] = t.position;
				isTouchArray[t.fingerId] = true;
			}
		}

		bool ans = false;
		foreach(var p in pos) {
			if(p != null) {
				ans = true;
				break;
			}

		}

		return ans;

	}

	/// <summary>
	/// すべての入力のうち、離されたものを取る
	/// </summary>
	/// <returns>離された番号のbool</returns>
	public static bool[] GetInputUpAll() {

		bool[] ans = new bool[10];
		bool[] saveIsTouchArray = new bool[10];
		isTouchArray.CopyTo(saveIsTouchArray, 0);


		Vector2?[] pos = new Vector2?[10];
		GetInputAll(out pos);

		for(int i = 0;i < 10;i++) {

			if(saveIsTouchArray[i] && pos[i] == null) {
				Debug.Log("TouchUp " + i);
				isTouchArray[i] = false;
				ans[i] = true;
			}
		}
		return ans;
	}
}
