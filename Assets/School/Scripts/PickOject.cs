using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PickOject : MonoBehaviour
{
    public float attackRange = 5f; // Range within which the object will be destroyed
    public string targetTag = "object"; // Tag of the object to attack
    public float dropRadius = 3f; // Radius around the player where objects will be dropped

    private List<GameObject> pickedObjects = new List<GameObject>(); // List to store picked objects
    private Dictionary<string, string> evidenceMessages = new Dictionary<string, string>(); // Dictionary to store evidence messages

    private bool isSelecting = false; // Flag to track if the player is selecting an object



    private Dictionary<GameObject, Vector3> objectStartPositions = new Dictionary<GameObject, Vector3>();
    private int failCount = 0;
    private const int maxFails = 3;
    public List<Image> stars; // Assign three UI stars in Inspector

    private float lastFailTime = -10f; // Initialize to allow the first failure immediately
    private float failCooldown = 5f; // 10 seconds cooldown before the next evidence is destroyed

    // New dictionary to store associated arrows for each evidence
    private Dictionary<GameObject, GameObject> evidenceToArrowMap = new Dictionary<GameObject, GameObject>();


    // Evidence progress tracking
    private int totalEvidences = 1;
    private int collectedEvidences = 1;

    public Slider progressBar; // UI Slider for progress bar (assign in inspector)
    public GameObject finalArrow; // Reference to the final arrow object

    public GameObject failPanel; // Assign your Win UI panel in the Inspector
    public GameObject blurPanel; // The Full-Screen Blur Panel



    // Start is called before the first frame update
    void Start()
    {
       
        GameObject[] objects = GameObject.FindGameObjectsWithTag(targetTag);

        foreach (GameObject obj in objects)
        {
            objectStartPositions[obj] = obj.transform.position;

            // Assume each evidence has an arrow as a child (or you can manually assign this in the inspector)
            GameObject arrow = obj.transform.Find("Arrow")?.gameObject;
            if (arrow != null)
            {
                evidenceToArrowMap[obj] = arrow; // Store the reference to the arrow
                arrow.SetActive(false); // Initially hide the arrow
            }
        }
        //Your objective is to collect evidence scattered in the school and deliver it to the roof. Avoid NPCs and enemies.

        // Initialize messages for each evidence
        evidenceMessages.Add("Evidence1", "The roof is the final destination.");
        evidenceMessages.Add("Evidence2", "Check the cafeteria for a tape.");



        evidenceMessages.Add("Evidence3", "Check the classrooms for the next clue.");
        evidenceMessages.Add("Evidence4", "Key to the Teacher Room (2nd floor), placed inside a cabinet.");
        evidenceMessages.Add("Evidence5", "A piece of a map showing the roof location. But there are the other rooms left.");
        // C1: Hint written on a chalkboard: "The final piece lies near where knowledge flows."
        evidenceMessages.Add("Evidence6", "A torn note with a code for a locked drawer in the Teacher Room.");
        evidenceMessages.Add("Evidence7", "Puzzle piece taped to the back of a mirror.");
        evidenceMessages.Add("Evidence8", "A damaged recorder that requires fixing (tool found on the 2nd floor).");


        evidenceMessages.Add("Evidence9", "A key hidden under a chair, opening the Computer Lab.");
        evidenceMessages.Add("Evidence12", "A computer requires solving a simple code (e.g., binary numbers) to reveal the next location. Final map piece showing the exact path to the roof.");
        evidenceMessages.Add("Evidence10", "Requires code (from Classroom 2).");
        evidenceMessages.Add("Evidence11", "Crucial diary entry detailing hints for remaining evidence.");
        evidenceMessages.Add("Evidence13", "Recording of being beaten up by bullies.");
        evidenceMessages.Add("Evidence14", "A toolbox with tools to repair the damaged recorder from the store room.");
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player presses the "P" key
        // Check if the player presses the "P" key to pick up the closest object
        // Check if the player presses the "P" key to pick up the closest object
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameObject closestObject = FindClosestObject();
            if (closestObject != null)
            {
                // Add the object to the list and disable it in the scene
                pickedObjects.Add(closestObject);
                closestObject.SetActive(false);

                // Hide the associated arrow
                if (evidenceToArrowMap.ContainsKey(closestObject))
                {
                    evidenceToArrowMap[closestObject].SetActive(false); // Hide the arrow when evidence is picked
                }

                // Update the progress bar
                collectedEvidences++;
                
                progressBar.value = collectedEvidences;
                

                // Check if all evidences are collected
                if (collectedEvidences >= totalEvidences)
                {
                    if (finalArrow != null)
                    {
                        finalArrow.SetActive(true); // Show the final arrow when all evidences are collected
                    }
                }

                // Check if a message exists for the picked object
                if (evidenceMessages.TryGetValue(closestObject.name, out string message))
                {
                    Debug.Log(message);
                }
                else
                {
                    Debug.Log("Picked up: " + closestObject.name);
                }
            }
            else
            {
                Debug.Log("No object within range to pick up.");
            }
        }


        // Check if the player presses the "O" key to drop an object
        if (Input.GetKeyDown(KeyCode.O) && !isSelecting)
        {
            DropAllObjectsInRadius();
        }
    }

    GameObject FindClosestObject()
    {
        // Find all objects with the specified tag
        GameObject[] objects = GameObject.FindGameObjectsWithTag(targetTag);

        GameObject closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject obj in objects)
        {
            // Calculate the distance between the player and the object
            float distance = Vector3.Distance(transform.position, obj.transform.position);

            // Check if the object is within range and is closer than the previously found object
            if (distance < closestDistance && distance <= attackRange)
            {
                closest = obj;
                closestDistance = distance;
            }
        }

        return closest;
    }
    void DropAllObjectsInRadius()
    {
        if (pickedObjects.Count == 0)
        {
            Debug.Log("No objects to drop.");
            return;
        }

        foreach (GameObject obj in pickedObjects)
        {
            // Calculate a random position within the specified drop radius
            Vector3 dropPosition = transform.position + Random.insideUnitSphere * dropRadius;
            dropPosition.y = transform.position.y; // Keep it on the same vertical level as the player

            // Perform a raycast to adjust the position to the nearest surface
            RaycastHit hit;
            if (Physics.Raycast(dropPosition + Vector3.up, Vector3.down, out hit, Mathf.Infinity))
            {
                dropPosition = hit.point; // Adjust to the surface hit by the raycast
            }

            // Activate the object and move it to the calculated position
            obj.SetActive(true);
            obj.transform.position = dropPosition;

            Debug.Log("Dropped: " + obj.name + " at " + dropPosition);
        }

        // Clear the list after dropping all objects
        pickedObjects.Clear();
        Debug.Log("All objects dropped.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Check if cooldown has passed
            if (Time.time - lastFailTime < failCooldown)
            {
                Debug.Log("Enemy hit you, but cooldown is active. Evidence remains safe for now.");
                return;
            }

            // Update lastFailTime
            lastFailTime = Time.time;

            GameObject lastPickedObject = pickedObjects[pickedObjects.Count - 1];
            pickedObjects.RemoveAt(pickedObjects.Count - 1);
            lastPickedObject.transform.position = objectStartPositions[lastPickedObject];
            lastPickedObject.SetActive(true);
            //failCount++;

            collectedEvidences--;
            if (progressBar != null)
            {
                progressBar.value = collectedEvidences;
            }
            // Reduce star opacity or remove a star
            if (failCount < stars.Count)
            {
                Image star = stars[failCount]; // Get the next star to modify
                Color starColor = star.color;
                starColor.a = 0.2f; // Reduce opacity (or use star.gameObject.SetActive(false) to remove)
                star.color = starColor;

                failCount++;

                if (failCount >= maxFails)
                {
                    Debug.Log("Game Over! All stars are gone.");
                    blurPanel.SetActive(true); // Enable blur effect
                    failPanel.SetActive(true); // Show the Win panel
                                               // Implement game over logic here
                }
            }

            // Show the associated arrow again when the player collides with an enemy
            if (evidenceToArrowMap.ContainsKey(lastPickedObject))
            {
                evidenceToArrowMap[lastPickedObject].SetActive(true); // Show the arrow
            }

            //if (failCount >= maxFails)
            //{
            //    Debug.Log("Game Over! Enemy has destroyed three evidences.");
            //    // Implement game over logic here
            //}
            Debug.Log("Your latest evidence is destroyed. Find it again.");
        }
    }
}

