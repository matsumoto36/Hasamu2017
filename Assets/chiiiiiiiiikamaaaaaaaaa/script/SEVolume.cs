using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SEVolume : MonoBehaviour {

	[SerializeField]
	UnityEngine.Audio.AudioMixer mixer;

	public Animator tentacleAnim;

	float animTime = 0;

	void Start()
	{
		float seVolume;
		mixer.GetFloat("SEVolume", out seVolume);
		GetComponent<Slider>().value = seVolume;
		tentacleAnim.speed = 0;

	}
	public float SEvolume
	{
		set {
			mixer.SetFloat("SEVolume",value);

			tentacleAnim.speed = 1;
			animTime = 0.1f;
		}
	}

	void Update() {

		if(animTime > 0) {
			animTime -= Time.deltaTime;
			if(animTime <= 0) {
				animTime = 0;
				tentacleAnim.speed = 0;
			}

		}
	}
}
