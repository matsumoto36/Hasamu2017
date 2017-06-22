using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pause : MonoBehaviour
{

	public static bool isPause;
static List<Pause> targets = new List<Pause>();
Behaviour[] pauseBehavs = null;
	// Use this for initialization
	public static void ClearPauseList()
	{
		targets.Clear();
	}
	void Start()
{
		Debug.Log("name:" + name); 
	targets.Add(this);
}

void OnPause()
{
	if (pauseBehavs != null)
	{
		return;
	}
	//有効なBehaviourを取得
	pauseBehavs = Array.FindAll(GetComponentsInChildren<Behaviour>(), (obj) => { return obj.enabled; });
	foreach (var com in pauseBehavs)
	{
		com.enabled = false;
	}
}
	void OnResume()
	{
		if (pauseBehavs == null)
		{
			return;
		}

		// ポーズ前の状態にBehaviourの有効状態を復元
		foreach (var com in pauseBehavs)
		{
			com.enabled = true;
		}

		pauseBehavs = null;
	}
//ポーズ
public static void Pauser()
{
		isPause = true;
	foreach (var obj in targets)
	{
		obj.OnPause();
	}
}
	//ポーズ解除
	public static void Resume()
	{
		isPause = false;
		foreach (var obj in targets)
		{
			obj.OnResume();
		}
	}
	// Update is called once per frame
	void Update()
	{

	}
}
