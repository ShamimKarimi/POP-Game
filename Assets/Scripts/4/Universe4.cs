using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Universe4 : MonoBehaviour
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

    // left to right
    float[] balloonsX = { -6.8f, -2.35f, 2.1f, 6.55f };

    int numberOfBalloonsInTotal = 0;


    // Start is called before the first frame update
    void Start()
    {

        balloons = new GameObject[4];

        audioSource = GetComponent<AudioSource>();

        //nextSceneButton = GameObject.Find("NextSceneButton");
        //nextSceneButton.SetActive(false);
    }

    private float nextActionTime = 0.0f;

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < balloons.Length; i++)
        {
            MoveBalloon(balloons[i], i);
        }

        if (Time.timeSinceLevelLoad > nextActionTime)
        {

            nextActionTime += Random.Range(Global.timeToNextBalloonMin, Global.timeToNextBalloonMax);

            if (numberOfBalloonsInTotal < Global.maxNumberOfBalloonsInTotal)
            {
                InstantiateRandomPositionedBalloon();
            }

        }

    }

    public void MoveBalloon(GameObject balloon, int index)
    {

        if (balloon != null)
        {
            Vector3 p = balloon.transform.position;

            // not popped, out of screen
            if (p.y > 9)
            {
                balloons[index] = null;
                Destroy(balloon);
            }
            // moving up, visible on the screen
            else
            {
                p.x = balloon.transform.position.x + Mathf.Sin(Time.time * Global.balloonAnimationSpeed) * Global.balloonAnimationDelta;
                p.y = balloon.transform.position.y + Global.balloonVerticalTranslationDelta;
                balloon.transform.position = p;
            }
        }
    }


    public void InstantiateRandomPositionedBalloon()
    {
        numberOfBalloonsInTotal++;

        GameObject instantiatedBalloon = null;

        while (instantiatedBalloon == null)
        {
            int randomIndex = Random.Range(0, 4);
            if (balloons[randomIndex] == null)
            {
                balloons[randomIndex] = InstantiateRandomColoredBalloon(balloonsX[randomIndex]);
                instantiatedBalloon = balloons[randomIndex];
            }
        }

    }

    public GameObject InstantiateRandomColoredBalloon(float positionX)
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

        return InstantiateBalloon(original, positionX);
    }

    public GameObject InstantiateBalloon(Object original, float positionX)
    {
        audioSource.PlayOneShot(blop, 0.7F);
        return Instantiate(original, new Vector3(positionX, -5, 0), Quaternion.identity) as GameObject;
    }

    public void OnMovement(string position)
    {
        Debug.Log("OnMovement called: " + position);

        float[] yLevelMin = { -6.35f, -3.85f, -1.35f };
        float[] yLevelMax = { -2f, 0.5f, 3f };

        switch (position)
        {
            case "DR":
                OnPop(2, yLevelMin[0], yLevelMax[0]);
                break;

            case "DL":
                OnPop(1, yLevelMin[0], yLevelMax[0]);
                break;

            case "UR":
                OnPop(2, yLevelMin[2], yLevelMax[2]);
                break;

            case "UL":
                OnPop(1, yLevelMin[2], yLevelMax[2]);
                break;

            case "SR":
                OnPop(3, yLevelMin[1], yLevelMax[1]);
                break;

            case "SL":
                OnPop(0, yLevelMin[1], yLevelMax[1]);
                break;

            default:
                break;
        }

        if (numberOfBalloonsInTotal == Global.maxNumberOfBalloonsInTotal)
        {
            nextSceneButton.SetActive(true);
        }
    }

    public void OnPop(int index, float yMin, float yMax)
    {
        if (balloons[index] != null)
        {
            Vector3 p = balloons[index].transform.position;
            if (p.y > yMin && p.y < yMax)
            {
                audioSource.PlayOneShot(pop, 0.7F);
                balloons[index].GetComponent<Animator>().enabled = true;
                Destroy(balloons[index], 0.333f);
                balloons[index] = null;
            }
        }
    }


}
