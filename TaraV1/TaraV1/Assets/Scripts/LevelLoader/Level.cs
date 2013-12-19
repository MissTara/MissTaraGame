using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {

    public float levelNumber;
    public List<GameObject> buildingsToHideForCamera;
	public List<GameObject> dancers;
    public bool isBossLevel;

    public Level()
    {
        
    }
	
	void Start(){
			
	}

    public void Delete()
    {
        Destroy(this.gameObject);
    }
}
