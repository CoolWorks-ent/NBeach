using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*************Darkness Enemy Script**********
 * Base script for mini-darkness.  very basic movement and AI
 */
public class Darkness : MonoBehaviour {

    public Transform target;
    
    ///<summary>Sets the initiation range for darkness units. Units will be qeued for attack in this range</summary>
    public int attackInitiationRange, actionIdle, queueID;

    public GameObject deathFX;

    public Pathfinding.RichAI aIRichPath;

    public Animator animeController;
    public int attackHash = Animator.StringToHash("Attack"),
                attackAfterHash = Animator.StringToHash("AfterAttack"),
                chaseHash = Animator.StringToHash("Chase"),
                wanderHash = Animator.StringToHash("Wander"),
                idleHash = Animator.StringToHash("Idle"),
                deathHash = Animator.StringToHash("Death");
    public Pathfinding.IAstarAI ai;

    public DarkState previousState, currentState;

    private HashSet<DarkState> darkStates;
    Dictionary<EnemyState, DarkState> States;

    public bool canAttack;

    void Start () {
        darkStates = new HashSet<DarkState>(Resources.LoadAll<DarkState>("Darkness States"));
        Debug.Log("Darkness states count " + darkStates.Count + "for this object " + this.gameObject);
        actionIdle = 3;
        canAttack = false;
        queueID = 0;
        animeController = GetComponentInChildren<Animator>();
        ai = GetComponent<Pathfinding.IAstarAI>();
        States = new Dictionary<EnemyState, DarkState>();
        foreach(DarkState dS in darkStates)
        {
            States.Add(dS.stateType, dS);
        }
        aIRichPath = GetComponent<Pathfinding.RichAI>();
        //aIDestSetter.target = owner.target;
        ChangeState(EnemyState.IDLE);
	}
	
	// Update is called once per frame
	void Update () {
        ExecuteCurrentState();
    }

    public void ChangeState(EnemyState eState)
    {
        previousState = currentState;
        currentState = States[eState];
        currentState.InitializeState(this);
    }
    public void ExecuteCurrentState()
    {
        currentState.UpdateState(this);
    }

    public void RevertState(Darkness owner)
    {
        currentState = previousState;
    }

    public bool TargetWithinDistance(int range)
    {
        if(Vector3.Distance(target.position, transform.position) <= range)
        {
            return true;
        }
        else return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
      //EventManager.TriggerEvent("DarknessDeath", gameObject.);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            Debug.Log("Darkness collided with Player");
        }
        else if (collider.gameObject.tag == "Projectile")
        {
            if (collider.gameObject.GetComponent<Projectile_Shell>().projectileFired == true)
            {
                Debug.Log("Darkness Destroyed");

                GameObject newFX = Instantiate(deathFX.gameObject, transform.position, Quaternion.identity) as GameObject;
                //gameObject.GetComponent<MeshRenderer>().material.SetColor(Color.white);
                
                //change darkness back to idle to state to prevent moving & set to Kinematic to prevent any Physics effects
                ChangeState(EnemyState.DEATH);
                gameObject.GetComponentInChildren<Rigidbody>().isKinematic = true;
                StartCoroutine(deathRoutine());

                //EventManager.TriggerEvent("DarknessDeath", gameObject.name);
            }
        }
    }

    IEnumerator deathRoutine()
    {
        float fxTime = 1;
        //Slowly increase texture power over the FX lifetime to show the Darkness "Glowing" and explode!
        int maxPower = 10;
        MeshRenderer renderer = gameObject.GetComponentInChildren<MeshRenderer>();
        float curPower = renderer.material.GetFloat("_MainTexturePower");
        float curTime = 0;
        while(curTime < fxTime)
        {
            curPower = curTime * maxPower;
            renderer.material.SetFloat("_MainTexturePower", curPower);
            curTime += Time.deltaTime;
            yield return 0;
        }
       
        //yield return new WaitForSeconds(fxTime);
        Destroy(this.gameObject);
        yield return 0;
    }

}