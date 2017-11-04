using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NFPSController : MonoBehaviour {

    // This script controls the gun for the shooter
    // scenes, including it's movement and shooting.
        [SerializeField]
        private float m_DefaultLineLength = 70f;                       // How far the line renderer will reach if a target isn't hit.
        [SerializeField]
        private float m_Damping = 0.5f;                                // The damping with which this gameobject follows the camera.
        [SerializeField]
        private float m_GunFlareVisibleSeconds = 0.07f;                // How long, in seconds, the line renderer and flare are visible for with each shot.
        [SerializeField]
        private float m_GunContainerSmoothing = 10f;                   // How fast the gun arm follows the reticle.
        [SerializeField]
        private AudioSource m_GunAudio;                                // The audio source which plays the sound of the gun firing.
        /*[SerializeField]
        private ShootingGalleryController m_ShootingGalleryController; // Reference to the controller so the gun cannot fire whilst the game isn't playing.
        [SerializeField]
        private VREyeRaycaster m_EyeRaycaster;                         // Used to detect whether the gun is currently aimed at something.
        [SerializeField]
        private VRInput m_VRInput;  */                                   // Used to tell the gun when to fire.
        [SerializeField]
        private Transform m_CameraTransform;                           // Used as a reference to move this gameobject towards.
        [SerializeField]
        private Transform m_GunContainer;                              // This contains the gun arm needs to be moved smoothly.
        [SerializeField]
        private Transform m_GunEnd;                                    // This is where the line renderer should start from.
        [SerializeField]
        private GvrBasePointer m_Reticle;                                     // This is what the gun arm should be aiming at.
        [SerializeField]
        private Projectile_Shell NewProjectile;
       
        float nextFire = 1f;
        float fireRate = 1f;
        private const float k_DampingCoef = -20f;                                       // This is the coefficient used to ensure smooth damping of this gameobject.


        private void Awake()
        {
            
        }


        private void OnEnable()
        {
            
            //m_VRInput.OnDown += HandleDown;
        }


        private void OnDisable()
        {
            //m_VRInput.OnDown -= HandleDown;
        }


        private void Update()
        {
            
            
            //create delay btw firing time, 0.5 sec between each shot

            if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;

                // If the game isn't playing don't do anything.
                if (GameController.instance.gameState != GameState.IsPlaying)
                    return;
                //raycast target hitting using GVR raycast

                GameObject shootingTarget; 
                GvrBasePointer reticle;
                //reticle.PointerIntersection   --Determine what object the reticle raycast is intersecting with...if there is one
                //Transform target = shootingTarget ? shootingTarget.transform : null;

                StartCoroutine(Fire());
        }


            /*
             * Smoothly interpolate the weapon rotation towards that of the user/camera.  NOT USING NOW
             */
             
            /*
            transform.rotation = Quaternion.Slerp(transform.rotation, Camera.main.transform.localRotation,
                m_Damping * (1 - Mathf.Exp(k_DampingCoef * Time.deltaTime)));

            // Move this gameobject to the camera.
            transform.position = m_CameraTransform.position;

            // Find a rotation for the gun to be pointed at the reticle.
            Quaternion lookAtRotation = Quaternion.LookRotation(m_Reticle.ReticleTransform.position - m_GunContainer.position);

            // Smoothly interpolate the gun's rotation towards that rotation.
            m_GunContainer.rotation = Quaternion.Slerp(m_GunContainer.rotation, lookAtRotation,
                m_GunContainerSmoothing * Time.deltaTime);
                */
        }


    private IEnumerator Fire()
    {
        //Custom reticle raycast
        //Raycast from player to check for PlayerContainer Collision
        Vector3 fwd = Camera.main.transform.TransformDirection(Vector3.forward) * 2;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //layer mask to only collide with PlayerOnly layer
        int layerMask = 1 << 8;

        Debug.DrawRay(Camera.main.transform.position, fwd, Color.red);
        Transform shootPoint = m_GunEnd;
        Instantiate<Projectile_Shell>(NewProjectile, shootPoint.position,shootPoint.rotation);

        // Play the sound of the gun firing.
        //m_GunAudio.Play();

        // Set the length of the line renderer to the default.
        float lineLength = m_DefaultLineLength;

        yield return null;

    }

    private IEnumerator FireTarget(Transform target)
        {
            // Play the sound of the gun firing.
            m_GunAudio.Play();

            // Set the length of the line renderer to the default.
            float lineLength = m_DefaultLineLength;

            // If there is a target, the line renderer's length is instead the distance from the gun to the target.
            if (target)
                lineLength = Vector3.Distance(m_GunEnd.position, target.position);

        yield return null;
        }

    }
