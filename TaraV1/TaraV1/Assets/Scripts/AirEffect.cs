using UnityEngine;
using System.Collections;

public class AirEffect : MonoBehaviour {

    public float killTime = 10.0f;
    private float _killTime;

	void Start () {
        _killTime = killTime + Time.deltaTime;
	
	}
	
	void Update () {
        _killTime -= 1;
        transform.localScale = transform.localScale * 1.2f;

        if (_killTime <= 0)
        {
            Destroy(this.gameObject);
        }
	
	}
}
