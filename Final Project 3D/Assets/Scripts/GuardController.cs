using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardController : MonoBehaviour {

    public Blackboard blackboard;
    public GameObject blackBoardObj;

    protected Selector tree;

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public float meleeWeaponRange, gunWeaponRange;
    public bool meleeWeapon;
    public float attackCooldown;
    public int weaponDamage;


    public LayerMask targetMask;
    public LayerMask obstacleMask;


    // Use this for initialization
    void Start ()
    {
        //Assign the blackboard
        blackboard = blackBoardObj.GetComponent<Blackboard>();

        //Create the nodes and populate the children Lists within them
        tree = new Selector();

            //Root of the spotted sub-tree
            Sequence spottedSequence = new Sequence();
            tree.children.Add(spottedSequence);
                //Player detected condition node
                PlayerDetected playerDetected = new PlayerDetected();
                spottedSequence.children.Add(playerDetected);
                //Add a Succeeder repeater for the melee sub-tree
                SucceederRepeater succeeder = new SucceederRepeater();
                spottedSequence.children.Add(succeeder);
                //Sequence for the melee sub-tree
                Sequence subSequence = new Sequence();
                succeeder.child = subSequence;
                    //Add the melee weapon check to the sequence
                    MeleeWeapon meleeWeapon = new MeleeWeapon();
                    subSequence.children.Add(meleeWeapon);
                    //Add in the persuit movement to 
                    PersuitMovement persuitMovement = new PersuitMovement();
                    subSequence.children.Add(persuitMovement);
                CheckRange checkRange = new CheckRange();
                spottedSequence.children.Add(checkRange);
                CheckAttackCooldown checkCooldown = new CheckAttackCooldown();
                spottedSequence.children.Add(checkCooldown);

            //Root of the Search sub-tree
            Sequence searchSequence = new Sequence();
            tree.children.Add(searchSequence);
                







    }
	
	// Update is called once per frame
	void Update ()
    {
        tree.Tick(this);
	}

    //Used to visualise the viewcone
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));

    }

    //
    void FindVisibleTargets()
    {
        //Collect an array of all the targets within range, using the targetmask to only collect target objects
        Collider[] targetsInView = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        //Then check if each of those targets is within the viewcone
        for (int i = 0; i < targetsInView.Length; i++)
        {
            //Set the target
            Transform target = targetsInView[i].transform;
            //Get the angle between the current direction the guard is facing and the target we are looking at
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            //Check if that angle is inside the viewcone
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle/2)
            {
                //see if there is an obstacle in between the guard and the player
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
                {
                    //No obstacles in between target and guard, do something about it here


                }

            }
        }
    }

    void ResetWeaponCooldown(int Cooldown)
    {

    }
}
