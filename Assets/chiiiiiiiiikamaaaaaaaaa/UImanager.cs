using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UImanager : MonoBehaviour {

	public bool isPause;
	bool _isPause;

	Animator animator;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		animator.CrossFadeInFixedTime("Hide", 0, 0, 1);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("4"))
		{
			Debug.Log("(/・ω・)/");
			animator.SetBool("Open", !animator.GetBool("Open"));
		}

		if(isPause != _isPause)
		{
			_isPause = isPause;
			SetPause(_isPause);
		}
	}

	void SetPause(bool enable)
	{
		if (enable)
		{
			Pause.Pauser();
		}
		else
		{
			Pause.Resume();
		}
	}
}
