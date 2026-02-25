using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Settings")]
    public Transform posA, posB; // Hai điểm Checkpoint
    public float speed = 2f;
    
    private Vector3 targetPos;

    void Start()
    {
        targetPos = posB.position;
    }

    void Update()
    {
        // Di chuyển Platform
        if (Vector3.Distance(transform.position, posA.position) < 0.1f) targetPos = posB.position;
        if (Vector3.Distance(transform.position, posB.position) < 0.1f) targetPos = posA.position;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    // Xử lý kéo theo Player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra nếu vật chạm vào là Player
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(this.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}