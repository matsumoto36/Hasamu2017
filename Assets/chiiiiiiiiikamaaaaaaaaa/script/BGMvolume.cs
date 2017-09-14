using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGMvolume : MonoBehaviour {
	
	[SerializeField]
	UnityEngine.Audio.AudioMixer mixer;

	public Animator tentacleAnim;
	public Image tentacleBlock;
	public Image stopBlock;

	float animTime = 0;
	float prevValue;

	void Start()
	{
		float bgmVolume;
		mixer.GetFloat("BGMVolume", out bgmVolume);
		GetComponent<Slider>().value = prevValue = bgmVolume;
		tentacleAnim.speed = 0;

		tentacleBlock.sprite = ResourceLoader.GetChips(R_MapChipType.MainChip)[1];
		stopBlock.sprite = ResourceLoader.GetChips(R_MapChipType.MainChip)[2];
	}

	public float BGMVolume
	{
		set {
			mixer.SetFloat("BGMVolume", value);

			float delta = value - prevValue;
			tentacleAnim.speed = Mathf.Abs(delta * 2);
			animTime = 0.1f;

			prevValue = value;
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
