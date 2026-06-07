using UnityEngine;

public class collisionSend : MonoBehaviour
{
    public SwordStateManager sword;

    void OnTriggerEnter2D(Collider2D other)
    {
//Searching for and hitting enmies affected by the laser
        if (other != null){
            sword.LaserSend(other);
        }
    }  
    
    //Searching for and hitting enmies affected by the laser
    void OnTriggernStay2D(Collider2D other)
    {
        if (other != null){
            sword.LaserSend(other);
        }
    }
}
