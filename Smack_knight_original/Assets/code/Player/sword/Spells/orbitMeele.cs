using UnityEngine;

public class orbitMeele : MonoBehaviour
{
    public float rotSpeed;
    Vector2 dir = Vector2.up;

    // Update is called once per frame
    void Update()
    {
        // rotates the orbital around the player
        transform.Rotate(0, 0, rotSpeed);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        dir = other.gameObject.transform.position - transform.position;
        dir = dir.normalized;
        try
        {
            other.transform.gameObject.GetComponent<enemy>().TakeDamage(10, dir, false);
        }
        catch (System.Exception)
        {
            
        }
    }
}
