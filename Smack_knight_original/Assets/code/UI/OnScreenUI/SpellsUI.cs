using UnityEngine;

public class SpellsUI : MonoBehaviour
{
    // enables the spell icons based on the spell int (1 to 3)
    public void EnableSpell(int spell){
        if (spell -1 < 3){
            for (int i = 0; i < spell; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

// Showcasing the actice spell state on UI
    public void ActiveSpellState(int spell){
        
        // erasing missplaced sprites
        for (int i = 0; i < 3; i++)
            {
                transform.GetChild(i).transform.GetChild(0).gameObject.SetActive(false);
            }

        //activating  the right sprites
        if(spell - 1 < 3 && spell - 1 > -1){
            transform.GetChild(spell - 1).transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
