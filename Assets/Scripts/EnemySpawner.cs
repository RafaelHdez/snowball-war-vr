using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración de Enemigos")]
    // CAMBIO 1: Ahora es un Array [] (una lista), no un solo objeto
    public GameObject[] listaEnemigos; 

    [Header("Configuración del Área")]
    public float anchoSpawn = 10f;
    public float distanciaZ = 0f;

    [Header("Tiempos")]
    public float tiempoMinimo = 2.0f;
    public float tiempoMaximo = 4.0f;

    private bool spawneando = true;

    void Start()
    {
        StartCoroutine(CicloSpawn());
    }

    IEnumerator CicloSpawn()
    {
        yield return new WaitForSeconds(1.0f);

        while (spawneando)
        {
            if (GameManager.Instance != null && GameManager.Instance.isGameActive == false)
            {
                spawneando = false;
                yield break;
            }

            SpawnEnemigo();

            float tiempoEspera = Random.Range(tiempoMinimo, tiempoMaximo);
            yield return new WaitForSeconds(tiempoEspera);
        }
    }

    void SpawnEnemigo()
    {
        // CAMBIO 2: Verificar que hay enemigos en la lista para evitar errores
        if (listaEnemigos.Length == 0) return;

        // CAMBIO 3: Elegir un número al azar entre 0 y el total de enemigos en la lista
        int indiceAleatorio = Random.Range(0, listaEnemigos.Length);
        
        // Seleccionamos el prefab correspondiente a ese número
        GameObject enemigoElegido = listaEnemigos[indiceAleatorio];

        // Lógica de posición (igual que antes)
        float randomX = Random.Range(-anchoSpawn / 2, anchoSpawn / 2);
        
        Vector3 posicionSpawn = new Vector3(
            transform.position.x + randomX, 
            transform.position.y, 
            transform.position.z + distanciaZ
        );

        // Instanciamos el enemigo elegido
        Instantiate(enemigoElegido, posicionSpawn, Quaternion.identity);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Vector3 centro = new Vector3(transform.position.x, transform.position.y, transform.position.z + distanciaZ);
        Vector3 tamaño = new Vector3(anchoSpawn, 1, 1);
        Gizmos.DrawCube(centro, tamaño);
    }
}