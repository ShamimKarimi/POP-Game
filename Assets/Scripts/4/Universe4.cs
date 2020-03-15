using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Universe4 : MonoBehaviour
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
    public AudioClip error;
    public AudioClip fanfare;
    AudioSource audioSource;

    GameObject canvas;

    // Array of balloons
    GameObject[] balloons;

    // left to right
    float[] balloonsX = { -6.3f, -2.2f, 2f, 6.2f, -6.3f, -2.2f, 2f, 6.2f };

    GameObject colorIndicator;

    int numberOfBalloonsInTotal = 0;

    float[] lastErrorSoundTime;

    [SerializeField] private Game4 game4Data = new Game4();


    // Start is called before the first frame update
    void Start()
    {

        balloons = new GameObject[8];
        lastErrorSoundTime = new float[8];

        audioSource = GetComponent<AudioSource>();

        canvas = GameObject.Find("Canvas");

        colorIndicator = InstantiateRandomColor();
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
                lastErrorSoundTime[index] = 0;

                IsGameFinished();
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
            int randomIndex = Random.Range(0, 8);
            if (balloons[randomIndex] == null)
            {
                balloons[randomIndex] = InstantiateRandomColoredBalloon(randomIndex);
                instantiatedBalloon = balloons[randomIndex];
            }
        }

    }

    public GameObject InstantiateRandomColoredBalloon(int position)
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

        // Save the data of the generated balloon in data object
        game4Data.events.Add(new Event(Global.generateType, position, random));

        return InstantiateBalloon(original, balloonsX[position]);
    }

    public GameObject InstantiateBalloon(Object original, float positionX)
    {
        audioSource.PlayOneShot(blop, 0.7F);
        return Instantiate(original, new Vector3(positionX, -5, 0), Quaternion.identity) as GameObject;
    }

    public GameObject InstantiateRandomColor()
    {
        int random = Random.Range(0, 3);
        Object original;

        switch (random)
        {
            case 0:
                original = green;
                game4Data.color = "green";
                break;
            case 1:
                original = red;
                game4Data.color = "red";
                break;
            case 2:
                original = blue;
                game4Data.color = "blue";
                break;
            default:
                original = green;
                break;
        }

        GameObject color = Instantiate(original, new Vector3(Global.colorX, Global.colorY, 0), Quaternion.identity) as GameObject;
        color.name = "Color";
        color.transform.SetParent(canvas.transform, false);

        return color;
    }

    public void OnMovement(string position)
    {
        //Debug.Log("OnMovement called: " + position);

        game4Data.messages.Add(position);

        float[] yLevelMin = { -6.35f, -3.85f, -1.35f };
        float[] yLevelMax = { -2f, 0.5f, 3f };

        switch (position)
        {
            case "DR":
                OnPop(2, yLevelMin[0], yLevelMax[0], position);
                OnPop(6, yLevelMin[0], yLevelMax[0], position);
                break;

            case "DL":
                OnPop(1, yLevelMin[0], yLevelMax[0], position);
                OnPop(5, yLevelMin[0], yLevelMax[0], position);
                break;

            case "UR":
                OnPop(2, yLevelMin[2], yLevelMax[2], position);
                OnPop(6, yLevelMin[2], yLevelMax[2], position);
                break;

            case "UL":
                OnPop(1, yLevelMin[2], yLevelMax[2], position);
                OnPop(5, yLevelMin[2], yLevelMax[2], position);
                break;

            case "SR":
                OnPop(3, yLevelMin[1], yLevelMax[1], position);
                OnPop(7, yLevelMin[1], yLevelMax[1], position);
                break;

            case "SL":
                OnPop(0, yLevelMin[1], yLevelMax[1], position);
                OnPop(4, yLevelMin[1], yLevelMax[1], position);
                break;

            default:
                break;
        }
    }

    public void OnPop(int index, float yMin, float yMax, string position)
    {
        if (balloons[index] != null)
        {
            if (balloons[index].name.Substring(0, 1).Equals(colorIndicator.name.Substring(0, 1)))
            {
                Vector3 p = balloons[index].transform.position;
                if (p.y > yMin && p.y < yMax)
                {
                    audioSource.PlayOneShot(pop, 0.7F);
                    balloons[index].GetComponent<Animator>().enabled = true;
                    Destroy(balloons[index], Global.popAnimationDuration);
                    balloons[index] = null;

                    // Save the data of the hit balloon in data object
                    game4Data.events.Add(new Event(Global.hitType, position));

                    IsGameFinished();
                }
            } else
            {
                if ((!Equals(lastErrorSoundTime[index], 0))
                     && (Time.timeSinceLevelLoad - lastErrorSoundTime[index] > Global.intervalBetweenErrorSounds))
                {
                    lastErrorSoundTime[index] = Time.timeSinceLevelLoad;
                    audioSource.PlayOneShot(error, 0.7F);

                    // Save the data of the missed balloon in data object
                    game4Data.events.Add(new Event(Global.missType, position));
                }
            }
        }
    }

    public void IsGameFinished()
    {
        if (numberOfBalloonsInTotal == Global.maxNumberOfBalloonsInTotal)
        {
            bool IsAnyBalloonLeft = false;

            foreach (GameObject b in balloons)
            {
                if (b != null)
                {
                    IsAnyBalloonLeft = true;
                }

            }

            if (!IsAnyBalloonLeft && !AlreadyPlayedEnding)
            {
                PlayEnding();
                SaveIntoJson();
            }
        }
    }

    bool AlreadyPlayedEnding;

    public void PlayEnding()
    {

        AlreadyPlayedEnding = true;

        GameObject.Find("Targets").SetActive(false);
        GameObject.Find("Color").SetActive(false);

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

        string data = JsonUtility.ToJson(game4Data);
        File.WriteAllText(dataFolderPath + "/Game_4_" + System.DateTime.Now.ToString("hh-mm-ss") + ".json", data);
    }

    void OnApplicationQuit()
    {
        SaveIntoJson();
    }

    [System.Serializable]
    public class Game4
    {
        public string color;
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

        // generate balloon data constructor
        public Event(string _type, int _position, int _color)
        {

            if (_position > 3)
            {
                _position -= 4;
            }

            timestamp = Time.timeSinceLevelLoad.ToString();
            type = _type;
            position = Global.columnPositions[_position];
            color = Global.mainColors[_color];
        }

        // hit data constructor
        public Event(string _type, string _position)
        {
            timestamp = Time.timeSinceLevelLoad.ToString();
            type = _type;
            position = _position;
        }
    }


}
