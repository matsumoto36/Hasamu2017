using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SEVolume : MonoBehaviour {

	[SerializeField]
	UnityEngine.Audio.AudioMixer mixer;
	void Start()
	{
		float seVolume;
		mixer.GetFloat("SEVolume", out seVolume);
		GetComponent<Slider>().value = seVolume;
	}
	public float SEvolume
	{
		set {
			mixer.SetFloat("SEVolume",value);
		}
	}
}
