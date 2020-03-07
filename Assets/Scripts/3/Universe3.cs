using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
    public AudioClip error;
    AudioSource audioSource;

    GameObject canvas;

    // Array of balloons
    GameObject[] balloons;

    // DR, DL, UR, UL, SR, SL
    float[] balloonsX = { 2f, -2.2f, 2f, -2.2f, 6.2f, -6.3f };
    float[] balloonsY = { -3.5f, -3.5f, 1.2f, 1.2f, -1.2f, -1.2f };

    float[] balloonsInstantiationTime;

    GameObject colorIndicator;

    int numberOfBalloonsOnScreen = 0;
    int numberOfBalloonsInTotal = 0;

    float[] lastErrorSoundTime;

    [SerializeField] private Game3 game3Data = new Game3();


    // Start is called before the first frame update
    void Start()
    {

        balloons = new GameObject[6];
        balloonsInstantiationTime = new float[6];
        lastErrorSoundTime = new float[6];

        audioSource = GetComponent<AudioSource>();

        canvas = GameObject.Find("Canvas");

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
                    if ((!Equals(balloonsInstantiationTime[i], 0)) && (Time.timeSinceLevelLoad - balloonsInstantiationTime[i] > 5))
                    {
                        balloonsInstantiationTime[i] = 0;
                        lastErrorSoundTime[i] = 0;
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
                balloons[randomIndex] = InstantiateRandomColoredBalloon(randomIndex);
                balloonsInstantiationTime[randomIndex] = Time.timeSinceLevelLoad;
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
        game3Data.events.Add(new Event(Global.generateType, position, random));

        return InstantiateBalloon(original, balloonsX[position], balloonsY[position]);
    }

    public GameObject InstantiateBalloon(Object original, float positionX, float positionY)
    {
        audioSource.PlayOneShot(blop, 0.7F);
        return Instantiate(original, new Vector3(positionX, positionY, 0), Quaternion.identity) as GameObject;
    }

    public GameObject InstantiateRandomColor()
    {
        int random = Random.Range(0, 3);
        Object original;

        switch (random)
        {
            case 0:
                original = green;
                game3Data.color = "green";
                break;
            case 1:
                original = red;
                game3Data.color = "red";
                break;
            case 2:
                original = blue;
                game3Data.color = "blue";
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
        //Debug.Log("OnMovement called: " + position);

        game3Data.messages.Add(position);

        switch (position)
        {
            case "DR":
                OnPop(0);
                break;

            case "DL":
                OnPop(1);
                break;

            case "UR":
                OnPop(2);
                break;

            case "UL":
                OnPop(3);
                break;

            case "SR":
                OnPop(4);
                break;

            case "SL":
                OnPop(5);
                break;

            default:
                break;
        }

        if (numberOfBalloonsInTotal == Global.maxNumberOfBalloonsInTotal)
        {
            // end of game
        }
    }

    public void OnPop(int index)
    {
        if (balloons[index] != null)
        {
            if (balloons[index].name.Substring(0, 1).Equals(colorIndicator.name.Substring(0, 1)))
            {
                numberOfBalloonsOnScreen--;
                audioSource.PlayOneShot(pop, 0.7F);
                balloons[index].GetComponent<Animator>().enabled = true;
                Destroy(balloons[index], Global.popAnimationDuration);
                balloons[index] = null;

                // Save the data of the hit balloon in data object
                game3Data.events.Add(new Event(Global.hitType, index));
            } else
            {
                if ((!Equals(lastErrorSoundTime[index], 0))
                    && (Time.timeSinceLevelLoad - lastErrorSoundTime[index] > Global.intervalBetweenErrorSounds))
                {
                    lastErrorSoundTime[index] = Time.timeSinceLevelLoad;
                    audioSource.PlayOneShot(error, 0.7F);

                    // Save the data of the missed balloon in data object
                    game3Data.events.Add(new Event(Global.missType, index));
                }
                
            }

        }
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

        string data = JsonUtility.ToJson(game3Data);
        File.WriteAllText(dataFolderPath + "/Game_3_" + System.DateTime.Now.ToString("hh-mm-ss") + ".json", data);
    }

    void OnApplicationQuit()
    {
        SaveIntoJson();
    }

    [System.Serializable]
    public class Game3
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

        public Event(string _type, int _position, int _color)
        {
            timestamp = Time.timeSinceLevelLoad.ToString();
            type = _type;
            position = Global.targetPositions[_position];
            color = Global.mainColors[_color];
        }

        public Event(string _type, int _position)
        {
            timestamp = Time.timeSinceLevelLoad.ToString();
            type = _type;
            position = Global.targetPositions[_position];
        }
    }


}

