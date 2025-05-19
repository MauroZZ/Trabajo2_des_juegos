using UnityEngine;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour
{
    public Transform objetivo;
    private NavMeshAgent agente;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (objetivo != null)
        {
            Vector3 objetivoEnPlano = new Vector3(objetivo.position.x, transform.position.y, objetivo.position.z);
            agente.SetDestination(objetivoEnPlano);
        }
    }
}