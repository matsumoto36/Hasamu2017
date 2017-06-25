using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// BGMのリストネーム。
/// 以下は例
/// </summary>
public enum BGMType {
	Title,
    Select,
	Game,
	Game2,
	Over,
	Clear,
}

/// <summary>
/// SEのリストネーム。
/// 以下は例
/// </summary>
public enum SEType {
	TentacleSpawn,
	TentacleMove,
	TentacleReturn,
	BombTimer,
	BombExplosion,
	HasamuNormal,
	Hot,
	Cold,
    Hole,
    Button,
}

/// <summary>
/// 音の管理をする
/// </summary>
public class AudioManager : MonoBehaviour {

	static AudioManager myManager;									//自分のリファレンス

	static AudioMixerGroup[] mixerGroups = new AudioMixerGroup[2];	//ミキサーのグループ [0]SE [1]BGM
	static AudioClip[] SEclips;										//再生用リスト
	static AudioClip[] BGMclips;									//再生用リスト
	static AudioSource nowPlayingBGM;								//現在再生されているBGM
	static BGMType latestPlayBGMType = BGMType.Title;               //再生されているBGMの種類

	static Coroutine fadeInCol;                                     //フェードインのコルーチン
	static AudioSource fadeInAudio;

	/// <summary>
	/// 一回生成
	/// </summary>
	static AudioManager() {
		new GameObject("[AudioManager]").AddComponent<AudioManager>();
	}

	void Awake() {

		//シングルトン
		if(!myManager) {
			myManager = this;
			DontDestroyOnLoad(gameObject);

			Initiarize();
		}
		else {
			Destroy(gameObject);
		}

	}

	/// <summary>
	/// 初期設定
	/// </summary>
	public void Initiarize() {

		//LoadMixer
		var mixer = Resources.Load<AudioMixer>("Sounds/NewAudioMixer");
		mixerGroups[0] = mixer.FindMatchingGroups("SE")[0];
		mixerGroups[1] = mixer.FindMatchingGroups("BGM")[0];


        //BGM読み込み
		BGMclips = new AudioClip[System.Enum.GetNames(typeof(BGMType)).Length];
		for (int i = 0; i < BGMclips.Length; i++) {
			//enumで定義された名前と同じものを読み込む
			BGMclips[i] = Resources.Load<AudioClip>("Sounds/BGM/" + System.Enum.GetName(typeof(BGMType), i));
		}

        //SE読み込み
        SEclips = new AudioClip[System.Enum.GetNames(typeof(SEType)).Length];
		for (int i = 0; i < SEclips.Length; i++) {
			//enumで定義された名前と同じものを読み込む
			SEclips[i] = Resources.Load<AudioClip>("Sounds/SE/" + System.Enum.GetName(typeof(SEType), i));
		}

	}

	/// <summary>
	/// SEを再生する
	/// </summary>
	/// <param name="type">SEの内容</param>
	/// <param name="vol">音量</param>
	public static void Play(SEType type, float vol) {

		AudioSource src = new GameObject("[Audio SE - " + type.ToString() + "]" ).AddComponent<AudioSource>();
		src.transform.SetParent(myManager.transform);
		src.clip = SEclips[(int)type];
		src.volume = vol;
		src.outputAudioMixerGroup = mixerGroups[0];
		src.Play();
        
		Destroy(src.gameObject, SEclips[(int)type].length + 0.1f);
	}

	/// <summary>
	/// SEを再生するが、編集可能
	/// </summary>
	/// <param name="type">SEの内容</param>
	/// <returns>再生されているSE</returns>
	public static AudioSource Play(SEType type) {

		AudioSource src = new GameObject("[Audio SE - " + type.ToString() + " - Editable]").AddComponent<AudioSource>();
		src.transform.SetParent(myManager.transform);
		src.clip = SEclips[(int)type];
		src.outputAudioMixerGroup = mixerGroups[0];
		src.Play();

		return src;
	}

	/// <summary>
	/// BGMを再生する
	/// </summary>
	/// <param name="type">BGMの内容</param>
	/// <param name="vol">音量</param>
	/// <param name="isLoop">ループ再生するか</param>
	public static void Play(BGMType type, float vol, bool isLoop) {

		if(nowPlayingBGM) Destroy(nowPlayingBGM.gameObject);

		AudioSource src = new GameObject("[Audio BGM - " + type.ToString() + "]").AddComponent<AudioSource>();
		src.transform.SetParent(myManager.transform);
		src.clip = BGMclips[(int)type];
		src.volume = vol;
		src.outputAudioMixerGroup = mixerGroups[1];
		src.Play();

		if(isLoop) {
			src.loop = true;
		}
		else {
			Destroy(src.gameObject, BGMclips[(int)type].length + 0.1f);
		}

		nowPlayingBGM = src;
		latestPlayBGMType = type;
	}

	/// <summary>
	/// BGMをフェードインさせる
	/// </summary>
	/// <param name="fadeTime">フェードする時間</param>
	/// <param name="type">新しいBGMのタイプ</param>
	/// <param name="vol">新しいBGMの大きさ</param>
	/// <param name="isLoop">新しいBGMがループするか</param>
	public static void FadeIn(float fadeTime, BGMType type, float vol, bool isLoop) {
		fadeInCol = myManager.StartCoroutine(FadeInAnim(fadeTime, type, vol, isLoop));
	}
	static IEnumerator FadeInAnim(float fadeTime, BGMType type, float vol, bool isLoop) {

		//初期設定
		fadeInAudio = new GameObject("[Audio BGM - " + type.ToString() + " - FadeIn ]").AddComponent<AudioSource>();
		fadeInAudio.transform.SetParent(myManager.transform);
		fadeInAudio.clip = BGMclips[(int)type];
		fadeInAudio.volume = 0;
		fadeInAudio.outputAudioMixerGroup = mixerGroups[1];
		fadeInAudio.Play();

		//フェードイン
		float t = 0;
		while(t < 1.0f) {
			t += Time.deltaTime / fadeTime;
			fadeInAudio.volume = t * vol;
			yield return null;
		}

		fadeInAudio.volume = vol;
		fadeInAudio.name = "[Audio BGM - " + type.ToString() + "]";

		if(nowPlayingBGM) Destroy(nowPlayingBGM.gameObject);

		if(isLoop) {
			fadeInAudio.loop = true;
		}
		else {
			Destroy(fadeInAudio.gameObject, BGMclips[(int)type].length + 0.1f);
		}

		nowPlayingBGM = fadeInAudio;
	}

	/// <summary>
	/// BGMをフェードアウトさせる
	/// </summary>
	/// <param name="fadeTime">フェードする時間</param>
	public static void FadeOut(float fadeTime) {
		myManager.StartCoroutine(FadeOutAnim(fadeTime));
	}
	static IEnumerator FadeOutAnim(float fadeTime) {

		//初期設定
		AudioSource src = nowPlayingBGM;

		//フェードイン中にフェードアウトが呼ばれた場合
		if (!src) {
			//フェードイン処理停止
			myManager.StopCoroutine(fadeInCol);
			src = fadeInAudio;
		}

		src.name = "[Audio BGM - " + latestPlayBGMType.ToString() + " - FadeOut ]";
		nowPlayingBGM = null;

		//フェードアウト
		float t = 0;
		float vol = src.volume;
		while(t < 1.0f) {
			t += Time.deltaTime / fadeTime;
			src.volume = (1 - t) * vol;
			yield return null;
		}

		Destroy(src.gameObject);
	}
}
