using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CreateGrid : MonoBehaviour
{
    public GameObject GridNode;//Node to be instantiated
    //The size of the grid I want to create 
    //(This will be doubled because the script creates them along the negative axis as well)
    public int GridSizeX, GridSizeY;
    public float GridSpacing;//The amount of space in between each of the nodes
    public bool Reinstantiate;
	void Awake ()
    {
        if (Reinstantiate)
        {
            for (int x = -GridSizeX; x < GridSizeX; x++)
            {
                for (int y = -GridSizeY; y < GridSizeY; y++)
                {
                    //instantiate the node
                    GameObject Node = Instantiate(GridNode, new Vector3(GridSpacing * x, GridSpacing * y, 0), Quaternion.identity);
                    //Access the NodeScript attached to the new prefab and set its X and Y
                    Node.GetComponent<NodeScript>().NodeX = x;
                    Node.GetComponent<NodeScript>().NodeY = y;

                    //Sets the GameObject this 
                    Node.transform.SetParent(this.transform);
                }
            }
        }
    }
}
