using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PenguinSpawner : MonoBehaviour
{
    public GameObject penguinPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 5f;
    public int maxPenguins = 10;

    private int spawnedCount = 0;

    void Start()
    {
        StartCoroutine(SpawningLoop());
    }

    IEnumerator SpawningLoop()
    {
        while (spawnedCount < maxPenguins)
        {
            SpawnPenguin();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnPenguin()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject newPenguin = Instantiate(penguinPrefab, spawnPoint.position, spawnPoint.rotation);
        spawnedCount++;

        Transform hatsGroup = newPenguin.transform.Find("npc_visual/Hats");
        if (hatsGroup != null && hatsGroup.childCount > 0)
        {
            // Desactiva los hats por si acaso
            for (int i = 0; i < hatsGroup.childCount; i++)
                hatsGroup.GetChild(i).gameObject.SetActive(false);

            // Activa uno de los Hats al azar
            int index = Random.Range(0, hatsGroup.childCount);
            hatsGroup.GetChild(index).gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No se encontró el grupo de sombreros en: " + newPenguin.name);
        }
    }
}