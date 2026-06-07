using UnityEngine;
using System.Collections;

public class DialogueTrigger : MonoBehaviour
{
    // seprate dialogues
    public Dialogue dialogueTutorial;
    public Dialogue dialogueTutorial5;
    public Dialogue dialogueTutorial10;
    public Dialogue dialogueTutorial15;
    public Dialogue dialogueTutorial20;

    public Dialogue dialogue;
    public Dialogue dialogue1;
    public Dialogue dialogue2;

    [SerializeField] GameObject nextButton;
    [SerializeField] GameObject optionsButton;

    private DialogManager dialogM;
    private RoundControler roundControler;
    public SwordStateManager sword;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogM = GameObject.FindWithTag("dialogue").GetComponent<DialogManager>();
        roundControler = gameObject.GetComponent<RoundControler>();
    }

    public void TriggerDialogue ()
    {
        // starts the basic dialogue of the entity
        dialogM.StartDialogue(dialogue, true, 0);
    }

    public void SendAnswer ()
    {
        // diaplays buttons for a yes or no question
        nextButton.SetActive(false);
        optionsButton.SetActive(true);
    }

// activated by the yes button
    public void StartRoundAnswerYes()
    {
        // Start the round
        dialogM.StartDialogue(dialogue1, false, 0);
        roundControler.RoundTrigger();
    }

// activated by the no button
    public void StartRoundAnswerNo()
    {
        // Dont start the round
        dialogM.StartDialogue(dialogue2, false, 0);
    }

// starts the tutorial dialogue
    public void Tutorial()
    {
        dialogM.StartDialogue(dialogueTutorial, false, 0);
        sword.EnableSpellSend();
    }
    public void TutorialRound5()
    {
        dialogM.StartDialogue(dialogueTutorial5, false, 0);
        sword.EnableSpellSend();
    }
    public void TutorialRound10()
    {
        dialogM.StartDialogue(dialogueTutorial10, false, 0);
        sword.EnableSpellSend();
    }
    public void TutorialRound15()
    {
        dialogM.StartDialogue(dialogueTutorial15, false, 0);
    }
    public void TutorialRound20()
    {
        dialogM.StartDialogue(dialogueTutorial20, false, 0);
    }

// passes the tutorial as complete to the controlin script
    public void Pass()
    {
        roundControler.tutorial = true;
        roundControler.talking = false;
    }
}
 