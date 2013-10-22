// made by Dexter

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Level))]

public class LevelLoader : MonoBehaviour
{

    #region Public Variables
    public Camera camera;

    public List<GameObject> levels;
    public int loadLevel = 1;

    [System.NonSerialized] public bool levelLoaded = false;
    [System.NonSerialized]
    public GameObject mainPlayer;

    #endregion
    
	protected Plane[] planes;

    private static LevelLoader m_Instance = null;
    private GameObject levelToLoad;

    public bool IsPlayerCreated()
    {
        if (mainPlayer == null)
            return false;
        else
            return true;
    }

    public bool IsLevelLoaded()
    {
        if (levelToLoad == null)
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
            if (IsLevelLoaded() == false)
            {
                foreach (GameObject levelNum in levels)
                {
                    Level level = (Level)levelNum.GetComponent(typeof(Level));
                    if (loadLevel == level.levelNumber)
                    {
                        levelToLoad = Instantiate(levelNum.gameObject, Vector3.zero, Quaternion.Euler(Vector3.zero)) as GameObject;
                        return;
                    }
                }
            }
            if (IsPlayerCreated() && IsLevelLoaded())
            {
                
                planes = GeometryUtility.CalculateFrustumPlanes(camera);
                foreach (GameObject building in ((Level)levelToLoad.GetComponent(typeof(Level))).buildingsToHideForCamera)
                {
                    if (building != null)
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
