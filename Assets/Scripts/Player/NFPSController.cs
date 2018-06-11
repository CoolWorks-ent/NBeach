using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;

[System.Serializable]
public class PlayerBoundary
{
    public GameObject boundary_left;
    public GameObject boundary_right;
}

//public enum FPSPlayerState { MOVING, SWIMMING, NOTMOVING }

public class NFPSController : PlayerController {

    // This script controls the gun for the shooter
    // scenes, including it'` movement and shooting.

        /*
         * Serializable Fields
         * 
         */
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
        [SerializeField]
        private ArmThrow throwArm;
        [SerializeField]
        private GUI playerHUD;
        [SerializeField]
        private Text ammoText;
        [SerializeField]
        private Text healthText;
        [SerializeField]
        public GameObject playerContainer;

        public GameController gController;
        int playerHealth;
        public int playerAmmo;
        public int maxAmmo;
        public PlayerBoundary pBoundary;

    /*
     * Local Variables
     */
        float nextFire = 1f;
        float fireRate = 1f; //fire once per second
        int invincibleTime = 1; //time player invincible after being hit 
        Projectile_Shell tempProjectile;
        bool projectileFired = false;
        bool invincibleState = false; 
        private const float k_DampingCoef = -20f;                                       // This is the coefficient used to ensure smooth damping of this gameobject.
        private LineRenderer laserLine;
        float weaponRange = 50f;
        bool bReloading = false;
        bool bProjectileInHand = false;

        //dodge mechanic variables
        float minRightRoll = -20;
        float maxRightRoll = -45;
        float minLeftRoll = 20;
        float maxLeftRoll = 45;
        float dodgeSpeed = 6f;
        Rigidbody nRigidbody;
        Rigidbody playerRigidbody;
        float dodgeCooldownTime = 1f; //1 second for cooldown
        float lastDodgeTime = 0;
        float curTime = 0;
        float normZ = 0;
        bool resetRot = true;
        bool canDodgeRight = true, canDodgeLeft = true;

        

    private void Awake()
        {
            gController = GameObject.Find("GameController").GetComponent<GameController>();
            EventManager.StartListening("FireProjectile", ProjectileFired);
            nRigidbody = playerContainer.GetComponent<Rigidbody>();
            playerRigidbody = GetComponent<Rigidbody>();
            laserLine = GetComponent<LineRenderer>();
            playerHealth = 100;
            //start player with 10 ammo
            playerAmmo = 0;
            maxAmmo = 10;
            bReloading = true;

        }


        private void OnEnable()
        {
            
            //m_VRInput.OnDown += HandleDown;
        }


        private void OnDisable()
        {
            //m_VRInput.OnDown -= HandleDown;
        }

    private void FixedUpdate()
    {
        //function to excute the player's dodge
        PlayerDodge();

    }

