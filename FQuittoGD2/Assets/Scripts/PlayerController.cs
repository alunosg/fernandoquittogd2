using UntyEngine;
using UnityEngine.InputSystem;
using UnityEngine;
using JetBrains.Annotations;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Rigidbody rig;
    public Transform cannon;

    public float speed = 10;
    public Vector2 rotationSpeed = new Vector2(18, 18);
    public float minRotationX = -75;
    public float minRotationx = 8f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
