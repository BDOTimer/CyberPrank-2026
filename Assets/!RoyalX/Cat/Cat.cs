using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Cat : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;
    private float wanderRadius = 10f;
    private float sampleRadius = 5f;
    private bool isWaiting = false;
    private bool showDebug = false;
    private Coroutine meowCoroutine;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MoveToRandomPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWaiting && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (showDebug)
                Debug.Log($"Кот достиг цели. Остановка и ожидание.");
            agent.velocity = Vector3.zero;
            animator.SetTrigger("sit");
            meowCoroutine = StartCoroutine(RandomMeow());
            StartCoroutine(WaitAndMove());
        }
    }

    private IEnumerator RandomMeow()
    {
        while (true)
        {
            // Случайная задержка между мяуканьями (от 3 до 10 секунд)
            float delay = Random.Range(3f, 10f);
            yield return new WaitForSeconds(delay);

            // Мяукаем с вероятностью 50%
            if (Random.value < 0.5f)
            {
                if (showDebug)
                    Debug.Log($"Кот мяукнул (задержка: {delay:F1} сек)");
                animator.SetTrigger("meow");
                audioSource.Play();
            }
            else if (showDebug)
            {
                Debug.Log($"Кот промолчал (задержка: {delay:F1} сек)");
            }
        }
    }

    private IEnumerator WaitAndMove()
    {
        isWaiting = true;
        agent.isStopped = true;
        float waitTime = Random.Range(10, 30);
        yield return new WaitForSeconds(waitTime);
        if (meowCoroutine != null)
            StopCoroutine(meowCoroutine);
        agent.isStopped = false;
        if (showDebug)
            Debug.Log($"Время ожидания истекло. Кот начинает движение.");
        MoveToRandomPosition();
        isWaiting = false;
    }

    private void MoveToRandomPosition()
    {
        Vector3 randomPoint = GetRandomNavMeshPoint();
        if (showDebug)
            Debug.Log($"Установка новой цели для NavMeshAgent. Кот начал движение.");
        agent.SetDestination(randomPoint);
        animator.SetTrigger("walk");
    }

    private Vector3 GetRandomNavMeshPoint()
    {
        // Делаем несколько попыток найти случайную точку на NavMesh в радиусе wanderRadius
        for (int i = 0; i < 30; i++)
        {
            // Генерируем случайную точку в радиусе wanderRadius вокруг текущей позиции
            Vector2 random2D = Random.insideUnitCircle * wanderRadius;
            Vector3 randomPoint = transform.position + new Vector3(random2D.x, 0, random2D.y);

            // Проверяем, находится ли эта точка на NavMesh
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
            {
                if (showDebug)
                    Debug.Log($"Найдена точка: {hit.position} за попыток: {i}");
                return hit.position;
            }
        }

        return transform.position;
    }
}
