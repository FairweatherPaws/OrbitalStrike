using UnityEngine;
using System.Collections;

public class explosionSelfdestruct : MonoBehaviour {

    private float timer = 0f, gMod = -0.001f;
    private ParticleSystem.Particle[] particles;
    private ParticleSystem parSys;
    private GameObject planet;
    private bool pulse;

	// Use this for initialization
	void Start () {
        parSys = GetComponent<ParticleSystem>();
        GameObject[] gos = GameObject.FindGameObjectsWithTag("SFX");
        parSys.maxParticles = (int)(4000 / (20 + gos.Length));

    }
	
	// Update is called once per frame
	void Update () {

        InitialiseIfNeeded();

        int livingParticles = parSys.GetParticles(particles);

        // hacky particle gravity solution, consider switching to accomodate multiple gravity wells

        Vector3 parPos;
        Vector3 parVel;
        for (int i = 0; i < livingParticles; i++)
        {
            parPos = particles[i].position;
            parVel = particles[i].velocity;
            parVel += new Vector3(parPos.x * gMod, parPos.y * gMod, parPos.z * gMod);
            if (pulse)
            {
                parVel += new Vector3(parPos.x * gMod, parPos.y * gMod, parPos.z * gMod);
            }

            particles[i].velocity = parVel;

        }

        parSys.SetParticles(particles, livingParticles);

        timer += Time.deltaTime;
        if (timer > 3 && pulse)
        {
            DestroyObject(this.gameObject);
        }
        if (timer > 10 && !pulse)
        {
            DestroyObject(this.gameObject);
        }

    }

    void InitialiseIfNeeded()
    {
        if (parSys == null)
        {
            parSys = GetComponent<ParticleSystem>();
        }

        if (particles == null || particles.Length < parSys.maxParticles)
        {
            particles = new ParticleSystem.Particle[parSys.maxParticles];
        }
    }

    public void givePulseOrder()
    {
        pulse = true;
    }
}
