using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
	public int speed = 5;
	public int rotationSpeed = 60;

	// Use this for initialization
	void Start () {
	   this.gameObject.AddComponent<BoxCollider>();
       Rigidbody body = this.gameObject.AddComponent<Rigidbody>();
       body.useGravity = false;
       body.drag = Mathf.Infinity;
       body.angularDrag = Mathf.Infinity;
	}
	
	// Update is called once per frame
    void Update() {
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