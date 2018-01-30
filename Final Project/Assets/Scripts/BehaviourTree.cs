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
    SUCCESS,
    FAILURE,
    ERROR,
    RUNNING
}

//Root class object of the tree
public class BTRoot
{

    public CompletionStates Tick()
    {
        return CompletionStates.ERROR;
    }

}

//======================================================
//                  Composites
//======================================================

//Composites can have one or more children and run them in some order,
//returning the value that is passed down to them from the child they are running
public class Composite : BTRoot
{
    
}

//Selectors attempt to tick its children sequentually until one of them returns.
//When a child returns, it will attempt to run the next one until it runs out of children,
//If all of its children return Failure, the node will return FAILURE
public class Selector : Composite
{

}

//The Sequence node ticks each of its children sequentially until one of them returns FAILURE, RUNNING or ERROR,
//if all children return SUCCESS, the Sequence also returns SUCCESS
public class Sequence : Composite
{

}

//The Parallel node ticks all of its children at once, allowing them to work in parallel.
//Parallel nodes return SUCCESS if the number of succeeding children is larger than the local constant S,
//return FAILURE if the number of children is larger than the local constant F, or otherwise return RUNNING.
// S and F may be different for each parallel node
public class Parallel : Composite
{

}

 

//======================================================
//                  Leaves
//======================================================

//Leaf nodes are made up of Conditions and Actions
//They do not have any children, instead they perform some computation and return a state.
public class BaseLeaf : BTRoot
{
    
}

//Action nodes execute an action
//Returns Success if the action completes successfully,
//Returns Failure if the action fails
//Returns Running while the action is executing
public class BaseAction : BaseLeaf
{

}

//Condition nodes act as binary condition checkers,
//Returning either Success or Failure based on how that condition returns.
public class BaseCondition : BaseLeaf
{

}
