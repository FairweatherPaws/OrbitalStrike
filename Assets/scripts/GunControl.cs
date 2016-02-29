using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GunControl : MonoBehaviour
{

    public GameObject gun, gunControl, planet, mouseCollider, rod, scoreField, cloudBlocker;
    public Camera mainCamera, rodCameraPrefab, rodCamera, freeCamera, checkCamera;
    private int score, extrapolationRange;
    //private int explosionZincrement;
    private GameObject[] reticules;
    private LineRenderer lineRend;
    private float gravityMultiplier, speedMultiplier, thrust;
    public RenderTexture cloudLayer;



    // Use this for initialization
    void Start()
    {
        //UnityEngine.Cursor.visible = false;

        speedMultiplier = 0.03f;

        thrust = 1500;
        gravityMultiplier = 30f;
        extrapolationRange = 5;
        Physics.IgnoreLayerCollision(0, 8);
        //explosionZincrement = 0;
        reticules = GameObject.FindGameObjectsWithTag("Reticule");
        lineRend = GetComponent<LineRenderer>();
        lineRend.SetVertexCount(extrapolationRange);
        lineRend.SetColors(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0));
        lineRend.material.mainTextureScale = new Vector2(extrapolationRange, 1);
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
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 pointHit = hit.point;
            transform.LookAt(pointHit);

            Vector3[] lrpoints = new Vector3[extrapolationRange];

            Vector3 startPoint = gun.transform.position;

            int i = 2;

            lrpoints[0] = startPoint;
            lrpoints[1] = startPoint + gun.transform.up * i * 3;

            while (i < extrapolationRange)
            {

                lrpoints[i] = lrpoints[i - 1] + (lrpoints[i - 1] - lrpoints[i - 2]);

                float gravity = Mathf.Pow(gravityMultiplier / Vector3.Distance(lrpoints[i], planet.transform.position), 2);

                lrpoints[i] -= lrpoints[i] * gravity * 0.032f;


                // lrpoints[i] = startPoint - new Vector3(-pointHit.x, -pointHit.y, pointHit.z) * (0.05f * i);
                // lrpoints[i] = startPoint - pointHit * (0.05f * i);
                //float gravity = Mathf.Pow(gravityMultiplier / Vector3.Distance(this.transform.position, new Vector3(0, 0, 0)), 2);
                i++;
            }
            lineRend.SetPositions(lrpoints);
        }

        if (Input.GetButtonDown("Fire1")) // && mainCamera.enabled
        {
            for (int i = 0; i < 5; i++)
            {
                GameObject newRod = Instantiate(rod, new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), gun.transform.position.z), gun.transform.rotation) as GameObject;
                newRod.GetComponent<rodScript>().giveThrust(thrust * speedMultiplier, gravityMultiplier * speedMultiplier, 0, gameObject, checkCamera, true);
            }

            //GameObject newRod = Instantiate(rod, new Vector3(0,0, gun.transform.position.z), gun.transform.rotation) as GameObject;
            //newRod.GetComponent<rodScript>().giveThrust(thrust * speedMultiplier, gravityMultiplier * speedMultiplier, 0, gameObject, checkCamera, true);
            //explosionZincrement++;


            //rodCamera = Instantiate(rodCameraPrefab, gun.transform.position * 1.01f, newRod.transform.rotation) as Camera;

            //rodCamera.transform.Rotate(new Vector3(-90, 0, 0));

            //rodCamera.transform.parent = newRod.transform;


            //  mainCamera.enabled = false;
            //  rodCamera.enabled = true;

        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            mainCamera.enabled = !mainCamera.enabled;
            freeCamera.enabled = !freeCamera.enabled;
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
                mapX = - xAngle * (31f / 180); // * (25 / adjust);
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


}
