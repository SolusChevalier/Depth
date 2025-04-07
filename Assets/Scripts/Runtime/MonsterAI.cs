using UnityEngine;

public class MonsterAI : MonoBehaviour
{

    public Transform player;
    public float detection = 10f;
    public float moveSpeed = 0.5f;
    public float swimHeight = 2f;
    public float swimFrequency = 3f;

    private Animator animator;

    private AudioSource audioSource;
    private bool isChasing = false;


    public Transform modelTransform;//model rotation var

    // Roaming variables
    public float directionChangeInterval = 5f;
    private float directionChangeTimer;
    private Vector2 idleDirection;

    // Map boundary variables
    public Vector2 minBounds = new Vector2(-100f, -100f);
    public Vector2 maxBounds = new Vector2(100f, 100f);
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = modelTransform.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        PickNewIdleDirection();
        directionChangeTimer = directionChangeInterval;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 targetDirection;

        //Vector3 swimOffset = Vector3.up * Mathf.Sin(Time.time + swimFrequency) * swimHeight;

        if (distanceToPlayer < detection)
        {
            //chase
            targetDirection = (player.position - transform.position).normalized;

            //trigger audio 
            bool wasChasing = isChasing;
            isChasing = Vector2.Distance(transform.position, player.position) < detection;

            if (isChasing)
            {
                // Just started chasing
                audioSource.Play();
            }
        }

        else
        {
            //idle
            directionChangeTimer -= Time.deltaTime;

            if (directionChangeTimer <= 0f)
            {
                PickNewIdleDirection();
                directionChangeTimer = directionChangeInterval;
            }

            targetDirection = idleDirection;

        }

        // Apply vertical swim
        float sineOffset = Mathf.Sin(Time.time * swimFrequency) * swimHeight;
        Vector3 swimOffset = new Vector3(targetDirection.x, targetDirection.y + sineOffset, 0f);

        // Move
        transform.position += swimOffset * moveSpeed * Time.deltaTime;

        // Clamp to stay within the map
        Vector3 clampedPos = transform.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, minBounds.x, maxBounds.x);
        clampedPos.y = Mathf.Clamp(clampedPos.y, minBounds.y, maxBounds.y);
        transform.position = clampedPos;

        //flipping model toward movement direction
        if (targetDirection.x != 0)
        {
            Vector3 scale = modelTransform.localScale;
            scale.x = Mathf.Sign(targetDirection.x) * Mathf.Abs(scale.y);
            modelTransform.localScale = scale;
        }

        if (animator && !animator.GetCurrentAnimatorStateInfo(0).IsName("swim"))
        {
            animator.Play("swim");
        }

    }

    void PickNewIdleDirection()
    {
        // Pick a new random normalized direction
        idleDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}
