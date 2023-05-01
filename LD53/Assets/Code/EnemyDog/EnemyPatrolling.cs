using UnityEngine;

public class EnemyPatrolling : MonoBehaviour
{
    public bool isActive;
    public float speed = 2f;
    public Transform[] waypoints;
    public bool isMovingLeft = false;

    private int currentWaypointIndex = 0;

    void Update()
    {
        if (!isActive)
        {
            return;
        }
        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
        {
            if (isMovingLeft)
            {
                currentWaypointIndex++;

                if (currentWaypointIndex >= waypoints.Length)
                {
                    currentWaypointIndex = waypoints.Length - 2;
                    isMovingLeft = false;
                }
            }
            else
            {
                currentWaypointIndex--;

                if (currentWaypointIndex < 0)
                {
                    currentWaypointIndex = 1;
                    isMovingLeft = true;
                }
            }
        }

        if (waypoints[currentWaypointIndex].position.x < transform.position.x)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
}




