using UnityEngine;

public class Camera_script : MonoBehaviour
{

    private GameObject player;
    [SerializeField] float damping;
    Vector3 vel = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }
    // Fixed update is called on a fixed framerate
    void FixedUpdate(){
        // following the player
        Vector3 targetPosition = player.transform.position;
        targetPosition.z = transform.position.z;

    // making the movement of the camera to the player smooth
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref vel, damping);
    }
}
