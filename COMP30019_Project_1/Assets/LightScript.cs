using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour {

	Vector4 lightColor = new Vector4(255, 255, 255, 10);

	// Use this for initialization
	void Start () {
		Shader.SetGlobalVector("_LightColor", lightColor);
	}
	
	// Update is called once per frame
	void Update () {
		// update location and give to shaders
		Shader.SetGlobalVector("_LightPosition", transform.position);
	}
}
