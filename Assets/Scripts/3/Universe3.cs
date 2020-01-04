using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Universe3 : MonoBehaviour
{

    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    public GameObject green_balloon;
    public GameObject red_balloon;
    public GameObject blue_balloon;

    public GameObject green;
    public GameObject red;
    public GameObject blue;

    public AudioClip pop;
    public AudioClip blop;
    AudioSource audioSource;

    GameObject canvas;

    // UI
    GameObject nextSceneButton;

    // Array of balloons
    GameObject[] balloons;

    // DR, DL, UR, UL, SR, SL
    float[] balloonsX = { 2f, -2.2f, 2f, -2.2f, 6.2f, -6.3f };
    float[] balloonsY = { -3.5f, -3.5f, 1.2f, 1.2f, -1.2f, -1.2f };

    float[] balloonsInstantiationTime;

    GameObject colorIndicator;

    int numberOfBalloonsOnScreen = 0;
    int numberOfBalloonsInTotal = 0;


    // Start is called before the first frame update
    void Start()
    {

        balloons = new GameObject[6];
        balloonsInstantiationTime = new float[6];

        audioSource = GetComponent<AudioSource>();

        canvas = GameObject.Find("Canvas");

        //nextSceneButton = GameObject.Find("NextSceneButton");
        //nextSceneButton.SetActive(false);

        colorIndicator = InstantiateRandomColor();


    }

    private float nextActionTime = 0.0f;

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < 6; i++)
        {
            if (balloons[i] != null)
            {
                if (!balloons[i].name.Substring(0, 1).Equals(colorIndicator.name.Substring(0, 1)))
                {
                    if ((balloonsInstantiationTime[i] != 0) && (Time.timeSinceLevelLoad - balloonsInstantiationTime[i] > 5))
                    {
                        balloonsInstantiationTime[i] = 0;
                        numberOfBalloonsOnScreen--;
                        Destroy(balloons[i], 0);
                    } 
                }
            }

        }

        foreach (GameObject balloon in balloons)
        {
            MoveBalloon(balloon);
        }

        if (Time.timeSinceLevelLoad > nextActionTime)
        {
            nextActionTime += Random.Range(Global.timeToNextBalloonMin, Global.timeToNextBalloonMax);

            if (numberOfBalloonsOnScreen < Global.maxNumberOfBalloonsOnScreen &&
                numberOfBalloonsInTotal < Global.maxNumberOfBalloonsInTotal)
            {
                InstantiateRandomPositionedBalloon();
            }

        }

    }


    public void MoveBalloon(GameObject balloon)
    {

        if (balloon != null)
        {
            Vector3 p = balloon.transform.position;
            p.x = balloon.transform.position.x + Mathf.Sin(Time.time * Global.balloonAnimationSpeed) * Global.balloonAnimationDelta;
            p.y = balloon.transform.position.y + Mathf.Cos(Time.time * Global.balloonAnimationSpeed) * Global.balloonAnimationDelta;
            balloon.transform.position = p;
        }
    }

    public void InstantiateRandomPositionedBalloon()
    {
        numberOfBalloonsInTotal++;
        numberOfBalloonsOnScreen++;

        GameObject instantiatedBalloon = null;

        while (instantiatedBalloon == null)
        {
            int randomIndex = Random.Range(0, 6);
            if (balloons[randomIndex] == null)
            {
                balloons[randomIndex] = InstantiateRandomColoredBalloon(balloonsX[randomIndex], balloonsY[randomIndex]);
                balloonsInstantiationTime[randomIndex] = Time.timeSinceLevelLoad;
                instantiatedBalloon = balloons[randomIndex];
            }
        }

    }

    public GameObject InstantiateRandomColoredBalloon(float positionX, float positionY)
    {
        int random = Random.Range(0, 3);
        Object original;

        switch (random)
        {
            case 0:
                original = green_balloon;
                break;
            case 1:
                original = red_balloon;
                break;
            case 2:
                original = blue_balloon;
                break;
            default:
                original = green_balloon;
                break;
        }

        return InstantiateBalloon(original, positionX, positionY);
    }

    public GameObject InstantiateBalloon(Object original, float positionX, float positionY)
    {
        audioSource.PlayOneShot(blop, 0.7F);
        return Instantiate(original, new Vector3(positionX, positionY, 0), Quaternion.identity) as GameObject;
    }

    public GameObject InstantiateRandomColor()
    {
        int random = Random.Range(0, 6);
        Object original;

        switch (random)
        {
            case 0:
                original = green;
                break;
            case 1:
                original = red;
                break;
            case 2:
                original = blue;
                break;
            default:
                original = green;
                break;
        }

        GameObject color = Instantiate(original, new Vector3(Global.colorX, Global.colorY, 0), Quaternion.identity) as GameObject;
        color.transform.SetParent(canvas.transform, false);
        return color;
    }

    public void OnMovement(string position)
    {
        Debug.Log("OnMovement called: " + position);

        switch (position)
        {
            case "DR":
                OnPop(balloons[0]);
                balloons[0] = null;
                break;

            case "DL":
                OnPop(balloons[1]);
                balloons[1] = null;
                break;

            case "UR":
                OnPop(balloons[2]);
                balloons[2] = null;
                break;

            case "UL":
                OnPop(balloons[3]);
                balloons[3] = null;
                break;

            case "SR":
                OnPop(balloons[4]);
                balloons[4] = null;
                break;

            case "SL":
                OnPop(balloons[5]);
                balloons[5] = null;
                break;

            default:
                break;
        }

        if (numberOfBalloonsInTotal == Global.maxNumberOfBalloonsInTotal)
        {
            // nextSceneButton.SetActive(true);
        }
    }

    public void OnPop(GameObject balloon)
    {
        if (balloon != null)
        {
            numberOfBalloonsOnScreen--;
            audioSource.PlayOneShot(pop, 0.7F);
            balloon.GetComponent<Animator>().enabled = true;
            Destroy(balloon, 0.333f);
        }
    }




}

