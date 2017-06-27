using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGMvolume : MonoBehaviour {
	
	[SerializeField]
	UnityEngine.Audio.AudioMixer mixer;

	void Start()
	{
		float bgmVolume;
		mixer.GetFloat("BGMVolume", out bgmVolume);
		GetComponent<Slider>().value = bgmVolume;
			}

	public float BGMVolume
	{
		set {
			Debug.Log(value);
			mixer.SetFloat("BGMVolume", value);
		}
		
	}
		
	}
