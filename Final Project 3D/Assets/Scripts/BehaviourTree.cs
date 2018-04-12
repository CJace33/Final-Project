using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;




public class BehaviourTree : MonoBehaviour
{

    //protected BTRoot BTRootObj = new BTRoot();


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
    public virtual void OnInitialise(GuardController guard) { }
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
        //If the guard isn't still running, run its starting function
        if (status != CompletionStates.RUNNING)
        {
            OnInitialise(guard);
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
                //Return the child's status
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
        player = guard.blackboard.player.GetComponent<PlayerController>();
        player.ReducePlayerHealth(guard.weaponDamage);
        guard.attackCooldown = guard.rateOfFire;
        Debug.Log("AttackTarget SUCCESS");
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
            //Check if the guard has reached its destination
            navGuard.destination = GetDestination(guard);
            if (!navGuard.pathPending)
            {
                if (navGuard.remainingDistance <= navGuard.stoppingDistance)
                {
                    if (!navGuard.hasPath || navGuard.velocity.sqrMagnitude == 0f)
                    {
                        
                        Debug.Log("BaseMovement SUCCESS");
                        return CompletionStates.SUCCESS;
                    }
                }
            }
            //And if it is still en-route, return running
            Debug.Log("BaseMovement RUNNING");
            return CompletionStates.RUNNING;
        }
        else
        {
            Debug.Log("Destination Not Found");
            return CompletionStates.FAILURE;
        }
    }

    //The base version of this script returns null
    public virtual Vector3 GetDestination(GuardController guard)
    {
        return Vector3.zero;
    }
}

    //Persuit Movement Node
//Provides logic to send the guard chasing after the player
public class PersuitMovement : BaseMovement
{
    public override Vector3 GetDestination(GuardController guard)
    {
        return guard.blackboard.playerLastSeen;
    }
}

    //UpdatePlayerPos
//Updates the player's position, regardless of where they are
public class UpdatePlayerPos : BaseLeaf
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        guard.blackboard.playerLastSeen = guard.blackboard.player.transform.position;
        Debug.Log("UpdatePlayerPos SUCCESS");
        return CompletionStates.SUCCESS;
    }
}

    //FollowCounter
//Updates the followingPlayer
public class FollowCounter : BaseLeaf
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        guard.followingPlayer--;
        Debug.Log("FollowCounter SUCCESS");
        return CompletionStates.SUCCESS;
    }
}

    //LookAround
//Swivels to look around him
public class LookAround : BaseLeaf
{
    Vector3 originalFacing;
    bool turned;
    float turnSpeed = 2;
    float turnCurrentAngle;
    float turnAmount = 45;

    public override void OnInitialise(GuardController guard)
    {
        //Store the original facing of the guard and set the turned to false
        originalFacing = guard.gameObject.transform.rotation.eulerAngles;
        turnCurrentAngle = 0;
        turned = false;
    }

    public override CompletionStates UpdateGuard(GuardController guard)
    {
        //First rotate it one way
        if (turnCurrentAngle <=  turnAmount && !turned)
        {
            guard.transform.rotation = Quaternion.Euler(0, originalFacing.y + turnCurrentAngle, 0);
            turnCurrentAngle += turnSpeed;
            Debug.Log("LookAround RUNNING");
            return CompletionStates.RUNNING;
        }
        else
        {
            turned = true;
        }
        //then the other
        if (turnCurrentAngle >= -turnAmount && turned)
        {
            guard.transform.rotation = Quaternion.Euler(0, originalFacing.y + turnCurrentAngle, 0);
            turnCurrentAngle -= turnSpeed;
            Debug.Log("LookAround RUNNING");
            return CompletionStates.RUNNING;
        }
        else
        {
            Debug.Log("LookAround SUCCESS");
            return CompletionStates.SUCCESS;
        }
    }
}

    //EnableCurious
//Enables the Guard's curious bool
public class EnableCurious : BaseLeaf
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        guard.curious = true;
        Debug.Log("EnableCurious SUCCESS");
        return CompletionStates.SUCCESS;
    }
}

    //DisableCurious
