using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;  // скорость персонажа на суше
    public float swimSpeed = 3f;  // скорость персонажа в воде
    public float swimSpeedBoost = 6f;  // скорость при ускорении (Shift) в воде
    public float jumpForce = 5f;  // сила прыжка на суше
    public float swimUpForce = 2f;  // скорость подъема в воде
    public float mouseSensitivity = 100f;  // чувствительность мыши
    public float fallInWaterTime = 0.5f;  // время погружения перед отключением гравитации
    public float exitWaterJumpMultiplier = 1.5f;  // коэффициент прыжка при выныривании

    private Vector3 movement;  // вектор движения
    private Transform cameraTransform;  // трансформация камеры
    private float xRotation = 0f;  // для вращения по оси X (вверх-вниз)
    private Rigidbody rb;  // Rigidbody компонента
    private bool isGrounded;  // проверка, на земле ли персонаж
    private bool isInWater;  // проверка, в воде ли персонаж
    private float waterEntryTime;  // время входа в воду
    private bool gravityDisabled;  // флаг, отключена ли гравитация после погружения

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

        if (isInWater)
        {
            // Движение в воде в направлении взгляда камеры
            Vector3 swimDirection = cameraTransform.forward * vertical + cameraTransform.right * horizontal;

            // Плавание вверх и вниз
            if (Input.GetKey(KeyCode.Space))
            {
                swimDirection += Vector3.up * swimUpForce;  // Плывем вверх
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                swimDirection += Vector3.down * swimUpForce;  // Плывем вниз
            }

            // Ускорение на Shift
            float currentSwimSpeed = Input.GetKey(KeyCode.LeftShift) ? swimSpeedBoost : swimSpeed;

            // Обновляем движение
            movement = swimDirection.normalized * currentSwimSpeed;

            // Отключаем гравитацию с задержкой
            if (!gravityDisabled && Time.time - waterEntryTime >= fallInWaterTime)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);  // Обнуляем скорость падения после задержки
                rb.useGravity = false;  // Отключаем гравитацию
                gravityDisabled = true;
            }
        }
        else
        {
            // Прыжок на суше, если персонаж на земле
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);  // Добавляем прыжок вверх
            }
        }
    }

    void FixedUpdate()
    {
        // Если персонаж в воде, двигаемся в зависимости от направления камеры
        if (isInWater)
        {
            rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
        }
        else
        {
            // Если на суше, двигаем персонажа мгновенно (без инерции)
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
        }
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

    // Вход в воду
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = true;
            waterEntryTime = Time.time;  // Запоминаем время входа в воду
            rb.useGravity = true;  // Пока оставляем гравитацию, чтобы игрок немного упал в воду
            gravityDisabled = false;
        }
    }

    // Выход из воды
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = false;
            rb.useGravity = true;  // Включаем гравитацию при выходе из воды

            // Подбрасываем игрока при выходе из воды
            if (rb.velocity.y > 0)
            {
                float exitForce = rb.velocity.y * exitWaterJumpMultiplier;  // Вычисляем силу подбрасывания
                rb.AddForce(Vector3.up * exitForce, ForceMode.Impulse);  // Добавляем импульс вверх
            }
        }
    }
}
