using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowMove : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed at which the arrow moves up and down
    public float rotateSpeed = 50f; // Speed at which the arrow rotates

    private float initialYPosition; // To store the initial Y position of the arrow

    // Start is called before the first frame update
    void Start()
    {
        // Save the initial Y position of the arrow to calculate movement
        initialYPosition = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Move the arrow up and down
        float newYPosition = Mathf.Sin(Time.time * moveSpeed) + initialYPosition;
        transform.position = new Vector3(transform.position.x, newYPosition, transform.position.z);

        // Rotate the arrow around the Y-axis
        transform.Rotate(Vector3.left, rotateSpeed * Time.deltaTime);
    }
}
