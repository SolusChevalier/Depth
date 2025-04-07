using UnityEngine;

public class MonsterAI : MonoBehaviour
{

    public Transform player;
    public float detection = 7f;
    public float moveSpeed = 10f;
    public float swimHeight = 1f;
    public float swimFrequency = 5f;

    private Vector3 startPos;
    private Vector3 direction;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        //Vector3 swimOffset = Vector3.up * Mathf.Sin(Time.time + swimFrequency) * swimHeight;

        if (distanceToPlayer < detection)
        {
            //chase
            direction = ( player.position - transform.position);
            direction.z = 0; // Stay on 2D plane
            direction.Normalize();
        }

        else
        {
            //idle
            direction = Vector3.left;

        }

        float verticalWave = Mathf.Sin(Time.time * swimFrequency) * swimHeight;
        Vector3 swimOffset = new Vector3(direction.x, direction.y + verticalWave, 0);

        transform.position += swimOffset * moveSpeed * Time.deltaTime;
    }
}
