using UnityEngine;
using System;


public class AssetDestuction : MonoBehaviour {
	
	public bool enabled;

	
	void OnBecameVisible(){
		enabled = false;
		print("VISIBLE");
		}

	void OnBecameInvisible(){	
		enabled = true;
		Destroy(this.gameObject);
		Resources.UnloadUnusedAssets();
		print("INVISIBLE!");		
	}
}