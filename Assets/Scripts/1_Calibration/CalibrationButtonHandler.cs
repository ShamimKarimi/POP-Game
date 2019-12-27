using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CalibrationButtonHandler : MonoBehaviour
{

    public void NextScene()
    {
        GameObject udpGameObject = GameObject.Find("UDP");
        UDP.UDPReceiver udpReceiver = (UDP.UDPReceiver)udpGameObject.GetComponent(typeof(UDP.UDPReceiver));
        udpReceiver.CloseSocket();
        SceneManager.LoadScene("2_BrickWall");
    }
}
