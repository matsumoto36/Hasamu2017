using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public enum EditToolMode { Hand, Pen, FillRect}

public class EditModeMain : MonoBehaviour {

	const int MAP_SIZE_X = 14;
	const int MAP_SIZE_Y = 9;
	const int VIEW_CHIP_SIZE = 128;


	public EditToolMode currentTool {
		get;
		private set;
	}

	public GameObject sceneRoot;

	//各ウィンドウのパネル
	public RectTransform fileView;
	public RectTransform saveFileView;
	public RectTransform mapchipView;
	public RectTransform setTimeView;

	//メニュー系
	public Slider viewMagnificationSlider;

	//ファイル系
	public InputField saveFileName;

	//制限時間系
	public Text viewTime;
	public InputField inputTimeField;
	int limitTime = 60;

	//ビュー系
	public GameObject viewChipPre;
	public RectTransform viewMapArea;
	public ScrollRect viewScroll;
	Image[,] generateViewImage;
	float viewMagnification = 1.0f;

	//ツール系
	public RectTransform toolSelectImage;
	public RectTransform toolHandImage;
	public RectTransform toolPenImage;
	public RectTransform toolFillRectImage;

	public RectTransform chipSelectImage;

	public Image currentChip;
	public Image[] selectableChip;

	//システム系
	Canvas canvas;

	Vector2 canvasScale;

	int[,] editMap;
	int selectChipID = 1;

	bool isLoading = false;
	bool canUseTool = true;

	Vector2 rectToolMouseStartPos;
	Vector2 rectToolMouseEndPos;

	public Image rectImagePre;
	Image rectImage;

	Sprite[] mapchipSprite;
	Sprite holeSprite;

	//Debug
	//public Text _debugText;

	// Use this for initialization
	void Start () {

		editMap = new int[MAP_SIZE_Y, MAP_SIZE_X];
		generateViewImage = new Image[MAP_SIZE_Y, MAP_SIZE_X];

		//テストマップ
		//editMap = new int[MAP_SIZE_Y, MAP_SIZE_X] {
		//	{1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
		//	{1,0,0,0,0,0,0,0,0,0,0,0,0,1 },
		//	{1,0,3,0,0,0,0,0,0,0,0,0,0,1 },
		//	{1,0,0,0,0,0,0,0,0,0,0,0,0,1 },
		//	{1,0,0,0,0,0,0,0,0,0,0,0,0,1 },
		//	{1,0,0,0,0,0,0,0,0,0,0,0,0,1 },
		//	{1,0,0,0,0,0,0,0,0,0,0,14,0,1 },
		//	{1,0,0,0,0,0,0,0,0,0,0,0,0,1 },
		//	{1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
		//};

		//キャンバスを持ってくる
		canvas = FindObjectOfType<Canvas>();


		//マップチップのロード
		FindObjectOfType<ResourceLoader>().LoadAll();
		mapchipSprite = ResourceLoader.GetChips(R_MapChipType.MainChip);
		holeSprite =    ResourceLoader.GetChips(R_MapChipType.Hole)[0];

		//画像の割当
		currentChip.sprite = GetMapchipFromID(selectChipID);

		//ボタンの機能を設定
		toolHandImage.GetComponent<Button>().onClick.AddListener(() => {
			ChangeEditTool(EditToolMode.Hand);
		});
		toolPenImage.GetComponent<Button>().onClick.AddListener(() => {
			ChangeEditTool(EditToolMode.Pen);
		});
		toolFillRectImage.GetComponent<Button>().onClick.AddListener(() => {
			ChangeEditTool(EditToolMode.FillRect);
		});

		for(int i = 0;i < selectableChip.Length;i++) {

			selectableChip[i].sprite = GetMapchipFromID(ConvertSelectChipID(i));

			//ラムダ式内にループ変数を使うために、ダミーに代入
			int d = i;
			selectableChip[i].GetComponent<Button>().onClick.AddListener(() => {
				ChangeSelectChip(d);
			});
		}

		//表示領域を生成
		for(int i = 0;i < MAP_SIZE_Y;i++) {
			for(int j = 0;j < MAP_SIZE_X;j++) {

				Image chip = Instantiate(viewChipPre).GetComponent<Image>();
				chip.gameObject.name = "[stage " + j + " " + i + " ]";
				chip.rectTransform.SetParent(viewMapArea);
				chip.rectTransform.localScale = new Vector3(1, 1, 1);

				EditModePiece piece = chip.GetComponent<EditModePiece>();
				piece.image = chip;
				piece.chipPosition.x = j;
				piece.chipPosition.y = i;

				generateViewImage[i, j] = chip;
			}
		}

		//制限時間の表示更新
		viewTime.text = limitTime.ToString() + "sec";

		//ビューポートがおかしくなる問題を修正
		RectTransform parent = viewMapArea.parent.GetComponent<RectTransform>();
		parent.anchorMin = new Vector2(0, 0);
		parent.anchorMax = new Vector2(1, 1);

		ChangeEditTool(EditToolMode.Hand);

		//描画
		SetViewSize();
		Draw();

		//音楽を再生
		AudioManager.FadeIn(2.0f, BGMType.Select, 1, true);
	}

