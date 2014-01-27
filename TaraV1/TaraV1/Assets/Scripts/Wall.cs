﻿using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {
    public Transform wallFront;
    public Transform wallBack;
	

    public void Move(float oldZ)
    {
        if (LevelLoader.Get().mainPlayer != null && !GameManager.isPaused && !CameraController.Get().Locked)
        {
            wallFront.transform.position = new Vector3(wallFront.transform.position.x, wallFront.transform.position.y,(LevelLoader.Get().mainPlayer.transform.position.z - 7.0f));
            wallBack.transform.position = new Vector3(wallBack.transform.position.x, wallBack.transform.position.y,(LevelLoader.Get().mainPlayer.transform.position.z + 9.0f));
        }
    }

    public void Delete()
    {
        Destroy(this.gameObject);
    }
}
