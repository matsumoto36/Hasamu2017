using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseTouch : MonoBehaviour {

	public Transform fulcrum;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		fulcrum.position += move * 5 * Time.deltaTime;

		Vector3 def = Camera.main.ScreenToWorldPoint(Input.mousePosition) - fulcrum.position;
		Vector3 pos = fulcrum.position - def;
		pos.z = 0f;
		transform.position = pos;

	}
}
