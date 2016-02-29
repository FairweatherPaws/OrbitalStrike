using UnityEngine;
using System.Collections;

public class cloudBlockScript : MonoBehaviour
{

    float timer = 16;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (timer > 0)
        {
            transform.localScale *= (60 + timer) / 60;
        }
        if (timer < -500)
        {
            transform.localScale *= 0.995f;
        }
        if (timer < -1000)
        {
            DestroyObject(gameObject);
        }
        timer--;
    }
}
