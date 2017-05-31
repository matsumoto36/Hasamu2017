using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEVolume : MonoBehaviour {

	[SerializeField]
	UnityEngine.Audio.AudioMixer mixer;
	public float SEvolume
	{
		set {
			mixer.SetFloat("SEVolume",value);
		}
	}
}
