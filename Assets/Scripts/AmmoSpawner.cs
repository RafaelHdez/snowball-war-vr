using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class AmmoSpawner : MonoBehaviour
{
    [Header("Config")]
    public GameObject snowballPrefab;
    public float respawnTime = 1.0f;

    [Tooltip("Distancia a la que debe alejarse la bola de nieve para que aparezca otra")]
    public float distanceToRespawn = 0.2f;

    private GameObject currentSnowball;
    private bool waitingForRespawn = false;

    void Start()
    {
        SnowballSpawn();
    }

    void Update()
    {
        if (currentSnowball == null && !waitingForRespawn)
        {
            StartCoroutine(SnowballRoutine());
        }
        else if (currentSnowball != null && !waitingForRespawn)
        {
            float distance = Vector3.Distance(transform.position, currentSnowball.transform.position);

            if (distance > distanceToRespawn)
            {
                currentSnowball = null;
                StartCoroutine(SnowballRoutine());
            }
        }
    }

    IEnumerator SnowballRoutine()
    {
        waitingForRespawn = true;
        yield return new WaitForSeconds(respawnTime);

        SnowballSpawn();

        waitingForRespawn = false;
    }

    void SnowballSpawn()
    {
        if (snowballPrefab != null)
        {
            currentSnowball = Instantiate(snowballPrefab, transform.position, Quaternion.identity);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
