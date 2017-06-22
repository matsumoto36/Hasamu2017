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
	Main,
	Clear
}

/// <summary>
/// SEのリストネーム。
/// 以下は例
/// </summary>
public enum SEType {
	Tap,
	Button,
	Stat
}

/// <summary>
/// 音の管理をする
/// </summary>
public class AudioManager : MonoBehaviour {

	static AudioManager myManager;      //自分のリファレンス

	static AudioMixer mixer;			//ミキサー
	static AudioClip[] SEclips;			//再生用リスト
	static AudioClip[] BGMclips;		//再生用リスト
	static AudioSource nowPlayingBGM;   //現在再生されているBGM
	static BGMType latestPlayBGMType;	//再生されているBGMの種類

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
		mixer = Resources.Load<AudioMixer>("Sounds/NewAudioMixer");

		#region LoadBGM
		BGMclips = new AudioClip[System.Enum.GetNames(typeof(BGMType)).Length];
		BGMclips[0] = Resources.Load<AudioClip>("Sounds/BGM/retrogamecenter");
		#endregion

		#region LoadSE
		SEclips = new AudioClip[System.Enum.GetNames(typeof(SEType)).Length];
		SEclips[0] = Resources.Load<AudioClip>("Sounds/SE/button35");
		#endregion
	}

	/// <summary>
	/// SEを再生する
	/// </summary>
	/// <param name="type">SEの内容</param>
	/// <param name="vol">音量</param>
	public static void Play(SEType type, float vol) {

		AudioSource src = new GameObject("[Audio SE - " + type.ToString() + "]" ).AddComponent<AudioSource>();
		src.transform.SetParent(myManager.transform);
		src.PlayOneShot(SEclips[(int)type], vol);
		src.outputAudioMixerGroup = mixer.FindMatchingGroups("SE")[0];

		Destroy(src.gameObject, SEclips[(int)type].length + 0.1f);
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
		src.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
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
		myManager.StartCoroutine(FadeInAnim(fadeTime, type, vol, isLoop));
	}
	static IEnumerator FadeInAnim(float fadeTime, BGMType type, float vol, bool isLoop) {

		//初期設定
		AudioSource src = new GameObject("[Audio BGM - " + type.ToString() + " - FadeIn ]").AddComponent<AudioSource>();
		src.transform.SetParent(myManager.transform);
		src.clip = BGMclips[(int)type];
		src.volume = 0;
		src.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
		src.Play();

		//フェードイン
		float t = 0;
		while(t < 1.0f) {
			t += Time.deltaTime / fadeTime;
			src.volume = t * vol;
			yield return null;
		}

		src.volume = vol;
		src.name = "[Audio BGM - " + type.ToString() + "]";

		if(nowPlayingBGM) Destroy(nowPlayingBGM.gameObject);

		if(isLoop) {
			src.loop = true;
		}
		else {
			Destroy(src.gameObject, BGMclips[(int)type].length + 0.1f);
		}

		nowPlayingBGM = src;
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

		yield return null;
	}
}
