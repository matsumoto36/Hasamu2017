using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMvolume : MonoBehaviour {

	[SerializeField]
	UnityEngine.Audio.AudioMixer mixer;
	public float BGMVolume
	{
		set {
			Debug.Log(value);
			mixer.SetFloat("BGMVolume", value);
		}
	}
		
	}
