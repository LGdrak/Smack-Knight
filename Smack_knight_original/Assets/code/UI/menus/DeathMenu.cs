using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    public bool GameOver = false;

    public GameObject deathMenu;

    public RoundControler RC;

    public GameObject WinMenu;
    public void Death(){

// kills the player, activates the death menu and stops time
        deathMenu.SetActive(true);
        Time.timeScale = 0f;
        GameOver = true;
    }

    public void LoadMenuAfterDeath(){

        // Loads the menu scene
        Time.timeScale = 1f;
        GameOver = false;
        //cancels the load of a save
        RC.SaveRound(false);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

    }

    public void RetryLastRound(){

        // Loads the game scene
        Time.timeScale = 1f;
        GameOver = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

        public void Win(){
// activates the win menu and stops time
        WinMenu.SetActive(true);
        Time.timeScale = 0f;
        GameOver = true;
    }
}
