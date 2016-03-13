using UnityEngine;
using System.Collections;

public class rodScript : MonoBehaviour
{

    public bool thrusted = true;
    private int explosionZIndex;
    public GameObject explosionPrefab, explosionLight, explosionDecal, pulsePrefab;
    public Camera checkCamera, myRodCamera;
    private GameObject gunController, planet;
    private Rigidbody rBod;
    private float gravityMultiplier, deltaV, thrustModifier, speedMultiplier;
    private bool spawnMore, inAtmosphere;

    // Use this for initialization
    void Start()
    {
        rBod = GetComponent<Rigidbody>();
        inAtmosphere = false;

        deltaV = rBod.velocity.magnitude;
    }

    // Update is called once per frame
    void Update()
    {

        if (!thrusted)
        {
            rBod.AddRelativeForce(Vector3.up * thrustModifier * speedMultiplier);
            thrusted = true;
        }
        else
        {
            float gravity = Mathf.Pow(gravityMultiplier / Vector3.Distance(transform.position, planet.transform.position), 2);
            rBod.AddForce((planet.transform.position - transform.position) * gravity);
            transform.up = rBod.velocity;
            //if (spawnMore)
            //{

            //    bool newSpawn = false;

            //    if (Random.Range(0, 10) % 2 == 0)
            //    {
            //        newSpawn = true;
            //    }

            //    GameObject newRod = Instantiate(gunController.GetComponent<GunControl>().rod, transform.position + Vector3.left, transform.rotation) as GameObject;
            //    newRod.GetComponent<rodScript>().giveThrust(thrustModifier, 0, gunController, newSpawn);

            //    if (Random.Range(0, 10) % 2 == 0)
            //    {
            //        spawnMore = false;
            //    }
            //}
        }

        deltaV = rBod.velocity.magnitude;


        if (Input.GetKeyDown(KeyCode.B))
        {
            rBod.velocity *= 0.7f;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            rBod.velocity += -rBod.position * 0.01f;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            rBod.velocity *= 1.3f;
        }

        if (inAtmosphere)
        {
            rBod.velocity *= 0.95f;
        }
    }

    public void giveThrust(float n, float g, float sm, int z, GameObject gc, Camera c, bool original)
    {
        checkCamera = c;
        speedMultiplier = sm;
        gravityMultiplier = g * sm;
        thrustModifier = n;
        thrusted = false;
        explosionZIndex = z;
        gunController = gc;
        planet = gc.GetComponent<GunControl>().getPlanetGO();
        spawnMore = original;
    }

    public void giveCamera(Camera c)
    {
        myRodCamera = c;
    }

    public void adjustTimeflow(float t)
    {
        gravityMultiplier *= t;
        rBod.velocity *= t;
    }

    void OnCollisionEnter(Collision collision)
    {

        // TODO check what it hits, if rod, don't blow up but merge
        // TODO get impact velocity and multiply score with it.
        ContactPoint cp = collision.contacts[0];

        gunController.GetComponent<GunControl>().giveHitCoordinates(cp.point, deltaV);

        Vector3 planetPos = collision.gameObject.transform.position;

        float factor = Vector3.Distance(cp.point, planetPos);

        if (collision.gameObject == planet)
        {



            GameObject newBoom = Instantiate(explosionPrefab, (cp.point - planetPos) * (25 / factor) * 1.005f, Quaternion.identity) as GameObject;
            newBoom.transform.rotation = Quaternion.LookRotation((transform.position - planetPos) * 2);

            GameObject newBoom2 = Instantiate(pulsePrefab, (cp.point - planetPos) * (25 / factor) * 1.005f, Quaternion.identity) as GameObject;
            newBoom2.transform.rotation = Quaternion.LookRotation((transform.position - planetPos) * 2);

            newBoom2.GetComponent<explosionSelfdestruct>().givePulseOrder();

            Instantiate(explosionLight, (cp.point - planetPos) * (25 / factor) * 1.005f, Quaternion.identity);

            float v = Vector3.Distance(transform.position, planetPos);

            gunController.GetComponent<GunControl>().spawnCloudBlocker(cp.point.x, cp.point.y, cp.point.z, v);

            //newBoom.GetComponent<ParticleSystemRenderer>().sortingOrder = explosionZIndex;

            //checkCamera.transform.position = (cp.point - planetPos) * 2;

            //RaycastHit hit;

            //checkCamera.transform.LookAt(planetPos);

            //Ray ray = checkCamera.ScreenPointToRay(new Vector3(checkCamera.pixelWidth / 2, checkCamera.pixelHeight, 0));

            //if (Physics.Raycast(ray, out hit))
            //{

            //}

            GameObject decal = Instantiate(explosionDecal, cp.point, Quaternion.identity) as GameObject;

            decal.transform.rotation = Quaternion.LookRotation((cp.point - collision.gameObject.transform.position) * 2);
            decal.transform.Rotate(new Vector3(90, 0, 0));

            
            if (deltaV / (50 * speedMultiplier) < 1)
            {
                decal.transform.localScale *= deltaV / (50 * speedMultiplier);
            } else
            {
                decal.gameObject.GetComponent<decalScript>().giveSize(deltaV / (50 * speedMultiplier));
                
            }

            RaycastHit hit;

            Ray ray = new Ray((cp.point - planetPos) * 2, (planetPos - cp.point) * 2);

            bool splash = false;

            int layerMask = 1 << 10;

            if (Physics.Raycast(ray, out hit, Vector3.Distance(planetPos, cp.point) * 2, layerMask))
            {
                //Component[] rends = hit.transform.GetComponentsInChildren<Renderer>();
                //foreach (Renderer r in rends)
                //{
                Renderer r = hit.transform.GetComponent<Renderer>();
                Texture2D tex = r.material.mainTexture as Texture2D;
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= tex.width;
                pixelUV.y *= tex.height;
                Color targetColour = tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);
                if (targetColour.a < 0.5f)
                {
                    splash = true;
                }

                //}
            }



            if (!splash)
            {
                decal.transform.parent = collision.gameObject.transform;
            }
            else
            {
                DestroyObject(decal.gameObject);
            }

            gunController.GetComponent<GunControl>().returnRodCamera(gameObject);
            DestroyObject(gameObject);
        }

        if (Vector3.Distance(transform.position, planet.transform.position) < 20)
        {

            gunController.GetComponent<GunControl>().returnRodCamera(gameObject);

            DestroyObject(gameObject);
        }

    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Atmosphere")
        {
            inAtmosphere = true;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag == "Atmosphere")
        {
            inAtmosphere = false;
        }
    }
}
