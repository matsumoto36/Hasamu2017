using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// パズルの進行管理をする
/// </summary>
public class GameManager : MonoBehaviour {

	public GameObject backGround;
	public Animator animator;
	// Use this for initialization
	void Start () {
		animator.CrossFadeInFixedTime("Hide", 0, 0, 1);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("4"))
		{
			Debug.Log("(/・ω・)/");

			backGround.SetActive(!backGround.activeSelf);
			animator.SetBool("Open", !animator.GetBool("Open"));
		}
	}
}
