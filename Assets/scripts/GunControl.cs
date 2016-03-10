using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GunControl : MonoBehaviour
{

    public GameObject gun, gunControl, planet, mouseCollider, rod, scoreField, cloudBlocker, guidePrefab;
    public Camera mainCamera, rodCameraPrefab, rodCamera, freeCamera, checkCamera;
    private int score, extrapolationRange;
    //private int explosionZincrement;
    private GameObject[] reticules, guides;
    private LineRenderer lineRend;
    private float gravityMultiplier, thrust;
    public float speedMultiplier;
    public RenderTexture cloudLayer;



    // Use this for initialization
    void Start()
    {
        //UnityEngine.Cursor.visible = false;

        speedMultiplier = .3f;

        thrust = 1500;
        gravityMultiplier = 30f;
        extrapolationRange = 10;
        Physics.IgnoreLayerCollision(0, 8);
        //explosionZincrement = 0;
        reticules = GameObject.FindGameObjectsWithTag("Reticule");
        lineRend = GetComponent<LineRenderer>();
        lineRend.SetVertexCount(extrapolationRange);
        lineRend.SetColors(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0));
        lineRend.material.mainTextureScale = new Vector2(extrapolationRange, 1);

        guides = new GameObject[extrapolationRange];

        for (int i = 0; i < guides.Length; i++)
        {
            guides[i] = Instantiate(guidePrefab, Vector3.zero, Quaternion.identity) as GameObject;
            guides[i].transform.Rotate(new Vector3(-90, 0, 0));
            guides[i].transform.parent = transform;
            float t = (i * 10 / extrapolationRange * 10);
            t /= 100;
            guides[i].GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(1 - t, 1 - t, 1 - t));
            guides[i].GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1 - t);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        int layerMask = 1 << 8;

        if (Physics.Raycast(ray, out hit, 100f, layerMask) && (rodCamera == null || !rodCamera.enabled))
        {
            Vector3 pointHit = hit.point;
            transform.LookAt(pointHit);

            Vector3 startPoint = gun.transform.position;

            int i = 2;

            guides[0].transform.position = startPoint + gun.transform.up * 5f;
            guides[1].transform.position = startPoint + gun.transform.up * 10f;

            for (; i < guides.Length; i++)
            {
                Vector3 gmo = guides[i - 1].transform.position;
                Vector3 gmt = guides[i - 2].transform.position;
                Vector3 gpos = gmo + (gmo - gmt);

                float gravity = gravityMultiplier / Mathf.Pow(Vector3.Distance((gmo + gpos) / 2, planet.transform.position), 2);

                gpos += (planet.transform.position - (gmo + gpos) / 2) * gravity;

                if (Vector3.Distance(gpos, planet.transform.position) < 20 || Vector3.Distance(gmo, planet.transform.position) < 20)
                {
                    gpos = planet.transform.position;
                }

                guides[i].transform.position = gpos;
                guides[i].transform.LookAt(guides[i - 1].transform.position);
                guides[i].transform.Rotate(new Vector3(90, 0, 0));
            }
            /*
            Vector3[] lrpoints = new Vector3[extrapolationRange];

            Vector3 startPoint = gun.transform.position;

            int i = 2;

            lrpoints[0] = startPoint;
            lrpoints[1] = startPoint + gun.transform.up * i * 2.5f;

            while (i < extrapolationRange)
            {


                lrpoints[i] = lrpoints[i - 1] + (lrpoints[i - 1] - lrpoints[i - 2]);

                float gravity = gravityMultiplier / Mathf.Pow(Vector3.Distance((lrpoints[i - 1] + lrpoints[i]) / 2, planet.transform.position), 2);

                lrpoints[i] += (planet.transform.position - (lrpoints[i - 1] + lrpoints[i]) / 2) * gravity;

                if (Vector3.Distance(lrpoints[i], planet.transform.position) < 20)
                {
                    lrpoints[i] = lrpoints[i - 1];
                }

                // lrpoints[i] = startPoint - new Vector3(-pointHit.x, -pointHit.y, pointHit.z) * (0.05f * i);
                // lrpoints[i] = startPoint - pointHit * (0.05f * i);
                //float gravity = Mathf.Pow(gravityMultiplier / Vector3.Distance(this.transform.position, new Vector3(0, 0, 0)), 2);
                i++;
            }
            lineRend.SetPositions(lrpoints);
            */
        }

        if (Input.GetButtonDown("Fire1")) // && mainCamera.enabled
        {
            //for (int i = 0; i < 5; i++)
            //{
            //    GameObject newRod = Instantiate(rod, new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), gun.transform.position.z), gun.transform.rotation) as GameObject;
            //    newRod.GetComponent<rodScript>().giveThrust(thrust * speedMultiplier, gravityMultiplier * speedMultiplier, 0, gameObject, checkCamera, true);
            //}

            GameObject newRod = Instantiate(rod, new Vector3(0, 0, gun.transform.position.z), gun.transform.rotation) as GameObject;
            newRod.GetComponent<rodScript>().giveThrust(thrust, gravityMultiplier, speedMultiplier, 0, gameObject, checkCamera, true);
            //explosionZincrement++;

            if (rodCamera == null)
            {
                rodCamera = Instantiate(rodCameraPrefab, gun.transform.position * 1.01f, newRod.transform.rotation) as Camera;
                rodCamera.enabled = false;

            }
            else
            {
                rodCamera.transform.position = gun.transform.position * 1.01f;
                rodCamera.transform.rotation = newRod.transform.rotation;
            }

            rodCamera.transform.Rotate(new Vector3(-90, 0, 0));

            rodCamera.transform.parent = newRod.transform;

            newRod.GetComponent<rodScript>().giveCamera(rodCamera);

        }

        if (Input.GetKeyDown(KeyCode.Q))
        {

            if (mainCamera.enabled)
            {
                mainCamera.enabled = false;
                freeCamera.enabled = true;
                if (rodCamera != null)
                {
                    rodCamera.enabled = false;
                }
            }
            else if (freeCamera.enabled)
            {
                mainCamera.enabled = true;
                freeCamera.enabled = false;
                if (rodCamera != null)
                {
                    rodCamera.enabled = true;
                    mainCamera.enabled = false;
                }
            }
            else if (rodCamera != null && rodCamera.enabled)
            {
                mainCamera.enabled = true;
                freeCamera.enabled = false;

                rodCamera.enabled = false;

            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            float t = 1f;
            if (speedMultiplier < 10)
            {
                t = 10;
            }
            else
            {
                t = 0.001f;
            }
            speedMultiplier *= t;
            GameObject[] rods = GameObject.FindGameObjectsWithTag("Rod");
            if (rods != null && rods.Length > 0)
            {
                foreach (GameObject r in rods)
                {
                    r.GetComponent<rodScript>().adjustTimeflow(t);
                }
            }
        }
    }

    void OnPostRender()
    {
        Graphics.DrawTexture(new Rect(0, 0, 1, 1), cloudLayer, new Rect(0, 0, 1, 1), 0, 0, 0, 0, new Color(1, 1, 1), null);
    }

    public void giveHitCoordinates(Vector3 cp, float dV)
    {

        // rodCamera.enabled = false;
        //mainCamera.enabled = true;

        foreach (GameObject r in reticules)
        {
            Vector3 rPos = r.transform.position;
            float rDis = Vector3.Distance(cp, rPos);
            if (rDis < 1)
            {
                score += (int)(4 * dV / rDis);

            }
        }

        scoreField.GetComponent<TextMesh>().text = score.ToString();
    }

    public GameObject getPlanetGO()
    {
        return planet;
    }

    public void spawnCloudBlocker(float x, float y, float z, float adjust)
    {

        //float yAngle = Vector3.Angle(new Vector3(0, 0, v.z), new Vector3(0, v.y, v.z)) * (Mathf.PI / 180);
        //float xAngle = Vector3.Angle(new Vector3(0, 0, v.z), new Vector3(v.x, 0, v.z)) * (Mathf.PI / 180);

        //float yAngle = Vector3.Angle(new Vector3(0, 0, z), new Vector3(0, y, z)) * Mathf.Deg2Rad;
        //float xAngle = Vector3.Angle(new Vector3(0, 0, z), new Vector3(x, 0, z)) * Mathf.Deg2Rad;

        float yAngle = 90 - Vector3.Angle(new Vector3(0, 1, 0), new Vector3(x, y, z));
        float xAngle = Vector3.Angle(new Vector3(0, 0, z), new Vector3(x, 0, z));





        float mapX = 0;
        float mapY = 0;

        // x hor, y ver, z depth
        // green up, blue forward, red right
        if (z >= 0)
        {


            if (x >= 0)
            {
                xAngle -= 120 * planet.transform.rotation.y;
                mapX = xAngle * (31f / 180); // * (25 / adjust);
            }
            else
            {
                xAngle += 120 * planet.transform.rotation.y;
                mapX = -xAngle * (31f / 180); // * (25 / adjust);
                // -31 vs -10, -5 actual
            }
        }
        else
        {
            if (x < 0)
            {
                xAngle = -xAngle;
            }

            //mapY = Mathf.Sin(yAngle) * 35 / 3;
            //mapX = 31 - Mathf.Sin(xAngle) * 15.5f;

            //mapY = Mathf.Sin(yAngle) * 17.5f; // over
            //mapX = 31 - Mathf.Sin(xAngle) * 15.5f; // under

            // over

            xAngle += 120 * planet.transform.rotation.y;

            mapX = 31 - xAngle * (31f / 180); //  * (25 / adjust); // under
        }

        mapY = yAngle * (35f / 180); // * (25 / adjust);

        GameObject newCBlock = Instantiate(cloudBlocker, new Vector3(mapX, mapY, -120), Quaternion.identity) as GameObject;

        if (mapX > 31)
        {
            Instantiate(cloudBlocker, new Vector3(mapX - 62, mapY, -120), Quaternion.identity);
        }
        else if (mapX < -31)
        {
            Instantiate(cloudBlocker, new Vector3(mapX + 62, mapY, -120), Quaternion.identity);
        }

        // 0, 0, -z = equator left
        // -0, 0, -z = equator right
    }

    public void returnRodCamera(GameObject go)
    {
        if (rodCamera.transform.parent == go.transform)
        {
            rodCamera.transform.parent = null;
            rodCamera.enabled = false;
            mainCamera.enabled = true;
            rodCamera.transform.position = new Vector3(0, 0, 0);
        }
    }
}