//Enables the Guard's curious bool
public class DisableCurious : BaseLeaf
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        guard.curious = false;
        Debug.Log("DisableCurious SUCCESS");
        return CompletionStates.SUCCESS;
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
        //Collect a list of all the targets within range, using the targetmask to only collect target objects
        List <Collider> targetsInView = new List<Collider>();
        //Add targets from primary viewcone
        targetsInView.AddRange(Physics.OverlapSphere(guard.transform.position, guard.viewRadius, guard.targetMask));

        //Check to see if the targets generate a hit
        Vector3 target = ViewConeCheck(guard, targetsInView, guard.viewAngle);
        //and if they do, 
        if ( target != Vector3.zero)
        {
            //set the target in the blackboard, increase the alertness counter and return SUCCESS
            guard.blackboard.playerLastSeen = target;
            guard.followingPlayer = guard.tenacity;
            Debug.Log("PlayerDetected SUCCESS primary viewcone");
            return CompletionStates.SUCCESS;
        }
        //Now check the secondary viewcone
        //Clearing the targets first
        targetsInView.Clear();
        //Add targets from secondary viewcone
        targetsInView.AddRange(Physics.OverlapSphere(guard.transform.position, guard.closeViewRadius, guard.targetMask));
        //Actually check the viewcone
        target = ViewConeCheck(guard, targetsInView, guard.closeViewAngle);
                //and if they do, 
        if (target != Vector3.zero)
        {
            //set the target in the blackboard, increase the alertness counter and return SUCCESS
            guard.blackboard.playerLastSeen = target;
            guard.followingPlayer = guard.tenacity;
            Debug.Log("PlayerDetected SUCCESS primary viewcone");
            return CompletionStates.SUCCESS;
        }
        //If the player has not been spotted for some reason, return failure
        Debug.Log("PlayerDetected FAILURE");
        return CompletionStates.FAILURE;

    }

    public Vector3 ViewConeCheck(GuardController guard, List<Collider> targets, float angle)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            //Set the target
            Transform target = targets[i].transform;
            //Get the angle between the current direction the guard is facing and the target we are looking at
            Vector3 dirToTarget = (target.position - guard.transform.position).normalized;
            //Check if that angle is inside the viewcone or the closeViewcone
            if (Vector3.Angle(guard.transform.forward, dirToTarget) < angle / 2)
            {
                //See if there is an obstacle in between the guard and the player
                float distanceToTarget = Vector3.Distance(guard.transform.position, target.position);
                if (!Physics.Raycast(guard.transform.position, dirToTarget, distanceToTarget, guard.obstacleMask))
                {
                    //Target spotted, update the blackboard and return true
                    return target.position;
                }

            }
        }
        return Vector3.zero;
    }
}

    //Meleeweapon
//Will return failure if the guard melee weapon bool is not true
public class MeleeWeapon : BaseLeaf
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        if (guard.meleeWeapon)
        {
            Debug.Log("MeleeWeapon SUCCESS");
            return CompletionStates.SUCCESS;
        }
        else
        {
            Debug.Log("MeleeWeapon FAILURE");
            return CompletionStates.FAILURE;
        }
    }
}

    //CheckRange
//Checks if the character is within range, returns success if they are and failure if not
public class CheckRange : BaseLeaf
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        //First check if the guard has a melee weapon or not
        if (guard.meleeWeapon)
        {
            //then check if the player is within melee range
            if (Vector3.Distance(guard.blackboard.player.transform.position, guard.transform.position) < guard.meleeWeaponRange)
            {
                Debug.Log("CheckRange Melee SUCCESS");
                return CompletionStates.SUCCESS;
            }
            else
            {
                Debug.Log("CheckRange Melee FAILURE");
                return CompletionStates.FAILURE;
            }
        }
        else
        {
            //then check if the player is within gun range
            if (Vector3.Distance(guard.blackboard.player.transform.position, guard.transform.position) < guard.gunWeaponRange)
            {
                Debug.Log("CheckRange Gun Success");
                return CompletionStates.SUCCESS;
            }
            else
            {
                Debug.Log("Check Range Gun FAILURE");
                return CompletionStates.FAILURE;
            }
        }
    }
}

    //CheckAttackCooldown
//Checks if the guard can attack or not
public class CheckAttackCooldown : BaseLeaf
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        if (guard.attackCooldown <= 0)
        {
            Debug.Log("CheckAttackCooldown SUCCESS");
            return CompletionStates.SUCCESS;
        }
        else
        {
            guard.attackCooldown--;
            Debug.Log("CheckAttackCooldown FAILURE");
            return CompletionStates.FAILURE;
        }
    }
}
 
    //CheckFollowing
//Checks if the guard should still be hunting for the player
public class CheckFollowing : BaseLeaf
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        if (guard.followingPlayer <= 0)
        {
            Debug.Log("CheckFollowing FAILURE");
            return CompletionStates.FAILURE;
        }
        else
        {
            Debug.Log("CheckFollowing SUCCESS");
            return CompletionStates.SUCCESS;
        }
    }
}

    //DisableCurious
//Enables the Guard's curious bool
public class CheckCurious : BaseLeaf
{
    public override CompletionStates UpdateGuard(GuardController guard)
    {
        if (guard.curious)
        {
            Debug.Log("CheckCurious SUCCESS");
            return CompletionStates.SUCCESS;
        }
        else
        {
            Debug.Log("CheckCurious FAILURE");
            return CompletionStates.FAILURE;
        }

    }
}





