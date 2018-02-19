using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    //Create a node array of 20x20 nodes
    public NodeRow[] nodeRow;
    //public Node node;

    // Use this for initialization
    void Start ()
    {
        nodeRow = new NodeRow[10];
        UpdateGrid();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    //Used to initially generate the grid
    void GenerateGrid()
    {

    }
    //Used to update the grid 
    void UpdateGrid()
    {
        for (int row = 0; row < nodeRow.Length; row++)
        {
            for (int column = 0; column < nodeRow[row].columnData.Length; column++)
            {
                nodeRow[row].columnData[column].nodePos = new Vector3(row, column, 0);
            }
        }



        //For each node that is a child of the gamemaster
        foreach (NodeRow row in nodeRow)
        {
            foreach (Node node in row.columnData)
            {

            }
        }
    }

}

[System.Serializable]
public class NodeRow
{
    public Node[] columnData = new Node[10];
}


[System.Serializable]
public class Node
{
    //The Node's position in the world
    public Vector3 nodePos;
    //Bools to determine if there is a wall 
    [Header("Passable")]
    public bool north;
    public bool east;
    public bool south;
    public bool west;
    



}