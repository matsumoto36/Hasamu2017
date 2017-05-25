using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バーチャルなプレイヤー
/// </summary>
public class Player : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {

		Vector2[] pos = new Vector2[2];

		if(InputManager.GetInputDouble(out pos)) {


		}
		else if(InputManager.GetInput(out pos[0])) {

		}
	}
}
