using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float PlayerSpeed;
    public Vector3 MoveDirection = Vector3.zero;

    


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}


    void FixedUpdate()
    {
        //Player movement using GetAxis (which can be rebound)
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * PlayerSpeed;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * PlayerSpeed;
        transform.Translate(x, z, 0);
    }
}
