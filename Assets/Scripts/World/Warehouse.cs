using System.Collections.Generic;
using UnityEngine;

public sealed class Warehouse : MonoBehaviour
{
    static readonly string[] Layout = {
        "#################",
        "#S....#........X#",
        "#.....#.........#",
        "###.###.#####.###",
        "#.......#.......#",
        "#.#####.#.#####.#",
        "#.....#...#.....#",
        "###.#.#####.###.#",
        "#F..#.....G.....#",
        "#################"
    };

    readonly Dictionary<Vector2Int, SpriteRenderer> tiles = new();
    readonly Dictionary<char, Vector2Int> markers = new();
    static readonly Vector2Int[] Directions = {
        Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down
    };
    Sprite square;
    public int Width => Layout[0].Length;
    public int Height => Layout.Length;

    public void Build()
    {
        square = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1),
            new Vector2(.5f, .5f), 1);
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                var cell = new Vector2Int(x, y);
                char tile = Layout[y][x];
                var visual = CreateVisual(tile == '#' ? "Wall" : "Floor", World(cell), .98f, 0);
                tiles.Add(cell, visual);
                if (tile == '#') visual.gameObject.AddComponent<BoxCollider2D>().size = Vector2.one / .98f;
                else if (tile != '.') markers.Add(tile, cell);
            }
    }

    public Vector2Int Marker(char symbol) => markers[symbol];
    public Vector2 World(Vector2Int cell) => new(cell.x, -cell.y);
    public Vector2Int Cell(Vector2 point) => new(Mathf.RoundToInt(point.x), Mathf.RoundToInt(-point.y));
    bool Open(Vector2Int cell) => cell.y >= 0 && cell.y < Height &&
        cell.x >= 0 && cell.x < Width && Layout[cell.y][cell.x] != '#';

    public Queue<Vector2Int> Route(Vector2Int start, Vector2Int destination)
    {
        var queue = new Queue<Vector2Int>();
        var parent = new Dictionary<Vector2Int, Vector2Int> { [start] = start };
        queue.Enqueue(start);
        while (queue.Count > 0 && !parent.ContainsKey(destination))
        {
            var current = queue.Dequeue();
            foreach (var direction in Directions)
            {
                var next = current + direction;
                if (!Open(next) || !parent.TryAdd(next, current)) continue;
                queue.Enqueue(next);
            }
        }
        var path = new Stack<Vector2Int>();
        if (!parent.ContainsKey(destination)) return new Queue<Vector2Int>();
        for (var cell = destination; cell != start; cell = parent[cell]) path.Push(cell);
        return new Queue<Vector2Int>(path);
    }

    public SpriteRenderer CreateVisual(string label, Vector2 position, float size, int order)
    {
        var obj = new GameObject(label);
        obj.transform.SetParent(transform);
        obj.transform.position = position;
        obj.transform.localScale = Vector3.one * size;
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = square;
        renderer.sortingOrder = order;
        return renderer;
    }

    public void Draw(EchoPulse echo)
    {
        foreach (var tile in tiles)
        {
            var color = Open(tile.Key) ? new Color(.16f, .25f, .32f) : new Color(.47f, .73f, .82f);
            tile.Value.color = Color.Lerp(new Color(.023f, .032f, .045f), color, echo.Visibility(World(tile.Key)));
        }
    }
}
