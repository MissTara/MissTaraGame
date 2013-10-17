// made by Dexter

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
    [System.NonSerialized]
    public GameObject mainPlayer;

    #endregion
    
	protected Plane[] planes;

    private static LevelLoader m_Instance = null;

    public bool isPlayerCreated()
    {
        if (mainPlayer == null)
            return false;
        else
            return true;
    }

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
            if (isPlayerCreated())
            {
                planes = GeometryUtility.CalculateFrustumPlanes(camera);
                foreach (GameObject building in buildings)
                {
                    if (camera.gameObject.transform.localPosition.z < building.gameObject.transform.localPosition.z)
                    {
                        building.gameObject.SetActive(true);
                    }
                    if (GeometryUtility.TestPlanesAABB(planes, building.collider.bounds) == false)
                    {
                        building.gameObject.SetActive(false);
                        //DestroyImmediate(building.gameObject, true);
                    }
                }

                foreach (GameObject enemy in GameManager.Get().objEnemies)
                {
                    enemy.GetComponent<AIPathCustom>().target = mainPlayer.transform;
                }
            }
            else
            {
                mainPlayer = Instantiate(ResourceManager.Get().preMainPlayer, Vector3.zero, Quaternion.Euler(Vector3.zero)) as GameObject;
                CameraController.Get().cameraTarget = mainPlayer.transform;
            }
        }
	}
}
