using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {
	
	private GameObject BGMusic;
	private GameObject foley;
	private GameObject voxPlayer;
	private GameObject voxMaster;
	private GameObject voxDarkWizard;
	
	//for instantiating foley sounds
	public GameObject newSound;

    //
    public AudioClip[] BGMusicList;
	
	private int playCount = 0;
	private int[] soundSeq;
    private List<AudioSource> allAudioSourcesTemp = new List<AudioSource>();
	
	private string musicPath = "Sounds/BGMusic/";
	private string foleyPath = "Sounds/FX/";
	private string playerVoxPath = "Sounds/VoxPlayer/";
	private string masterVoxPath = "Sounds/VoxMaster/";
	private string monsterPath = "Sounds/Monsters/";
	private string darkWizardVoxPath = "Sounds/VoxDarkWizard/";
	//private string feedbackPath = "Sounds/Feedback/";
	
	//private int[] results = new int[] {15, 1645, 135, 567};
	
	//Music
	private string[] music = new string[] {"NB!2", "Cooly2","Cooly2_edit"};
	
	//Foley sounds
	private string[] foleySounds = new string[] {"wavesIncoming", "wavesCrashing"};
	
	//Player Vox
	private string[] playerLines = new string[] {"ICanUseThisTeleportation", "Master", "MyMasterTaughtMe", "MaybeFireMagic", "ThereAreMoreSpells", "FireMonstersMyWaterSpell"};

	//Dark Wizard Vox
	private string[] darkWizardLines = new string[] {"FoolishChild", "IKnewYouDidnt", "RunHome", "YouAreNoMatch", "YouGotLucky", "NoYouAreTooPowerful"};
	
	//Master Vox
	private string[] masterLines = new string[] {"Aaah", "DontTakeMyAmulet", "SomeoneHelpMe", "IntroLines", "YouHaveGrownInto"};
	
	//Monster Vox
	private string[] monsterSounds = new string[] {"TreeMonster1", "TreeMonster2", "WaterMonster1", "WaterMonster2", "WaterMonster3", "FireMonster1", "FireMonster2", "FireMonster3"};
	
	//System Vox
	//private string[] systemLines = new string[] {"AlmostThere", "GameStart", "GoodJob", "GoodLuck", "ItemsMightBlowAway", "LookAroundFindItems", "PressTriggerPSMove", "ReadyToHelpYourFriend", "ReleaseTriggerCoverHead", "ShakeToFindOut", "Warning25Left", "WellDone", "YouLose", "YouWin!"};
	
	
	//Explosion sounds
	private string[] explosionSounds = new string[] {""};

	void Awake(){
		BGMusic = transform.Find ("BGMusic").gameObject;
		//foley = transform.Find ("Foley").gameObject;
		voxPlayer = transform.Find ("VoxPlayer").gameObject;
		voxMaster = transform.Find ("VoxMaster").gameObject;
		voxDarkWizard = transform.Find ("VoxDarkWizard").gameObject;

        //BGMusicList = Resources.LoadAll("Resources/Sounds/BGMusic") as AudioClip;
	}
	void Start () 
	{

	}

    public void LoadMusic()
    {
        BGMusicList = new AudioClip[3];
        BGMusicList[0] = Resources.Load("Sounds/BGMusic/NB!2") as AudioClip;
        BGMusicList[1] = Resources.Load("Sounds/BGMusic/Cooly2") as AudioClip;
        BGMusicList[2] = Resources.Load("Sounds/BGMusic/Cooly2_edit") as AudioClip;
    }
	
	public void PlayMusic(int num)
	{
        AudioClip newClip;
        if (BGMusicList[num] != null)
        {
            newClip = BGMusicList[num];
        }
        else
        {
            newClip = (AudioClip)Resources.Load(string.Concat(musicPath, music[num]), typeof(AudioClip));
        }
        BGMusic.GetComponent<AudioSource> ().clip = newClip;
		BGMusic.GetComponent<AudioSource> ().Play ();
	}
	
	public void PlayFoleySound(int num, float volume=0.9f)
	{

		print ("foley sound : " + num);
        AudioClip newClip = (AudioClip)Resources.Load(string.Concat(foleyPath, foleySounds[num]), typeof(AudioClip));
		//trying new logic, spawning a gameobject for the sound
		GameObject newFoley = GameObject.Instantiate(newSound);
		newFoley.transform.parent = transform;

        AudioSource newFoleyAudio = newFoley.GetComponent<AudioSource>();

        newFoleyAudio.clip = newClip;
        newFoleyAudio.volume = volume;

        //add new audio source to list
        allAudioSourcesTemp.Add(newFoleyAudio);

        //destroy all the other sounds after playing
        StartCoroutine(PlayAndDestroy (newFoley, newClip.length));
	}

	public void PlayChargingSound(string type)
	{
		int num = 0;
		
		//charging sound
		AudioClip newClip = (AudioClip)Resources.Load(string.Concat(foleyPath,foleySounds[num]), typeof(AudioClip));
		
		GameObject newFoley = (GameObject)Instantiate (newSound);
		newFoley.name = "ChargingSound";
		newFoley.transform.parent = transform;
		
		newFoley.GetComponent<AudioSource> ().clip = newClip;
		
		newFoley.GetComponent<AudioSource> ().Play ();
		newFoley.GetComponent<AudioSource>().loop = true;
		
		//loop the intro tense scene sound
		
		//StartCoroutine(PlayAndDestroy (newFoley, newClip.length));
	}
	
	public void StopChargingSound()
	{
		GameObject charge = GameObject.Find ("ChargingSound");
		Destroy (charge);
	}

	public void PlaySpellSound(int num)
	{
		print ("foley sound : " + num);
		AudioClip newClip = (AudioClip)Resources.Load(string.Concat(foleyPath,foleySounds[num]), typeof(AudioClip));
		
		//trying new logic, spawning a gameobject for the sound
		GameObject newFoley = (GameObject)Instantiate (newSound);
		newFoley.transform.parent = transform;

        AudioSource newFoleyAudio = newFoley.GetComponent<AudioSource>();

        newFoleyAudio.clip = newClip;

        newFoleyAudio.volume = 0.9f;

        StartCoroutine(PlayAndDestroy (newFoley, newClip.length));
	}
	
	public void PlayMonsterSound(string type)
	{
		
		int num;
		if(type == "tree")
			num = Random.Range (0,2);
		else if(type == "water")
			num = Random.Range (2,5);
		else if(type == "fire")
			num = Random.Range (5,8);
		else
		{
			print ("invalid monster sound");
			return;
		}
		
		AudioClip newClip = (AudioClip)Resources.Load(string.Concat(monsterPath,monsterSounds[num]), typeof(AudioClip));
		
		//trying new logic, spawning a gameobject for the sound
		GameObject newFoley = (GameObject)Instantiate (newSound);
		newFoley.transform.parent = transform;
        AudioSource newFoleyAudio = newFoley.GetComponent<AudioSource>();

        newFoleyAudio.clip = newClip;

        newFoleyAudio.volume = 0.9f;

        //add new audio source to list
        allAudioSourcesTemp.Add(newFoleyAudio);

		//loop the intro tense scene sound
		StartCoroutine(PlayAndDestroy (newFoley, newClip.length));
	}
	
	private IEnumerator PlayAndDestroy(GameObject newFoley, float pLength)
	{
        AudioSource tempAudio;
        tempAudio = newFoley.GetComponent<AudioSource>();
        tempAudio.Play();

        yield return new WaitForSeconds(pLength);
        //even when audio is stopped, this audio will be destroyed after the waitTime
        for(int i=0;i< allAudioSourcesTemp.Count;i++)
        {
            if(allAudioSourcesTemp[i] == tempAudio)
            {
                allAudioSourcesTemp.Remove(tempAudio);
            }
        }
		Destroy (newFoley);
	}

    /// <summary>
    /// stops all temporary audio.  temp audio is considered anything that is not playing in the background, mainly music
    /// </summary>
    public void StopAllTempAudio()
    {
        EventManager.TriggerEvent("StopAllTempAudio", "false");
        foreach (AudioSource audioS in allAudioSourcesTemp)
        {
            audioS.Stop();
        }
    }

    public void StopBGAudio()
    {
        AudioSource curAudio = BGMusic.GetComponent<AudioSource>();
        curAudio.Stop();
    }

    public void PauseBGAudio()
    {
        AudioSource curAudio = BGMusic.GetComponent<AudioSource>();
        curAudio.Pause();
    }

    public void ResumeBGAudio()
    {
        AudioSource curAudio = BGMusic.GetComponent<AudioSource>();
        curAudio.UnPause();
    }

    public void ChangeBgmPlaybackPos(float newTime)
    {
        AudioSource curAudio = BGMusic.GetComponent<AudioSource>();
        curAudio.time = newTime;
    }

    public void CrossFadeMusicIntroScene()
	{
		AudioClip newClip = (AudioClip)Resources.Load(string.Concat(musicPath,music[8]), typeof(AudioClip));
		GameObject newMusic = (GameObject)Instantiate (newSound);
		newMusic.transform.parent = transform;
		newMusic.GetComponent<AudioSource> ().clip = newClip;
		StartCoroutine(CrossFadeIntroScene (newMusic));
	}
	
	IEnumerator CrossFadeIntroScene (GameObject newMusic)
	{
		float curMusicLevel = BGMusic.GetComponent<AudioSource> ().volume; //current volume
		float newMusicLevel = newMusic.GetComponent<AudioSource> ().volume; //new volume
		
		AudioSource curAudio = BGMusic.GetComponent<AudioSource> ();
		AudioSource newAudio = newMusic.GetComponent<AudioSource> ();
		
		newMusicLevel = 0;
		newAudio.volume = 0;
		
		print ("starting crossfade");
		newAudio.Play (); //play new music at zero volume
		
		float crossFadeSpeed = 0.5f;
		
		while(curMusicLevel >= 0.01f && newMusicLevel <= 0.6f)
		{
			curMusicLevel = Mathf.Lerp(curMusicLevel, 0, crossFadeSpeed * Time.deltaTime); //decrease current volume
			curAudio.volume = curMusicLevel;
			
			newMusicLevel = Mathf.Lerp(newMusicLevel, 0.6f, crossFadeSpeed * Time.deltaTime); //increase new track volume
			newAudio.volume = newMusicLevel;
			
			yield return null;
		}
		
		curAudio.volume = 0;
		newAudio.volume = 0.6f;
		newAudio.loop = true; //loop the new track for intro cutscene
		
		print("Volumes set");
		
		yield return new WaitForSeconds(0f);
	}
	
	public void CrossFadeMusic(int num)
	{
		AudioClip newClip = (AudioClip)Resources.Load(string.Concat(musicPath,music[num]), typeof(AudioClip));
		GameObject newMusic = (GameObject)Instantiate (newSound);
		newMusic.transform.parent = transform;
		newMusic.GetComponent<AudioSource> ().clip = newClip;
		StartCoroutine(CrossFade (newMusic));
	}
	
	IEnumerator CrossFade (GameObject newMusic)
	{
		float curMusicLevel = BGMusic.GetComponent<AudioSource> ().volume; //current volume
		float newMusicLevel = newMusic.GetComponent<AudioSource> ().volume; //new volume
		
		AudioSource curAudio = BGMusic.GetComponent<AudioSource> ();
		AudioSource newAudio = newMusic.GetComponent<AudioSource> ();
		
		newMusicLevel = 0;
		newAudio.volume = 0;
		
		print ("starting crossfade");
		newAudio.Play (); //play new music at zero volume
		
		float crossFadeSpeed = 2.0f;
		
		while(curMusicLevel >= 0.01f)
		{
			curMusicLevel = Mathf.Lerp(curMusicLevel, 0, crossFadeSpeed * Time.deltaTime); //decrease current volume
			curAudio.volume = curMusicLevel;
			
			newMusicLevel = Mathf.Lerp(newMusicLevel, 1.0f, crossFadeSpeed * Time.deltaTime); //increase new track volume
			newAudio.volume = newMusicLevel;
			
			yield return null;
		}
		
		curAudio.volume = 0;
		newAudio.volume = 1.0f;
		newAudio.loop = true; //loop the new track for intro cutscene
		
		print("Volumes set");
		
		yield return new WaitForSeconds(0f);
	}

    public void FadeInMusic(int num)
    {
        AudioClip newClip;
        if (BGMusicList[num] != null)
        {
            newClip = BGMusicList[num];
        }
        else {
            newClip = (AudioClip)Resources.Load(string.Concat(musicPath, music[num]), typeof(AudioClip)); }

        BGMusic.GetComponent<AudioSource>().clip = newClip;
        StartCoroutine(FadeIn(BGMusic));
    }

    public IEnumerator FadeIn(GameObject newMusic)
    {
        BGMusic.GetComponent<AudioSource>().volume = 0;
        //set music level to 0
        float curMusicLevel = BGMusic.GetComponent<AudioSource>().volume;

        AudioSource curAudio = BGMusic.GetComponent<AudioSource>();

        curAudio.Play(); //play new music at zero volume

        //fade in slowly
        float crossFadeSpeed = .2f;

        while (curMusicLevel <= 0.8f)
        {

            curMusicLevel = Mathf.Lerp(curMusicLevel, 1.0f, crossFadeSpeed * Time.deltaTime); //increase new track volume
            curAudio.volume = curMusicLevel;

            yield return null;
        }
        curAudio.volume = 1.0f;
        print("Volumes set");

        yield return new WaitForSeconds(0f);
    }

	public void PlayPlayerVox(int num)
	{
		//int num = Random.Range (0, 4);
		AudioClip newClip = (AudioClip)Resources.Load(string.Concat(playerVoxPath,playerLines[num]), typeof(AudioClip));
		voxPlayer.GetComponent<AudioSource> ().clip = newClip;
		voxPlayer.GetComponent<AudioSource> ().Play ();
		//Invoke("PlayGuestVox", newClip.length);
	}
	
	public void PlayMasterVox(int num)
	{
		//int num = Random.Range (0, 12);
		AudioClip newClip = (AudioClip)Resources.Load(string.Concat(masterVoxPath,masterLines[num]), typeof(AudioClip));
		voxMaster.GetComponent<AudioSource> ().clip = newClip;
		voxMaster.GetComponent<AudioSource> ().Play ();
		//Invoke("PlayZombieVox", newClip.length + Random.Range (2.0f,7.5f));
	}

	public void PlayDarkWizardVox(int num)
	{
		//int num = Random.Range (0, 12);
		AudioClip newClip = (AudioClip)Resources.Load(string.Concat(darkWizardVoxPath,darkWizardLines[num]), typeof(AudioClip));
		voxMaster.GetComponent<AudioSource> ().clip = newClip;
		voxMaster.GetComponent<AudioSource> ().Play ();
		//Invoke("PlayZombieVox", newClip.length + Random.Range (2.0f,7.5f));
	}
	
	/*public void PlayExplosionSound()
	{
		int num = Random.Range (0, 5);
		AudioClip newClip = (AudioClip)Resources.Load(string.Concat(explosionPath,explosionSounds[num]), typeof(AudioClip));
		explosion.GetComponent<AudioSource> ().clip = newClip;
		explosion.GetComponent<AudioSource> ().Play ();
	}*/
	
	/*private void testingSequence()
	{
		int[] testArray = {8,1,9};
		PlaySystemVoxSequence (testArray);
	}*/
	
	/*IEnumerator ScheduleThunder() 
	{
		yield return new WaitForSeconds(Random.Range (5.0f, 10.0f));
		Debug.Log("After Waiting Seconds");
		PlayThunder ();
	}*/
	
	/*public void PlayWizardVox(int num)
	{
		//int num = Random.Range (0, 12);
		AudioClip newClip = (AudioClip)Resources.Load(string.Concat(zombieVoxPath,zombieLines[num]), typeof(AudioClip));
		voxZombie.GetComponent<AudioSource> ().clip = newClip;
		voxZombie.GetComponent<AudioSource> ().Play ();
		//Invoke("PlayZombieVox", newClip.length + Random.Range (2.0f,7.5f));
	}*/
	
	
	
	/*public void PlaySystemVox(int n)
	{
		AudioClip newClip = (AudioClip)Resources.Load(string.Concat(systemVoxPath,systemLines[n]), typeof(AudioClip));
		voxSystem.GetComponent<AudioSource> ().clip = newClip;
		voxSystem.GetComponent<AudioSource> ().Play ();
	}*/
	
	/*public void PlayFeedback(int n)
	{
		AudioClip newClip = (AudioClip)Resources.Load(string.Concat(feedbackPath,feedbackSounds[n]), typeof(AudioClip));
		feedback.GetComponent<AudioSource> ().clip = newClip;
		feedback.GetComponent<AudioSource> ().Play ();
	}*/
	
	/*public void PlaySystemVoxSequence(int[] seq) //eg. 1,3,5
	{
		soundSeq = seq;

		if(playCount < seq.Length) //haven't finished all clips yet
		{
			AudioClip newClip = (AudioClip)Resources.Load(string.Concat(systemVoxPath,systemLines[seq[playCount]]), typeof(AudioClip));
			voxSystem.GetComponent<AudioSource> ().clip = newClip;
			voxSystem.GetComponent<AudioSource> ().Play ();
			playCount++;
			Invoke("CheckIfSequenceDone", newClip.length);
		}
		else //all clips done, reset playcount to 0
		{
			playCount = 0;
		}
	}*/
	
	/*private void CheckIfSequenceDone()
	{
		PlaySystemVoxSequence (soundSeq);
	}*/
	
	
	
	
	
}
