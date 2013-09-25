using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelLoader : MonoBehaviour
{

    #region Public Variables
    //public int howManyBuildings;
    public Camera camera;
    
    //public float distanceBetweenBuildings = -40;
    //public float initialPositionBuilding = 40;
    public List<GameObject> buildings;
    [System.NonSerialized] public bool levelLoaded = false;

    #endregion


	protected Plane[] planes;

    private static LevelLoader m_Instance = null;

    public static LevelLoader Get()
    {
        if (m_Instance == null)
            m_Instance = (LevelLoader)FindObjectOfType(typeof(LevelLoader));

        return m_Instance;
    }

    // Use this for initialization
	void Start () {
        levelLoaded = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (GameManager.isPaused == false)
        {
            planes = GeometryUtility.CalculateFrustumPlanes(camera);
            foreach (GameObject building in buildings)
            {
                if (!building.gameObject.activeSelf && building.gameObject.transform.localPosition.z < camera.gameObject.transform.localPosition.z)
                {
                    
                    Debug.Log(building.name + " has been detected!");
                    building.gameObject.SetActive(true);
                }
                if (GeometryUtility.TestPlanesAABB(planes, building.collider.bounds) == false)
                {
                    Debug.Log("Nothing has been detected");
                    building.gameObject.SetActive(false);
                    //DestroyImmediate(building.gameObject, true);
                }
            }
        }
	}
}
