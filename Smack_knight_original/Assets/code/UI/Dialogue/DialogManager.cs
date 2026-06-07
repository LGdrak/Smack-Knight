using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class DialogManager : MonoBehaviour
{
    private AudioSource talk;
    private Queue<string> sentences;
    [SerializeField] GameObject DialogueBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public Animator anim;
    [SerializeField] private DialogueTrigger dialogueTrigger;
    [SerializeField] private DialogueTrader dialogueTrader;

    private bool options;
    private int trigger;

    private InputAction DialogueSkip;
    private bool talking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sentences = new Queue<string>();
        DialogueSkip = InputSystem.actions.FindAction("switch-sword");

        talk = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (DialogueSkip.triggered && talking)
        {
            DisplayNextSentence();
        }
    }

// is called by the dialogue triggers
    public void StartDialogue (Dialogue dialogue, bool _options, int _trigger)
    {
        talking = true;
        nameText.text = dialogue.name;
        Time.timeScale = 0f;
        DialogueBox.SetActive(true);

        options = _options;
        trigger = _trigger;
        // assighning dialogue variables for future use

        anim.SetBool("IsOpen", true);

        sentences.Clear();

// enques the sentences
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

// displays a sentence
    public void DisplayNextSentence ()
    {
// options is defined for the possibility of a yes no answer
        if (sentences.Count == 0 && options == true)
        {
            if(trigger == 0){
                dialogueTrigger.SendAnswer();
            }
            else if(trigger == 1){
                dialogueTrader.SendAnswer();
            }
            talking = false;
            return;
        }
        else if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

// types the sentence inte the dialogue box
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            talk.Play();
            yield return new WaitForSecondsRealtime(0.03f);
        }
    }

// ends dialogue
    void EndDialogue()
    {
        if(trigger == 0){
                dialogueTrigger.Pass();
            }
        else if(trigger == 1){
                dialogueTrader.Pass();
            }
        anim.SetBool("IsOpen", false);
        talking = false;
        Time.timeScale = 1f;
    }

// skips dialogue
    public void Skip()
    {
        // checks for a posible answer
        if (options == true)
        {
            if(trigger == 0){
                dialogueTrigger.SendAnswer();
            }
            else if(trigger == 1){
                dialogueTrader.SendAnswer();
            }
            talking = false;
            return;
        }// else ends dialogue and skips tutorial
        else if (options == false)
        {
            if(trigger == 0){
            dialogueTrigger.Pass();
            }
            else if(trigger == 1){
                dialogueTrader.Pass();
            }
            anim.SetBool("IsOpen", false);
            talking = false;
            Time.timeScale = 1f;
        }
    }
}
