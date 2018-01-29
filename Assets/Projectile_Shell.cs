using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Shell : MonoBehaviour {

    float airTime = 2f; //m/s
    float speed = .5f;
    float startTime;
    float elapsedTime = 0f;
    int maxRange = 5; //max distance
    float bulletForce = 2000;
    public bool projectileFired = false;
    public Transform shootPoint;
    Vector3 fwd;

    // Use this for initialization
    void Start ()
    {
        startTime = 0;

        Debug.Log("New projectile created");
        EventManager.StartListening("FireProjectile", OnFire);
 
    }

    public void Initialize(Transform gunPoint)
    {
        shootPoint = gunPoint;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Rigidbody myRigidBody = GetComponent<Rigidbody>();

        //move with player gun while in hand
        if (projectileFired == false)
        {
            transform.position = shootPoint.position;
            Vector3 temp = Camera.main.transform.position - shootPoint.position;
            transform.LookAt(Camera.main.transform.position);
            transform.rotation = Quaternion.LookRotation(temp,Camera.main.transform.up) * Quaternion.Euler(90, 90, 0);
        }

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

    private void OnCollisionEnter(Collision collision)
    {

    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Darkness")
            Destroy(this.gameObject);
    }

    void OnFire(string evt)
    {
        //StartCoroutine(MoveProjectile());
        projectileFired = true;
        fwd = Camera.main.transform.TransformDirection(Vector3.forward);
        GetComponent<Rigidbody>().AddForce(fwd * bulletForce);

        Destroy(gameObject, airTime);
    }

    float _currMovementPos_Lerp = 0;     // Holds a value to the current lerp "t" value.
    void LerpRB(Transform obj, Vector3 targetPosition, float _speedModifier)
    {
        _currMovementPos_Lerp += Time.deltaTime * _speedModifier; // Adds the speed at which you want to move. 
        obj.position = Vector3.Lerp(obj.position, targetPosition, _currMovementPos_Lerp);
    }

    public IEnumerator MoveProjectileForce()
    {
        fwd = Camera.main.transform.TransformDirection(Vector3.forward);
        GetComponent<Rigidbody>().AddForce(fwd * bulletForce);
        Destroy(this.gameObject, airTime);

        yield return 0;
    }

    public IEnumerator MoveProjectile(Vector3 endPos)
    {
        Rigidbody myRigidBody = GetComponent<Rigidbody>();

        float elapsedTime = 0f;
        float airTime = 1f;
        _currMovementPos_Lerp = 0;
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
