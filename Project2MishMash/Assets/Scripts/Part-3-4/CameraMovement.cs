using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed;
    public float rotateSpeed;
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
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(speed * Time.deltaTime, 0, 0);
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

        //Rotate Camera
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler
                (
                    transform.rotation.eulerAngles.x,
                    transform.rotation.eulerAngles.y - rotateSpeed,
                    transform.rotation.eulerAngles.z
                );
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler
                (
                    transform.rotation.eulerAngles.x,
                    transform.rotation.eulerAngles.y + rotateSpeed,
                    transform.rotation.eulerAngles.z
                );
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

        GUI.Box(new Rect((Screen.width - 175) - 10, 10, 175, 200), "WASD to move\ncamera.\n\nUp / Down to\nchange height." +
            "\n\nLeft / Right to\nrotate camera.");
    }
}
