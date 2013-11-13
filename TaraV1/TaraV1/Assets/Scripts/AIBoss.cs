﻿using UnityEngine;
using System.Collections;



public class AIBoss : MonoBehaviour
{

    public enum State
    {
        Defending,
    };

    #region public variables
    // prefab
    [System.NonSerialized]
    public Transform targetPlayer;

    [System.NonSerialized]
    public Transform bullet;

    public float distanceBetweenBulletAndEnemy = 40f;

    public float dodgeAngle = 5f;

    #endregion

    #region private variables
    private bool isRotating;
    private string direction;
    private float tempX;
    private float rotateSpeed = 8.0f;
    #endregion

    void Awake()
    {
        isRotating = false;
        direction = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPlayer == null)
        {
            targetPlayer = GetComponent<AIPathCustom>().target;
            return;
        }
        if (targetPlayer != null)
            defend(targetPlayer);
    }

    protected void defend(Transform player)
    {

        if (player != null)
        {
            if (player.GetComponent<UnitPlayer>().projectile != null && player.GetComponent<UnitPlayer>().projectile.isReadByBoss == false && !isRotating)
            {
                if (Vector3.Distance(this.transform.position, player.GetComponent<UnitPlayer>().projectile.transform.position) > 0)
                {
                    if (Vector3.Distance(this.transform.position, player.GetComponent<UnitPlayer>().projectile.transform.position) < distanceBetweenBulletAndEnemy)
                    {
                        Debug.Log("left");
                        isRotating = true;
                        tempX = 0;
                        direction = "left";
                        player.GetComponent<UnitPlayer>().projectile.isReadByBoss = true;

                    }
                }
                else
                {
                    if (Vector3.Distance(this.transform.position, player.GetComponent<UnitPlayer>().projectile.transform.position) < -distanceBetweenBulletAndEnemy)
                    {
                        Debug.Log("right");
                        isRotating = true;
                        tempX = 0;
                        direction = "right";
                        player.GetComponent<UnitPlayer>().projectile.isReadByBoss = true;

                    }
                }
            }

            if (isRotating)
            {
                dodge(direction);
            }
        }
    }

    void dodge(string direction)
    {
        tempX += this.GetComponent<AIPathCustom>().speed * rotateSpeed / 3;
        if (tempX < dodgeAngle)
        {
            if (direction == "left")
            {
                transform.Translate(this.GetComponent<AIPathCustom>().speed * -rotateSpeed * Time.deltaTime, 0, -this.GetComponent<AIPathCustom>().speed * Time.deltaTime);
            }
            else
            {
                transform.Translate(this.GetComponent<AIPathCustom>().speed * rotateSpeed * Time.deltaTime, 0, this.GetComponent<AIPathCustom>().speed * Time.deltaTime);
            }
        }
        else
        {
            isRotating = false;
            tempX = 0;
            return;
        }
    }

}