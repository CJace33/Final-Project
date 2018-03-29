﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;




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

    //First thing the child runs when it is created, will be overriden for each child
    public virtual void OnInitialise() { }
    //Last thing the child runs when it is destroyed, will be overriden for each child
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
    public List<BTRoot> children = new List<BTRoot>();
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
    public BTRoot child;
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

    //Flipper
//Decorator that flips the result of a success or failure, leaving running or error alone
public class FlipperRepeater : BaseDecorator
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        //Run the child first
        CompletionStates returnState = child.Tick(guard);
        //Check if the child was a success, and if so flip it to a failure
        if (returnState == CompletionStates.SUCCESS)
            return CompletionStates.FAILURE;
        //Check if the child was a failure, and if so flip it to a success
        else if (returnState == CompletionStates.FAILURE)
            return CompletionStates.SUCCESS;
        //Don't mess with a running or error result
        else
            return returnState;
    }
}



//======================================================
//                  Leaves
//======================================================

//Leaf nodes are made up of Conditions and Actions
//They do not have any children, instead they perform some computation and return a state.
public class BaseLeaf : BTRoot { }



//Action Nodes
//Action nodes execute an action
//Returns Success if the action completes successfully,
//Returns Failure if the action fails
//Returns Running while the action is executing

    //Attack Node
//Attack Node gets called after checking the range and the attack cooldown
public class AttackTarget : BaseLeaf
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        PlayerController player;
        player = GameObject.FindGameObjectWithTag("Target").GetComponent<PlayerController>();
        player.ReducePlayerHealth(guard.weaponDamage);
        return CompletionStates.SUCCESS;
    }
}


    //Base Movement Node
    //I need a base movement node that will then lead to more specific sub-nodes. 
public class BaseMovement : BaseLeaf
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        //Get the navmesh agent component of our guard
        NavMeshAgent navGuard = guard.GetComponent<NavMeshAgent>();
        //Set its destination

        if (GetDestination(guard) != null)
        {
            navGuard.destination = GetDestination(guard).position;
            return CompletionStates.SUCCESS;
        }
        else
        {
            Debug.Log("Destination Not Found");
            return CompletionStates.FAILURE;
        }

    }

    //The base version of this script returns null
    public virtual Transform GetDestination(GuardController guard)
    {
        return null;
    }
}

//Persuit Movement Node
//Provides logic to send the guard chasing after the player
public class PersuitMovement : BaseMovement
{
    public override Transform GetDestination(GuardController guard)
    {
        return guard.blackboard.playerLastSeen;
    }
}

//Condition Nodes
//Condition nodes act as binary condition checkers,
//Returning either Success or Failure based on how that condition returns.


//PlayerDetected
//This node checks if a player is within the guards viewcone, and returns true if it is
public class PlayerDetected: BaseLeaf
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        //Collect an array of all the targets within range, using the targetmask to only collect target objects
        Collider[] targetsInView = Physics.OverlapSphere(guard.transform.position, guard.viewRadius, guard.targetMask);

        //Then check if each of those targets is within the viewcone
        for (int i = 0; i < targetsInView.Length; i++)
        {
            //Set the target
            Transform target = targetsInView[i].transform;
            //Get the angle between the current direction the guard is facing and the target we are looking at
            Vector3 dirToTarget = (target.position - guard.transform.position).normalized;
            //Check if that angle is inside the viewcone
            if (Vector3.Angle(guard.transform.forward, dirToTarget) < guard.viewAngle / 2)
            {
                //See if there is an obstacle in between the guard and the player
                float distanceToTarget = Vector3.Distance(guard.transform.position, target.position);
                if (!Physics.Raycast(guard.transform.position, dirToTarget, distanceToTarget, guard.obstacleMask))
                {
                    //Target spotted, update the blackboard and return true
                    guard.blackboard.playerLastSeen = target;
                    guard.blackboard.alertnessCounter = 100;
                    return CompletionStates.SUCCESS;
                }

            }
        }

        //If the player has not been spotted for some reason, return failure
        return CompletionStates.FAILURE;

    }
}



    //Condition Melee weapon
//Will return failure if the guard melee weapon bool is not true
public class MeleeWeapon : BaseLeaf
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        if (guard.meleeWeapon)
        {
            return CompletionStates.SUCCESS;
        }
        else
        {
            return CompletionStates.FAILURE;
        }
    }
}

    //Condition CheckRange
//Checks if the character is within range, returns success if they are and failure if not
public class CheckRange : BaseLeaf
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        //First check if the guard has a melee weapon or not
        if (guard.meleeWeapon)
        {
            //then check if the player is within melee range
            if (Vector3.Distance(guard.blackboard.playerPos.transform.position, guard.transform.position) < guard.meleeWeaponRange)
            {
                return CompletionStates.SUCCESS;
            }
            else
            {
                return CompletionStates.FAILURE;
            }
        }
        else
        {
            //then check if the player is within gun range
            if (Vector3.Distance(guard.blackboard.playerPos.transform.position, guard.transform.position) < guard.gunWeaponRange)
            {
                return CompletionStates.SUCCESS;
            }
            else
            {
                return CompletionStates.FAILURE;
            }
        }
    }
}

    //Condition CheckAttackCooldown
//Checks if the guard can attack or not
public class CheckAttackCooldown : BaseLeaf
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        if (guard.attackCooldown <= 0)
        {
            return CompletionStates.SUCCESS;
        }
        else
        {
            return CompletionStates.FAILURE;
        }
    }
}

    //CheckAlertnessCounter
//Checks if the guard should still be hunting for the player
public class CheckAlertnessCounter : BaseLeaf
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        if (guard.blackboard.alertnessCounter <= 0)
            return CompletionStates.FAILURE;
        else
            return CompletionStates.SUCCESS;
    }
}







