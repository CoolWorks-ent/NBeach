using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Shell : MonoBehaviour {

    float airTime = 2f; //m/s
    float speed = 20f;
    float startTime;
    float elapsedTime = 0f;
    int maxRange = 5; //max distance
    float bulletForce = 1f;
    public bool projectileFired = false;
    public Transform shootPoint;
    Vector3 fwd;
    Vector3 normalizeDirection;

    // Use this for initialization
    void Start ()
    {
        startTime = 0;

        Debug.Log("New projectile created");
        EventManager.StartListening("FireProjectile", OnFire);
        StartCoroutine(updatePos());
    }

    public void Initialize(Transform gunPoint)
    {
        shootPoint = gunPoint;
        //StartCoroutine(updatePos());
    }

    private void FixedUpdate()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (shootPoint != null)
        {
            if (projectileFired == false)
            {
                fwd = Camera.main.transform.TransformDirection(Vector3.forward);
                //Vector3 temp = Camera.main.transform.position - shootPoint.position;
                //Vector3 targetPos = shootPoint.position + (fwd * 20);
                transform.LookAt(fwd * 10);
                Vector3 targetPos = shootPoint.position + (fwd * 10);
                normalizeDirection = (targetPos - transform.position).normalized;
            }
            //transform.LookAt(Camera.main.transform.position);
            //fwd = GameObject.FindGameObjectWithTag("MainCamera").transform.forward;
            else if (projectileFired == true)
            {
                if (startTime < airTime)
                {
                    //Vector3 tempVect = fwd.normalized * speed * Time.deltaTime;
                    //GetComponent<Rigidbody>().MovePosition(transform.position + tempVect);
                    //Vector3 normalizeDirection = (targetPos - transform.position).normalized;
                    //normalizeDirection = new Vector3(normalizeDirection.x, normalizeDirection.y-.2f, normalizeDirection.z+2).normalized;
                    //transform.position += (normalizeDirection * Time.deltaTime) * attackSpeed;

                    transform.Translate((normalizeDirection * Time.deltaTime) * speed, Space.World);
                    startTime += Time.deltaTime;
                }
                else
                {
                    //!!CHANGE CODE to DESTROY gameobject when outside of the battle boundary!!
                    Destroy(this.gameObject);
                }
            }
        }
        //move with player gun while in hand
        /*if (projectileFired == false)
        {
            transform.position = shootPoint.position;
            Vector3 temp = Camera.main.transform.position - shootPoint.position;
            transform.LookAt(Camera.main.transform.position);
            transform.rotation = Quaternion.LookRotation(temp,Camera.main.transform.up) * Quaternion.Euler(90, 90, 0);
        }*/

        //myRigidBody.velocity = Vector3.forward * 1;
        /*if (elapsedTime <= airTime)
        {
            //move forward until max range or collision
            transform.localPosition += fwd * speed*Time.deltaTime;
            elapsedTime += Time.deltaTime;
        }
        else
            Destroy(this.gameObject);*/
    }

    IEnumerator updatePos()
    {
        while (projectileFired == false)
        {
            transform.position = shootPoint.position;
            Vector3 temp = Camera.main.transform.position - shootPoint.position;
            transform.LookAt(Camera.main.transform.position);
            transform.rotation = Quaternion.LookRotation(temp, Camera.main.transform.up) * Quaternion.Euler(90, 90, 0);
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Darkness")
        {
            if (projectileFired == true)
            {
                collider.GetComponentInParent<Darkness>().DestroyDarkness();
                //collider.gameObject.GetComponent<Darkness>().DestroyDarkness();
                //Destroy(this.gameObject);
            }
        }            
    }

    //event called via player animator at end of throw animation
    void OnFire(string evt)
    {
        //StartCoroutine(MoveProjectile());
        projectileFired = true;
        //StartCoroutine(MoveProjectileVelocity());
        //StartCoroutine(MakeNonKinematic());

        //Vector3 fwd = GameObject.FindGameObjectWithTag("MainCamera").transform.forward;
        //fwd = fwd.normalized * 1;
        //GetComponent<Rigidbody>().MovePosition(transform.position + fwd);

        /*
        //fwd = Camera.main.transform.TransformDirection(Vector3.forward);
        Vector3 fwd = GameObject.FindGameObjectWithTag("MainCamera").transform.forward;
        //Vector3 fwd = Camera.main.transform.TransformDirection(Vector3.forward) * 2;
        GetComponent<Rigidbody>().AddForce(fwd * bulletForce, ForceMode.Impulse);
        Rigidbody myRigidBody = GetComponent<Rigidbody>();
        myRigidBody.velocity = Vector3.ClampMagnitude(myRigidBody.velocity, 1);        
        */

        Destroy(gameObject, airTime);
    }

    /*float _currMovementPos_Lerp = 0;     // Holds a value to the current lerp "t" value.
    void LerpRB(Transform obj, Vector3 targetPosition, float _speedModifier)
    {
        _currMovementPos_Lerp += Time.deltaTime * _speedModifier; // Adds the speed at which you want to move. 
        obj.position = Vector3.Lerp(obj.position, targetPosition, _currMovementPos_Lerp);
    }
    */

    public IEnumerator MakeNonKinematic()
    {
        yield return new WaitForSeconds(.5f);
        GetComponent<Rigidbody>().isKinematic = false;
        yield return 0;
    }

    public IEnumerator MoveProjectileVelocity()
    {
        fwd = Camera.main.transform.TransformDirection(Vector3.forward);
        Vector3 v = GetComponent<Rigidbody>().velocity;
        v += fwd * bulletForce * Time.deltaTime;
        GetComponent<Rigidbody>().velocity = v;
        yield return 0;
    }

    public IEnumerator MoveProjectileForce()
    {
        fwd = Camera.main.transform.TransformDirection(Vector3.forward);
        GetComponent<Rigidbody>().AddForce(fwd * bulletForce);
        Destroy(this.gameObject, airTime);

        yield return 0;
    }


    public IEnumerator MoveProjectile()
    {
        Rigidbody myRigidBody = GetComponent<Rigidbody>();

        float elapsedTime = 0f;
        float airTime = 1f;
        //_currMovementPos_Lerp = 0;
        while (elapsedTime < airTime)
        {
            //myRigidBody.velocity = Vector3.forward * 4;
            myRigidBody.MovePosition(myRigidBody.transform.position + Vector3.forward * speed * Time.deltaTime);
            //Vector3.Lerp(projectile.transform.position, temp, elapsedTime / airTime);
            /*LerpRB(transform, endPos, 0.001f);
            //transform.localPosition += (gunEnd.forward*-1) * speed * Time.deltaTime;*/
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }

}
