using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    float timeCounter;
    float speed;
    float width;
    float height;
    public Vector4 lightColor = new Vector4(255, 255, 255, 10);

    // Use this for initialization
    void Start()
    {
        speed = 0.25f;
        width = 64;
        height = 96;
        timeCounter = 0;
        transform.position = new Vector3(0.0f, 0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector4 lightLoc = new Vector4(transform.position.x, transform.position.y, transform.position.z, 1.0f);
        Shader.SetGlobalVector("_LightColor", lightColor);
        // update location and give to shaders
        Shader.SetGlobalVector("_LightPosition", lightLoc);

        timeCounter += Time.deltaTime * speed;

        float x = 64 + Mathf.Cos(timeCounter) * width;
        float y = Mathf.Sin(timeCounter) * height;
        float z = x;

        transform.position = new Vector3(x, y, z);
     

    }
}
