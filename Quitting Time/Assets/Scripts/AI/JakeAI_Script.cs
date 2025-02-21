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
    public float WaitingTime = 7f;

    [Header("References")]
    public AudioClip dialogueSound; 
    public TextMeshProUGUI dialogueText; 

    private NavMeshAgent agent;
    private bool hasSpoken = false;
    private bool isLeaving = false;
    private Transform player; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        transform.position = entryPoint.position;
        agent.SetDestination(player.position);
        if (dialogueText != null) dialogueText.gameObject.SetActive(false);
    }

    void Update()
    {
        if(!hasSpoken && PlayerInRange())
        {
            StartCoroutine(DeliverDialogueAndLeave());
        }

        if (isLeaving && agent.remainingDistance < 0.5f)
        {
            Despawn();
        }
    }

    bool PlayerInRange()
    {
        return Vector3.Distance(transform.position, player.position) < detectionRange;
     
    }

    IEnumerator DeliverDialogueAndLeave()
    {
        hasSpoken = true;

        agent.isStopped = true;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = lookRotation;
        TriggerDialogue();
        yield return new WaitForSeconds(WaitingTime);
        agent.isStopped = false;
        agent.SetDestination(exitPoint.position);
        isLeaving = true;
    }

        void TriggerDialogue()
    {

        if (dialogueSound != null)
            AudioSource.PlayClipAtPoint(dialogueSound, transform.position);

        if (dialogueText != null)
        {
            dialogueText.text = dialogue;
            dialogueText.gameObject.SetActive(true);
            Invoke("HideText", WaitingTime); 
        }

        Debug.Log(dialogue);
    }

    void HideText()
    {
        if (dialogueText != null)
            dialogueText.gameObject.SetActive(false);
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
