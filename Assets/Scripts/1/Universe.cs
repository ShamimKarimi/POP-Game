using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
    public AudioClip fanfare;
    AudioSource audioSource;

    // Array of balloons
    GameObject[] balloons;

    // DR, DL, UR, UL, SR, SL
    float[] balloonsX = { 2f, -2.2f, 2f, -2.2f, 6.2f, -6.3f };
    float[] balloonsY = { -3.5f, -3.5f, 1.2f, 1.2f, -1.2f, -1.2f };


    int numberOfBalloonsOnScreen = 0;
    int numberOfBalloonsInTotal = 0;

    [SerializeField] private Game1 game1Data = new Game1();


    // Start is called before the first frame update
    void Start()
    {

        balloons = new GameObject[6];

        audioSource = GetComponent<AudioSource>();

    }

    private float nextActionTime = 0.0f;

    // Update is called once per frame
    void Update()
    {

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
                balloons[randomIndex] = InstantiateRandomColoredBalloon(randomIndex);
                instantiatedBalloon = balloons[randomIndex];
            }
        }

    }

    public GameObject InstantiateRandomColoredBalloon(int randomIndex)
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

        // Save the data of the generated balloon in data object
        game1Data.events.Add(new Event(Global.generateType, randomIndex, random));

        return InstantiateBalloon(original, balloonsX[randomIndex], balloonsY[randomIndex]);
    }

    public GameObject InstantiateBalloon(Object original, float positionX, float positionY)
    {
        audioSource.PlayOneShot(blop, 0.7F);
        return Instantiate(original, new Vector3(positionX, positionY, 0), Quaternion.identity) as GameObject;
    }

    public void OnMovement(string position)
    {
        //Debug.Log("OnMovement called: " + position);

        game1Data.messages.Add(position);

        switch (position)
        {
            case "DR":
                OnPop(balloons[0], 0);
                balloons[0] = null;
                break;

            case "DL":
                OnPop(balloons[1], 1);
                balloons[1] = null;
                break;

            case "UR":
                OnPop(balloons[2], 2);
                balloons[2] = null;
                break;

            case "UL":
                OnPop(balloons[3], 3);
                balloons[3] = null;
                break;

            case "SR":
                OnPop(balloons[4], 4);
                balloons[4] = null;
                break;

            case "SL":
                OnPop(balloons[5], 5);
                balloons[5] = null;
                break;

            default:
                break;
        }
    }

    public void OnPop(GameObject balloon, int position)
    {
        if (balloon != null)
        {
            numberOfBalloonsOnScreen--;
            audioSource.PlayOneShot(pop, 0.7F);
            balloon.GetComponent<Animator>().enabled = true;
            Destroy(balloon, Global.popAnimationDuration);

            // Save the data of the hit balloon in data object
            game1Data.events.Add(new Event(Global.hitType, position));

            if (numberOfBalloonsInTotal == Global.maxNumberOfBalloonsInTotal)
            {
                foreach (GameObject b in balloons)
                {
                    if (b != null)
                    {
                        break;
                    }

                    if (!IsGameFinished)
                    {
                        PlayEnding();
                    }
                }
            }
        }
    }

    bool IsGameFinished;

    public void PlayEnding()
    {

        IsGameFinished = true;

        GameObject.Find("Targets").SetActive(false);

        Debug.Log("play ending");

        for (var i = 1; i < 8; i++)
        {
            GameObject.Find("p" + i.ToString()).GetComponentInChildren<ParticleSystem>().Play();
        }

        audioSource.PlayOneShot(fanfare, 1.0F);


    }

    public void SaveIntoJson()
    {
        Debug.Log("save button was clicked");

        string dataFolderPath = Application.persistentDataPath + "/Data/" + System.DateTime.Now.ToString("dd-MM-yyyy");

        if (!Directory.Exists(dataFolderPath))
        {
            //if it doesn't, create it
            Directory.CreateDirectory(dataFolderPath);

        }

        string data = JsonUtility.ToJson(game1Data);
        File.WriteAllText(dataFolderPath + "/Game_1_" + System.DateTime.Now.ToString("hh-mm-ss") + ".json", data);
    }

    void OnApplicationQuit()
    {
        SaveIntoJson();
    }

    [System.Serializable]
    public class Game1
    {
        public List<Event> events = new List<Event>();
        public List<string> messages = new List<string>();
    }

    [System.Serializable]
    public class Event
    {
        public string timestamp;
        public string type;
        public string position;
        public string color;

        public Event(string _type, int _position, int _color)
        {
            timestamp = Time.timeSinceLevelLoad.ToString();
            type = _type;
            position = Global.targetPositions[_position];
            color = Global.colors[_color];
        }

        public Event(string _type, int _position)
        {
            timestamp = Time.timeSinceLevelLoad.ToString();
            type = _type;
            position = Global.targetPositions[_position];
        }
    }
}

