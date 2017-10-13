using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAspector : MonoBehaviour {

	const float WIDTH = 1920;
	const float HEIGHT = 1080;
	const float PPU = 100;

	Camera cam;

	// Use this for initialization
	void Awake () {
		cam = Camera.main;
		cam.orthographicSize = HEIGHT / 2.0f / PPU;

		float aspect = (float)Screen.height / Screen.width;
		float bgAcpect = HEIGHT / WIDTH;

		if(bgAcpect > aspect) {

			float bgScale = HEIGHT / Screen.height;
			float camWidth = WIDTH / (Screen.width * bgScale);
			cam.rect = new Rect((1 - camWidth) / 2, 0, camWidth, 1);
		}
		else {

			float bgScale = WIDTH / Screen.width;
			float camHeight = HEIGHT / (Screen.height * bgScale);
			cam.rect = new Rect(0, (1 - camHeight) / 2, 1, camHeight);
		}


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
