///---------------------------------------------------------------------------|
/// Компонент для ноды.
/// В инспекторе можно вешать альтернативные наборы с задачами.
/// Компонент реализован, как синглетон.
/// Получить питон-задачу можно из любого места программы:
/// PythonTask task = CP2026.XlatPythonTasks.Instance.GetTask(0);
///---------------------------------------------------------------------------|
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

namespace CP2026
{
    ///-----------------------------------------------------------------------|
    /// Питон-задача.
    ///-----------------------------------------------------------------------|
    [System.Serializable] 
    public struct PythonTask
    {
        public string Name      { get; set; }
        public string Condition { get; set; }
        public string Decision  { get; set; }
        public string Answer    { get; set; }
        public int    Status    { get; set; }

        public void debug()
        {   Debug.Log($"Name     : {Name}");
            Debug.Log($"Condition: {Condition}");
            Debug.Log($"Decision : {Decision}");
            Debug.Log($"Answer   : {Answer}");
            Debug.Log($"Status   : {Status}");
        }
    }

    ///-----------------------------------------------------------------------|
    /// Интерфейс к базе данных с задачами.
    ///-----------------------------------------------------------------------|
    public interface IPythonTasks
    {
        public PythonTask Get(int idTask);
        public int GetAmount ();
    }

    ///-----------------------------------------------------------------------|
    /// Компонента для ноды - мост к юзеру.
    ///-----------------------------------------------------------------------|
    public class XlatPythonTasks : MonoBehaviour
    {
        [Header("=== Ассет с Питон-задачами ===")]
        [SerializeField] private ScriptableObject _pythonTasksAsset;
                         private IPythonTasks     _pythonTasks;

        private static XlatPythonTasks _instance;

        void Awake()
        {
            // Инициализируем синглтон
            if (_instance == null)
            {   _instance =  this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {   Destroy(gameObject);
            }

            // Получаем интерфейс из ассета
            _pythonTasks = _pythonTasksAsset as IPythonTasks;
        }

        void Start()
        {
            Debug.Assert(_pythonTasks != null);

            Test();
        }

        public PythonTask GetTask(int idTask)
        {   return _pythonTasks.Get(idTask);
        }

        private void Test()
        {   Debug.Log($"🔭");
            PythonTask task = XlatPythonTasks.Instance.GetTask(0);
                       task.debug();
        }

          // Статический метод для доступа к синглтону
        public static XlatPythonTasks Instance
        {
            get
            {
                if (_instance == null)
                {   _instance = FindAnyObjectByType<XlatPythonTasks>();
                    
                    if (_instance == null)
                    {
                        GameObject  go = new GameObject("XlatPythonTasks");
                        _instance = go.AddComponent<XlatPythonTasks>();
                    }
                }
                return _instance;
            }
        }
    }
}


