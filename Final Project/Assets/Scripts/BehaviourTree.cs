using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class BehaviourTree : MonoBehaviour {

    protected BTRoot BTRootObj = new BTRoot();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public enum CompletionStates
{
    Success,
    Failure,
    Error
}

//Root class object of the tree
public class BTRoot
{

    public CompletionStates Tick()
    {
        return CompletionStates.Error;
    }

}

//Composites can have one or more children and run them in some order,
//returning the value that is passed down to them from the child they are running
public class Composite : BTRoot
{

}

//Selectors attempt to tick its children sequentually until one of them returns.
//When a child returns, it will attempt to run the next one until it runs out of children,
//If all of its children 
public class Selector : Composite
{

}



