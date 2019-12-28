using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class webcam : MonoBehaviour
{
    public RawImage background;
    
    void Start()
    {

        WebCamDevice[] devices = WebCamTexture.devices;

        // setting default camera
        WebCamTexture camera = new WebCamTexture(devices[0].name);

        for (int i = 0; i < devices.Length; i++)
        {
            // setting Many Webcam as camera if found
            if (devices[i].name == "ManyCam Virtual Webcam")
            {
                camera = new WebCamTexture(devices[i].name);
            }
        }
        
        camera.Play();
        background.texture = camera;
        
    }
}
