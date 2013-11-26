using UnityEngine;
using System.Collections;

public class MissileTarget : MonoBehaviour {

    public float rotationSpeed = 0.5f;
	
	// Update is called once per frame
	void Update () {

        this.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
	
	}
}
