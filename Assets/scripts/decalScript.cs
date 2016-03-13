using UnityEngine;
using System.Collections;

public class decalScript : MonoBehaviour
{

    public float endSize, curSize;

    // Use this for initialization
    void Start()
    {
        curSize = 1f;

    }

    // Update is called once per frame
    void Update()
    {
        if (curSize < endSize)
        {
            curSize *= 1.01f;
            transform.localScale *= 1.01f;
        }
    }

    public void giveSize(float ez)
    {
        if (ez > 1)
        {
            endSize = ez;
        }
        else
        {
            endSize = 1;
        }
        
    }
}
