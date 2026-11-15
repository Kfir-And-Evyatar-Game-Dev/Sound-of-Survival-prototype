using System.Collections.Generic;
using UnityEngine;

public sealed class EchoHunter : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] float searchSeconds = 3f;

    Warehouse map;
    EchoPulse echo;
    Rigidbody2D body;
    Queue<Vector2Int> route = new();
    Vector2Int patrolStart, patrolEnd;
    bool headingToEnd = true;
    bool investigating;
    float searchUntil;

    public void Initialize(Warehouse warehouse, EchoPulse pulse)
    {
        map = warehouse;
        echo = pulse;
        patrolStart = map.Marker('G');
        patrolEnd = patrolStart + Vector2Int.right * 4;
        body = gameObject.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        gameObject.AddComponent<CircleCollider2D>();
        echo.Heard += Hear;
    }

    void OnDestroy()
    {
        if (echo != null) echo.Heard -= Hear;
    }

    void Hear(Vector2 source, float range)
    {
        if (Vector2.Distance(body.position, source) > range) return;
        route = map.Route(map.Cell(body.position), map.Cell(source));
        investigating = true;
        searchUntil = 0;
    }

    void FixedUpdate()
    {
        if (route.Count == 0)
        {
            if (investigating)
            {
                if (searchUntil == 0) searchUntil = Time.time + searchSeconds;
                if (Time.time < searchUntil) return;
                investigating = false;
            }
            route = map.Route(map.Cell(body.position), headingToEnd ? patrolEnd : patrolStart);
            headingToEnd = !headingToEnd;
            if (route.Count == 0) return;
        }

        Vector2 target = map.World(route.Peek());
        Vector2 next = Vector2.MoveTowards(body.position, target, speed * Time.fixedDeltaTime);
        body.MovePosition(next);
        if (next == target) route.Dequeue();
    }
}
