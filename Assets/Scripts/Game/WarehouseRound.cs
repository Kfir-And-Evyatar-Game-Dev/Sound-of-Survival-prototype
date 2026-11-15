using System;
using UnityEngine;

public sealed class WarehouseRound : MonoBehaviour
{
    [SerializeField] float interactionDistance = .85f;
    Warehouse map;
    EchoPulse echo;
    Survivor player;
    EchoHunter hunter;
    SpriteRenderer fuse, exit, enemy;
    bool hasFuse, ended;
    public event Action<string> StatusChanged;
    public event Action<bool> Finished;
    public float Cooldown => echo.RemainingCooldown;

    void Awake()
    {
        map = gameObject.AddComponent<Warehouse>();
        map.Build();
        echo = gameObject.AddComponent<EchoPulse>();

        var playerVisual = Spawn('S', "Survivor", .52f);
        playerVisual.color = new Color(.35f, .95f, 1f);
        player = playerVisual.gameObject.AddComponent<Survivor>();
        player.Initialize(echo);
        enemy = Spawn('G', "Hunter", .62f);
        hunter = enemy.gameObject.AddComponent<EchoHunter>();
        hunter.Initialize(map, echo);
        fuse = Spawn('F', "Fuse", .46f);
        exit = Spawn('X', "Exit", .70f);

        var camera = new GameObject("Warehouse camera").AddComponent<Camera>();
        camera.orthographic = true;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(.012f, .018f, .025f);
        camera.transform.position = new Vector3((map.Width - 1) / 2f, -(map.Height - 1) / 2f, -10f);
        gameObject.AddComponent<WarehouseHud>().Initialize(this, camera, map.Width, map.Height);
        LateUpdate();
    }

    void OnEnable()
    {
        player.Interact += Interact;
        player.Caught += Lose;
    }

    void OnDisable()
    {
        player.Interact -= Interact;
        player.Caught -= Lose;
    }

    SpriteRenderer Spawn(char symbol, string label, float size) =>
        map.CreateVisual(label, map.World(map.Marker(symbol)), size, 1);

    void LateUpdate()
    {
        map.Draw(echo);
        Reveal(enemy, new Color(1, .28f, .37f));
        Reveal(fuse, new Color(1, .76f, .24f));
        Reveal(exit, hasFuse ? new Color(.32f, 1f, .65f) : new Color(.77f, .65f, 1f));
    }

    void Reveal(SpriteRenderer visual, Color color)
    {
        color.a = echo.Visibility(visual.transform.position);
        visual.color = color;
    }

    void Interact()
    {
        if (ended) return;
        if (!hasFuse && Near(fuse))
        {
            hasFuse = true;
            fuse.gameObject.SetActive(false);
            StatusChanged?.Invoke("Fuse acquired. Find the emergency exit.");
        }
        else if (Near(exit))
        {
            if (hasFuse) End(true);
            else StatusChanged?.Invoke("Exit locked. Find the fuse first.");
        }
    }

    bool Near(SpriteRenderer item) =>
        Vector2.Distance(player.transform.position, item.transform.position) <= interactionDistance;

    void Lose() => End(false);

    void End(bool won)
    {
        if (ended) return;
        ended = true;
        player.enabled = false;
        hunter.enabled = false;
        Finished?.Invoke(won);
    }
}

