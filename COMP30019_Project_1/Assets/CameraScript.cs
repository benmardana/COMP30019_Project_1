using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
	public int speed = 5;
	public int rotationSpeed = 60;

	// Use this for initialization
	void Start () {
		// hides cursor - ESC to show again
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
    void Update() {
    	Debug.Log(yaw);
        float impulse = Input.GetAxis("Vertical") * speed;
        float slide = Input.GetAxis("Horizontal") * speed;
        float yaw = Input.GetAxis("Mouse X") * rotationSpeed;
        float pitch = Input.GetAxis("Mouse Y") * rotationSpeed;
        float rotate = 0;

        if (Input.GetKey("q"))
        {
        	rotate = rotationSpeed;
        }
        if (Input.GetKey("e"))
        {
        	rotate = -rotationSpeed;
        }

        impulse *= Time.deltaTime;
        slide *= Time.deltaTime;
        yaw *= Time.deltaTime;
        pitch *= Time.deltaTime;
        rotate *= Time.deltaTime;

        transform.Translate(0, 0, impulse);
        transform.Translate(slide, 0, 0);
        transform.Rotate(0, yaw, 0);
        transform.Rotate(pitch, 0, 0);
        transform.Rotate(0, 0, rotate);
    }
}

        // float impulse = Input.GetAxis("Vertical") * speed;
        // float slide = Input.GetAxis("Horizontal") * speed;
        // yaw += Input.GetAxis("Mouse X");
        // pitch += Input.GetAxis("Mouse Y");
        // float rotate = 0;

        // if (Input.GetKey("q"))
        // {
        // 	rotate = rotationSpeed;
        // }
        // if (Input.GetKey("e"))
        // {
        // 	rotate = -rotationSpeed;
        // }

        // impulse *= Time.deltaTime;
        // slide *= Time.deltaTime;
        // yaw = Mathf.Clamp((yaw * Mathf.Max(Time.deltaTime, 1.0f)), -0.5f, 0.5f);
        // pitch = Mathf.Clamp((pitch * Mathf.Max(Time.deltaTime, 1.0f)), -0.5f, 0.5f);
        // rotate *= Time.deltaTime;