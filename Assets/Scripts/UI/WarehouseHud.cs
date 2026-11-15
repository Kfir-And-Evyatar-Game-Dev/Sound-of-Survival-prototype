using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class WarehouseHud : MonoBehaviour
{
    WarehouseRound round;
    Camera view;
    int mapWidth, mapHeight;
    string status = "Find the fuse, then unlock the emergency exit.";
    bool? won;

    public void Initialize(WarehouseRound warehouseRound, Camera levelCamera, int width, int height)
    {
        round = warehouseRound;
        view = levelCamera;
        mapWidth = width;
        mapHeight = height;
        round.StatusChanged += ShowStatus;
        round.Finished += ShowResult;
    }

    void OnDestroy()
    {
        if (round == null) return;
        round.StatusChanged -= ShowStatus;
        round.Finished -= ShowResult;
    }

    void ShowStatus(string message) => status = message;
    void ShowResult(bool success) => won = success;

    void Update() => view.orthographicSize = Mathf.Max((mapHeight + 3f) / 2f,
        (mapWidth + 1f) / (2f * view.aspect));

    void OnGUI()
    {
        GUI.matrix = Matrix4x4.Scale(new Vector3(Screen.width / 960f, Screen.height / 600f, 1));
        GUI.Box(new Rect(12, 10, 936, 75), "");
        GUI.Label(new Rect(26, 16, 900, 22), "SOUND OF SURVIVAL   /   THE DARK WAREHOUSE");
        GUI.Label(new Rect(26, 37, 900, 22), status);
        string echo = round.Cooldown <= 0 ? "Echo ready" : $"Echo recharging: {round.Cooldown:0.0}s";
        GUI.Label(new Rect(26, 58, 900, 22),
            $"WASD / arrows: Move    Space: Echo    Shift + Space: Strong echo    E: Interact    |    {echo}");

        if (!won.HasValue) return;
        GUI.Box(new Rect(300, 225, 360, 150), "");
        GUI.Label(new Rect(325, 250, 310, 25), won.Value ? "YOU ESCAPED" : "GAME OVER");
        GUI.Label(new Rect(325, 275, 310, 25),
            won.Value ? "You opened the emergency exit." : "The hunter caught you.");
        if (GUI.Button(new Rect(325, 314, 160, 34), "Play again"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
