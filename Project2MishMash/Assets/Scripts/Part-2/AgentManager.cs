using UnityEngine;
using Random = UnityEngine.Random;

public class AgentManager : MonoBehaviour
{
    public GameObject target;
    public Vehicle vehicle;
    public FishControls fishControls;

    public float camSpeed;
    public float distanceToCatch;

    private Vector3 originalPos;
    private float originalZRot;

    private void Start()
    {
        originalPos = transform.position;
        originalZRot = transform.eulerAngles.z;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            fishControls.enabled = !fishControls.enabled;

            //reset rotation to prepare for change in state
            transform.rotation = Quaternion.identity;

            //initialization for static state
            if (!fishControls.enabled)
            {
                //set player object to normal rotation and put the camera in its spot
                target.transform.rotation = Quaternion.identity;
                transform.position = originalPos;
            }
        }

        //Switch between the move modes when their respective key is pressed.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            vehicle.MoveMode = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            vehicle.MoveMode = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            vehicle.MoveMode = 2;
        }

        if (!fishControls.enabled)
        {
            //Reminded me of how to use slerp: https://forum.unity.com/threads/smooth-look-at.26141/
            Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

            // Smoothly rotate camera towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, camSpeed * Time.deltaTime);

            //when fish is caught, it dies (warps elsewhere)
            float distance = Vector3.Distance(target.transform.position, vehicle.transform.position);
            if (distance < distanceToCatch)
            {
                target.transform.position = new Vector3(Random.Range(-14, 14), Random.Range(-14, 14), 0);
            }
        }
        else
        {
            transform.position = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z - 10);
        }
    }

    private void LateUpdate()
    {
        //Lock z rotation by setting it to zero after calculations are done
        //idea from http://answers.unity.com/answers/423035/view.html
        transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, originalZRot);
    }

    void OnGUI()
    {
        GUI.color = Color.cyan;
        GUI.skin.box.fontSize = 20;

        //Set up an empty string to put in the GUI later
        string guiText = "";

        //Make the string reflect the current moveMode
        switch (vehicle.MoveMode)
        {
            default:
            case 0:
                guiText += "Seeking";
                break;
            case 1:
                guiText += "Fleeing";
                break;
            case 2:
                guiText += "Neutral";
                break;
        }

        guiText += "\n\nFish: ";

        if (fishControls.enabled)
        {
            guiText += "Manual";
        }
        else
        {
            guiText += "Static";
        }

        GUI.Box(new Rect(10, 10, 125, 80), guiText);
    }
}
