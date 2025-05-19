using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab del enemigo a instanciar
    public int numberOfEnemies = 5;
    public Transform[] spawnPoints; // Puntos de spawn predeterminados (se pueden asignar en inspector)

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No se han asignado puntos de spawn en EnemySpawner.");
            return;
        }

        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Elegir punto de spawn (puede ser cíclico o aleatorio)
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];

            // Instanciar enemigo en ese punto y con rotación por defecto
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            // Opcional: nombrar el enemigo para identificarlo
            enemy.name = "Enemy_" + i;
        }
    }
}

