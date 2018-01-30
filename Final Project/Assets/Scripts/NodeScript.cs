using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour {

    //Node's location on the grid
    public int nodeX, nodeY;
    //If the node is passable or not
    public bool passable;
    //Enum Node Type
    public enum TileType
    {
        Ground,
        Wall
    }
    //Actual type associated with this
    public TileType tiletype;
    GameObject thisObject;
    SpriteRenderer sr;
    public Sprite[] sprites;

    void Awake()
    {
        thisObject = this.gameObject;
        //Set the sprite renderer
        sr = thisObject.GetComponent<SpriteRenderer>();

        if (tiletype == TileType.Ground)
        {
            sr.sprite = sprites[0];
            passable = true;
        }
        if (tiletype == TileType.Wall)
        {
            sr.sprite = sprites[1];
            passable = false;
        }

        if (passable)
        {
            Passable();
        }
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

}
