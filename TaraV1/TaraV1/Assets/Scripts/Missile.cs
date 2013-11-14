using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour {
	
	void OnTriggerEnter(Collider other){
		if(other.tag == "Target"){
			GameObject hit = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere),this.transform.position, this.transform.rotation) as GameObject;
			hit.transform.localScale = new Vector3(5,5,5);
			Destroy(other.gameObject);
			Destroy(this.gameObject);
		}
	}
	
}
