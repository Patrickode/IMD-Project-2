using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffscreenCulling : MonoBehaviour
{
    private GameManager manager;

    void Start()
    {
        //Get a reference to the manager, because we can't drag and drop references to prefabs
        //  Maybe we could drag and drop a reference onto the rock spawner, and pass it in through that?
        //  AntiRock spawning complicates that, though. Maybe we'd need a custom instantiate.
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();

        StartCoroutine(Cull());
    }

    private IEnumerator Cull()
    {
        //Wait until the object is far enough offscreen, and once it is, make it unexist
        yield return new WaitUntil(() => transform.position.y > 8 || transform.position.y < -8);
        manager?.DisposeObject(gameObject);
    }
}
