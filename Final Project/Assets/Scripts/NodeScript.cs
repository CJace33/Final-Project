using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour {

    //Node's location on the grid
    public int nodeX, nodeY;
    //How much this tile arbitrarily costs to move through
    public float heuristic;

    GameObject thisObject;
    SpriteRenderer sr;

    void Awake()
    {
        thisObject = this.gameObject;
        //Set the sprite renderer
        sr = thisObject.GetComponent<SpriteRenderer>();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Passable()
    {
        BoxCollider2D collider = thisObject.AddComponent<BoxCollider2D>();
        Rigidbody2D rigidb = thisObject.AddComponent<Rigidbody2D>();

        //Set gravity to 0
        rigidb.gravityScale = 0;
    }

    //Updates the walls 
    public void UpdateSolids()
    {

    }
    //Finds the neighboring nodes, should be called after UpdateSolids
    public void UpdateNeighbors()
    {

    }

}