	void Update() {

		//Debug
		if(Input.GetKeyDown(KeyCode.A)) {
			Debug.Break();
		}

		if(Input.GetKeyDown(KeyCode.S)) {
			Debug.Log(Application.dataPath);
		}

		if(!canUseTool) return;

		//ズーム系
		if(Input.GetKey(KeyCode.LeftControl)) {
			ChangeViewMagnification(
				viewMagnification + Input.mouseScrollDelta.y * 0.1f);
		}

		//個別の処理
		switch(currentTool) {
			case EditToolMode.Hand:
				HandTool();
				break;

			case EditToolMode.Pen:
				if(Input.GetMouseButton(0)) {
					Paint(GetEditModePiceFromRaycast(Input.mousePosition));
				}
				break;

			case EditToolMode.FillRect:
				if(Input.GetMouseButtonDown(0)) {
					FillToolStart(Input.mousePosition);
				}

				if(Input.GetMouseButtonUp(0)) {
					FillToolEnd(rectToolMouseEndPos);
				}
				
				//範囲の描画
				FillToolUpdate();
				break;

			default:
				break;
		}
	}

	#region HandTool

	void HandTool() {

		if(Application.isEditor) return;

		bool canPinch = Input.touchCount == 2;

		viewScroll.horizontal = !canPinch;
		viewScroll.vertical = !canPinch;

		if(canPinch) Pinch();
	}

	void Pinch() {

		Touch t1 = Input.GetTouch(0);
		Touch t2 = Input.GetTouch(1);

		// 各タッチの前フレームでの位置をもとめます
		Vector2 t1PrevPos = t1.position - t1.deltaPosition;
		Vector2 t2PrevPos = t2.position - t2.deltaPosition;

		// 各フレームのタッチ間のベクター (距離) の大きさをもとめます
		float prevTouchDeltaMag = (t1PrevPos - t2PrevPos).magnitude;
		float touchDeltaMag = (t1.position - t2.position).magnitude;

		// 各フレーム間の距離の差をもとめます
		float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

		//差に基づいて拡大率を変更
		ChangeViewMagnification(viewMagnification - deltaMagnitudeDiff * 0.001f);
	}

	#endregion

	#region PenTool

	void Paint(EditModePiece piece) {
		if(!piece) return;

		editMap[(int)piece.chipPosition.y, (int)piece.chipPosition.x] = selectChipID;

		//一個だけなのでDraw()は呼ばない
		piece.image.sprite = GetMapchipFromID(selectChipID);
	}

	#endregion

	//改善中
	#region FillTool

	/// <summary>
	/// 2つのVector2をRectに成形する。
	/// </summary>
	/// <param name="vec1"></param>
	/// <param name="vec2"></param>
	/// <returns></returns>
	Rect Vec2Rect(Vector2 vec1, Vector2 vec2) {

		Rect rect = new Rect();

		rect.x = vec1.x < vec2.x ? vec1.x : vec2.x;
		rect.y = vec1.y < vec2.y ? vec1.y : vec2.y;
		Vector2 diff = vec1 - vec2;
		rect.size = new Vector2(Mathf.Abs(diff.x), Mathf.Abs(diff.y));

		return rect;
	}

	/// <summary>
	/// Fillツールでの最初の点を決める。
	/// ツールの範囲表示も行う。
	/// </summary>
	/// <param name="startPos">最初の点</param>
	void FillToolStart(Vector2 startPos) {

		if(!GetEditModePiceFromRaycast(startPos)) return;

		rectToolMouseStartPos = startPos;

		rectImage = Instantiate(rectImagePre);
		rectImage.rectTransform.SetParent(canvas.transform);

		//zに値が入ってしまっているので初期化する
		rectImage.rectTransform.position = new Vector3();
		rectImage.rectTransform.localScale = new Vector3(1, 1, 1);
	}

