using UnityEngine;

public class CreepyDoorSound : MonoBehaviour
{
    public AudioClip[] creakSounds;
    public float creakThreshold = 0.8f;
    public float minPitch = 0.8f, maxPitch = 1.2f;
    private AudioSource audioSource;
    private Rigidbody rb;
    private HingeJoint joint;
    private bool isMoving;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        joint = GetComponent<HingeJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.angularVelocity.magnitude > creakThreshold && !isMoving)
        {
            PlayCreakSound ();
        }
    }

    void PlayCreakSound()
    {
        audioSource.clip = creakSounds[Random.Range(0, creakSounds.Length)];
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();
        isMoving = true;
        Invoke(nameof(ResetMovement), 1f);
    }

    void ResetMovement() => isMoving = false;
    
    bool IsPlayerNear()
    {
        RaycastHit hit;
        float height = 10f;
        float hitDistance = 2f;
        Vector3 dis = transform.position + transform.forward;

        if(Physics.SphereCast(dis, height / 2, transform.forward, out hit, 10))
        {
            hitDistance = hit.distance;
            PlayCreakSound ();
        }
        
        return true;
    }
    
}
