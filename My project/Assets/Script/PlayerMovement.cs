using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float growthFactor = 0.01f;

    private Rigidbody rb;
    private Vector3 movement;
    private Vector3 previousPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        previousPosition = transform.position;
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        movement = new Vector3(moveHorizontal * speed, 0f, moveVertical * speed);

        rb.AddForce(movement);

        Growing();

        previousPosition = transform.position;
    }

    void Growing()
    {
        float distanceMoved = Vector3.Distance(previousPosition, transform.position);

        if (distanceMoved > 0)
        {
            Vector3 newScale = transform.localScale + Vector3.one * (distanceMoved * growthFactor);
            transform.localScale = newScale;
        }
    }
}
