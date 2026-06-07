using UnityEngine;
using System.Collections;

public class DialogueTrader : MonoBehaviour
{
    public Dialogue dialogueTutorial;
    public Dialogue dialogueTutorial3;
    public Dialogue dialogueTutorial7;
    public Dialogue dialogueTutorial13;
    public Dialogue dialogueTutorial17;
    public Dialogue dialogue;
    public Dialogue dialogue1;
    public Dialogue dialogue2;
    public Dialogue dialoguePoor;

    [SerializeField] GameObject nextButton;
    [SerializeField] GameObject optionsButton;

    private DialogManager dialogM;
    private Trader Trader;
    private GameObject player;
    SwordStateManager sword;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogM = GameObject.FindWithTag("dialogue").GetComponent<DialogManager>();
        Trader = gameObject.GetComponent<Trader>();

        player = GameObject.FindWithTag("Player");
        sword = player.transform.GetChild(0).gameObject.GetComponent<SwordStateManager>();
    }

    public void TriggerDialogue()
    {
        // starts the basic dialogue of the trader
        dialogM.StartDialogue(dialogue, true, 1);
    }

// starts the tutorial dialogue
    public void Tutorial()
    {
        dialogM.StartDialogue(dialogueTutorial, false, 1);
    }
    public void TutorialRound3()
    {
        dialogM.StartDialogue(dialogueTutorial3, false, 1);
    }
    public void TutorialRound7()
    {
        dialogM.StartDialogue(dialogueTutorial7, false, 1);
    }
    public void TutorialRound13()
    {
        dialogM.StartDialogue(dialogueTutorial13, false, 1);
    }
    public void TutorialRound17()
    {
        dialogM.StartDialogue(dialogueTutorial17, false, 1);
    }

    public void SendAnswer ()
    {
        // diaplays buttons for a yes or no question
        nextButton.SetActive(false);
        optionsButton.SetActive(true);
    }

// activated by the yes button
    public void RestockYes()
    {
        // resstocks items if he has a soul
        if (sword.souls >= 1){
            dialogM.StartDialogue(dialogue1, false, 1);
            sword.souls -= 1;
            Trader.Restock();
        }
        else{
            dialogM.StartDialogue(dialoguePoor, false, 1);
        }

    }

// activated by the no button
    public void RestockNo()
    {
        // doesn't restock
        dialogM.StartDialogue(dialogue2, false, 1);
    }
    
// passes the tutorial as complete to the controlin script
    public void Pass()
    {
        Trader.tutorial = true;
        Trader.talking = false;
    }
}
