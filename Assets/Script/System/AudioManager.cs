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

	static AudioMixer mixer;			//ミキサー
	static AudioClip[] SEclips;			//再生用リスト
	static AudioClip[] BGMclips;		//再生用リスト
	static AudioSource nowPlayingBGM;	//現在再生されているBGM

	static AudioManager myManager;      //自分のリファレンス

	public AudioMixer _mixer;			//設定用のミキサー
	public AudioClip[] _SEclips;		//SE設定用リスト
	public AudioClip[] _BGMclips;		//BGM設定用リスト

	void Awake() {
		myManager = this;
		mixer = _mixer;
		SEclips = _SEclips;
		BGMclips = _BGMclips;
	}
	
	// Update is called once per frame
	void Update () {
		
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
		src.PlayOneShot(BGMclips[(int)type], vol);
		src.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];

		nowPlayingBGM = src;

		if(isLoop) {
			src.loop = true;
		}
		else {
			Destroy(src.gameObject, BGMclips[(int)type].length + 0.1f);
		}
	}
}
