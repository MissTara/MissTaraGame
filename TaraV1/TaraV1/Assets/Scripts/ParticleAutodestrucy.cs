/* ParticleAutodestrucy.cs
 * Author: Luke Jingwei Sun
 * Last Modified By: Luke Jingwei Sun
 * Description: 
 * Auto destories the particle object when it has multiple children particle effects
 * */
using UnityEngine;
using System.Collections;

public class ParticleAutodestrucy : MonoBehaviour {

	void LateUpdate(){
		if (transform.childCount == 0){
			Destroy(gameObject);
		}
	}
}
