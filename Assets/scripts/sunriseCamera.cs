using UnityEngine;
using System.Collections;

public class sunriseCamera : MonoBehaviour
{

    public float t, uM, fM;

    // Use this for initialization
    void Start()
    {
        t = 50f;
        uM = 0.6f;
        fM = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {


        transform.Translate(new Vector3(0, Time.deltaTime * uM, Time.deltaTime * fM));
        transform.Rotate(new Vector3(Time.deltaTime * (t / 40), 0, 0));

        if (t > 20)
        {
            t -= Time.deltaTime;
        }
        else
        {
            t = 100f;
            fM = -0.2f;
            uM = 1.2f;
        }
    }
}
