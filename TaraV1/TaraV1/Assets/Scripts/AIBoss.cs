﻿using UnityEngine;
using System.Collections;



public class AIBoss : MonoBehaviour
{
    [System.NonSerialized]
    public GameObject healthBar;


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

    public int missilesToFireMin = 2;
    public int missilesToFireMax = 12;

    #endregion

    #region private variables
    private bool isRotating;
    private bool canRotate;
    private string direction;
    private float tempX;
    private float rotateSpeed = 4.0f;

    private Vector3 worldPosition;
    private Vector3 screenPosition;
    private float hp;
    private float maxHP;
    private bool angry;
    #endregion

    void Awake()
    {
        angry = false;
        isRotating = false;
        direction = "";
        canRotate = true;

        this.GetComponent<AIPathCustom>().CurHP = this.GetComponent<AIPathCustom>().MaxHP;

        hp = this.GetComponent<AIPathCustom>().CurHP;
        maxHP = this.GetComponent<AIPathCustom>().MaxHP;

        healthBar = Instantiate(ResourceManager.Get().bossHealthBar, LevelLoader.Get().camera.WorldToViewportPoint(GameObject.Find("bar").transform.position), Quaternion.Euler(Vector3.zero)) as GameObject;
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

        if (healthBar != null)
        {
            screenPosition = LevelLoader.Get().camera.WorldToScreenPoint(GameObject.Find("bar").transform.position);
            screenPosition.z = 0;
            healthBar.transform.position = LevelLoader.Get().camera.WorldToViewportPoint(GameObject.Find("bar").transform.position);
            hp = this.GetComponent<AIPathCustom>().CurHP;

            float healthBarWidth = hp / maxHP;
            healthBar.gameObject.transform.Find("bar").guiTexture.pixelInset = new Rect(-4f, -6f, 60 * healthBarWidth, 10);
        }

        if (healthBar != null && this.GetComponent<AIStates>().EnemyState == AIStates.states.Death)
        {
            Destroy(this.healthBar);
        }
    }

    protected void defend(Transform player)
    {

        if (player != null)
        {
            if (player.GetComponent<UnitPlayer>().projectile != null 
                && player.GetComponent<UnitPlayer>().projectile.isReadByBoss == false 
                && !isRotating
                && this.GetComponent<AIStates>().EnemyState != AIStates.states.Death
                && this.GetComponent<AIStates>().EnemyState != AIStates.states.Attack
				&& (this.GetComponent<AIPathCustom>().CurHP / this.GetComponent<AIPathCustom>().MaxHP) < 0.5f)
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
                this.GetComponent<AIStates>().EnemyState = AIStates.states.Jump;
                dodge(direction);
            }
        }
    }

    void dodge(string direction)
    {
        tempX += this.GetComponent<AIPathCustom>().speed * rotateSpeed / 8;
        if (tempX < dodgeAngle)
        {
            if (direction == "left" && canRotate == true)
            {
                transform.Translate(this.GetComponent<AIPathCustom>().speed * -rotateSpeed * Time.deltaTime, 0, -this.GetComponent<AIPathCustom>().speed * Time.deltaTime);
            }
            else if (direction == "right" && canRotate == true)
            {
                transform.Translate(this.GetComponent<AIPathCustom>().speed * rotateSpeed * Time.deltaTime, 0, this.GetComponent<AIPathCustom>().speed * Time.deltaTime);
            }
        }
        else
        {
            if (!this.GetComponent<AIStates>().died)
                this.GetComponent<AIStates>().EnemyState = AIStates.states.Run;
            else
                this.GetComponent<AIStates>().EnemyState = AIStates.states.Death;
            isRotating = false;
            tempX = 0;
            this.GetComponent<AIPathCustom>().target = LevelLoader.Get().mainPlayer.transform;
            this.GetComponent<AIPathCustom>().SearchPathAgain();
            return;
        }
    }

    void OnTriggerEnter(Collider other)
    {
    	if (other.tag == "Sidewalls" && isRotating){
            
		}
    } 

}
