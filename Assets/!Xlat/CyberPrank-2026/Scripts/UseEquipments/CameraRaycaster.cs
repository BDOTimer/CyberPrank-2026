using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CP2026
{
    public class CameraRaycaster : MonoBehaviour, IGoodbye
    {
        [SerializeField] private float      rayDistance  = 100f;
        [SerializeField] private LayerMask  targetLayers =   -1; // Все слои по умолчанию
        [SerializeField] private Transform _body;


        private Camera  mainCamera;
        private IControl _iControl; /// Интерфейс взаимодействия с оборудованием
    
        void Start()
        {
            mainCamera = GetComponent<Camera>();
            if (mainCamera == null)
                mainCamera = Camera.main;
        }
    
        void Update()
        {
            // Для тестирования при клике мышкой
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                RaycastHit hit;
                if (CastRayFromCamera(out hit))
                {
                    // Проверяем, есть ли у объекта триггер
                    if (HasTriggerCollider(hit.collider))
                    {
                        Debug.Log($"Обнаружен: {hit.collider.gameObject.name}");
                        OnTriggerObjectDetected(hit.collider.gameObject);
                    }
                }
            }

            ///--------------------|
            /// Фокус управления.  |
            ///--------------------:
            if( _iControl != null )
            {   _iControl.Control();

                if(Keyboard.current.digit1Key.wasPressedThisFrame)
                {
                    _iControl.Hello(null);
                    _iControl = null;
                }
            }
        }
    
        /// <summary>
        /// Основной метод рейкаста от камеры
        /// </summary>
        public bool CastRayFromCamera(out RaycastHit hit)
        {
            if (mainCamera == null)
            {
                hit = default;
                return false;
            }
        
            Ray ray = mainCamera.ScreenPointToRay(
                new Vector3(Screen.width / 2, Screen.height / 2, 0));

            return Physics.Raycast(
                ray, out hit, rayDistance, targetLayers, QueryTriggerInteraction.Collide);
        }
    
        /// Рейкаст от камеры по направлению курсора мыши
        public bool CastRayFromMousePosition(out RaycastHit hit)
        {
            if (mainCamera == null)
            {
                hit = default;
                return false;
            }
        
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(
                ray, out hit, rayDistance, targetLayers, QueryTriggerInteraction.Collide);
        }
    
        /// Проверка наличия триггерного коллайдера
        private bool HasTriggerCollider(Collider collider)
        {
            return collider.isTrigger;
        }
    
        /// Обработка обнаруженного объекта
        private void OnTriggerObjectDetected(GameObject detectedObject)
        {
            // Здесь ваша логика обработки
            // Например: взаимодействие, подсветка, сбор предметов и т.д.
            detectedObject.SendMessage(
                "OnCameraRaycastHit", SendMessageOptions.DontRequireReceiver);

            IControl iControl = detectedObject.GetComponent<IControl>();

            if( iControl != null && iControl != _iControl)
            {
                /// Debug.Log("=🟢🟢🟢=");

                if( _iControl != null )
                {   _iControl.Hello(null);
                }

                if( iControl.IsAccess(GetDistance(_body, detectedObject.transform)) )
                {   iControl.Hello(this);

                    Debug.Log($"Объект {iControl.Name()} взят под управление ...");

                    _iControl = iControl;
                }
                else
                {
                    /// Объект отказал в доступе...
                }
            }
        }

        public static float GetDistance(Transform a, Transform b)
        {
            if (a == null || b == null) return -1f;
            return Vector3.Distance(a.position, b.position);
        }
    
        /// Публичный метод для проверки конкретной позиции на экране
        public bool CastRayFromScreenPoint(Vector2 screenPoint, out RaycastHit hit)
        {
            if (mainCamera == null)
            {
                hit = default;
                return false;
            }
        
            Ray ray = mainCamera.ScreenPointToRay(screenPoint);
            return Physics.Raycast(
                ray, out hit, rayDistance, targetLayers, QueryTriggerInteraction.Collide);
        }

        public void Goodbye()
        {
            _iControl = null;

            Debug.Log($"Объект {_iControl.Name()} закончил контакт ...");
        }
    }
}
