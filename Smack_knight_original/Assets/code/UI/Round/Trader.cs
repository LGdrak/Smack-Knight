using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class Trader : MonoBehaviour
{
    private GameObject player;
    SwordStateManager sword;
    MoveControler walk;
    private InputAction dialogueAction;
    private DialogueTrader dialogueTrigger;
    [SerializeField] private GameObject Trigger;
    [SerializeField] public GameObject Orbital;
    [SerializeField] public GameObject OrbitalShoot;

    public bool tutorial = true;
    public bool talking = false;
    private bool inRange = false;

    int index;
    int counting = 0;
    [SerializeField] GameObject holder;
    GameObject item;
    PowerUp powerUp;
    [SerializeField] int chance;
    [SerializeField] private AudioSource restock;
    [SerializeField] private AudioSource upgrade;

    private RoundControler RC;
    public GameObject ItemUI;
    private TextMeshProUGUI TMP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RC = gameObject.transform.parent.GetComponent<RoundControler>();
        player = GameObject.FindWithTag("Player");

        sword = player.transform.GetChild(0).gameObject.GetComponent<SwordStateManager>();
        walk = player.GetComponent<MoveControler>();

        dialogueTrigger = gameObject.GetComponent<DialogueTrader>();
        dialogueAction = InputSystem.actions.FindAction("dialogue");
        TMP = ItemUI.GetComponent<TextMeshProUGUI>();
        if (RC.round == 0)
        {
            tutorial = false;
        }
        Restock();
// assighn the index variable
    }

    void Update()
    {
        // checks for a dialogue action and triggers a restock dialog or the tutorial dialogue
        // talking serves for not reseting dialogue with e
        if (dialogueAction.triggered && inRange && talking == false)
        {
            if (RC.round == 0 && tutorial == false){
                dialogueTrigger.Tutorial();
                talking = true;
            }
            else if (RC.round == 3 && tutorial == false){
                    dialogueTrigger.TutorialRound3();
                    talking = true;
            }
            else if (RC.round == 5 && tutorial == false){
                    dialogueTrigger.TutorialRound7();
                    talking = true;
            }
            else if (RC.round == 12 && tutorial == false){
                    dialogueTrigger.TutorialRound13();
                    talking = true;
            }
            else if (RC.round == 16 && tutorial == false){
                    dialogueTrigger.TutorialRound17();
                    talking = true;
            }
            else{
                dialogueTrigger.TriggerDialogue();
                talking = true;
            }
        }
    }

    public void Tutorial()
    {
        if(RC.round == 3 || RC.round == 6 || RC.round == 12 || RC.round == 16) {tutorial = false;}
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        // dislpays a sign that the player can talk with him
        if(other.transform.gameObject.tag == "Player"){
            Trigger.SetActive(true);
            if (tutorial == false)
            {
                Trigger.transform.GetChild(0).gameObject.SetActive(true);
            }
            inRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // deactivating the sign
        if(other.transform.gameObject.tag == "Player"){
            Trigger.SetActive(false);
            Trigger.transform.GetChild(0).gameObject.SetActive(false);
            talking = false;
            inRange = false;
        }
    } 

    public void Buy(int damage, float speed, int health, float range, int chance_, bool regeneration, float attackRate, bool blocking, bool poison, int healtValue, int LaserDamageValue, int blastDamage, bool orbital, bool orbitalShoot, int MaxSouls, bool bonusSouls, string name){
        // increasing player stats based on what stats the power up sends
        StartCoroutine(ItemName(name));
        upgrade.Play();
        Debug.Log(sword);
        sword.Damage(damage, range, attackRate, blocking, poison, healtValue, LaserDamageValue, blastDamage, MaxSouls, bonusSouls);
        walk.Stats(speed, health);
        chance = chance_;
        if(regeneration == true){walk.Regeneration();}
        if(orbital == true){Orbital.SetActive(true);}
        if(orbitalShoot == true){OrbitalShoot.SetActive(true);}
    }

// chance increases the ods for a better item
// rolls for new items
    public void Restock(){
// erases old itmes
        Close();
        restock.Play();
        for (int space = 0; space < 3; space++)
        {
            Rolling(space);
        }
    }

// deactivates all items and sets the bool selected to false
    public void Close(){
        for (int i = 0; i < holder.transform.childCount; i++)
        {
            item = holder.transform.GetChild(i).gameObject;
            item.SetActive(false);
            item.GetComponent<PowerUp>().selected = false;
        }
    }

    public IEnumerator ItemName(string name)
    {
        TMP.text = name;
        ItemUI.SetActive(true);

        yield return new WaitForSecondsRealtime(1.5f);

        ItemUI.SetActive(false);
    }

    private void Rolling(int space){

    // picking a item and substracting chance
        index = Random.Range(0, holder.transform.childCount);

        index -= chance;

        if (index < 0){
            index = 0;
        }
        if (index >= holder.transform.childCount){
            index = holder.transform.childCount - 1;
        }

        if (holder.transform.childCount != 0)
        {
            item = holder.transform.GetChild(index).gameObject;
        }

    // checking if the item picked isn't already selected and activating it
        if (item.GetComponent<PowerUp>().selected == true){
            if(counting < 10)
            {
                return;
            }
            Rolling(space);
            counting ++;
        }
        else{
            item.GetComponent<PowerUp>().selected = true;
            item.transform.position = gameObject.transform.GetChild(space).position;
            item.SetActive(true);
            counting = 0;
        }
    }

    public void ActivateItems(int[] upgrades)
    {
        player = GameObject.FindWithTag("Player");

        sword = player.transform.GetChild(0).gameObject.GetComponent<SwordStateManager>();
        walk = player.GetComponent<MoveControler>();

        dialogueTrigger = gameObject.GetComponent<DialogueTrader>();
        dialogueAction = InputSystem.actions.FindAction("dialogue");
        
        foreach (int upgrade in upgrades)
        {
            for (int i = 0; i < holder.transform.childCount; i++)
            {
                item = holder.transform.GetChild(i).gameObject;

                if (item.GetComponent<PowerUp>().index == upgrade)
                {
                    item.GetComponent<PowerUp>().BuyUpgrade();
                }
            }
        }
    }
}