	/// <summary>
	/// Fillツールの範囲表示の更新
	/// </summary>
	void FillToolUpdate() {

		if(!rectImage) return;

		//塗りつぶせないときは更新しない
		if(!GetEditModePiceFromRaycast(Input.mousePosition)) return;

		rectToolMouseEndPos = Input.mousePosition;

		//ウィンドウサイズによってずれないように調整
		Rect rect = Vec2Rect(rectToolMouseStartPos / canvas.scaleFactor, rectToolMouseEndPos / canvas.scaleFactor);

		rectImage.rectTransform.anchoredPosition = rect.position;
		rectImage.rectTransform.sizeDelta = rect.size;
	}

	/// <summary>
	/// Fillツールでの最後の点を決める。
	/// </summary>
	/// <param name="endPos">最後の点</param>
	void FillToolEnd(Vector2 endPos) {

		if(rectImage) Destroy(rectImage.gameObject);

		//範囲からPieceの座標を調べる
		Rect rect = Vec2Rect(rectToolMouseStartPos, endPos);

		Vector2? startPiecePos = null;
		Vector2? endPiecePos = null;

		var startPosPiece = GetEditModePiceFromRaycast(rect.position);
		if(startPosPiece) startPiecePos = startPosPiece.chipPosition;

		var endPosPiece = GetEditModePiceFromRaycast(rect.max);
		if(endPosPiece) endPiecePos = endPosPiece.chipPosition;

		if(startPiecePos != null && endPiecePos != null)
			FillRect(Vec2Rect((Vector2)startPiecePos, (Vector2)endPiecePos));
	}

	/// <summary>
	/// 指定した範囲を塗る。
	/// </summary>
	/// <param name="rect">塗る範囲</param>
	void FillRect(Rect rect) {

		Debug.Log(rect);

		rect.size += new Vector2(1, 1);
		for(int i = (int)rect.y;i < rect.yMax;i++) {
			for(int j = (int)rect.x;j < rect.xMax;j++) {
				editMap[i, j] = selectChipID;
			}
		}

		Draw();
	}

	#endregion

	/// <summary>
	/// スクリーン座標からRayで取得する
	/// </summary>
	/// <param name="screenPos"></param>
	/// <returns></returns>
	EditModePiece GetEditModePiceFromRaycast(Vector2 screenPos) {

		var pointerData = new PointerEventData(EventSystem.current);
		pointerData.position = screenPos;

		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerData, results);
		foreach(var hit in results) {
			var p = hit.gameObject.GetComponent<EditModePiece>();
			if(p) return p;
		}

