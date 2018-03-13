using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree : MonoBehaviour
{

    protected BTRoot BTRootObj = new BTRoot();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
    private CompletionStates status;

    //First thing the child runs when it is created, will be overrided for each child
    public virtual void OnInitialise() { }
    //Last thing the child runs when it is destroyed, will be overrided for each child
    public virtual void OnExit(CompletionStates state) { }
    //Function to update the guard, will be overridden
    public virtual CompletionStates UpdateGuard(GuardController guard)
    {
        //Will only return this if not overridden
        return CompletionStates.ERROR;
    }

    //Run each behaviour tree tick
    public CompletionStates Tick(GuardController guard)
    {
        //If the guard isn't still running, run his starting function
        if (status != CompletionStates.RUNNING)
        {
            OnInitialise();
        }
        //Update the guard
        status = UpdateGuard(guard);

        //If the guard is no longer running, call its exit function
        if (status != CompletionStates.RUNNING)
        {
            OnExit(status);
        }
        return status;
    }

    //Returns the current status
    public CompletionStates currentStatus() { return status; }

}

//======================================================
//                  Composites
//======================================================

//Composites can have one or more children and run them in some order,
//returning the value that is passed down to them from the child they are running
public class Composite : BTRoot
{
    //Create a new list to store the Composite's children
    protected List<BTRoot> children = new List<BTRoot>();
}

//Selectors attempt to tick its children sequentually until one of them returns.
//When a child returns, it will attempt to run the next one until it runs out of children,
//If all of its children return Failure, the node will return FAILURE
public class Selector : Composite
{
    //Override the default UpdateGuard
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        //Loop through the children list
        foreach (BTRoot child in children)
        {
            //If the child does not fail its Tick function
            if (child.Tick(guard) != CompletionStates.FAILURE)
            {
                //return the child's status if it did not fail
                return child.currentStatus();
            }
        }

        //If all of its children return failure, then the node will also return failure
        return CompletionStates.FAILURE;
    }
}

//The Sequence node ticks each of its children sequentially until one of them returns FAILURE, RUNNING or ERROR,
//if all children return SUCCESS, the Sequence also returns SUCCESS
public class Sequence : Composite
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        //Loop through the children list
        foreach (BTRoot child in children)
        {
            //If the child does not Succeed its Tick function
            if (child.Tick(guard) != CompletionStates.SUCCESS)
            {
                //Return the child's status if it succeeded
                return child.currentStatus();
            }
        }
        return CompletionStates.SUCCESS;
    }
}

//The Parallel node ticks all of its children at once, allowing them to work in parallel.
//Parallel nodes return SUCCESS if the number of succeeding children is larger than the local constant S,
//return FAILURE if the number of children is larger than the local constant F, or otherwise return RUNNING.
// S and F may be different for each parallel node
public class Parallel : Composite
{

}

//======================================================
//                  Decorators
//======================================================

//Decorators are nodes that have a single child 
//They add additional abilities to the leaves/composites
//they are attached to.

//Like Leaves, Decorators are very situational, coded to specific tasks.
public class BaseDecorator : BTRoot
{
    protected BTRoot child;
}


public class RepeatDecorator : BaseDecorator
{
    //Number of times this decorator repeats its child
    protected int numRepeat;

    public override CompletionStates UpdateGuard(GuardController guard)
    {
        //For the specified number of times, tick the child of this Decorator
        //Note, the normal "i++" has been switched to "++i"
        for (int i = 0; i < numRepeat; ++i)
        {
            child.Tick(guard);
            if (i == numRepeat)
            {
                return CompletionStates.SUCCESS;
            }
        }
        //If it hasn't returned success yet, return ERROR
        return CompletionStates.ERROR;
    }
}

//Decorator that always returns success
public class SucceederRepeater : BaseDecorator
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        //Run the Tick function on the child, 
        child.Tick(guard);
        //Regardless of if the child succeeded or failed, return success
        return CompletionStates.SUCCESS;
    }
}

//Decorator that always returns failure
public class FailureRepeater : BaseDecorator
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        //Run the Tick function on the child, 
        child.Tick(guard);
        //Regardless of if the child succeeded or failed, return success
        return CompletionStates.FAILURE;
    }
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


