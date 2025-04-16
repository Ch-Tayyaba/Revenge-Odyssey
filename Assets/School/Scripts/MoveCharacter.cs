using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public class MoveCharacter : MonoBehaviour
{
    // Public Variables
    public float speed = 2.0f; // Speed of the enemy
    public float targetRadius = 3.0f; // Radius to check if the target is reached

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    // Private Variables
    private CharacterController _controller;
    private Animator animator;


    public NavMeshAgent agent; // Reference to the NavMeshAgent
    public Transform[] targets; // Array of target GameObjects to move between
    private int currentTargetIndex = 0; // To keep track of the current target

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        if (animator == null)
        {
            Debug.LogError("Animator component missing from the enemy!");
        }
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        

        // Move the character to the first target at the start
        MoveToNextTarget();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isWalking", true);
        // Check if the character has reached the current target
        if (!agent.pathPending && agent.remainingDistance <= targetRadius)
        {
            MoveToNextTarget();
        }

    }

    void MoveToNextTarget()
    {
        if (targets.Length == 0)
            return;

        // Set the current target
        Transform target = targets[currentTargetIndex];

        // Move the agent to the target position
        agent.SetDestination(target.position);

        // Update the target index to the next one
        currentTargetIndex = (currentTargetIndex + 1) % targets.Length; // Loop back to the first target when at the end
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
}
