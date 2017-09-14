using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anima2D;

public class UImanager : MonoBehaviour {

	public bool isPause;
	public static bool isFreeze = false;
	bool _isPause;

	Animator animator;

	public SpriteMeshInstance bgmTentacleInstance;
	public SpriteRenderer bgmMaskRenderer;

	public SpriteMeshInstance seTentacleInstance;
	public SpriteRenderer seMaskRenderer;


	EditModeMain editModeMain;

	public void Awake()
	{
		Pause.ClearPauseList();

		if(GameManager.IsEditMode) {
			editModeMain = FindObjectOfType<EditModeMain>();
		}
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

		if(GameManager.IsEditMode) {
			//リトライ
			editModeMain.StartCoroutine(editModeMain.RetryPreview());
		}
		else {
			SumCanvasAnimation.MoveScene("GameScene");
		}
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

		if(GameManager.IsEditMode) {
			//戻る
			editModeMain.StartCoroutine(editModeMain.UnloadPreview());
		}
		else {
			SumCanvasAnimation.MoveScene("StageSelectScene");
		}

	}


	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		animator.CrossFadeInFixedTime("Hide", 0, 0, 1);
	}
	
	// Update is called once per frame
	void Update () {

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

		if (isFreeze) return;

		//音の再生
		AudioManager.Play(SEType.Button, 1);

		Debug.Log("(/・ω・)/");
		bool isOpen = !animator.GetBool("Open");
		animator.SetBool("Open", isOpen);

		if(isOpen) {
			//マスクのIDを設定
			int bgmID = Tentacle.GetNextMaskID();
			bgmTentacleInstance.sharedMaterial = ResourceLoader.GetMaterial(R_MaterialType.MaskableSprite);
			bgmTentacleInstance.sharedMaterial.SetInt("_ID", bgmID);

			bgmMaskRenderer.material = ResourceLoader.GetMaterial(R_MaterialType.MaskingSprite);
			bgmMaskRenderer.material.SetInt("_ID", bgmID);

			int seID = Tentacle.GetNextMaskID();
			seTentacleInstance.sharedMaterial = ResourceLoader.GetMaterial(R_MaterialType.MaskableSprite);
			seTentacleInstance.sharedMaterial.SetInt("_ID", seID);

			seMaskRenderer.material = ResourceLoader.GetMaterial(R_MaterialType.MaskingSprite);
			seMaskRenderer.material.SetInt("_ID", seID);

			Debug.Log("set");
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
