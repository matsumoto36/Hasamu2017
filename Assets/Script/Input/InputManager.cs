using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スマホの入力を管理する
/// </summary>
public class InputManager : MonoBehaviour {

	public Text debug_text;

	public int FakeCount = 0;

	static Transform debug_FalseTouch;

	static bool isTouch, isTouchDouble;
	static bool[] isTouchArray = new bool[10];

	public bool[] istouch;
	public bool[] saveistouch;

	// Use this for initialization
	void Start () {

		debug_FalseTouch = GameObject.Find("FalseTouch").transform;

	}
	
	// Update is called once per frame
	void Update () {
		DebugGetInput();

		istouch = isTouchArray;
	}

	/// <summary>
	/// 入力を取る
	/// </summary>
	/// <param name="pos">out 位置</param>
	/// <returns>あるかどうか</returns>
	public static bool GetInput(out Vector2 pos) {

		pos = new Vector2();

		if(Application.isEditor) {
			if(!Input.GetMouseButton(0)) return false;

			pos = Input.mousePosition;
		}
		else {
			if(!(Input.touchCount > 0)) return false; 

			pos = Input.touches[0].position;
		}

		//Debug.Log("Touch " + pos);

		return isTouch = true;
	}

	/// <summary>
	/// 入力を取る(2本)
	/// </summary>
	/// <param name="pos">out 位置</param>
	/// <returns>あるかどうか</returns>
	public static bool GetInputDouble(out Vector2[] pos) {

		pos = new Vector2[2];

		if(Application.isEditor) {
			if(!Input.GetMouseButton(0)) return false;

			pos[0] = Input.mousePosition;
			pos[1] = Camera.main.WorldToScreenPoint(debug_FalseTouch.position);
		}
		else {
			if(!(Input.touchCount > 1)) return false;

			pos[0] = Input.touches[0].position;
			pos[1] = Input.touches[1].position;
		}

		//Debug.Log("TouchD " + pos[0] + " " + pos[1]);

		return isTouchDouble = true;
	}

	/// <summary>
	/// すべてのタッチ入力を取る
	/// </summary>
	/// <param name="pos">out 位置(null許容)</param>
	/// <returns>一つでもあればtrue</returns>
	public static bool GetInputAll(out Vector2?[] pos) {

		pos = new Vector2?[10];

		if(Application.isEditor) {
			if(!Input.GetMouseButton(0)) return false;

			pos[0] = Input.mousePosition;
			pos[1] = Camera.main.WorldToScreenPoint(debug_FalseTouch.position);

			isTouchArray[0] = true;
			isTouchArray[1] = true;
		}
		else {
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
	/// タッチが離されたかとる
	/// </summary>
	/// <returns>離されたかどうか</returns>
	public static bool GetInputUp() {

		Vector2 pos = new Vector2();

		if(isTouch && !GetInput(out pos)) {
			isTouch = false;
			return true;
		}

		return false;
	}

	/// <summary>
	/// タッチが離されたかとる
	/// </summary>
	/// <returns>離されたかどうか</returns>
	public static bool GetInputUpDouble() {

		Vector2[] pos = new Vector2[2];

		if(isTouchDouble && !GetInputDouble(out pos)) {
			isTouchDouble = false;
			return true;
		}

		return false;
	}

	/// <summary>
	/// すべての入力のうち、離されたものを取る
	/// </summary>
	/// <returns>一つでも離された番号のbool</returns>
	public static bool[] GetInputUpAll() {

		bool[] ans = new bool[10];
		bool[] saveIsTouchArray = new bool[10];
		isTouchArray.CopyTo(saveIsTouchArray, 0);


		Vector2?[] pos;
		GetInputAll(out pos);

		for(int i = 0;i < 10;i++) {
			//Debug.Log("arr" + i + saveIsTouchArray[i] + " " + isTouchArray[i]);
		}


		for(int i = 0;i < 10;i++) {

			if(saveIsTouchArray[i] && pos[i] == null) {
				isTouchArray[i] = false;
				ans[i] = true;
			}
		}
		return ans;
	}

	/// <summary>
	/// デバッグ用に、すべてのタッチ情報を出力する
	/// </summary>
	public void DebugGetInput() {

		if(!debug_text) return;

		string result = "";
		result += string.Format("touchCount : {0}\r\n", Input.touchCount);
		for(int i = 0;i < Input.touchCount;i++) {
			result += string.Format("{0} touchPos : {1} , fingerId : {2} \r\n", i, Input.touches[i].position, Input.touches[i].fingerId);
		}
		debug_text.text = result;

	}
}
