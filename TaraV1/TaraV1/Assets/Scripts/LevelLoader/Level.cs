using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {

    public float levelNumber;
    public List<GameObject> buildingsToHideForCamera;

    public Level()
    {
        
    }

    public void Delete()
    {
        Destroy(this.gameObject);
    }
}
