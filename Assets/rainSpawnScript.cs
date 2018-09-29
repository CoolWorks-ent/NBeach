using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Rain Spawn Script for Enemies
 * Script that handles spawning of enemies based upong rain particle collision with water
 */
public class rainSpawnScript : MonoBehaviour {
    [SerializeField]
    EnemySpawner enemySpawner;

    public bool enter;
    public bool exit;
    public bool inside;
    public bool outside;
    private ParticleSystem PSystem;
    private List<ParticleCollisionEvent> CollisionEvents;

    //List<ParticleCollisionEvent> collisionEvents;

    // Use this for initialization
    void Start () {

        PSystem = GetComponent<ParticleSystem>();
        //var trigger = ps.trigger;
        //trigger.enabled = false;
        CollisionEvents = new List<ParticleCollisionEvent>();
    }
	
	// Update is called once per frame
	void Update () {
       /* var trigger = ps.trigger;
        trigger.enter = enter ? ParticleSystemOverlapAction.Callback : ParticleSystemOverlapAction.Ignore;
        trigger.exit = exit ? ParticleSystemOverlapAction.Callback : ParticleSystemOverlapAction.Ignore;
        trigger.inside = inside ? ParticleSystemOverlapAction.Callback : ParticleSystemOverlapAction.Ignore;
        trigger.outside = outside ? ParticleSystemOverlapAction.Callback : ParticleSystemOverlapAction.Ignore;
        */
    }

    public void OnParticleCollision(GameObject other)
    {
        int collCount = PSystem.GetSafeCollisionEventSize();

        if (collCount > CollisionEvents.Count)
        { 
        //CollisionEvents = new ParticleCollisionEvent[collCount];
        }
        int eventCount = ParticlePhysicsExtensions.GetCollisionEvents(PSystem, other, CollisionEvents);
        for (int i = 0; i < eventCount; i++)
        {
            enemySpawner.DarknessGruntSpawnCheck(CollisionEvents[i].intersection);
            //TODO: Do your collision stuff here. 
            // You can access the CollisionEvent[i] to obtaion point of intersection, normals that kind of thing
            // You can simply use "other" GameObject to access it's rigidbody to apply force, or check if it implements a class that takes damage or whatever
        }
    }

    /*
    void OnParticleTrigger()
    {
        if (enter)
        {
            List<ParticleSystem.Particle> enterList = new List<ParticleSystem.Particle>();
            int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterList);

            for (int i = 0; i < numEnter; i++)
            {
                ParticleSystem.Particle p = enterList[i];
                p.startColor = new Color32(255, 0, 0, 255);
                enterList[i] = p;
            }

            ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterList);
        }
    }
    */
    public void OnParticleSystemStopped()
    {
        Debug.Log("particle stopped");
    }
}
