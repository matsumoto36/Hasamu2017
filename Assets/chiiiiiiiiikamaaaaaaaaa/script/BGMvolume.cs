using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGMvolume : MonoBehaviour {
	
	[SerializeField]
	UnityEngine.Audio.AudioMixer mixer;

	public Animator tentacleAnim;

	float animTime = 0;

	void Start()
	{
		float bgmVolume;
		mixer.GetFloat("BGMVolume", out bgmVolume);
		GetComponent<Slider>().value = bgmVolume;
		tentacleAnim.speed = 0;

	}

	public float BGMVolume
	{
		set {
			Debug.Log(value);
			mixer.SetFloat("BGMVolume", value);
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
