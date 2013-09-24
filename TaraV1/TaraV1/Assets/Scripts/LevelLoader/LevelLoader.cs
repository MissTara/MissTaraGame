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
        //if (GameManager.isPaused == false && levelLoaded == false)
        //{
        //    for (int it = 0; it < howManyBuildings; it++)
        //    {
        //        Building tempBuild;
        //        Vector3 tempBuildPosition = new Vector3(-65, 90, initialPositionBuilding + distanceBetweenBuildings);
        //        tempBuild = Instantiate(Resources.Load("Buildings/building1", typeof(Building)), tempBuildPosition, Quaternion.Euler(0, 270, 0)) as Building;
        //        //tempBuild.transform.localPosition = new Vector3(-65, 90, initialPositionBuilding + distanceBetweenBuildings);
        //        Debug.Log(tempBuild);
        //        buildings.Add(tempBuild as Building);

        //        distanceBetweenBuildings += distanceBetweenBuildings;
        //    }
        //    levelLoaded = true;
        //}
        if (GameManager.isPaused == false)
        {
            planes = GeometryUtility.CalculateFrustumPlanes(camera);
            foreach (GameObject building in buildings)
            {
                if (!building.gameObject.activeSelf && building.gameObject.transform.localPosition.z < camera.gameObject.transform.localPosition.z)
                    //(GeometryUtility.TestPlanesAABB(planes, building.collider.bounds) && Physics.Raycast(camera.ScreenPointToRay(camera.gameObject.transform.position + new Vector3(0,0,-100)),out hit, camera.farClipPlane))
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
