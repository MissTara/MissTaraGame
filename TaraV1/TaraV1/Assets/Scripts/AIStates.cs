/* AIStates.cs
 * Author: Alfred Lai
 * Last Modified By: Dexter
 * Description: 
 * Provides states for enemy and plays different animations depends on different states.
 * */
using UnityEngine;
using System.Collections;

public class AIStates : MonoBehaviour
{

    public enum states { Idle, Run, Death, Attack, Hit, Dance };
    public bool hitted = false;
    AIPathCustom AIPathing;
    CharacterController controller;

    public states EnemyState;
    protected bool died, batAtt = false;
    protected float delay, timer;
    public float delayer = 5.0f;
    protected float batdelay = 3.4f;

    public Transform player;
    private float speed = 1.0f;
    private Vector3 dir;
	
	private GameManager GM;
	private UnitEnemy tmp;				//Holder for the Bat's UnitEnemy script
	private Vector3 speedSwoop;			//Speed for the attack of the bat
	private bool swoop,batWait = false;			//For the bat only

    void Start()
    {
        EnemyState = states.Idle;
        delay = Time.time;
        AIPathing = GetComponent<AIPathCustom>();
        controller = GetComponent<CharacterController>();
		GM= GameManager.Get();
		if (gameObject.tag == "Bat"){
			player = GM.objPlayer.transform;
			tmp = GetComponent<UnitEnemy>();
		}
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
        else if (EnemyState == states.Dance)
        {
            if (!died)
            {
                Debug.Log("I am dancing");
                animation.Play("CriticalHit");

                if (!animation.IsPlaying("CriticalHit"))
                {
                    animation.Stop();
                    animation.Play("Death1");
                    died = true;
                }
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
			StopCoroutine("batAttack");
            EnemyState = states.Run;
            animation.Play("BatFly");
			AIPathing.canMove = true;
            AIPathing.canSearch = true;
            tmp.enabled= true;
			speedSwoop = new Vector3(0.0f,0.0f,0.0f);
            batAtt = false;
			swoop = false;
			batWait = true;
			StartCoroutine("attackDelay");
        }

        if (EnemyState == states.Idle && !batAtt)
            animation.PlayQueued("BatFly");
        else if (EnemyState == states.Run && !batAtt)
            animation.PlayQueued("BatFly");
        else if (EnemyState == states.Attack && !batAtt && !batWait)
        {
            AIPathing.canMove = false;
            AIPathing.canSearch = false;
            delay = Time.time;
            dir = player.position - transform.position; // calculate the target direction...
            speedSwoop = new Vector3(dir.x*0.06f,-0.1f,dir.z*0.06f);
			this.transform.LookAt(new Vector3(GM.objPlayer.transform.position.x,this.transform.position.y,GM.objPlayer.transform.position.z));	//Look in the direction of the player
			tmp.enabled = false;
			batAtt = true;
			swoop = true;
        }
		else if (EnemyState == states.Attack && batAtt && !died){
			AIPathing.canMove = false;
			AIPathing.canSearch = false;
			animation.Play("BatAttack");
			StartCoroutine("batAttack");			
		}
		else if (EnemyState == states.Dance){
			if (!died)
            {
			   batAtt = false;
			   StopCoroutine("batAttack");
			   speedSwoop = new Vector3(0.0f,0.0f,0.0f);
               animation.Play("BatDance");
               if (!animation.IsPlaying("BatDance"))
               {
                   EnemyState = states.Death;
               }
			}
		}
        else if (EnemyState == states.Death)
        {
            if (!died)
            {
				
                animation.Stop();
                animation.Play("BatDead");
				StopCoroutine("batAttack");
				if (!tmp.enabled)
					tmp.enabled = true;
				speedSwoop = new Vector3(0.0f,0.0f,0.0f);
				controller.center = new Vector3(0, 0.03f, 0);
				this.GetComponent<CharacterController>().stepOffset = 0.03f;
				died = true;
            }
        }
        else
			controller.Move(dir * speed * Time.deltaTime);
		
		if (hitted && !died){
			animation.Play("BatHit");
			hitted = false;
		}
    }
	
	IEnumerator batAttack(){
		yield return new WaitForSeconds(1.9f);					//Delay for the bat to attack
		if (swoop && !died && EnemyState != states.Dance){
			this.transform.position += speedSwoop;
			speedSwoop.y += 0.15f*Time.deltaTime;
		}		
	}
	
	IEnumerator attackDelay(){
		yield return new WaitForSeconds(1.0f);
		batWait = false;
	}
}
