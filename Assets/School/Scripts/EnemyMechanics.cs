using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;



public class EnemyMechanics : MonoBehaviour
{
    // Start is called before the first frame update

    /// <summary>
    /// Add all the following mechanics for Enemy
    /// 1. Movement
    /// 2. Fight
    /// 3. Push
    /// 4. Throw object
    /// 5. Pick/Drop Object
    /// 6. Open/Close drawer/door
    /// 
    /// First add all the seperate functions for them....
    /// 
    /// </summary>


    // Public Variables
    public float speed = 2.0f; // Speed of the enemy


    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    // Private Variables
    private CharacterController _controller;
    private Animator animator;
    private Vector3 movementDirection;
    private float changeDirectionInterval = 5.0f; // Time interval to change direction
    private float timeSinceLastChange = 0.0f;









    /// <summary>
    /// Fighting
    /// </summary>
    /// 
    private int punchCombo = 0;
    private int kickCombo = 0;
    private float comboTimer = 0f;
    public float comboResetTime = 1f; // Time window to continue combo

    void Start()
    {
        

        animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        if (animator == null)
        {
            Debug.LogError("Animator component missing from the enemy!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Walk();

        //Fight();
        //ResetComboTimer();

    }


    /// <summary>
    /// Handles walking in all directions with animation
    /// </summary>
    private void Walk()
    {
        timeSinceLastChange += Time.deltaTime;

        // Change direction at regular intervals
        if (timeSinceLastChange >= changeDirectionInterval)
        {
            timeSinceLastChange = 0.0f;
            ChooseRandomDirection();
        }

        // Apply movement and rotation
        if (movementDirection != Vector3.zero)
        {
            Vector3 movement = movementDirection * speed * Time.deltaTime;
            _controller.Move(movement); // Use CharacterController for movement
            transform.rotation = Quaternion.LookRotation(movementDirection);

            // Trigger walking animation
            animator.SetBool("isWalking", true);

        }
        else
        {
            // Ensure idle animation plays if no other animation is active
            animator.SetBool("isWalking", false);
            animator.Play("Idle", 0); // Play idle animation explicitly
        }
    }


    /// <summary>
    /// Chooses a random direction for the enemy to move
    /// </summary>
    private void ChooseRandomDirection()
    {
        int randomDirection = Random.Range(0, 4); // Generate a random number between 0 and 3

        switch (randomDirection)
        {
            case 0:
                movementDirection = Vector3.forward; // Forward
                break;
            case 1:
                movementDirection = Vector3.back; // Backward
                break;
            case 2:
                movementDirection = Vector3.left; // Left
                break;
            case 3:
                movementDirection = Vector3.right; // Right
                break;
        }
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }
    }


    IEnumerator WaitForSecondsCoroutine(float time)
    {
        // Wait for 30 seconds
        yield return new WaitForSeconds(time);

        // Actions to perform after 30 seconds
        Debug.Log(time);
    }

    /// <summary>
    /// Placeholder for fight mechanic
    /// </summary>
    void Fight()
    {
        
    }

    void HandlePunchCombo()
    {
        punchCombo++;

        if (punchCombo > 3)
        {
            punchCombo = 1; // Loop back to the first punch
        }

        comboTimer = comboResetTime; // Reset combo timer
        animator.SetInteger("punchCombo", punchCombo);
        animator.SetTrigger("Punch");
    }

    void HandleKickCombo()
    {
        kickCombo++;

        if (kickCombo > 3)
        {
            kickCombo = 1; // Loop back to the first kick
        }

        comboTimer = comboResetTime; // Reset combo timer
        animator.SetInteger("kickCombo", kickCombo);
        animator.SetTrigger("kick");
    }

    void ResetComboTimer()
    {
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;

            if (comboTimer <= 0)
            {
                punchCombo = 0;
                kickCombo = 0;
                animator.SetInteger("punchCombo", 0);
                animator.SetInteger("kickCombo", 0);
                animator.SetBool("isFighting", true);
            }
        }
    }

    void Dodge()
    {
        Vector3 direction = Vector3.zero;

        //StartCoroutine(WaitForSecondsCoroutine(10f));
        //direction = Vector3.forward;
        //StartCoroutine(WaitForSecondsCoroutine(10f));
        //direction = Vector3.back;

        if (direction != Vector3.zero)
        {
            animator.SetBool("isDodging", true);
            animator.SetFloat("dodgeDirection", direction.z);
        }
        else
        {
            animator.SetBool("isDodging", false);

        }
    }

    /// <summary>
    /// Placeholder for push mechanic
    /// </summary>
    private void Push()
    {
        Debug.Log("Enemy is pushing an object!");
        // Add push logic here
    }

    /// <summary>
    /// Placeholder for throw object mechanic
    /// </summary>
    private void ThrowObject()
    {
        Debug.Log("Enemy is throwing an object!");
        // Add throw logic here
    }

    /// <summary>
    /// Placeholder for pick/drop object mechanic
    /// </summary>
    private void PickDropObject()
    {
        Debug.Log("Enemy is picking or dropping an object!");
        // Add pick/drop logic here
    }

    /// <summary>
    /// Placeholder for open/close drawer or door mechanic
    /// </summary>
    private void OpenCloseDrawerOrDoor()
    {
        Debug.Log("Enemy is opening or closing a drawer/door!");
        // Add open/close logic here
    }
}
