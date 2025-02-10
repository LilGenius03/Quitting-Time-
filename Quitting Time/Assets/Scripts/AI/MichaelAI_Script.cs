using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class MichaelAI_Script : MonoBehaviour
{
    public Transform PlayerTarget;
    public float AttackDistance;
    private NavMeshAgent m_Agent;
    private float m_Distance;
    public PlayerHealthValue health;
    public FP_Movement playerMovement;
    private bool hasDamaged;
    public float damageCooldown = 1f;
    private float damageTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        health.damageValue = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        m_Distance = Vector3.Distance(m_Agent.transform.position, PlayerTarget.position);

        if(m_Distance < AttackDistance)
        {
            m_Agent.isStopped = true;
            
            if(!hasDamaged && m_Distance < AttackDistance )
            {
                health.healthValue -= health.damageValue;
                hasDamaged = true;
                damageTimer = damageCooldown;
                playerMovement.ResetHealingTimer();
            }

            if(health.healthValue <= 0f)
            {
                m_Agent.isStopped = true;
            }
          
        }
        else
        {
            m_Agent.isStopped = false;
            m_Agent.destination = PlayerTarget.position;
            hasDamaged = false;
        } 
        
        if(hasDamaged)
        {
            damageTimer -= Time.deltaTime;
            if(damageTimer <= 0f)
            {
                hasDamaged = false;
            }
        }
    }

}
