using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed;
    private bool isPart3 = true;

    void Update()
    {
        //Forward and Back
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, 0, speed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0, 0, -speed * Time.deltaTime, Space.World);
        }

        //Left and Right
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(-speed * Time.deltaTime, 0, 0, Space.World);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(speed * Time.deltaTime, 0, 0, Space.World);
        }

        //Up and Down
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(0, speed * Time.deltaTime, 0, Space.World);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(0, -speed * Time.deltaTime, 0, Space.World);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPart3 = !isPart3;
        }
    }

    private void OnGUI()
    {
        if (isPart3)
        {
            GUI.Box(new Rect(10, 10, 175, 175), "Part 3\nMax/Min\nLeft/Right\n\nPress Space to\nswitch between\nparts 3 & 4.");
        }
        else
        {
            GUI.Box(new Rect(10, 10, 175, 175), "Part 4\nNorth/South\nEast/West\n\nPress Space to\nswitch between\nparts 3 & 4.");
        }
    }
}
