using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Numerics;
using UnityEngine.Audio;

public class RoundControler : MonoBehaviour
{
    private SaveFile file;
    private EnemySpawner Sp;
    // counts the round the player plays
    public GameObject equipped;
    public UIHeart uiHeart;
    public int round = 0;
    // checks if there are enemyes
    public bool fighting;
    public bool inRange = false;
    public bool done = true;
    public bool tutorial = false;
    public bool talking = false;
    // the trader
    [SerializeField] GameObject trader;
    [SerializeField] GameObject DialogueBox;
    Trader trader_script;
    [SerializeField] gateScript GT;
//the player
    private GameObject player;
    private MoveControler walk;
    private SwordStateManager sword;
    public GameObject RoundText;
// dialogue
    private InputAction dialogueAction;
    private DialogueTrigger dialogueTrigger;
    [SerializeField] private GameObject Trigger;

    [SerializeField] private AudioSource fight;
    [SerializeField] private AudioSource clear;
    public AudioMixerSnapshot chillTrack;
    public AudioMixerSnapshot battleTrack;
    public GameObject AudioOBJ;
    public Cubert Cubert;
    public DeathMenu DM;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Sp = transform.GetChild(0).gameObject.GetComponent<EnemySpawner>();
        trader_script = trader.GetComponent<Trader>();

        player = GameObject.FindWithTag("Player");
        walk = player.GetComponent<MoveControler>();
        sword = player.transform.GetChild(0).gameObject.GetComponent<SwordStateManager>();

        dialogueTrigger = gameObject.GetComponent<DialogueTrigger>();
        dialogueAction = InputSystem.actions.FindAction("dialogue");

        SwitchToChillTrack();
        LoadPlayer();

        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if(fighting){
            // Checking for enemyes left in the round
            if(transform.GetChild(0).childCount == 1 && done){
                // ending the round and activatig the trader, increasing the round
                round += 1;
                fighting = false;
                trader.SetActive(true);
                RoundText.SetActive(false);
                trader_script.Restock();
                trader_script.Tutorial();
                SwitchToChillTrack();
                if (round == 5 || round == 10 || round == 15 || round == 19){tutorial = false;}
                if (round == 7){Cubert.Activate();}
                if (round == 20){DM.Win();}
                clear.Play();
                walk.EndRoundRegen();
                SaveRound(true);
            }
        }
        else{
            // talking serves for not reseting dialogue with e
            if (dialogueAction.triggered && inRange && talking == false)
            {
                // managing dialogue
                if (round == 0 && tutorial == false){
                    DialogueBox.SetActive(true);
                    dialogueTrigger.Tutorial();
                    talking = true;
                }
                else if (round == 5 && tutorial == false){
                    dialogueTrigger.TutorialRound5();
                    talking = true;
                }
                else if (round == 10 && tutorial == false){
                    dialogueTrigger.TutorialRound10();
                    talking = true;
                }
                else if (round == 15 && tutorial == false){
                    dialogueTrigger.TutorialRound15();
                    talking = true;
                }
                else if (round == 19 && tutorial == false){
                    dialogueTrigger.TutorialRound20();
                    talking = true;
                }
                else{
                    DialogueBox.SetActive(true);
                    dialogueTrigger.TriggerDialogue();
                    talking = true;
                }
            }
        }
        
    }

        // dislpays a sign that the player can talk with him
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.transform.gameObject.tag == "Player" && fighting == false){
            Trigger.SetActive(true);
            if (tutorial == false)
            {
                Trigger.transform.GetChild(0).gameObject.SetActive(true);
            }
            inRange = true;
        }
    }

// deactivating the sign
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.transform.gameObject.tag == "Player" && fighting == false){
            Trigger.SetActive(false);
            Trigger.transform.GetChild(0).gameObject.SetActive(false);
            talking = false;
            inRange = false;
        }
    }    

// Is trigered by dialogue and prepars for starting the round
    public void RoundTrigger(){
        RoundStart();
        // on round 0 creating boundaries
        if(round == 0){
            GT.GateMain.enabled = true;
            GT.gateAnim.SetTrigger("OpenDoor");
            walk.Boundaries(true);
        }
    }

// starts the round, spawning enemyes and deactivates the trader
    public void RoundStart(){
        SwitchToBattleTrack();
        fight.Play();
        trader_script.Close();
        trader.SetActive(false);
        Trigger.SetActive(false);
        done = false;
        // calling the round with Sp
        Sp.Spawning(round);
        // Can be used for the button animation
        fighting = true;
    }

    public void SaveRound(bool _load)
    {
        SaveManager.SavePlayer(_load, round, walk.hurtState.currentHealth, sword.souls, equipped);
    }

    public void LoadPlayer()
    {

        try
        {
            file = SaveManager.LoadPlayer();

            if (file.load == true)
            {
                try
                {
                    Debug.Log("LOADING THE PLAYER?");
                    round = file.round;

                    UnityEngine.Vector3 position;
                    position.x = file.position[0];
                    position.y = file.position[1];
                    position.z = file.position[2];

                    player.transform.position = position;

                    sword.souls = file.souls;

                    walk.hurtState.currentHealth = file.currentHealth;
                    uiHeart.currentHealth = file.currentHealth;
                    uiHeart.Heal(0);

                    if (file.round > 0)
                    {
                        GT.gate.enabled = false;
                        GT.GateMain.enabled = true;
                        walk.Boundaries(true);
                    }

                    tutorial = true;

                    if (file.round >= 1)
                    {
                        sword.EnableSpellSend();
                    }
                    if (file.round >= 6)
                    {
                        sword.EnableSpellSend();
                    }
                    if (file.round >= 11)
                    {
                        sword.EnableSpellSend();
                    }
                    if (round == 5 || round == 10 || round == 15 || round == 19){tutorial = false;}
                    if (round == 7){Cubert.Activate();}
                    trader.SetActive(true);
                    RoundText.SetActive(false);

                    trader_script.ActivateItems(file.upgrades);
                    Debug.Log("Items loaded!!!!!");

                    trader_script.Restock();

                    Debug.Log("yes");
                }
                catch (System.Exception)
                {
                    
                    UnityEngine.Debug.Log("New Run!");
                    round = 0;
                    sword.souls = 0;
                    uiHeart.currentHealth = 6;
                    walk.hurtState.currentHealth = 6;
                }

            }
            else
            {
                UnityEngine.Debug.Log("New Run!");
                round = 0;
                sword.souls = 0;
                uiHeart.currentHealth = 6;
                walk.hurtState.currentHealth = 6;
            }
        }
        catch (System.Exception)
        {
            UnityEngine.Debug.Log("New Run! - no file");
        }        
    }

    private void SwitchToChillTrack()
    {
        // resets the song from the beginning and transitions
        chillTrack.TransitionTo(0f);
        AudioOBJ.SetActive(false);
        AudioOBJ.SetActive(true);
    }

    private void SwitchToBattleTrack()
    {
        // resets the song from the beginning and transitions
        battleTrack.TransitionTo(0f);
        AudioOBJ.SetActive(false);
        AudioOBJ.SetActive(true);
    }
}
