///---------------------------------------------------------------------------|
/// Ассет данных для компоненты.
/// Данные не создаются динамически, а уже существуют внути асета!
/// После правки набора задач - в инспекторе нажмите Reset,
/// чтобы обновить данные!
///---------------------------------------------------------------------------|
using CP2026;
using System.Collections.Generic;
using UnityEngine;

namespace CP2026
{
    ///-----------------------------------------------------------------------|
    /// Асет с набором Питон-задач.
    ///-----------------------------------------------------------------------|
    [CreateAssetMenu(fileName = "PythonTasksData_01",
                     menuName = "Xlat Scriptable/PythonTasksData_01")]
    public class PythonTasksData_01 : ScriptableObject, IPythonTasks
    {
        [Header("=== Набор с Питон-задачами ===")]
        [SerializeField] private List<PythonTask> data;
        
        // Этот метод вызывается при создании ассета через меню
        private void OnEnable()
        {
            // Инициализируем данные только если они пустые
            if (data == null || data.Count == 0)
            {
                InitializeDefaultData();
            }
        }
        
        // Этот метод вызывается при Reset в инспекторе
        private void Reset()
        {
            InitializeDefaultData();
        }
        
        private void InitializeDefaultData()
        {
            Debug.Log("🔴");

            data = new List<PythonTask>
            {
                new PythonTask
                {   
                    Name = "Задача малыша Гаусса",
                    Condition =
                "# Программа подсчета суммы натуральных чисел до 100\r\n", 
                    Decision = 
                "total = 0\r\nfor i in range(1, 101):\r\n" +
                "    total += i\r\n" +
                "print(\"Сумма чисел от 1 до 100 равна:\", total)",
                    Answer = "5050", 
                    Status = 0 
                },

                new PythonTask
                {   
                    Name = "Числа Фибоначчи",
                    Condition = 
                "# Программа выводит первые 10 чисел Фибоначчи\n" +
                "# Последовательность: 0, 1, 1, 2, 3, 5, 8, 13, 21, 34", 
                    Decision = 
                "a, b = 0, 1\n" +
                "for i in range(10):\n" +
                "    print(a, end=' ')\n" +
                "    a, b = b, a + b",
                    Answer = "0 1 1 2 3 5 8 13 21 34", 
                    Status = 0 
                },

                new PythonTask
                {   
                    Name = "Поиск простых чисел",
                    Condition = 
                "# Программа находит все простые числа от 2 до 50\n" +
                "# Простое число делится только на 1 и на себя", 
                    Decision = 
                "for num in range(2, 51):\n" +
                "    is_prime = True\n" +
                "    for i in range(2, int(num ** 0.5) + 1):\n" +
                "        if num % i == 0:\n" +
                "            is_prime = False\n" +
                "            break\n" +
                "    if is_prime:\n" +
                "        print(num, end=' ')",
                    Answer = "2 3 5 7 11 13 17 19 23 29 31 37 41 43 47", 
                    Status = 0 
                },

                new PythonTask
                {   
                    Name = "Факториал числа",
                    Condition = 
                "# Программа вычисляет факториал числа 10\n" +
                "# Факториал n! = 1 * 2 * 3 * ... * n", 
                    Decision = 
                "n = 10\n" +
                "factorial = 1\n" +
                "for i in range(1, n + 1):\n" +
                "    factorial *= i\n" +
                "print(f\"Факториал {n}! = {factorial}\")",
                    Answer = "3628800", 
                    Status = 0 
                },

                new PythonTask
                {   
                    Name = "Таблица умножения",
                    Condition = 
                "# Программа выводит таблицу умножения для числа 7\n" +
                "# От 1 до 10: 7 * 1 = 7, 7 * 2 = 14, ...", 
                    Decision = 
                "number = 7\n" +
                "for i in range(1, 11):\n" +
                "    print(f\"{number} * {i} = {number * i}\")",
                    Answer = "7 * 1 = 7\n7 * 2 = 14\n7 * 3 = 21\n7 * 4 = 28\n7 * 5 = 35\n7 * 6 = 42\n7 * 7 = 49\n7 * 8 = 56\n7 * 9 = 63\n7 * 10 = 70", 
                    Status = 0 
                }
            };
        }

        public PythonTask Get(int idTask)
        {
            if (data == null || data.Count == 0)
            {
                Debug.LogError("No data available!");
                return default;
            }
            idTask = idTask % data.Count;
            return data[idTask];
        }

        public int GetAmount()
        {   
            return data?.Count ?? 0;
        }
        
        // Опционально: метод для редактирования данных в редакторе
#if UNITY_EDITOR
        [ContextMenu("Reset to Default Data")]
        private void ResetToDefaultData()
        {
            InitializeDefaultData();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}