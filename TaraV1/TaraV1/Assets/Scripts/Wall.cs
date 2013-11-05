using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

    public Transform wallLeft;
    public Transform wallRight;
    public Transform wallFront;
    public Transform wallBack;
	

    public void Move(float oldZ)
    {
        if (LevelLoader.Get().mainPlayer != null && !GameManager.isPaused && !CameraController.Get().Locked)
        {
            wallLeft.transform.position = new Vector3(wallLeft.transform.position.x, wallLeft.transform.position.y, wallLeft.transform.position.z + (((LevelLoader.Get().mainPlayer.transform.position.z - oldZ) * Time.deltaTime) * 10));
            wallRight.transform.position = new Vector3(wallRight.transform.position.x, wallRight.transform.position.y, wallRight.transform.position.z + (((LevelLoader.Get().mainPlayer.transform.position.z - oldZ) * Time.deltaTime) * 10));
            wallFront.transform.position = new Vector3(wallFront.transform.position.x, wallFront.transform.position.y, wallFront.transform.position.z + (((LevelLoader.Get().mainPlayer.transform.position.z - oldZ) * Time.deltaTime) * 10));
            wallBack.transform.position = new Vector3(wallBack.transform.position.x, wallBack.transform.position.y, wallBack.transform.position.z + (((LevelLoader.Get().mainPlayer.transform.position.z - oldZ) * Time.deltaTime) * 10));
        }
    }

    public void Delete()
    {
        Destroy(this.gameObject);
    }
}
