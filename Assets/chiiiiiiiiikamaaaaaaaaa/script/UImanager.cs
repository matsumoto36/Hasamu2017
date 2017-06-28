using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UImanager : MonoBehaviour {

	public bool isPause;
	bool _isPause;

	Animator animator;
	public void Awake()
	{
		Pause.ClearPauseList();
	}

	public void RetryButton()
	{
		//シーン移動中は実行しない
		if (SumCanvasAnimation.isMovingScene) return;

		if (_isPause == false)
		{
			Pause.Pauser();
		}

		//音の再生
		AudioManager.Play(SEType.Button, 1);
		//BGMフェードアウト
		AudioManager.FadeOut(2);

		SumCanvasAnimation.MoveScene("GameScene");
	}
	public void BackButton()
	{
		//シーン移動中は実行しない
		if (SumCanvasAnimation.isMovingScene) return;

		if (_isPause == false)
		{
			Pause.Pauser();
		}

		//音の再生
		AudioManager.Play(SEType.Button, 1);
		//BGMフェードアウト
		AudioManager.FadeOut(2);

		SumCanvasAnimation.MoveScene("StageSelectScene");
	}


	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		animator.CrossFadeInFixedTime("Hide", 0, 0, 1);
	}
	
	// Update is called once per frame
	void Update () {

		//if (Input.GetMouseButtonDown(0))
		//{
		//	AudioManager.Play(SEType.Tap, 1);
		//	Debug.Break();
		//}

		if(isPause != _isPause)
		{
			_isPause = isPause;
			SetPause(_isPause);
		}
	}

	/// <summary>
	/// メニューの表示・非表示を切り替える
	/// </summary>
	public void ToggleMenu() {
		//if(Input.GetKeyDown("4")) {

		//音の再生
		AudioManager.Play(SEType.Button, 1);

		Debug.Log("(/・ω・)/");
		animator.SetBool("Open", !animator.GetBool("Open"));
		//}
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
