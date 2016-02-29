using UnityEngine;
using System.Collections;

public class elfScript : MonoBehaviour {

    private float brightCap = 1f;
    private LensFlare lensFlare;
    private Light licht;
    private bool peaked = false;
	// Use this for initialization
	void Start () {

        lensFlare = GetComponent<LensFlare>();
        licht = GetComponent<Light>();

	}
	
	// Update is called once per frame
	void Update () {
	
        if (!peaked)
        {
            lensFlare.brightness *= 1.08f;
            licht.intensity *= 1.004f;

            if (licht.intensity > brightCap)
            {
                peaked = true;
            }
        }
        else
        {
            lensFlare.brightness *= 0.95f;
            licht.intensity *= 0.95f;

            brightCap -= Time.deltaTime;
            
            if (brightCap < 0)
            {
                DestroyObject(this.gameObject);
            }
        }

	}
}
