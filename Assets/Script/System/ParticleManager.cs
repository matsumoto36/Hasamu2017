using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum R_ParticleType {
	TentacleSpawn,
	TentacleSpawnEdge,
	TentacleStay,
	HotBlock,
	ColdBlock,
	CreateContainer,
	ContainerEdge,
	StageEffect,
	BombBlast,
	BombDestrtoy,

}

public class ParticleManager : MonoBehaviour {

	static ParticleManager myManager;
	static GameObject[] particleArray;
	public GameObject[] particleSrc;

	void Awake() {
		//シングルトン
		if(!myManager) {
			myManager = this;
			transform.SetParent(null);
			DontDestroyOnLoad(gameObject);

			particleArray = particleSrc;
		}
		else {
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// パーティクルを再生する
	/// </summary>
	/// <param name="type">パーティクルの種類</param>
	/// <param name="position">再生する位置</param>
	/// <param name="rotation">回転</param>
	/// <param name="playTime">再生する時間</param>
	public static void PlayParticle(R_ParticleType type, Vector2 position, Quaternion rotation, float playTime) {

		GameObject g = Instantiate(particleArray[(int)type]);
		g.transform.position = position;
		g.transform.rotation = rotation;

		var p = g.GetComponent<ParticleSystem>();
		float time = playTime;
	}
}
