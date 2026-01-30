using UnityEngine;

public class ShootingController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private string playerNum = "1"; // Nhập "1" hoặc "2" để phân biệt người chơi
    private string fireButton;

    [Header("Shooting Settings")]
    public Transform firePoint;    
    public GameObject bulletPrefab; 
    public float fireRate = 0.5f;   

    private float nextFireTime = 0f;
    private SpriteRenderer sprite;  

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        // Tự động tạo tên nút bấm: Fire1 hoặc Fire2
        // Bạn cần cấu hình nút này trong Project Settings > Input Manager
        fireButton = "Fire" + playerNum;
    }

    void Update()
    {
        // Kiểm tra nút bắn của từng người chơi
        if (Input.GetButtonDown(fireButton))
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void Shoot()
    {
        if (firePoint == null) return;

        // Xác định hướng đạn dựa trên việc lật ảnh (flipX) của nhân vật
        Quaternion bulletRotation;
        
        if (sprite.flipX)
        {
             // Bắn sang trái (xoay 180 độ quanh trục Y)
             bulletRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
             // Bắn sang phải
             bulletRotation = Quaternion.identity; 
        }

        Instantiate(bulletPrefab, firePoint.position, bulletRotation);

        // Phát âm thanh bắn (nếu có AudioManager)
        if (AudioManager.instance != null)
        {
            // AudioManager.instance.PlaySFX(AudioManager.instance.shootSound);
        }
    }
}