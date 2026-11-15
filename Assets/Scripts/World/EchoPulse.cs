using System;
using UnityEngine;

public sealed class EchoPulse : MonoBehaviour
{
    [SerializeField] float cooldown = 2f;
    [SerializeField] float visibilitySeconds = 3f;
    [SerializeField] Vector2 shortRanges = new(3f, 4f);
    [SerializeField] Vector2 strongRanges = new(7f, 9f);

    Vector2 origin;
    float emittedAt = float.NegativeInfinity;
    float radius;
    public event Action<Vector2, float> Heard;
    public float RemainingCooldown => Mathf.Max(0, cooldown - (Time.time - emittedAt));

    public void Emit(Vector2 point, bool strong)
    {
        if (RemainingCooldown > 0) return;
        Vector2 ranges = strong ? strongRanges : shortRanges;
        origin = point;
        radius = ranges.x;
        emittedAt = Time.time;
        Heard?.Invoke(point, ranges.y);
    }

    public float Visibility(Vector2 point)
    {
        float elapsed = Time.time - emittedAt;
        float distance = Vector2.Distance(origin, point);
        if (distance > radius || distance > elapsed * 18f) return 0;
        return Mathf.Clamp01(1 - elapsed / visibilitySeconds);
    }
}
