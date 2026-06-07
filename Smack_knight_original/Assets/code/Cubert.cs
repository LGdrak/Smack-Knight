using UnityEngine;

public class Cubert : MonoBehaviour
{
    public Dialogue dialogueTutorial;
    private DialogManager dialogM;

    private bool tutorial = true;
    public GameObject Trigger;
    private bool inRange = false;
    public Collider2D col;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogM = GameObject.FindWithTag("dialogue").GetComponent<DialogManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if( tutorial == false && inRange == true)
        {
            TriggerDialogue();
            tutorial = true;
        }
    }

       void OnTriggerEnter2D(Collider2D other)
    {
        // dislpays a sign that the player can talk with him
        if(other.transform.gameObject.tag == "Player" && tutorial == false){
            Trigger.SetActive(true);
            inRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // deactivating the sign
        if(other.transform.gameObject.tag == "Player" && tutorial == false){
            Trigger.SetActive(false);
            inRange = false;
        }
    } 

    private void TriggerDialogue ()
    {
        // starts the dialogue with cubert
        dialogM.StartDialogue(dialogueTutorial, false, 2);
        col.enabled = false;
        Trigger.SetActive(false);
        inRange = false;
    }

    public void Activate()
    {
        tutorial = false;
        col.enabled = true;
    }
}
