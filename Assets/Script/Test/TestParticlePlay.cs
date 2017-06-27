using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestParticlePlay : MonoBehaviour {

	public ParticleType type;	//再生するパーティクルのタイプ

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.E)) TestPlay();
		if (Input.GetKeyDown(KeyCode.R)) TestPlay2();
	}

	void TestPlay() {
		ParticleManager.PlayOneShot(type, new Vector2(), Quaternion.identity, 5);
	}

	void TestPlay2() {
		ParticleManager.Play(type, new Vector2(), Quaternion.identity);
	}
}
