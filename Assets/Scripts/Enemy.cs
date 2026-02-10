using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2.5f;
    private Transform player;

    void Start()
    {
        if (Camera.main != null)
        {
            player = Camera.main.transform;
        }
    }

    void Update()
    {
        if (player != null)
        {
            Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(targetPosition);

            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Snowball"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddPoints(5);
            }

            Destroy(gameObject);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Base"))
        {
            Destroy(gameObject);
        }
    }
}