using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunRootParticle : MonoBehaviour {
    public GameObject stunParticlePrefab;
    public float radius = 0.3f;

	// Use this for initialization
	void Start () {
		if(stunParticlePrefab == null)
        {
            stunParticlePrefab = Resources.Load<GameObject>("Particles/StunParticle");
        }

        //InvokeRepeating("CreateParticle", 0.5f, 4);
	}

    void CreateParticle()
    {
        GameObject particle = GameObject.Instantiate(stunParticlePrefab, this.transform);
        particle.transform.position = new Vector3(transform.position.x, transform.position.y, radius);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
