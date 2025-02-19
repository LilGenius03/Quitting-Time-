using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections;

public class JakeAI_Script : MonoBehaviour
{
    [Header("Settings")]
    public string dialogue = "Hi you can go home now, were going to close the shop now";
    public Transform exitPoint;
    public Transform entryPoint;
    public float moveSpeed = 3f;
    public float detectionRange = 5f;
    public float WaitingTime = 10f;

    [Header("References")]
    public AudioClip dialogueSound; 
    public TextMeshProUGUI dialogueText; 

    private NavMeshAgent agent;
    private bool hasSpoken = false;
    private bool isLeaving = false;
    private bool hasEntered = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        transform.position = entryPoint.position;
        agent.SetDestination(exitPoint.position);
        if (dialogueText != null) dialogueText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!hasEntered && Vector3.Distance(transform.position, entryPoint.position) < 0.5f)
        {
            hasEntered = true;
            agent.SetDestination(exitPoint.position); 
        }

        if (!hasSpoken && PlayerInRange())
        {
            StartCoroutine(WaitBeforeLeaving());
            Debug.Log("Player in Range");          
        }

        if (isLeaving && agent.remainingDistance < 0.5f)
        {
            Despawn();
        }
    }

    bool PlayerInRange()
    {
        return Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) < detectionRange;
     
    }

    IEnumerator WaitBeforeLeaving()
    {
        TriggerDialogue();
        yield return new WaitForSeconds(WaitingTime);
        StartLeaving();
    }

        void TriggerDialogue()
    {
        hasSpoken = true;

        if (dialogueSound != null)
            AudioSource.PlayClipAtPoint(dialogueSound, transform.position);

        if (dialogueText != null)
        {
            dialogueText.text = dialogue;
            dialogueText.gameObject.SetActive(true);
            Invoke("HideText", 5f); 
        }

        Debug.Log(dialogue);
    }

    void HideText()
    {
        if (dialogueText != null)
            dialogueText.gameObject.SetActive(false);
    }

    void StartLeaving()
    {
        isLeaving = true;
        agent.SetDestination(exitPoint.position);
    }

    void Despawn()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
