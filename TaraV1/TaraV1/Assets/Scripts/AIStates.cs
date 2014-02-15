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

    public enum states { Idle, Run, Death, Attack, Hit, Dance, Jump };
    public bool hitted = false;
    AIPathCustom AIPathing;
    CharacterController controller;

    public states EnemyState;
    [System.NonSerialized] public bool died, attacking = false;
    protected float delay, timer;
    public float delayer = 5.0f;
    protected float batdelay = 3.4f;

    public Transform player;
    private float speed = 1.0f;
    private Vector3 dir;
	
	private GameManager GM;
	private UnitEnemy tmp;				//Holder for the Bat's UnitEnemy script
	public Vector3 speedSwoop;			//Speed for the attack of the bat
	/* Steven:
	 * swoop is if the bat is swooping.
	 * BatAtt is if the bat is attacking
	 * attackWait is a cooldown on all attacks so they dont attack in quick succession
	 * */
	public bool swoop,autoBossAttack = false;
	private bool batAtt,attackWait = false;
	public int bossAttack = 0;
	public float bossAutoDelay = 12.0f;
	
	private Vector3 larvaDir;
	public bool larvaRoll = false;

    void Start(){
        EnemyState = states.Idle;
        delay = Time.time;
        AIPathing = GetComponent<AIPathCustom>();
        controller = GetComponent<CharacterController>();
		GM= GameManager.Get();
		if (gameObject.tag == "Bat" || gameObject.tag == "Larva"){
			player = GM.objPlayer.transform;
			tmp = GetComponent<UnitEnemy>();
		}else if (gameObject.tag == "MechBoss"){
			StartCoroutine("bossAutoAttack");
		}
    }

    void Update(){
		//Level 1 enemies
		if (gameObject.tag == "Bunny")
			PlayBunny();
        else if (gameObject.tag == "GunAlien")
            PlayGunAlien();
        else if (gameObject.tag == "Bat")
            PlayBat();
		else if (gameObject.tag == "Bear")
			PlayBear();
		else if (gameObject.tag == "Wolf")
			PlayWolf();
		//Level 2 enemies
		else if (gameObject.tag == "Alien")
			PlayAlien();
		else if (gameObject.tag == "Hover")
			PlayHover();
		else if (gameObject.tag == "Slime")
			playSlime();
		else if (gameObject.tag == "Sword")
			playSword();
		//Level 3 enemies
		else if (gameObject.tag == "Spear")
			PlaySpear();
		else if (gameObject.tag == "Doggy")
			playDoggy();
		else if (gameObject.tag == "Larva")
			playLarva();
		else if (gameObject.tag == "Helmet")
			playHelmet();
		//Bosses
		else if (gameObject.tag == "MechBoss")
			PlayMechBoss();
		else if (gameObject.tag == "CaptainBoss")
			playCaptainBoss();
		else if (gameObject.tag == "Queen")
			playQueen();
    }
	
	private void PlayBunny(){
		if (EnemyState == states.Idle)
            animation.Play("LBunnyIdle");
        else if (EnemyState == states.Run)
			animation.Play("LBunnyHop");
	}

    private void PlayGunAlien()
    {
        if (EnemyState == states.Idle)
            animation.Play("GunAlienIdle");
        else if (EnemyState == states.Run)
            animation.Play("GunAlienWalk");
        else if (EnemyState == states.Attack && !died && !attackWait){
			if(!attacking){					//If he isnt already attacking, start doing so
				AIPathing.canMove = false;
				AIPathing.canSearch = false;
				animation.Play("GunAlienAttack");
	            delay = Time.time;
				attacking = true;
			}else{							//Once the attack animation is done
				if(!animation.IsPlaying("GunAlienAttack")){
					animation.Stop("GunAlienAttack");
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
					EnemyState = states.Run;
					attacking = false;
					StartCoroutine("attackDelay");
				}
			}
        }
        else if (EnemyState == states.Death){
            if (!died)
            {
                animation.Play("GunAlienDead");
                died = true;
            }
        }
        else if (EnemyState == states.Dance){
            if (!died){
                animation.Play("GunAlienDance");
                if (!animation.IsPlaying("GunAlienDance"))
                    EnemyState = states.Death;
            }
        }

        if (EnemyState == states.Hit && !died){
			if(!animation.IsPlaying("Hit")){
				animation.Play("GunAlienIdle");
				EnemyState = states.Idle;
				AIPathing.canMove = true;
				AIPathing.canSearch = true;
			}
        }
    }

    private void PlayBat(){
        if (batAtt && Time.time >= delay + batdelay && !died){
			StopCoroutine("batAttack");
            EnemyState = states.Run;
            animation.Play("BatFly");
			AIPathing.canMove = true;
            AIPathing.canSearch = true;
            tmp.enabled= true;
			speedSwoop = new Vector3(0.0f,0.0f,0.0f);
            batAtt = false;
			swoop = false;
			attackWait = true;
			animation["BatAttack"].speed = 1.0f;
			transform.FindChild("BatZoidtoAnimate:BatZoidtoAnimate").GetComponent<BoxCollider>().enabled = false;
			StartCoroutine("attackDelay");
        }

        if (EnemyState == states.Idle && !batAtt)
            animation.PlayQueued("BatFly");
        else if (EnemyState == states.Run && !batAtt)
            animation.PlayQueued("BatFly");
        else if (EnemyState == states.Attack && !batAtt && !attackWait){
            AIPathing.canMove = false;
            AIPathing.canSearch = false;
            delay = Time.time;
            dir = player.position - transform.position; // calculate the target direction...
            speedSwoop = new Vector3(dir.x*0.03f,-0.1f,dir.z*0.03f);
			this.transform.LookAt(new Vector3(GM.objPlayer.transform.position.x,this.transform.position.y,GM.objPlayer.transform.position.z));	//Look in the direction of the player
			tmp.enabled = false;
			batAtt = true;
			swoop = true;
			transform.FindChild("BatZoidtoAnimate:BatZoidtoAnimate").GetComponent<BoxCollider>().enabled = true;
        }
		else if (EnemyState == states.Attack && batAtt && !died){
			AIPathing.canMove = false;
			AIPathing.canSearch = false;
			animation.Play("BatAttack");
			StartCoroutine("batAttack");			
		}
		else if (EnemyState == states.Dance){
			if (!died){
			   batAtt = false;
			   StopCoroutine("batAttack");
			   speedSwoop = new Vector3(0.0f,0.0f,0.0f);
               animation.Play("BatDance");
               if (!animation.IsPlaying("BatDance")){
                   EnemyState = states.Death;
               }
			}
		}
        else if (EnemyState == states.Death){
            if (!died){
                animation.Stop();
                animation.Play("BatDead");
				StopCoroutine("batAttack");
				if (!tmp.enabled)
					tmp.enabled = true;
				swoop = false;
				speedSwoop = new Vector3(0.0f,0.0f,0.0f);
				controller.center = new Vector3(0, 0.03f, 0);
				this.GetComponent<CharacterController>().stepOffset = 0.03f;
				died = true;
            }
        }
        else
			controller.Move(dir * speed * Time.deltaTime);
		
        if (EnemyState == states.Hit && !died && !attacking){
			if(!animation.IsPlaying("Hit")){
				animation.Play("BatFly");
				EnemyState = states.Idle;
				AIPathing.canMove = true;
				AIPathing.canSearch = true;
			}
        }
    }
	
	IEnumerator batAttack(){
		yield return new WaitForSeconds(1.9f);					//Delay for the bat to attack
		if (swoop && !died && EnemyState != states.Dance){
			this.transform.position += speedSwoop;
			speedSwoop.y += 0.15f*Time.deltaTime;
			animation["BatAttack"].speed = 0.7f;
		}
	}
	
	IEnumerator attackDelay(){				//Cooldown on enemy attacks so they don't attack in quick succession
		yield return new WaitForSeconds(1.0f);
		attackWait = false;
	}
	
	private void PlayBear(){				
		if (EnemyState == states.Attack && !died && !attackWait){
			if(!attacking){					//If he isnt already attacking, start doing so
				AIPathing.canMove = false;
				AIPathing.canSearch = false;
				animation.Play("BearAttack");
	            delay = Time.time;
				attacking = true;
			}else{							//Once the attack animation is done
				if(!animation.IsPlaying("BearAttack")){
					animation.Stop("BearAttack");
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
					EnemyState = states.Run;
					attacking = false;
					StartCoroutine("attackDelay");
				}
			}
        }else if (EnemyState == states.Idle && !attacking)
            animation.Play("BearIdle");
        else if (EnemyState == states.Run && !attacking)
            animation.Play("BearWalk");
        else if (EnemyState == states.Death)
        {
            if (!died)
            {
                animation.Stop();
                animation.Play("BearDead");
                Debug.Log(states.Death.ToString());
                died = true;
            }
        }
        else if (EnemyState == states.Dance){
            if (!died){
                animation.Play("BearDance");
                if (!animation.IsPlaying("BearDance"))
                    EnemyState = states.Death;
            }
        }
        if (EnemyState == states.Hit && !died && !attacking){
			if(!animation.IsPlaying("Hit")){
					animation.Play("BearIdle");
					EnemyState = states.Idle;
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
			}
        }
	}
	
	private void PlayWolf(){
		if (EnemyState == states.Attack && !died && !attackWait){
			if(!attacking){					//If he isnt already attacking, start doing so
				AIPathing.canMove = false;
				AIPathing.canSearch = false;
				animation.Play("WolfAttack");
		        delay = Time.time;
				attacking = true;
			}else{							//Once the attack animation is done
				if(!animation.IsPlaying("WolfAttack")){
					animation.Stop("WolfAttack");
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
					EnemyState = states.Run;
					attacking = false;
					StartCoroutine("attackDelay");
				}
			}
    	}
		else if (EnemyState == states.Idle)
            animation.Play("WolfIdle");
	    else if (EnemyState == states.Run)
    	    animation.Play("WolfWalk");
        else if (EnemyState == states.Death){
    	    if (!died){
            	animation.Stop();
				animation["WolfDead"].speed = 0.5f;
                animation.Play("WolfDead");
	            Debug.Log(states.Death.ToString());
    	        died = true;
        	}
       	}
	    else if (EnemyState == states.Dance){
            if (!died){
               	animation.Play("WolfDance");
    	        if (!animation.IsPlaying("WolfDance"))
          	        EnemyState = states.Death;
	        }
	    }
	}
	
    private void PlayAlien()
    {
        if (EnemyState == states.Idle)
            animation.Play("AlienIdle");
        else if (EnemyState == states.Run)
            animation.Play("AlienWalk");
        else if (EnemyState == states.Attack && !died && !attackWait){
			if(!attacking){					//If he isnt already attacking, start doing so
				AIPathing.canMove = false;
				AIPathing.canSearch = false;
				animation.Play("AlienAttack");
	            delay = Time.time;
				attacking = true;
			}else{							//Once the attack animation is done
				if(!animation.IsPlaying("AlienAttack")){
					animation.Stop("AlienAttack");
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
					EnemyState = states.Run;
					attacking = false;
					StartCoroutine("attackDelay");
				}
			}
        }
        else if (EnemyState == states.Death){
            if (!died)
            {
                animation.Play("AlienDead");
				animation["AlienDead"].speed = 1.5f;
                died = true;
            }
        }
        else if (EnemyState == states.Dance){
            if (!died){
                animation.Play("AlienDance");

                if (!animation.IsPlaying("AlienDance"))
                    EnemyState = states.Death;
            }
        }

        if (EnemyState == states.Hit && !died){
			if(!animation.IsPlaying("Hit")){
					animation.Play("AlienIdle");
					EnemyState = states.Idle;
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
			}
        }
    }
	
	private void PlayHover(){
		if (EnemyState == states.Attack && !died && !attackWait){
			if(!attacking){					//If he isnt already attacking, start doing so
				AIPathing.canMove = false;
				AIPathing.canSearch = false;
				animation.Play("HoverAttack");
		        delay = Time.time;
				attacking = true;
			}else{							//Once the attack animation is done
				if(!animation.IsPlaying("HoverAttack")){
					animation.Stop("HoverAttack");
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
					EnemyState = states.Run;
					attacking = false;
				}
			}
    	}
		else if (EnemyState == states.Idle)
            animation.Play("HoverIdle");
	    else if (EnemyState == states.Run)
    	    animation.Play("HoverWalk");
        else if (EnemyState == states.Death){
			this.GetComponent<CharacterController>().enabled = false;
    	    if (!died){
            	animation.Stop();
                animation.Play("HoverDead");
	            Debug.Log(states.Death.ToString());
    	        died = true;
        	}
       	}
	    else if (EnemyState == states.Dance){
            if (!died){
               	animation.Play("HoverDance");
    	        if (!animation.IsPlaying("HoverDance"))
          	        EnemyState = states.Death;
	        }
	    }
        if (EnemyState == states.Hit && !died && !attacking){
			if(!animation.IsPlaying("Hit")){
				animation.Play("HoverIdle");
				EnemyState = states.Idle;
				AIPathing.canMove = true;
				AIPathing.canSearch = true;
			}
        }
	}
	
	private void playSlime(){
		if (EnemyState == states.Attack && !died && !attackWait){
			if(!attacking){					//If he isnt already attacking, start doing so
				AIPathing.canMove = false;
				AIPathing.canSearch = false;
				animation.Play("SlimeAttack");
		        delay = Time.time;
				attacking = true;
			}else{							//Once the attack animation is done
				if(!animation.IsPlaying("SlimeAttack")){
					animation.Stop("SlimeAttack");
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
					EnemyState = states.Run;
					attacking = false;
				}
			}
    	}
		else if (EnemyState == states.Idle)
            animation.Play("SlimeIdle");
	    else if (EnemyState == states.Run)
    	    animation.Play("SlimeWalk");
        else if (EnemyState == states.Death){
    	    if (!died){
            	animation.Stop();
                animation.Play("SlimeDead");
	            Debug.Log(states.Death.ToString());
    	        died = true;
        	}
       	}
	    else if (EnemyState == states.Dance){
            if (!died){
               	animation.Play("SlimeIdle");
    	        if (!animation.IsPlaying("SlimeIdle"))
          	        EnemyState = states.Death;
	        }
	    }
        if (EnemyState == states.Hit && !died){
			if(!animation.IsPlaying("Hit")){
				animation.Play("SlimeIdle");
				EnemyState = states.Idle;
				AIPathing.canMove = true;
				AIPathing.canSearch = true;
			}
		}
	}
	
	private void playSword(){
		if (EnemyState == states.Attack && !died && !attackWait){
			if(!attacking){					//If he isnt already attacking, start doing so
				AIPathing.canMove = false;
				AIPathing.canSearch = false;
				animation.Play("SwordAttack");
		        delay = Time.time;
				attacking = true;
			}else{							//Once the attack animation is done
				if(!animation.IsPlaying("SwordAttack")){
					animation.Stop("SwordAttack");
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
					EnemyState = states.Run;
					attacking = false;
				}
			}
    	}
		else if (EnemyState == states.Idle)
            animation.Play("SwordIdle");
	    else if (EnemyState == states.Run)
    	    animation.Play("SwordWalk");
        else if (EnemyState == states.Death){
    	    if (!died){
            	animation.Stop();
                animation.Play("SwordDead");
	            Debug.Log(states.Death.ToString());
    	        died = true;
        	}
       	}
	    else if (EnemyState == states.Dance){
            if (!died){
               	animation.Play("SwordDance");
    	        if (!animation.IsPlaying("SwordDance"))
          	        EnemyState = states.Death;
	        }
	    }
	}
	
	private void PlaySpear(){
		if (EnemyState == states.Attack && !died && !attackWait){
			if(!attacking){					//If he isnt already attacking, start doing so
				AIPathing.canMove = false;
				AIPathing.canSearch = false;
				animation.Play("SpearAttack");
		        delay = Time.time;
				attacking = true;
			}else{							//Once the attack animation is done
				if(!animation.IsPlaying("SpearAttack")){
					animation.Stop("SpearAttack");
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
					EnemyState = states.Run;
					attacking = false;
				}
			}
    	}
		else if (EnemyState == states.Idle)
            animation.Play("SpearIdle");
	    else if (EnemyState == states.Run)
    	    animation.Play("SpearWalk");
        else if (EnemyState == states.Death){
    	    if (!died){
            	animation.Stop();
                animation.Play("SpearDead");
	            Debug.Log(states.Death.ToString());
    	        died = true;
        	}
       	}
	    else if (EnemyState == states.Dance){
            if (!died){
               	animation.Play("SpearDance");
    	        if (!animation.IsPlaying("SpearDance"))
          	        EnemyState = states.Death;
	        }
	    }
        if (EnemyState == states.Hit && !died){
			if(!animation.IsPlaying("Hit")){
				animation.Play("SpearIdle");
				EnemyState = states.Idle;
				AIPathing.canMove = true;
				AIPathing.canSearch = true;
			}
        }
	}
	
	private void playDoggy(){
		if (EnemyState == states.Attack && !died && !attackWait){
			if(!attacking){					//If he isnt already attacking, start doing so
				AIPathing.canMove = false;
				AIPathing.canSearch = false;
				animation.Play("DogAttack");
		        delay = Time.time;
				attacking = true;
			}else{							//Once the attack animation is done
				if(!animation.IsPlaying("DogAttack")){
					animation.Stop("DogAttack");
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
					EnemyState = states.Run;
					attacking = false;
				}
			}
    	}
		else if (EnemyState == states.Idle)
            animation.Play("DogIdle");
	    else if (EnemyState == states.Run)
    	    animation.Play("DogWalk");
        else if (EnemyState == states.Death){
    	    if (!died){
            	animation.Stop();
                animation.Play("DogDead");
	            Debug.Log(states.Death.ToString());
    	        died = true;
        	}
       	}
	    else if (EnemyState == states.Dance){
            if (!died){
               	animation.Play("DogDance");
    	        if (!animation.IsPlaying("DogDance"))
          	        EnemyState = states.Death;
	        }
	    }
        if (EnemyState == states.Hit && !died){
			if(!animation.IsPlaying("Hit")){
				animation.Play("DogIdle");
				EnemyState = states.Idle;
				AIPathing.canMove = true;
				AIPathing.canSearch = true;
			}
        }
	}
	
	private void playLarva(){
		if (EnemyState == states.Attack && !died && !attackWait){
			if(larvaRoll){
				this.transform.position += new Vector3(larvaDir.x*0.05f,0.0f,larvaDir.z*0.05f);
			}
			if(!attacking){					//If he isnt already attacking, start doing so
				AIPathing.canMove = false;
				AIPathing.canSearch = false;
				animation.Play("LarvaAttack");
		        delay = Time.time;
				this.transform.LookAt(new Vector3(GM.objPlayer.transform.position.x,this.transform.position.y,GM.objPlayer.transform.position.z));	//Look in the direction of the player
				larvaDir = player.position - transform.position;
				attacking = true;
			}else{							//Once the attack animation is done
				if(!animation.IsPlaying("LarvaAttack")){
					animation.Stop("LarvaAttack");
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
					EnemyState = states.Run;
					attacking = false;
				}
			}
    	}
		else if (EnemyState == states.Idle)
            animation.Play("LarvaIdle");
	    else if (EnemyState == states.Run)
    	    animation.Play("LarvaWalk");
        else if (EnemyState == states.Death){
			animation["LarvaDead"].speed = 0.5f;
    	    if (!died){
            	animation.Stop();
                animation.Play("LarvaDead");
	            Debug.Log(states.Death.ToString());
    	        died = true;
        	}
       	}
	    else if (EnemyState == states.Dance){
            if (!died){
               	animation.Play("LarvaDance");
    	        if (!animation.IsPlaying("LarvaDance"))
          	        EnemyState = states.Death;
	        }
	    }
        if (EnemyState == states.Hit && !died){
			if(!animation.IsPlaying("Hit")){
				animation.Play("LarvaIdle");
				EnemyState = states.Idle;
				AIPathing.canMove = true;
				AIPathing.canSearch = true;
			}
        }
	}
	
	private void playHelmet(){
		if (EnemyState == states.Attack && !died && !attackWait){
			if(!attacking){					//If he isnt already attacking, start doing so
				AIPathing.canMove = false;
				AIPathing.canSearch = false;
				animation.Play("HelmetAttack");
		        delay = Time.time;
				attacking = true;
			}else{							//Once the attack animation is done
				if(!animation.IsPlaying("HelmetAttack")){
					animation.Stop("HelmetAttack");
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
					EnemyState = states.Run;
					attacking = false;
				}
			}
    	}
		else if (EnemyState == states.Idle)
            animation.Play("HelmetIdle");
	    else if (EnemyState == states.Run)
    	    animation.Play("HelmetWalk");
        else if (EnemyState == states.Death){
    	    if (!died){
            	animation.Stop();
                animation.Play("HelmetDead");
	            Debug.Log(states.Death.ToString());
    	        died = true;
        	}
       	}
	    else if (EnemyState == states.Dance){
            if (!died){
               	animation.Play("HelmetDance");
    	        if (!animation.IsPlaying("HelmetDance"))
          	        EnemyState = states.Death;
	        }
	    }
        if (EnemyState == states.Hit && !died){
			if(!animation.IsPlaying("Hit")){
				animation.Play("HelmetIdle");
				EnemyState = states.Idle;
				AIPathing.canMove = true;
				AIPathing.canSearch = true;
			}
        }
	}
	
	private void PlayMechBoss(){
		if (EnemyState == states.Attack && !died && !attackWait){
			if (bossAttack == 1){ 								//Gatling attack
				if(!attacking){
					AIPathing.canMove = false;
					AIPathing.canSearch = false;
					animation.Play("MechAttack1");
					animation["MechAttack1"].speed = 0.5f;
					attacking = true;
					StopCoroutine("bossAutoAttack");
					if(autoBossAttack)
						autoBossAttack = false;
				}else{							
					if(!animation.IsPlaying("MechAttack1")){
						AIPathing.canMove = true;
						AIPathing.canSearch = true;
						animation.Stop("MechAttack1");
						EnemyState = states.Run;
						attacking = false;
						StartCoroutine("bossAutoAttack");
						StartCoroutine("attackDelay");
					}
				}
			}else{
				if(!attacking){									//Missile attack
					AIPathing.canMove = false;
					AIPathing.canSearch = false;
					animation.Play("MechAttack2");
					animation["MechAttack2"].speed = 0.5f;
					attacking = true;
					StopCoroutine("bossAutoAttack");
					if(autoBossAttack)
						autoBossAttack = false;
				}else{							
					if(!animation.IsPlaying("MechAttack2")){
						AIPathing.canMove = true;
						AIPathing.canSearch = true;
						animation.Stop("MechAttack2");
						EnemyState = states.Run;
						attacking = false;
						StartCoroutine("bossAutoAttack");
						StartCoroutine("attackDelay");
					}
				}
			}
    	}
		else if (EnemyState == states.Idle)
            animation.Play("MechIdle");
	    else if (EnemyState == states.Run){
    	    animation.Play("MechWalk");
			animation["MechWalk"].speed = 0.5f;
		}else if (EnemyState == states.Death){
            if (animation.IsPlaying("MechJump")){
                animation.Stop();
                animation.Play("MechDead");
            }
    	    if (!died){
            	animation.Stop();
				animation["MechDead"].speed = 0.5f;
                animation.Play("MechDead");
    	        died = true;
        	}
       	}
	    if (EnemyState == states.Hit && !died && !attacking){
			animation.Play("MechHit");
			AIPathing.canMove = false;
			AIPathing.canSearch = false;
				if(!animation.IsPlaying("MechHit")){
					animation.Play("MechIdle");
					EnemyState = states.Idle;
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
			}
        }
        else if (EnemyState == states.Jump)
            animation.Play("MechJump");
	}
	
	IEnumerator getBossAttack(){
		bossAttack = Random.Range(0,2);
		yield return new WaitForSeconds(0.2f);
		StopCoroutine("getBossAttack");
	}
	
	IEnumerator bossAutoAttack(){
		if(!GameManager.isPaused){
			yield return new WaitForSeconds(bossAutoDelay);
			if(!autoBossAttack){
				autoBossAttack = true;
				StartCoroutine("getBossAttack");
				EnemyState = states.Attack;
			}
			autoBossAttack = false;
		}
	}
	
	private void playCaptainBoss(){
		/* Steven:
		 * Boss of stage 2.
		 * Attack1: Left hook: Slow at the start to give the player time to react (like 1/4 speed), then return to normal speed
		 * Attack2: Slam: Normal speed for the windup, then slow it down at the apex (1/10 speed), then back to normal for the attack
		 * */
		if (EnemyState == states.Attack && !died && !attackWait){
			if (bossAttack == 1){ 								
				if(!attacking){
					AIPathing.canMove = false;
					AIPathing.canSearch = false;
					
					animation.Play("captainBossAttack1");
					animation["captainBossAttack1"].speed = 0.25f;
					attacking = true;
				}else{							
					if(!animation.IsPlaying("captainBossAttack1")){
						AIPathing.canMove = true;
						AIPathing.canSearch = true;
						transform.FindChild("polySurface1").GetComponent<BoxCollider>().enabled = false;
						EnemyState = states.Run;
						attacking = false;
						StartCoroutine("attackDelay");
					}
				}
			}else{
				if(!attacking){									
					AIPathing.canMove = false;
					AIPathing.canSearch = false;
					animation.Play("captainBossAttack2");
					attacking = true;
				}else{							
					if(!animation.IsPlaying("captainBossAttack2")){
						AIPathing.canMove = true;
						AIPathing.canSearch = true;
						transform.FindChild("polySurface1").GetComponent<BoxCollider>().enabled = false;
						EnemyState = states.Run;
						attacking = false;
						StartCoroutine("attackDelay");
					}
				}
			}
    	}
		else if (EnemyState == states.Idle)
            animation.Play("captainBossIdle");
	    else if (EnemyState == states.Run){
    	    animation.Play("captainBossWalk");
		}else if (EnemyState == states.Death){
    	    if (!died){
            	animation.Stop();
                animation.Play("captainBossDead");
    	        died = true;
        	}
       	}
        if (EnemyState == states.Hit && !died && !attacking){
			animation.Play("captainBossHit");
			AIPathing.canMove = false;
			AIPathing.canSearch = false;
				if(!animation.IsPlaying("captainBossHit")){
					animation.Play("captainBossIdle");
					EnemyState = states.Idle;
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
			}
        }
	}
	
	private void playQueen(){
		if (EnemyState == states.Attack && !died && !attackWait){
			if(!attacking){									
				AIPathing.canMove = false;
				AIPathing.canSearch = false;
				animation.Play("QueenAttack");
				attacking = true;
			}else{							
				if(!animation.IsPlaying("QueenAttack")){
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
					transform.FindChild("attackBox").GetComponent<BoxCollider>().enabled = false;
					EnemyState = states.Run;
					attacking = false;
					StartCoroutine("attackDelay");
				}
			}
    	}
		else if (EnemyState == states.Idle)
            animation.Play("QueenIdle");
	    else if (EnemyState == states.Run){
    	    animation.Play("QueenWalk");
		}else if (EnemyState == states.Death){
    	    if (!died){
            	animation.Stop();
                animation.Play("QueenDead");
    	        died = true;
        	}
       	}
		if (EnemyState == states.Hit && !died && !attacking){
			animation.Play("QueenHit");
			AIPathing.canMove = false;
			AIPathing.canSearch = false;
				if(!animation.IsPlaying("QueenHit")){
					animation.Play("QueenIdle");
					EnemyState = states.Idle;
					AIPathing.canMove = true;
					AIPathing.canSearch = true;
			}
        }
	    else if (EnemyState == states.Dance){
			 if (!died)
				animation.Play("QueenDance");
	    }
	}
}
