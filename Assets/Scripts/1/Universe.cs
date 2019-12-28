using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Universe : MonoBehaviour
{

    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    public GameObject green_balloon;
    public GameObject red_balloon;
    public GameObject yellow_balloon;
    public GameObject blue_balloon;
    public GameObject pink_balloon;
    public GameObject cyan_balloon;
    public AudioClip pop;
    public AudioClip blop;
    AudioSource audioSource;

    // UI
    GameObject nextSceneButton;

    // Array of balloons
    GameObject[] balloons;

    // DR, DL, UR, UL, SR, SL
    int[] balloonsX = { 2, -2, 2, -2, 6, -6 };
    int[] balloonsY = { -3, -3, 1, 1, -1, -1 };

    int maxNumberOfBalloonsOnScreen = 3;
    int numberOfBalloonsOnScreen = 0;
    int maxNumberOfBalloonsInTotal = 15;
    int numberOfBalloonsInTotal = 0;


    // Start is called before the first frame update
    void Start()
    {

        balloons = new GameObject[6];

        audioSource = GetComponent<AudioSource>();

        //nextSceneButton = GameObject.Find("NextSceneButton");
        //nextSceneButton.SetActive(false);


    }

    private float nextActionTime = 0.0f;
    public float period = 2.0f;

    // Update is called once per frame
    void Update()
    {

        foreach (GameObject balloon in balloons)
        {
            MoveBalloon(balloon);
        }

        if (Time.timeSinceLevelLoad > nextActionTime)
        {
            nextActionTime += Random.Range(2, 5);

            if (numberOfBalloonsOnScreen < maxNumberOfBalloonsOnScreen &&
                numberOfBalloonsInTotal < maxNumberOfBalloonsInTotal)
            {
                InstantiateRandomPositionedBalloon();
            }

        }

    }



    float speed = 2.0f; //how fast it moves
    float amount = 0.005f; //how much it moves

    public void MoveBalloon(GameObject balloon)
    {

        if (balloon != null)
        {
            Vector3 p = balloon.transform.position;
            p.x = balloon.transform.position.x + Mathf.Sin(Time.time * speed) * amount;
            p.y = balloon.transform.position.y + Mathf.Cos(Time.time * speed) * amount;
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
                instantiatedBalloon = balloons[randomIndex];
            }
        }

    }

    public GameObject InstantiateRandomColoredBalloon(float positionX, float positionY)
    {
        int random = Random.Range(0, 6);
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
                original = yellow_balloon;
                break;
            case 3:
                original = blue_balloon;
                break;
            case 4:
                original = pink_balloon;
                break;
            case 5:
                original = cyan_balloon;
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

        if (numberOfBalloonsInTotal == maxNumberOfBalloonsInTotal)
        {
            nextSceneButton.SetActive(true);
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

