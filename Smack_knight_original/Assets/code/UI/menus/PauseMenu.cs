using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    private InputAction pause;
    public GameObject pauseMenuUI;
    DeathMenu deathMenu;
    private float then;

// start is updated once on the start 
    void Start()
    {
        //asighning variables
        pause = InputSystem.actions.FindAction("Pause");
        deathMenu = gameObject.GetComponent<DeathMenu>();
    }


    // Update is called once per frame
    void Update()
    {
        // Checking for player input (pause input) to activate pause menu
        if(pause.triggered && deathMenu.GameOver == false){
            if(GameIsPaused){
                Resume();
            }
            else{
                Pause();
            }
        }
    }

    void Pause(){

// activates the pause menu and stops time
        pauseMenuUI.SetActive(true);
        then = Time.timeScale;
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Resume(){
// deactivates the pause menu and resumes time
        pauseMenuUI.SetActive(false);
        if (then != 0f){
            Time.timeScale = 1f;
        }
        GameIsPaused = false;
    }

    public void LoadMenu(){

        // Load the menu scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void QuitButton(){
        // shuts the aplication down
        Application.Quit();
    }
}