    private void Update()
        {

        //update ammo text on HUD
        ammoText.text = playerAmmo.ToString();
        healthText.text = playerHealth.ToString();




        /*******
         * Default - Clamp Player's Movement (only during gameplay)
         * Clamp the movement to the x-axis btw. 170 & 200
         *******/
        
        //nRigidbody.transform.localPosition = new Vector3(Mathf.Clamp(nRigidbody.transform.localPosition.x, nRigidbody.transform.localPosition.x - 21, nRigidbody.transform.localPosition.x + 10), nRigidbody.transform.localPosition.y, nRigidbody.transform.localPosition.z);

        //playerContainer.transform.position = new Vector3(Mathf.Clamp(playerContainer.transform.position.x, 170, 200), playerContainer.transform.position.y, playerContainer.transform.position.z);
        //Manage Player's Ammo and check for Firing
        if (playerAmmo > 0)
            {
                
                if (bReloading == true)
                {
                    Transform shootPoint = m_GunEnd;
                    Quaternion shootRot = Quaternion.Euler(90 + shootPoint.rotation.eulerAngles.x, 90 + shootPoint.rotation.eulerAngles.y,
                    NewProjectile.transform.rotation.z + shootPoint.rotation.z);
                    Projectile_Shell projectile = Instantiate<Projectile_Shell>(NewProjectile, shootPoint.transform.position,shootRot);
                    projectile.Initialize(m_GunEnd);    
                    bReloading = false;
                    bProjectileInHand = true;
                    nextFire = Time.time + fireRate;
            }
                else if(bProjectileInHand == false)
                {
                    StopCoroutine("CreateNewProjectile");
                    StartCoroutine(CreateNewProjectile());
                    bProjectileInHand = true;
                }
            }

        //create delay btw firing time, 0.5 sec between each shot
            if (Input.GetButtonDown("Fire1") && Time.time > nextFire )
            {
                
                // If the game isn't playing don't do anything.
                if (GameController.instance.gameState != GameState.IsPlaying)
                    return;

                //if player has no ammo, don't fire
                if (playerAmmo <= 0)
                    return;


                if (bProjectileInHand == true)
                {

                    nextFire = Time.time + fireRate;

                    //raycast target hitting using GVR raycast

                    //ShootingTarget shootingTarget = GvrReticlePointer.PointerRay.CurrentInteractible ? m_EyeRaycaster.CurrentInteractible.GetComponent<ShootingTarget>() : null;
                    GameObject shootingTarget = null;
                    GvrBasePointer reticle;
                    //CurrentRaycastResult.WorldPosi   //Determine what object the reticle raycast is intersecting with...if there is one
                    Transform target = shootingTarget ? shootingTarget.transform : null;

                    /* New FPS code from UNITY*
                     */
                    Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
                    Vector3 fwd = Camera.main.transform.TransformDirection(Vector3.forward);
                    Debug.DrawRay(rayOrigin, fwd * weaponRange, Color.green);
                    Ray ray = Camera.main.ScreenPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

                    RaycastHit hit;
                    GameObject projectile_spawn = GameObject.Find("CamCenter");
                    if (Physics.Raycast(ray, out hit, 100000.0f))
                    {
                        Debug.DrawLine(projectile_spawn.transform.position, hit.point, Color.red);
                        //projectile.GetComponent<Rigidbody>().AddForce((hit.point - projectile_spawn.transform.position).normalized * 10, ForceMode.Impulse);
                        //projectile.GetComponent<Rigidbody>().AddForce(projectile_spawn.transform.position * 50, ForceMode.Impulse);
                    }
                    else
                    {
                        //projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.position * 50, ForceMode.Impulse);
                    }
                    /*laserLine.SetPosition(0, m_GunEnd.position);

                    if (Physics.Raycast(rayOrigin, fwd, out hit, weaponRange))
                    {
                        laserLine.SetPosition(1, hit.point);
                    }
                    else
                    {
                        laserLine.SetPosition(1, rayOrigin + (fwd * weaponRange));
                    }
                    */
                    Vector3 targetPos = rayOrigin + (fwd * weaponRange);
                    StartCoroutine(Fire(targetPos));
                }
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


    /*
     * Player Dodging
     * player tilts head 45 degrees left/right and the character strafes left/right for X units
     * These is a 0.5 sec delay between dodges
     * There is a wall surround the player's play-space that constraints the dodge along the x-axis.  Player can not dodge past the 5,0 or -5,0 coordinate
     */
    void PlayerDodge()
    {
        // initials
        Quaternion headRotation = Camera.main.transform.localRotation;
        int dodgeAmt = 100;
        curTime = Time.time;
        
        Quaternion centerRot = new Quaternion(0, 0, 0, 0);
        float centerRotMin = -1 * Mathf.Deg2Rad;
        float centerRotMax = 1 * Mathf.Deg2Rad;
        float normMin = 2 * Mathf.PI + centerRotMin;
        float normTargetZ=0;
        //headRotation = (pitch, yaw, roll)
        //check for head rotation angle that is btw, 
        float zRotation = headRotation.eulerAngles.z;
        
        if (zRotation >= 180) //normalize btw. -180 < z < 180
        {
            normZ = zRotation - 360;
        }
        else
        {
            normZ = zRotation;
        }
        //add checks to see if player is colliding with triggers on the left/right sides to prevent player from dodging too far off screen

        //if dodge time has cooled down, check for the dodge 
        if ((lastDodgeTime + dodgeCooldownTime < curTime) && resetRot == true)
        {
            //Vector3 curPos = transform.position;
            bool canMove = false;

            //dodge left
            if ((normZ > minLeftRoll && normZ < maxLeftRoll))
            {
                canMove = CheckMvmtBoundary("left");
                if (canMove)
                {
                    //Check if player is colliding with the left dodge limit

                    Vector3 curPos = playerContainer.transform.localPosition;
                    Vector3 newPos = new Vector3(curPos.x - dodgeAmt, curPos.y, curPos.z);

                    //StopCoroutine("executeDodge");
                    StartCoroutine(executeDodge(nRigidbody, newPos));

                    //rigidbody.velocity = Vector3.right * dodgeSpeed;
                    lastDodgeTime = Time.time;
                    print("dodge left!");
                    resetRot = false;
                }
            }
            //dodge right
            else if ((normZ < minRightRoll && normZ > maxRightRoll))
            {
                canMove = CheckMvmtBoundary("right");
                if (canMove)
                {
                    Vector3 curPos = playerContainer.transform.localPosition;
                    Vector3 newPos = new Vector3(curPos.x + dodgeAmt, curPos.y, curPos.z);

                    //StopCoroutine("executeDodge");
                    StartCoroutine(executeDodge(nRigidbody, newPos));

                    //rigidbody.velocity = Vector3.left * dodgeSpeed;
                    lastDodgeTime = Time.time;
                    print("dodge right");
                    resetRot = false;
                }
            }
        }
        //reset dodge conditions when player returns neutral head rotation
        if ((normZ < minLeftRoll) && (normZ > minRightRoll))
        {
            resetRot = true;
        }


        //Accelerometer adding to input
        //if(Input.acceleration.x > 5)...
    }

    bool CheckMvmtBoundary(string direction)
    {
        bool canMove = true;
        //dodge barriers via trigger volumes
        switch (direction)
        {
            case "left":
                if (playerRigidbody.transform.position.x >= pBoundary.boundary_left.transform.position.x)
                {
                    //prevent further dodging to the left
                    canDodgeLeft = false;
                    canDodgeRight = true;
                    canMove = false;
                }
                break;
            case "right":
                if (playerRigidbody.transform.position.x <= pBoundary.boundary_right.transform.position.x)
                {
                    //prevent further dodging to the right
                    canDodgeLeft = true;
                    canDodgeRight = false;
                    canMove = false;
                }
                break;
            default:
                canMove = false;
                break;
        }
        return canMove;
        /*
        if (direction == "left")
        {
            if (transform.position.x >= pBoundary.boundary_left.transform.position.x)
            {
                //prevent further dodging to the left
                canDodgeLeft = false;
                canDodgeRight = true;
                return true;
            }
        }
        else if (transform.position.x <= pBoundary.boundary_right.transform.position.x)
        {
            //prevent further dodging to the right
            canDodgeLeft = true;
            canDodgeRight = false;
            return true;
        }
        return false;*/
    }

    IEnumerator executeDodge(Rigidbody body, Vector3 newPos)
    {
        float dodgeTime = 0.5f;
        float dodgeSpeed = 10f; //the lower the speed, the longer the dodge distance
        float t = 0;
        while (t < dodgeTime)
        {
            //body.MovePosition(newPos * dodgeSpeed * Time.deltaTime);
            //playerContainer.transform.position = Vector3.MoveTowards(playerContainer.transform.position, newPos, dodgeSpeed * Time.deltaTime);
            playerContainer.transform.localPosition = Vector3.MoveTowards(playerContainer.transform.localPosition, newPos, dodgeSpeed * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator SpeedBoostRoutine()
    {
        //increase speed of player 
        float speedTime = 4f;//speedBoost.speedTime;
        float initBoostTime = 2f;//speedBoost.initBoostTime;
        float boostAmt = 2f;//speedBoost.boostAmt;
        float timeElapsed = 0f;

        //plays the speed effect for x seconds
        while (timeElapsed < speedTime)
        {
            //increase speed instantly.  but need to set up a gradual increase instead.
            gController.pathControl.pathSpeed += boostAmt;

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        //stop speed boost
        EventManager.TriggerEvent("Player_SpeedBoostOff", "Player_SpeedBoost");
    }


    /* 
     * Projectile and Weapon Based Functions
     */
    IEnumerator CreateNewProjectile()
    {
        float time = .4f;
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            yield return 0;
        }
        if (t > time)
        {
            Transform shootPoint = m_GunEnd;
            Quaternion shootRot = Quaternion.Euler(90 + shootPoint.rotation.eulerAngles.x, 90 + shootPoint.rotation.eulerAngles.y,
            NewProjectile.transform.rotation.z + shootPoint.rotation.z);
            Instantiate(NewProjectile, shootPoint.transform.position, shootRot).GetComponent<Projectile_Shell>().Initialize(m_GunEnd);
            //projectile.Initialize(m_GunEnd)
        }

        yield return null;
    }

    private IEnumerator Fire(Vector3 targetPos)
    {
        //Custom reticle raycast
        //Raycast from player to check for PlayerContainer Collision
        Vector3 fwd = Camera.main.transform.TransformDirection(Vector3.forward) * 2;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //layer mask to only collide with PlayerOnly layer
        int layerMask = 1 << 15;

        Debug.DrawRay(Camera.main.transform.position, fwd, Color.red);
        throwArm.GetComponent<Animator>().SetTrigger("ThrowTrigger");

        //EventManager.TriggerEvent("FireProjectile", "fireprojectile");
        //projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * 200,);
        //StartCoroutine(projectile.MoveProjectileForce());
        //projectile.moveProjectileFunc(targetPos);

        //projectile
        /*Transform shootPoint = m_GunEnd;
        Quaternion shootRot = Quaternion.Euler(90 + shootPoint.rotation.eulerAngles.x, 90+ shootPoint.rotation.eulerAngles.y, 
            NewProjectile.transform.rotation.z + shootPoint.rotation.z);
        Instantiate<Projectile_Shell>(NewProjectile, shootPoint.position, shootRot);
        */
        // Play the sound of the gun firing.new
        //m_GunAudio.Play();

        // Set the length of the line renderer to the default.
        float lineLength = m_DefaultLineLength;

        // If there is a target, the line renderer's length is instead the distance from the gun to the target.
        //if (target)
          //  lineLength = Vector3.Distance(m_GunEnd.position, target.position);

        yield return null;

    }

    void ProjectileFired(string evt)
    {
        playerAmmo -= 1;

        bProjectileInHand = false;
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
    public void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Darkness")
        {
            //call damage function
            OnPlayerDamaged(collision.transform);
        }
        if (collision.gameObject.tag == "DarkBossAttack")
        {
            OnPlayerDamaged(collision.transform);
        }

    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Darkness")
        {
            //call damage function
            OnPlayerDamaged(collider.transform);
        }
        if(collider.gameObject.tag == "DarkBossAttack")
        {
            OnPlayerDamaged(collider.transform);
        }
        

        //dodge barriers via trigger volumes
        /*if(collider.gameObject == pBoundary.boundary_left)
        {
            //prevent further dodging to the left
            canDodgeLeft = false;
            canDodgeRight = true;
        }
        else if (collider.gameObject == pBoundary.boundary_right)
        {
            //prevent further dodging to the right
            canDodgeLeft = true;
            canDodgeRight = false;
        }*/
    }

    private void OnPlayerDamaged(Transform enemy)
    {
        float curTime;
        float t;
        if (invincibleState == false)
        {
            //show damage overlay when hurt
            StartCoroutine(OnPlayerDamaged_corout());

            //player loses health based upon the type of darkness or attack. I'm hard scripting the values for now
            if (enemy.tag == "Darkness")
                playerHealth -= 10;
            if (enemy.tag == "DarkBossAttack")
            {
                playerHealth -= 10;

                //if player hit with RockSmashAttack, trigger event to fly back and hit island
                DarkBossAttack bossAttack = enemy.GetComponent<DarkBossAttack>();
                if (bossAttack.attackType == "RockSmash" && gController.lvlManager.currentLvl.GetComponent<song2_lvl>().stageNum == 3)
                {
                    EventManager.TriggerEvent("Song2_End_Cutscene_Start", "Song2_End_Cutscene_Start");
                }
            }

            curTime = Time.time;
            t = Time.time;
            invincibleState = true;
            while (curTime < t + invincibleTime)
            {

                curTime += Time.deltaTime;
            }
            invincibleState = false;
        }
        
        
    }

    //Coroutine to player effect when player is damaged
    IEnumerator OnPlayerDamaged_corout()
    {
        Image dmgOverlay = gController.dmgOverlay;

        if (dmgOverlay != null)
        {
            float time = 0;
            float screenFadeOutTime = 1f;
            float screenFadeInTime = .1f;
            Color baseColor = dmgOverlay.color;
            //fade in quickly
            while (time < screenFadeInTime)
            {
                //Debug.Log("fading in");
                dmgOverlay.color = Color.Lerp(baseColor, new Color(dmgOverlay.color.r, dmgOverlay.color.g, dmgOverlay.color.b, .8f), time / screenFadeInTime);
                time += Time.deltaTime;
                yield return null;
            }

            //fade sign out every second
            while (time < screenFadeOutTime)
            {
                //Debug.Log("fading out");
                dmgOverlay.color = Color.Lerp(baseColor, new Color(dmgOverlay.color.r, dmgOverlay.color.g, dmgOverlay.color.b, 0f), time / screenFadeOutTime);
                time += Time.deltaTime;
                yield return null;
            }
        }
        yield return 0;
    }

 }
