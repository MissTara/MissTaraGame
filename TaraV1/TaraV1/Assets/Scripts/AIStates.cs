/* AIStates.cs
 * Author: Alfred Lai
 * Last Modified By: Alfred Lai
 * Description: 
 * Provides states for enemy and plays different animations depends on different states.
 * */
using UnityEngine;
using System.Collections;

public class AIStates : MonoBehaviour
{
	//hey, lookit me
	//this is a test
    public enum states { Idle, Run, Death, Attack, Hit };
    public bool hitted = false;
    AIPathCustom AIPathing;
    CharacterController controller;

    public states EnemyState;
    protected bool died, batAtt = false;
    protected float delay, timer;
    public float delayer = 5.0f;
    protected float batdelay = 5.0f;

    public Transform player;
    private float speed = 1.0f;
    private Vector3 dir;

    void Start()
    {
        EnemyState = states.Idle;
        //PlayState(states.idle);
        delay = Time.time;
        AIPathing = GetComponent<AIPathCustom>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        //transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        if (gameObject.tag == "Alien")
            PlayAlien();
        else if (gameObject.tag == "Bat")
            PlayBat();
    }

    private void PlayAlien()
    {
        //Random number generator for testing
        //Random random = new Random();
        float randomNumber = Random.value * 100;
        //Debug.Log(randomNumber);

        //animation.Play(EnemyState.ToString());

        if (EnemyState == states.Idle)
            animation.Play("Idle1");
        else if (EnemyState == states.Run)
            animation.Play("Run2");
        else if (EnemyState == states.Attack)
        {
            if (Time.time <= delay + delayer)
            {
                animation.PlayQueued("FightIdle");
            }
            else
            {
                if (randomNumber < 33.3f)
                    animation.Play("Punch");
                else if (randomNumber > 33.3f && randomNumber < 66.7f)
                    animation.Play("Punch2");
                else
                    animation.Play("Punch3");
                delay = Time.time;
                //EnemyState = states.Idle;
            }
        }
        else if (EnemyState == states.Death)
        {
            if (!died)
            {
                animation.Stop();
                if (randomNumber < 33)
                    animation.Play("Death1");
                else
                    animation.Play("Death2");
                Debug.Log(states.Death.ToString());
                died = true;
            }
        }

        if (hitted == true && !died)
        {
            if (randomNumber < 50.0f)
            {
                animation["BellyHit"].layer = 1;
                animation.Play("BellyHit");
                animation["BellyHit"].weight = 0.7f;
            }
            else
            {
                animation["HeadHit"].layer = 1;
                animation.Play("HeadHit");
                animation["HeadHit"].weight = 0.7f;
            }
            hitted = false;
        }
    }

    //Function for Bat enemies
    private void PlayBat()
    {
        if (batAtt && Time.time >= delay + batdelay && !died)
        {
            EnemyState = states.Run;
            //AIPathing.canMove = true;
            AIPathing.canSearch = true;
            Debug.Log(EnemyState);
            batAtt = false;
        }

        if (EnemyState == states.Idle && !batAtt)
            animation.Play("Fly");
        else if (EnemyState == states.Run && !batAtt)
            animation.Play("Fly");
        else if (EnemyState == states.Attack && !batAtt)
        {
            //AIPathing.canMove = false;
            AIPathing.canSearch = false;
            delay = Time.time;
            dir = player.position - transform.position; // calculate the target direction...
            batAtt = true;
            animation.Play("FlyAttack");
            animation.PlayQueued("FlyAttackMid");
            animation.PlayQueued("FlyAttackMiss");
            animation.PlayQueued("Fly");
        }
        else if (EnemyState == states.Death)
        {
            controller.height = 0.1f;
            controller.center = new Vector3(0, 0.1f, 0);
            if (!died)
            {
                animation.Stop();
                animation.Play("BatDeath");
                died = true;
            }
        }
        else
            controller.Move(dir * speed * Time.deltaTime);
        //transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }
}
