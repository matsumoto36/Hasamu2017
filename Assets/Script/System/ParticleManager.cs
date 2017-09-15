using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParticleType {

	TentacleSpawn,
	TentacleSpawnEdge,
	TentacleStay,
	HotBlock,
	ColdBlock,
	ChangeBlockTemp,
	ContainerCreate,
	ContainerMove,
	AmbientEffect,
	BombBlast,
	BombDestrtoy,
}

/// <summary>
/// パーティクルを管理するクラス
/// </summary>
public class ParticleManager : MonoBehaviour {

	static ParticleManager myManager;		//自分
	static ParticleSystem[] particleArray;	//パーティクル本体

	/// <summary>
	/// 一回生成
	/// </summary>
	static ParticleManager() {
		new GameObject("[ParticleManager]").AddComponent<ParticleManager>();
	}

	void Awake() {
		//シングルトン
		if(!myManager) {
			myManager = this;
			DontDestroyOnLoad(gameObject);

			Initialize();
		}
		else {
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// 初期設定
	/// </summary>
	void Initialize() {

		//パーティクルの読み込み
		particleArray = new ParticleSystem[System.Enum.GetNames(typeof(ParticleType)).Length];
		for (int i = 0; i < particleArray.Length; i++) {
			//enumで定義された名前と同じものを読み込む
			particleArray[i] = Resources.Load<ParticleSystem>("Particles/" + System.Enum.GetName(typeof(ParticleType), i));
			Debug.Log("Particles/" + System.Enum.GetName(typeof(ParticleType), i));
		}

		Debug.Log("Init_Complete");
	}

	/// <summary>
	/// パーティクルを再生する
	/// </summary>
	/// <param name="type">パーティクルの種類</param>
	/// <param name="position">再生する位置</param>
	/// <param name="rotation">回転</param>
	/// <param name="playTime">再生する時間</param>
	public static void PlayOneShot(ParticleType type, Vector2 position, Quaternion rotation, float playTime) {

		//生成・位置合わせ
		var ps = Instantiate(particleArray[(int)type]);
		ps.name = "[Particle - " + type.ToString() + " ]";
		ps.transform.position = position;
		ps.transform.rotation = rotation;

		ps.Play();

		//削除を予約
		Destroy(ps.gameObject, playTime);
	}

	/// <summary>
	/// パーティクルを再生する
	/// </summary>
	/// <param name="type">パーティクルの種類</param>
	/// <param name="position">再生する位置</param>
	/// <param name="rotation">回転</param>
	/// <returns>再生しているパーティクル</returns>
	public static ParticleSystem Play(ParticleType type, Vector2 position, Quaternion rotation) {

		//生成・位置合わせ
		var ps = Instantiate(particleArray[(int)type]);
		ps.name = "[Particle - " + type.ToString() + " - Editable]";
		ps.transform.position = position;
		ps.transform.rotation = rotation;

		ps.Play();

		//返却
		return ps;
	}

	/// <summary>
	/// パーティクルを違和感なく削除
	/// </summary>
	/// <param name="ps">対象</param>
	public static void ParticleDestroy(ParticleSystem ps) {
		ps.Stop(true);
		Destroy(ps.gameObject, 5.0f);
	}
}
