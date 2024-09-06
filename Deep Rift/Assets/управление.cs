using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;  // скорость персонажа
    public float jumpForce = 5f;  // сила прыжка
    public float mouseSensitivity = 100f;  // чувствительность мыши

    private Vector3 movement;  // вектор движения
    private Transform cameraTransform;  // трансформация камеры
    private float xRotation = 0f;  // для вращения по оси X (вверх-вниз)
    private Rigidbody rb;  // Rigidbody компонента
    private bool isGrounded;  // проверка, на земле ли персонаж

    void Start()
    {
        cameraTransform = Camera.main.transform;  // находим главную камеру
        Cursor.lockState = CursorLockMode.Locked;  // фиксируем курсор

        rb = GetComponent<Rigidbody>();  // получаем Rigidbody
        rb.freezeRotation = true;  // блокируем вращение, чтобы физика не накручивала персонажа
    }

    void Update()
    {
        // Управление камерой (мышь)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Ограничиваем вращение камеры по вертикали

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);  // Вращение камеры по оси X (вверх-вниз)
        
        // Поворот персонажа только по оси Y
        transform.Rotate(Vector3.up * mouseX);

        // Получаем ввод с клавиатуры
        float horizontal = Input.GetAxis("Horizontal");  // движение по оси X (A и D)
        float vertical = Input.GetAxis("Vertical");  // движение по оси Z (W и S)

        // Создаем вектор движения относительно направления персонажа
        movement = transform.right * horizontal + transform.forward * vertical;

        // Мгновенная остановка
        if (movement.magnitude > 1f) movement.Normalize();  // нормализуем движение

        // Прыжок, если персонаж на земле
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);  // Добавляем прыжок вверх
        }
    }

    void FixedUpdate()
    {
        // Двигаем персонажа мгновенно (без инерции)
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    // Метод для проверки, находится ли персонаж на земле
    void OnCollisionStay(Collision collision)
    {
        // Проверяем, что персонаж касается объектов с тегом "Ground" (пол)
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Когда персонаж отрывается от земли
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
