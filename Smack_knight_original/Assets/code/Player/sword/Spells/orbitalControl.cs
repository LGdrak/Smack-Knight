using UnityEngine;

public class orbitalControl : MonoBehaviour
{

    public float rotSpeed;

    // Update is called once per frame
    void Update()
    {
        // rotates the orbital around the player
        transform.Rotate(0, 0, rotSpeed);
    }
}
