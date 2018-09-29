using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknessBoss : MonoBehaviour {

    public enum BossStatus
    {
        none, start,  fight, charging, rest, end
    };

    public enum BossStage
    {
        stage0, stage1, stage2, stage3
    }


    public enum BossAttackType
    {
        none, Smash, Ball, RockSmash
    };

    public GameObject MagicPos, target, ArmPivot, darkHand;
    public BossStatus status = BossStatus.start;
    public BossStage stage = BossStage.stage1;
    public BossAttackType attackState = BossAttackType.none;
    public float attackTimer = 0f, maxAttackTimer = 4f, baseMaxAttackTimer = 4f; //seconds between attacks
    public float ballMoveSpeed = 0f;
    //public FlashShield flashShield;
    public bool isDead;
    public SpriteRenderer spRenderer;
    public int hitCount = 4;
    public Sprite WoodIcon;
    public Sprite FireIcon;
    public bool doRockSmash = false;
    public ParticleSystem DarkBallFX;

    private SoundManager sm;
    string[] attackPrefabs = { "DarkBoss/Boss_Bullet", "DarkBoss/Boss_Bullet", "DarkBoss/Boss_Bullet" };
    List<GameObject> attacks = new List<GameObject>();
    List<GameObject> attacksInProgress = new List<GameObject>();
    //private GameObject flashParent;
    Animator animationControl;
    bool canAttack = false, changeAttackType = false;
    bool attackAnimFinished;
    bool rockSmashInterruptAttack = false;
    int[] attackOrder = new int[3];
    float attackChance;
    int lastAttack = 0, maxAttacks = 1;
    GameObject rockSmashTarget =null;
    Color red, blue, brown;
    Hashtable doAttackHit = new Hashtable();
    private SpriteRenderer spellIcon;

    // Use this for initialization
    void Start()
    {
        EventManager.StartListening("BossAttackAnimFinished", OnAttackAnimFinished);
        //Screen.showCursor = false;
        animationControl = GetComponent<Animator>();
        stage = BossStage.stage0;
        target = GameObject.FindGameObjectWithTag("Player");

        doAttackHit.Add("BALL", BossAttackType.Ball);
        doAttackHit.Add("SMASH", BossAttackType.Smash);
        /*flashParent = flashShield.transform.parent.gameObject;
		flashShield.activeCollider(false);*/

        status = BossStatus.none;
        sm = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        sm.PlayMusic(6);
        sm.PlayDarkWizardVox(0);

        ballMoveSpeed = 20f;

        //spellIcon = transform.Find("SpellIcon").GetComponent<SpriteRenderer>();
    }

    //Returns the rgba values to a float between 0 to 1
    float getRGBA(int n)
    {
        return Mathf.Clamp((float)n / 255f, 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        //MagicPos.transform.LookAt(target.transform.position);
        /*	if (attacks.Count < 1)
                animationControl.SetBool ("NoAttackAlive", true);
            else
                animationControl.SetBool ("NoAttackAlive", false);*/


        if (Input.GetKeyDown(KeyCode.Alpha1))
            makeTestAttack(67);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            makeTestAttack(34);
        //if(currentCharge!= null && currentCharge.activeSelf)
        //	isCharge= currentCharge.GetComponent<ChargeSpell> ().isCharge;
        //animationControl.SetBool("enterBoss",true);
        if (isDead)
        {
            status = BossStatus.end;
        }

        /*
         * Check on Player's Status and Determine what status Boss should be in
         */
         PlayerController pController;
         if(pController = target.GetComponent<PlayerController>())
        {
            //If Player is in Hurt State, Pause the Boss' Attacks
            if(pController.playerStatus == PLAYER_STATUS.HURT)
            {
                status = BossStatus.rest;
            }
        }

        /************************
         **Switch to Manage Boss Stages
         ************************/
       /* switch(stage)
        {
            case BossStage.stage1:
                checkTime(BossStage.stage2);
                areAttacksAlive();
                UpdateBoss();
                break;
            case BossStage.stage2:
                checkTime(BossStage.stage3);
                break;
        }
        */

        /************************
         **Switch to Manage Boss Status
         ***********************/   
        switch (status)
        {
            //START OF BATTLE - Setup the Boss
            case BossStatus.start:
                if (animationControl.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    changeAttack();
                    status = BossStatus.charging;
                }
                break;
                //Fight State -> boss is not attacking and can be hit
            case BossStatus.fight:
                checkHit(BossStatus.rest);
                areAttacksAlive();
                UpdateBoss();
                break;
                //Charging State -> If boss is not ready to attack, go back to "Fight" state and wait
            case BossStatus.charging:
                if (Input.GetKeyDown(KeyCode.F5) || attackTimer >= maxAttackTimer)
                {
                    attackTimer = 0;
                    status = BossStatus.fight;
                    animationControl.SetBool("AttackWait", true);
                    animationControl.SetBool("IsRest", false);
                    //StartCoroutine("FlashSpellIcon");
                }
                else
                {
                    attackTimer += Time.deltaTime;
                }
                checkHit(BossStatus.rest);
                break;
            //REST1 the Boss is vulnerable to be hit
            case BossStatus.rest:
                animationControl.SetBool("IsRest", true);
                attackState = BossAttackType.none;
                checkHit(BossStatus.end);
                break;
            case BossStatus.end:
                animationControl.SetBool("isDead", true);
                animationControl.SetBool("GotHit", false);
                animationControl.SetBool("isRest", false);
                if (animationControl.GetCurrentAnimatorStateInfo(0).IsName("BossDie2"))
                    Application.LoadLevel("6. HappyEndScene");
                //Destroy(gameObject);
                break;
        }

    }

    //Over time its either increasing the timer or not
    //when not increasing timer it does other stuff
    void UpdateBoss()
    {

        //target = GameObject.FindGameObjectWithTag("Player");
        if (attacks.Count < maxAttacks && changeAttackType)
        {
            animationControl.SetTrigger("NoAttackAlive");
            changeAttack();
            changeAttackType = false;
            status = BossStatus.charging;
        }
        else if (animationControl.GetCurrentAnimatorStateInfo(0).IsName("AttackWait") && !changeAttackType && attackState == BossAttackType.Ball)
        {   
            //doAttack(MagicPos.transform);
            //changeAttackType = true;
            animationControl.SetBool("DoProjectileAttack", true);
            StartCoroutine(DoAttackCoroutine(MagicPos.transform, target, false));
        }
        else if(animationControl.GetCurrentAnimatorStateInfo(0).IsName("AttackWait") && !changeAttackType && attackState == BossAttackType.Smash)
        {
            //doAttack(MagicPos.transform);
            //changeAttackType = true;
            animationControl.SetBool("DoSmashAttack", true);
            StartCoroutine(DoAttackCoroutine(MagicPos.transform, target, false));
        }
        else if (animationControl.GetCurrentAnimatorStateInfo(0).IsName("AttackWait") && !changeAttackType && attackState == BossAttackType.RockSmash)
        {

            //doAttack(MagicPos.transform);
            //changeAttackType = true;
            //set the target + rotation/position of "HAND" for attack
            //ArmPivot.transform.LookAt(target.transform.position);
            Vector3 tempTarget = new Vector3(0,0,0);
            if(GameController.instance.lvlManager.currentLvl.GetComponent<song2_lvl>().stageNum == 3)
                tempTarget = new Vector3(rockSmashTarget.transform.position.x, rockSmashTarget.transform.position.y, rockSmashTarget.transform.position.z);
            else
                tempTarget = new Vector3(rockSmashTarget.transform.position.x, rockSmashTarget.transform.position.y, rockSmashTarget.transform.position.z);
            float curTime = 0, rotateTime = 2f;
            //Rotate Boss to lookAt target
            while (curTime <= rotateTime)
            {
                //find the vector pointing from our position to the target
                Vector3 direction = (tempTarget - transform.position).normalized;
                Quaternion lookDirection = Quaternion.LookRotation(direction);

                //rotate us over time according to speed until we are in the required rotation
                transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, lookDirection, Time.deltaTime * rotateTime);
                curTime += Time.deltaTime;
            }

            //transform.LookAt(tempTarget);
            animationControl.SetBool("DoRockSmashAttack", true);
            StartCoroutine(DoAttackCoroutine(ArmPivot.transform, target, false));
        }
    }
    /**************************
     **Checks if the Final Boss has been hit
     **************************/
    void checkHit(BossStatus nextState)
    {
        if (nextState == BossStatus.rest && hitCount <= 1)
        {//flashShield.isShieldBroken()
            transform.localScale *= 0.7f;
            transform.localPosition += transform.up * -3;
            status = nextState;
        }
        else if (nextState == BossStatus.end && hitCount <= 0)
        {
            transform.localScale *= 0.7f;
            transform.localPosition += transform.up * -3;
            status = nextState;
        }
        else
            animationControl.SetBool("GotHit", false);
    }

    public bool getHit(string hitTag)
    {
        //If the MagicAttacks tag matches with the shield that it beats
        //or if the Boss is at Rest stage
        //then the players magic attack hurts the Boss
        if (((BossAttackType)doAttackHit[hitTag] == attackState && status != BossStatus.start))
        {
            //Debug.Log ("Boss Hit by "+hitTag);
            animationControl.SetBool("DoAttack", false);
            //animationControl.SetBool("GotHit", true);
            hitCount--;
            changeAttack();
            attackTimer = 0;

            return true;
        }
        else if (status == BossStatus.rest)
        {
            hitCount--;

            return true;
        }

        return false;
    }
    /*******************************
     **Checks the time of the scene and how many times boss has been hit
     *******************************/
    void checkTime(BossStage nextStage)
    {
        if(nextStage == BossStage.stage2 && GameController.instance.lvlManager.currentLvl.GetComponent<song2_lvl>().stageNum == 2)
        {
            transform.localScale *= 1.2f;
            stage = nextStage;
        }
        else if (nextStage == BossStage.stage3 && GameController.instance.lvlManager.currentLvl.GetComponent<song2_lvl>().stageNum == 2)
        {
            transform.localScale *= 1.2f;
            stage = nextStage;
        }
    }

    int changeAttack()
    {
        int index = -1;
        //value determines the chance of a certain attack
        float value = Random.Range(0f, 100f);
        if (doRockSmash == true)
            value = 101;
       // do
        //{
            if (value >= 70)
            {
                attackState = BossAttackType.Smash;
                index = 2;
            }
            else if(value >= 101)
            {
                attackState = BossAttackType.RockSmash;
                index = 3;
            }
            else
            {
                attackState = BossAttackType.Ball;
                index = 0;
            }
            value = Random.Range(0f, 100f);
        //} while (lastAttack == index);
        lastAttack = index;

        return index;
    }

    int makeTestAttack(float range)
    {
        Debug.Log("Test Attack?");
        int index = -1;
        //value determines the chance of a certain attack
        float value = Random.Range(0f, 100f);
        if (doRockSmash == true)
            value = 101;
        do
        {
            if (value >= 70)
            {
                attackState = BossAttackType.Smash;
                index = 2;
            }
            else if(value >= 101)
            {
                attackState = BossAttackType.RockSmash;
                index = 3;
            }
            else
            {
                attackState = BossAttackType.Ball;
                index = 0;
            }
            value = Random.Range(0f, 100f);
        } while (lastAttack == index);
        lastAttack = index;

        return index;
    }

    //Checks if any of the attacks are alive
    //if they are no longer active, then they are removed from attacks List
    void areAttacksAlive()
    {
        for (int i = attacks.Count - 1; i > -1; i--)
        {
            if (attacks[i].gameObject == null)
            {
                attacks.RemoveAt(i);
                i--;
                break;
            }
        }
    }

    IEnumerator DoAttackCoroutine(Transform transf, GameObject target, bool overRide)
    {
        string path = "";
        Vector3 offset = Vector3.zero;
        //RESET attackTime for boss
        attackTimer = 0;
        if (attacks.Count < maxAttacks || overRide == true)
        {
            switch (attackState)
            {
                case BossAttackType.none:
                    yield return 0;
                    break;
                case BossAttackType.Ball:
                    path = attackPrefabs[0];
                    offset = new Vector3(offset.x, offset.y, -3);
                    //animationControl.SetBool("IsRest", true);
                    //animationControl.SetBool("AttackWait", false);
                    break;
                case BossAttackType.Smash:
                    //animationControl.SetBool("IsRest", true);
                    // animationControl.SetBool("AttackWait", false);
                    path = attackPrefabs[1];
                    //offset = new Vector3(offset.x, offset.y, -3);
                    break;
                case BossAttackType.RockSmash:
                    //animationControl.SetBool("IsRest", true);
                    //animationControl.SetBool("AttackWait", false);

                    path = attackPrefabs[2];
                    offset = new Vector3(offset.x, offset.y, -3);
                    break;
            }
            //set boss status to wait for next attack time
            //status = BossStatus.charging;
            changeAttackType = true;
            GameObject newGb;// = Instantiate(Resources.Load<GameObject>(path), transf.position + offset, Quaternion.identity) as GameObject;
            GameObject newGb_ball2 = null;
            GameObject newGb_ball3 = null;
            if (attackState == BossAttackType.RockSmash)
            {
                //newGb.SetActive(false);
                //newGb.GetComponent<DarkBossAttack>().Target = target;
                //newGb.GetComponent<DarkBossAttack>().attackType = "RockSmash";
                newGb = darkHand;
                newGb.GetComponent<DarkBossAttack>().Target = target;
                newGb.GetComponent<DarkBossAttack>().attackType = "RockSmash";
            }
            else if(attackState == BossAttackType.Smash)
            {
                
                //Ball1
                newGb = Instantiate(Resources.Load<GameObject>(path), MagicPos.transform, false) as GameObject;
                newGb.GetComponent<DarkBossAttack>().Target = target;
                newGb.GetComponent<DarkBossAttack>().attackType = "Smash";
                newGb.GetComponent<DarkBossAttack>().attackSpeed = ballMoveSpeed;

                //Ball2
                newGb_ball2 = Instantiate(Resources.Load<GameObject>(path), MagicPos.transform, false) as GameObject;
                newGb_ball2.GetComponent<DarkBossAttack>().Target = target;
                newGb_ball2.GetComponent<DarkBossAttack>().attackType = "Smash";
                newGb_ball2.GetComponent<DarkBossAttack>().attackSpeed = ballMoveSpeed;

                //Ball3
                newGb_ball3 = Instantiate(Resources.Load<GameObject>(path), MagicPos.transform, false) as GameObject;
                newGb_ball3.GetComponent<DarkBossAttack>().Target = target;
                newGb_ball3.GetComponent<DarkBossAttack>().attackType = "Smash";
                newGb_ball3.GetComponent<DarkBossAttack>().attackSpeed = ballMoveSpeed;
            }
            else
            {
                newGb = Instantiate(Resources.Load<GameObject>(path), MagicPos.transform, false) as GameObject;
                newGb.GetComponent<DarkBossAttack>().Target = target;
                newGb.GetComponent<DarkBossAttack>().attackType = "Ball";
                newGb.GetComponent<DarkBossAttack>().attackSpeed = ballMoveSpeed;
            }

            Debug.Log("[Dark Boss] Attack: " + attackState);
            newGb.transform.parent = transf;
            attacks.Add(newGb);
            attacksInProgress.Add(newGb);
            if(newGb_ball2 != null)
            {
                newGb_ball2.transform.parent = transf;
                newGb_ball3.transform.parent = transf;

                attacks.Add(newGb_ball2);
                attacksInProgress.Add(newGb_ball2);
                attacks.Add(newGb_ball3);
                attacksInProgress.Add(newGb_ball3);
            }
            //Wait for attack animation to finish
            /*
            yield return new WaitUntil(() => attackAnimFinished == true);

            //Attach my FinalBossController to the new FinalBossAttack script in newGb
            //attack will move after the animation
            newGb.GetComponent<DarkBossAttack>().moveAttack();

            //Create Attack Projectile HERE
            //target.GetComponent<CreateMagicBoss>().BossAttack = newGb;

            

            attackAnimFinished = false;
            animationControl.SetBool("DoProjectileAttack", false);
            animationControl.SetBool("DoRockSmashAttack", false);
            animationControl.SetBool("DoSmashAttack", false);
            animationControl.SetBool("IsRest", true);
            animationControl.SetBool("AttackWait", false);*/
            yield return 0;
        }
    }
    //NO LONGER USING...Use Coroutine instead.  Checks the Bosses current attack type and instantiates the correct boss attack
    void doAttack(Transform transf)
    {
        string path = "";
        Vector3 offset = Vector3.zero;
        if (attacks.Count < maxAttacks)
        {
            switch (attackState)
            {
                case BossAttackType.none:
                    return;
                case BossAttackType.Ball:
                    path = attackPrefabs[0];
                    offset = new Vector3(-2, 2, 0);
                    //animationControl.SetBool("IsRest", true);
                    //animationControl.SetBool("AttackWait", false);
                    break;
                case BossAttackType.Smash:
                    //animationControl.SetBool("IsRest", true);
                   // animationControl.SetBool("AttackWait", false);
                    path = attackPrefabs[1];
                    offset = new Vector3(0, 0, 0);
                    break;
                case BossAttackType.RockSmash:
                    //animationControl.SetBool("IsRest", true);
                    //animationControl.SetBool("AttackWait", false);
                    path = attackPrefabs[2];
                    offset = new Vector3(0, 0, 0);
                    break;
            }

            //Wait for attack animation to finish
            do
            {
                Debug.Log("animation in progress");
            } while (!attackAnimFinished);

            //attack will be created after the animation
            Debug.Log("[Dark Boss] Attack: " + attackState);
            GameObject newGb = Instantiate(Resources.Load<GameObject>(path), transf.position + offset, Quaternion.identity) as GameObject;
            //Attach my FinalBossController to the new FinalBossAttack script in newGb
            newGb.GetComponent<DarkBossAttack>().Target = target;
            newGb.transform.parent = transf;
            attacks.Add(newGb);
            //Create Attack Projectile HERE
            //target.GetComponent<CreateMagicBoss>().BossAttack = newGb;

            attackAnimFinished = false;
            animationControl.SetBool("DoProjectileAttack", false);
            animationControl.SetBool("DoRockSmashAttack", false);
            animationControl.SetBool("DoSmashAttack", false);
            animationControl.SetBool("IsRest", true);
            animationControl.SetBool("AttackWait", false);
        }

    }

    //Call this function to force the Boss to use the RockSmash and end the stage...
    public void DoRockSmash_Interrupt(GameObject rockObj)
    {
        //animationControl.SetBool("AttackWait", true);
        //animationControl.SetBool("IsRest", false);
        //animationControl.SetBool("DoSmashAtack", false);
        //animationControl.SetBool("DoProjectileAttack", false);
        //animationControl.SetBool("DoRockSmashAttack", true);
        if (attacksInProgress.Count > 0)
            rockSmashInterruptAttack = true;
        else
        {
            attackState = BossAttackType.RockSmash;
            //StartCoroutine(DoAttackCoroutine(MagicPos.transform, rockTransform, true));
            //doAttackCoroutine(MagicPos.transform);
            changeAttackType = false;
            status = BossStatus.charging;
        }
        rockSmashTarget = rockObj;
    }


    /*
     *Event Callbacks *
     */
    public void OnAttackAnimFinished(string evt)
    {
        GameObject attack;
        if (attacksInProgress.Count > 0)
        {
            attack = attacksInProgress[0];
            attacksInProgress.RemoveAt(0);

            //reset attack timer when attack's animation has finished
            attackTimer = 0;
            if (DarkBallFX.IsAlive())
                DarkBallFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            //attackAnimFinished = true;
            Debug.Log("attackAnimFinished");
            //Attach my FinalBossController to the new FinalBossAttack script in newGb
            //projectile attack will move after the animation 
            if (attack.GetComponent<DarkBossAttack>().attackType == "Ball")
            {
                attack.GetComponent<DarkBossAttack>().moveAttack(Vector3.zero);
                attacks.Remove(attack);
            }
            else if (attack.GetComponent<DarkBossAttack>().attackType == "Smash")
            {

                Vector3 basePos = attack.GetComponent<DarkBossAttack>().Target.transform.position; //GameObject.FindGameObjectWithTag("PlayerContainer").transform.position;
                //Vector3 targetPosLocal = attack.GetComponent<DarkBossAttack>().Target.transform.localPosition;
                //Vector3 offsetVector = new Vector3(targetPosLocal.x + 7, targetPosLocal.y, targetPosLocal.z);
                Vector3 targetPos = attack.GetComponent<DarkBossAttack>().Target.transform.TransformPoint(new Vector3(7, 0, 0));
                Vector3 targetPos2 = attack.GetComponent<DarkBossAttack>().Target.transform.TransformPoint(new Vector3(-7, 0, 0));
                //Vector3 targetPos = new Vector3(basePos.x + 7, basePos.y, basePos.z);

                //Vector3 targetPos2 = new Vector3(basePos.x - 7, basePos.y, basePos.z);

                GameObject attack3;
                GameObject attack2;
                attack2 = attacksInProgress[0];
                attack3 = attacksInProgress[1];

                attack.GetComponent<DarkBossAttack>().moveAttack(Vector3.zero);
                attack2.GetComponent<DarkBossAttack>().moveAttack(targetPos);
                attack3.GetComponent<DarkBossAttack>().moveAttack(targetPos2);
                
                //DarkBossAttack temp = attack.GetComponent<DarkBossAttack>();
               // attack.GetComponent<DarkBossAttack>().moveAttack(temp.Target.transform.position);
                attacks.Remove(attack);
                attacks.Remove(attack2);
                attacks.Remove(attack3);
                attacksInProgress.RemoveAt(0);
                attacksInProgress.RemoveAt(0);
            }
            else if (attack.GetComponent<DarkBossAttack>().attackType == "RockSmash")
            {
                attacks.Remove(attack);
            }


            if (rockSmashInterruptAttack)
            {
                animationControl.SetBool("DoProjectileAttack", false);
                animationControl.SetBool("DoRockSmashAttack", false);
                animationControl.SetBool("DoSmashAttack", false);
                animationControl.SetBool("IsRest", false);
                animationControl.SetBool("AttackWait", true);
                rockSmashInterruptAttack = false;
                changeAttackType = false;
                attackState = BossAttackType.RockSmash;
            }
            else
            {
                if (attack)
                    //reset animation bools
                    animationControl.SetBool("DoProjectileAttack", false);
                animationControl.SetBool("DoRockSmashAttack", false);
                animationControl.SetBool("DoSmashAttack", false);
                animationControl.SetBool("IsRest", true);
                animationControl.SetBool("AttackWait", false);
            }
        }
    }

    IEnumerator FlashSpellIcon()
    {
        if (attackState != BossAttackType.none)
        {
            switch (attackState)
            {
                case BossAttackType.Ball:
                    spellIcon.sprite = FireIcon;
                    break;
            }

            spellIcon.color = new Color(1, 1, 1, 0.0f);
            float counter = 0.0f;
            while (true)
            {
                spellIcon.color = new Color(1, 1, 1, counter * 0.7f);

                counter += Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
                if (counter >= 1.0f)
                    break;
            }

            yield return new WaitForSeconds(0.5f);
            counter = 0.0f;
            while (true)
            {
                spellIcon.color = new Color(1, 1, 1, 0.7f - counter);

                counter += Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
                if (counter >= 1.0f)
                    break;
            }
        }
    }

    //Create FX for DarkBall Attack
    void DarkBallAttack(int side)
    {
        /*
         * 1 = left side
         * 2 = right side
         */
        Transform transf = MagicPos.transform;
        
        //null reference here...in mobile
        //GameObject newFX = Instantiate(DarkBallFX.gameObject, transf.position, Quaternion.identity) as GameObject;
        DarkBallFX.Play();
        //newFX.transform.parent = transf;

    }
}
