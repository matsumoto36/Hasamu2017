using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	static AudioClip[] clips;		//再生用リスト
	static AudioManager myManager;	//自分のリファレンス
	public AudioClip[] _clips;		//設定用リスト

	void Awake() {
		myManager = this;
		clips = _clips;
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

		AudioSource src = new GameObject("[Audio SE]").AddComponent<AudioSource>();
		src.transform.SetParent(myManager.transform);
		src.PlayOneShot(clips[(int)type], vol);
		Destroy(src.gameObject, clips[(int)type].length + 0.1f);
	}
}
