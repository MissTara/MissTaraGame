// made by Dexter

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Level))]

public class LevelLoader : MonoBehaviour
{

    #region Public Variables
    public Camera camera;
    public GUIText levelText;
    //public Transform cameraObject;

    public List<GameObject> levels;
    public float loadLevel = 1;

    [System.NonSerialized] public bool levelLoaded = false;
    [System.NonSerialized]
    public GameObject mainPlayer;
	public GameObject Bunny;

    #endregion
    
	protected Plane[] planes;

    private static LevelLoader m_Instance = null;
    private GameObject levelToLoad;
    private int levelTextNum;
	public bool bossLevel = false;

    [System.NonSerialized]public GameObject collideWall;
    [System.NonSerialized]public bool boolSetNewLevel=false;

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
        levelTextNum = (int)loadLevel;
        levelLoaded = true;
        boolSetNewLevel = true;
        levelText.text = "Level " + levelTextNum;
        
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (GameManager.isPaused == false && levelLoaded == true)
        {
            if (boolSetNewLevel)
            {
                if (IsLevelLoaded() == false)
                {
                    foreach (GameObject levelNum in levels)
                    {
                        Level level = (Level)levelNum.GetComponent(typeof(Level));
                        if (loadLevel == level.levelNumber)
                        {
                            GameObject tmpLevel;
                            tmpLevel = Instantiate(levelNum.gameObject, Vector3.zero, Quaternion.Euler(Vector3.zero)) as GameObject;
                            levelToLoad = tmpLevel;

                            if (level.GetComponent<Level>().isBossLevel){
								bossLevel = true;
								if(loadLevel == 1.1){
									levelText.text = "Club";
								}else if (loadLevel == 2.1){
									levelText.text = "Spaceship";
								}else if (loadLevel == 3.1){
									levelText.text = "Home World";
								}
								CameraController.Get().cameraSelf.transform.position -= new Vector3(20,-20,-10);
							}else{
								GameObject tmpCollideWall;
								if(levelTextNum == 1){
									levelText.text = "Paris";	
								}else if(levelTextNum == 2){
									levelText.text = "Spaceship";
								}else if(levelTextNum == 3){
                                	levelText.text = "Home World";
								}
                            	tmpCollideWall = Instantiate(ResourceManager.Get().preCollideWall, Vector3.zero, Quaternion.Euler(Vector3.zero)) as GameObject;
	                            collideWall = tmpCollideWall;
								bossLevel = false;
								CameraController.Get().Reset();
                            	return;
							}
                        }
                    }
                }
                if (mainPlayer != null)
                {
                    mainPlayer.transform.localPosition = new Vector3(0, 0, -10);
					Bunny = Instantiate(ResourceManager.Get().preBunny,Vector3.zero,Quaternion.Euler(Vector3.zero)) as GameObject;
                    //cameraObject.transform.localPosition = new Vector3(-27, 0, 0);
                    boolSetNewLevel = false;
                    return;
                }
                else
                {
                    mainPlayer = Instantiate(ResourceManager.Get().preMainPlayer, Vector3.zero, Quaternion.Euler(Vector3.zero)) as GameObject;
                    mainPlayer.transform.localPosition = new Vector3(0, 0, -10);
					Bunny = Instantiate(ResourceManager.Get().preBunny,Vector3.zero,Quaternion.Euler(Vector3.zero)) as GameObject;
                    boolSetNewLevel = false;
                    return;
                }
            }

            if (IsPlayerCreated() && IsLevelLoaded() && boolSetNewLevel == false)
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
                if (mainPlayer != null)
                {
                    foreach (GameObject enemy in GameManager.Get().objEnemies)
                    {
                        enemy.GetComponent<AIPathCustom>().target = mainPlayer.transform;
                    }
						CameraController.Get().cameraTarget = mainPlayer.transform;
                }
            }
        }
	}

    public void SetLevel(float levelNum){
        boolSetNewLevel = true;
        mainPlayer.transform.localPosition = new Vector3(0, 0, -10);
        CameraController.Get().Reset();
        //cameraObject.transform.localPosition = new Vector3(-27, 0, 0);
        levelLoaded = false;

        if (levelToLoad){
            ((Level)(levelToLoad.GetComponent(typeof(Level)))).Delete();
            ((Wall)(collideWall.GetComponent(typeof(Wall)))).Delete();
            levelToLoad = null;
            levelLoaded = true;
            loadLevel = levelNum;
            
        }
    }
}
