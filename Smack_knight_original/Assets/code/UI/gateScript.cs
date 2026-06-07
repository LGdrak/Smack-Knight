using UnityEngine;
using System.Collections;

public class gateScript : MonoBehaviour
{

    [SerializeField] public Collider2D gate;
    [SerializeField] public Animator gateAnim;
    [SerializeField] public Collider2D GateMain;

// opens the gate when the player starts the game
    void OnTriggerEnter2D(Collider2D col){
        gate.enabled = false;
        StartCoroutine(OpenGate());
    }

    public IEnumerator OpenGate()
    {
        gateAnim.SetTrigger("OpenDoor");

        yield return new WaitForSecondsRealtime(3f);

        GateMain.enabled = false;
    }
}
