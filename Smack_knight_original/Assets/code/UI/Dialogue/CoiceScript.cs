using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CoiceScript : MonoBehaviour
{

    private InputAction Yes;
    private InputAction No;

    [SerializeField] private Button YesButton;
    [SerializeField] private Button NoButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Yes = InputSystem.actions.FindAction("attack-left");
        No = InputSystem.actions.FindAction("attack-right");
    }

    // Update is called once per frame
    void Update()
    {
        if(Yes.triggered){
            YesButton.onClick.Invoke();
        }
        else if(No.triggered){
            NoButton.onClick.Invoke();
        }
    }
}
