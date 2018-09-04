using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
	public int speed = 5;
	public int rotationSpeed = 60;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
    void Update() {
        float impulse = Input.GetAxis("Vertical") * speed;
        float slide = Input.GetAxis("Horizontal") * speed;
        float yaw = Input.GetAxis("Mouse X") * rotationSpeed;
        float pitch = Input.GetAxis("Mouse Y") * rotationSpeed;

        impulse *= Time.deltaTime;
        slide *= Time.deltaTime;
        yaw *= Time.deltaTime;
        pitch *= Time.deltaTime;

        transform.Translate(0, 0, impulse);
        transform.Translate(slide, 0, 0);
        transform.Rotate(0, yaw, 0);
        transform.Rotate(-pitch, 0, 0);
    }
}
