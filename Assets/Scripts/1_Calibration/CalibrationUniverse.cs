using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CalibrationUniverse : MonoBehaviour
{

    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    public GameObject green_balloon;
    public GameObject red_balloon;
    public GameObject yellow_balloon;
    public GameObject blue_balloon;
    public GameObject pink_balloon;
    public GameObject cyan_balloon;
    GameObject UR, UL, DR, DL, SR, SL;

    GameObject nextSceneButton;

    public AudioClip pop;
    public AudioClip blop;
    AudioSource audioSource;

    bool down, up, sides;


    // Start is called before the first frame update
    void Start()
    {

        audioSource = GetComponent<AudioSource>();

        nextSceneButton = GameObject.Find("NextSceneButton");
        // nextSceneButton.SetActive(false);

        DL = InstantiateRandomBalloon(-2, -3);
        DR = InstantiateRandomBalloon(2, -3);

        down = true;

    }
    
    // Update is called once per frame
    void Update()
    {
        MoveBalloon(DR);
        MoveBalloon(DL);
        MoveBalloon(UR);
        MoveBalloon(UL);
        MoveBalloon(SR);
        MoveBalloon(SL);

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

    public GameObject InstantiateBalloon(Object original, float positionX, float positionY)
    {
        audioSource.PlayOneShot(blop, 0.7F);
        return Instantiate(original, new Vector3(positionX, positionY, 0), Quaternion.identity) as GameObject;
    }

    public GameObject InstantiateRandomBalloon(float positionX, float positionY)
    {
        int random = Random.Range(0, 6);
        Object original;

        switch (random) {
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



    public void OnMovement(string position)
    {
        Debug.Log("OnMovement called: " + position);

        switch(position)
        {
            case "DL":
                OnPop(DL);
                DL = null;
                break;

            case "DR":
                OnPop(DR);
                DR = null;
                break;

            case "UL":
                OnPop(UL);
                UL = null;
                break;

            case "UR":
                OnPop(UR);
                UR = null;
                break;

            case "SL":
                OnPop(SL);
                SL = null;
                break;

            case "SR":
                OnPop(SR);
                SR = null;
                break;

            default:
                break;
        }

        CheckForNewState();
    }

    public void OnPop(GameObject balloon)
    {
        if (balloon != null)
        {
            audioSource.PlayOneShot(pop, 0.7F);
            balloon.GetComponent<Animator>().enabled = true;
            //Destroy(balloon, balloon.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
            Destroy(balloon, 0.333f);
        }
    }



    public void CheckForNewState()
    {
        if (down)
        {
            if (DL == null && DR == null)
            {
                UL = InstantiateRandomBalloon(-2, 1);
                UR = InstantiateRandomBalloon(2, 1);

                down = false;
                up = true;
            }
        }

        if (up)
        {
            if (UL == null && UR == null)
            {
                SL = InstantiateRandomBalloon(-6, -1);
                SR = InstantiateRandomBalloon(6, -1);

                up = false;
                sides = true;
            }
        }

        if (sides)
        {
            if (SL == null && SR == null)
            {
                nextSceneButton.SetActive(true);
            }
        }
    }
}
