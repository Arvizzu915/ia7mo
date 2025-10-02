using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DishonorAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public bool lineOfSight = false;
    public bool perifericLineOfSight = false;
    [SerializeField] private Transform playerTrans;
    public Vector3 lastSeen;

    public float searchingTime = 5f;

    public Transform[] patrolSpots;
    private int patrolIndex = 0;

    private void Start()
    {
        agent.SetDestination(patrolSpots[patrolIndex].position);
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position + (Vector3.up * 0.5f), transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            lineOfSight = hit.collider.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement player);
            agent.SetDestination(playerTrans.position);
        }

        if (lineOfSight)
        {
            transform.LookAt(playerTrans);
        }

        if (perifericLineOfSight)
        {
            lastSeen = playerTrans.position;
            agent.SetDestination(lastSeen);
        }

        Vector3 baseDirection = transform.forward;
        Quaternion rotationOffset = Quaternion.AngleAxis(35f, transform.up);
        Quaternion rotationOffset2 = Quaternion.AngleAxis(-35f, transform.up);

        Debug.DrawRay(transform.position + (Vector3.up * 0.5f), rotationOffset * baseDirection);
        Debug.DrawRay(transform.position + (Vector3.up * 0.5f), rotationOffset2 * baseDirection);


        Ray pray1 = new Ray(transform.position + (Vector3.up * 0.5f), rotationOffset * baseDirection);
        if (Physics.Raycast(pray1, out RaycastHit hit2))
        {
            
            perifericLineOfSight = hit2.collider.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement player);
            StartCoroutine(Search());
        }


        Ray pray2 = new Ray(transform.position + (Vector3.up * 0.5f), rotationOffset * baseDirection);
        if (Physics.Raycast(pray1, out RaycastHit hit3))
        {
            perifericLineOfSight = hit3.collider.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement player);
            StartCoroutine(Search());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");

        if (other.CompareTag("Patrol"))
        {
            Debug.Log("change");
            patrolIndex = (patrolIndex + 1) % patrolSpots.Length;
            agent.SetDestination(patrolSpots[patrolIndex].position);
        }
    }

    private IEnumerator Search()
    {
        yield return new WaitForSeconds(searchingTime);
        agent.SetDestination(patrolSpots[patrolIndex].position);
    }
}
