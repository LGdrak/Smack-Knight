using UnityEngine;
using UnityEngine.UI;

public class UIHeart : MonoBehaviour
{

    [SerializeField] Sprite fullHeart;
    [SerializeField] Sprite halfHeart;
    [SerializeField] Sprite heartContainer;
    int count;

    public int currentHealth = 6;
    public int maxHealth = 6;

 // the player has 6 health (3 hearts) on the start of the game

// is called to update the damage the player recieved in the UI
    public void Damaged(int _currentHealth){
// erasing any missplaced sprites
        for (int i = 0; i < 20; i++)
        {
            transform.GetChild(i).gameObject.GetComponent<Image>().sprite = heartContainer;
        }

        count = 0;
// activating the right amount of sprites to show. if statements decide on a full or a half heart
        for (int i = 0; i < _currentHealth; i++)
        {
            if(i % 2 == 0){
                transform.GetChild(count).gameObject.GetComponent<Image>().sprite = halfHeart;
            }
            else{
                transform.GetChild(count).gameObject.GetComponent<Image>().sprite = fullHeart;
            }

            if(i % 2 == 1){count += 1;} 
        }
// updating local variables
        currentHealth = _currentHealth;

    }

    public void HealthUp(int heartContainers, int heal){

//function for increasing maximal healt
        maxHealth += heartContainers;
        if (maxHealth <= 0){currentHealth = 0;}
        if(maxHealth > 40){maxHealth = 40;}

        for (int i = 0; i < 20; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < maxHealth / 2; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }

        Heal(heal);
    }

// a function for healing the player and dislay in UI
    public void Heal(int _heal){

// erasing missplaced sprites
        for (int i = 0; i < 20; i++)
        {
            transform.GetChild(i).gameObject.GetComponent<Image>().sprite = heartContainer;
        }

        count = 0;
        currentHealth += _heal;
        
        if(currentHealth > maxHealth){currentHealth = maxHealth;}//making sure the player doesent heal more than he can

// activating UI hearts
        for (int i = 0; i < currentHealth; i++)
        {
            if(i % 2 == 0){
                transform.GetChild(count).gameObject.GetComponent<Image>().sprite = halfHeart;
            }
            else{
                transform.GetChild(count).gameObject.GetComponent<Image>().sprite = fullHeart;
            }

            if(i % 2 == 1){count += 1;} 
        }
    }
}