		return null;
	}

	void ChangeViewMagnification(float mag) {
		float maxMagnification = 2.0f;
		float minMagnification = 0.5f;

		mag = Mathf.Clamp(mag, minMagnification, maxMagnification);

		viewMagnification = mag;
		viewMagnificationSlider.value = mag;

		SetViewSize();
		Draw();
	}

	/// <summary>
	/// 現在の編集データをStageDataに成形
	/// </summary>
	/// <returns></returns>
	public StageData GenerateStageData() {
		return new StageData(limitTime, editMap);
	}

	#region MenuArea

	public void Preview() {

		if(isLoading) return;
		isLoading = true;

		StartCoroutine(PreviewLoad());
	}

	IEnumerator PreviewLoad() {

		GameManager.IsEditMode = true;

		//ロード待ち
		yield return SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);

		isLoading = false;

		SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameScene"));

		//プレビューに移行
		sceneRoot.SetActive(false);
	}

	public IEnumerator UnloadPreview() {

		GameManager.IsEditMode = false;

		//プレビュー開放
		yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("GameScene"));

		SceneManager.SetActiveScene(SceneManager.GetSceneByName("EditScene"));

		//移行していた内容を元に戻す
		sceneRoot.SetActive(true);
	}

	public IEnumerator RetryPreview() {

		//プレビュー開放
		SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("GameScene"));
		//ロード待ち
		yield return SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);

		SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameScene"));

	}

	public void ViewMagnificationSliderChange() {
		ChangeViewMagnification(viewMagnificationSlider.value);
	}

	#endregion

	#region FileArea

	public void NewFile() {

		//配列の初期化
		editMap = new int[MAP_SIZE_Y, MAP_SIZE_X];

		Draw();
	}

	public void OpenFile() {
		string path;

		if(Application.isEditor) {

		}
	}

	public void SaveFile() {

		EditFileIO.SaveFile(saveFileName.text, GenerateStageData());
		//_debugText.text = "saveComp.";

		//エディットモードに戻る
		ChangeSaveFileWindowActive(false);
		ChangeFileWindowActive(false);
	}

	public void ExitEditMode() {
		SumCanvasAnimation.MoveScene("StageSelectScene");
	}

	public void ChangeFileWindowActive(bool enable) {

		canUseTool = !enable;
		fileView.gameObject.SetActive(enable);
	}

	#endregion

	#region SaveFileArea

	public void ChangeSaveFileWindowActive(bool enable) {

		saveFileView.gameObject.SetActive(enable);
	}

	#endregion


	#region ViewArea

	void SetViewSize() {

		float chipSize = VIEW_CHIP_SIZE * viewMagnification;
		float width = MAP_SIZE_X * chipSize;
		float height = MAP_SIZE_Y * chipSize;

		//表示サイズ設定
		viewMapArea.sizeDelta = new Vector2(width, height);
		viewMapArea.anchoredPosition = new Vector2();

		//チップの位置サイズを設定
		for(int i = 0;i < MAP_SIZE_Y;i++) {
			for(int j = 0;j < MAP_SIZE_X;j++) {

				Image chip = generateViewImage[i, j];

				chip.rectTransform.anchoredPosition =
					new Vector2(j, -i) * chipSize;

				chip.rectTransform.sizeDelta =
					new Vector2(chipSize, chipSize);

				generateViewImage[i, j] = chip;
			}
		}
	}

	void Draw() {

		for(int i = 0;i < MAP_SIZE_Y;i++) {
			for(int j = 0;j < MAP_SIZE_X;j++) {

				generateViewImage[i, j].sprite = 
					GetMapchipFromID(editMap[i, j]);
			}
		}
	}

	#endregion

	#region EditToolArea

	void ChangeEditTool(EditToolMode newMode) {

		//対象のツール画像にアクティブを移動
		switch(newMode) {
			case EditToolMode.Hand:
				toolSelectImage.anchoredPosition = toolHandImage.anchoredPosition;

				break;
			case EditToolMode.Pen:
				toolSelectImage.anchoredPosition = toolPenImage.anchoredPosition;

				break;
			case EditToolMode.FillRect:
				toolSelectImage.anchoredPosition = toolFillRectImage.anchoredPosition;

				break;
			default:
				break;
		}

		//ハンドツール時はクリックでスクロールができる
		bool canScroll = newMode == EditToolMode.Hand;

		viewScroll.horizontal = canScroll;
		viewScroll.vertical   = canScroll;
		foreach(var item in generateViewImage) {
			item.gameObject.GetComponent<EventTrigger>().enabled = !canScroll;
		}

		//モードを変更
		currentTool = newMode;

	}

	#endregion

	#region SelectChipArea

	public void ChangeSelectChip(int id) {

		//対象のチップ画像にアクティブを移動
		chipSelectImage.anchoredPosition = 
			selectableChip[id].GetComponent<RectTransform>().anchoredPosition;

		//マップ配置用のIDに変換
		selectChipID = ConvertSelectChipID(id);

		//画像更新
		currentChip.sprite = GetMapchipFromID(selectChipID);
	}

	Sprite GetMapchipFromID(int id) {

		if(id < 0) return null;
		if(id < 7) return mapchipSprite[id];
		if(id == 14) return holeSprite;

		return null;


	}

	int ConvertSelectChipID(int id) {

		switch(id) {
			//穴の画像
			case 7:
				return 14;
			default:
				break;
		}

		return id;
	}

	public void ChangeMapchipWindowActive(bool enable) {

		canUseTool = !enable;
		mapchipView.gameObject.SetActive(enable);
	}

	#endregion

	#region SetTimeArea

	public void ApplyTime() {

		int inTime;
		if(int.TryParse(inputTimeField.text, out inTime)) {

			limitTime = inTime;
			viewTime.text = limitTime.ToString() + "sec";

			ChangeSetTimeWindowActive(false);
		}
	}

	public void ChangeSetTimeWindowActive(bool enable) {

		canUseTool = !enable;
		setTimeView.gameObject.SetActive(enable);
	}

	#endregion

	public void DebugButton() {


#if UNITY_ANDROID
		using(AndroidJavaClass environment = new AndroidJavaClass("android.os.Environment"))
		using(AndroidJavaObject exDir = environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory")) {

			//_debugText.text = exDir.Call<string>("toString");
		}
#endif

		//_debugText.text = Environment.GetFolderPath(Environment.SpecialFolder.pu();
	}
}
