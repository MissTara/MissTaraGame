using UnityEngine;
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
    private bool canRotate;
    private string direction;
    private float tempX;
    private float rotateSpeed = 4.0f;
    #endregion

    void Awake()
    {
        isRotating = false;
        direction = "";
        canRotate = true;
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
            if (player.GetComponent<UnitPlayer>().projectile != null 
                && player.GetComponent<UnitPlayer>().projectile.isReadByBoss == false 
                && !isRotating
                && this.GetComponent<AIStates>().EnemyState != AIStates.states.Death
                && this.GetComponent<AIStates>().EnemyState != AIStates.states.Attack)
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
        if (other.tag == "Wall"){
            Debug.Log("Inside Wall");
        }else if (other.tag == "Sidewalls" && isRotating){
			isRotating = false;
            tempX = 0;
            return;
		}
    } 

}
