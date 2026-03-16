using UnityEngine;
using System.Collections.Generic;

public class EnemyZoneManager : MonoBehaviour
{
    public GameObject enemyPrefab;    
    public Transform spawnPoint;      
    public float spawnInterval = 2f;  
    public int maxSpawnLimit = 10; // GIỚI HẠN 10 CON

    private List<SuicidalEnemy> activeEnemies = new List<SuicidalEnemy>();
    private bool isPlayerInside = false;
    private float timer;
    private int totalSpawnedCount = 0; // Biến đếm số lượng đã sinh

    void Update()
    {
        // Dọn dẹp những Enemy đã nổ khỏi danh sách
        activeEnemies.RemoveAll(enemy => enemy == null);

        // Chỉ sinh thêm nếu Player ở trong vùng VÀ chưa đủ 10 con
        if (isPlayerInside && totalSpawnedCount < maxSpawnLimit)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnEnemy();
                timer = 0;
            }
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnPoint == null) return;

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        SuicidalEnemy enemyScript = newEnemy.GetComponent<SuicidalEnemy>();
        
        if (enemyScript != null)
        {
            activeEnemies.Add(enemyScript);
            enemyScript.SetAttackMode(true);
            totalSpawnedCount++; // Tăng biến đếm sau khi sinh thành công
            Debug.Log($"Đã sinh: {totalSpawnedCount}/{maxSpawnLimit}");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            foreach (var enemy in activeEnemies)
            {
                if (enemy != null) enemy.SetAttackMode(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            foreach (var enemy in activeEnemies)
            {
                if (enemy != null) enemy.SetAttackMode(false);
            }
        }
    }
}