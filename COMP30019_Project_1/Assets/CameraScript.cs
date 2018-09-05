using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
	public int speed = 5;
	public int rotationSpeed = 60;
    int size;
    GameObject referenceObject;
    PlaneScript referenceScript;

	void Start () {
        this.gameObject.AddComponent<BoxCollider>();
        Rigidbody body = this.gameObject.AddComponent<Rigidbody>();
        body.useGravity = false;
        body.drag = Mathf.Infinity;
        body.angularDrag = Mathf.Infinity;

        referenceObject = GameObject.Find("Plane");
        referenceScript = referenceObject.GetComponent<PlaneScript>();
        this.size = referenceScript.getLimit();
	}
	
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


        // constrain boundaries
        if (transform.position.x < 0)
        {
            transform.position = new Vector3(0.0f, transform.position.y, transform.position.z);
        } 
        if (transform.position.x > size) 
        {
            transform.position = new Vector3(size, transform.position.y, transform.position.z);
        }

        if (transform.position.z < 0.0f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f);
        }
        if (transform.position.z > size) 
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, size);
        }

    }
}