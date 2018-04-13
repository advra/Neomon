using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunParticle : MonoBehaviour {
    public float radius = 0.3f;
    float t;
    [SerializeField]
    float startingX;
    [SerializeField]
    float startingZ;
	// Use this for initialization
	void Start () {
        startingX = this.transform.localPosition.x;
        startingZ = this.transform.localPosition.z;
    }
	
	// Update is called once per frame
	void Update () {
        t +=Time.deltaTime;
        float x =   Mathf.Cos(t) * startingX;
        float z =   Mathf.Sin(t) * startingZ;
        Vector3 newPos = new Vector3(x, this.transform.position.y, z);
        transform.position = newPos;

    }
}
