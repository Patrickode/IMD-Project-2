using UnityEngine;

public class AntiRock : MonoBehaviour
{
	public float speed = 10f;

    Vector3 pos;

    private void Update()
    {
        pos = transform.position;
        pos.y += speed * Time.deltaTime;
        transform.position = pos; 
    }

}