using UnityEngine;

public class GlowOrbController : MonoBehaviour
{
    [Header("Color Range")]
    public Gradient glowColorRange; // assign in inspector

    [Header("Optional Random Settings")]
    public Vector2 glowIntensityRange = new Vector2(2, 8);
    public Vector2 pulseSpeedRange = new Vector2(1, 6);
    public Vector2 pulseStrengthRange = new Vector2(0.2f, 0.9f);

    Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;

        // Pick a random color from gradient
        Color randomGlow = glowColorRange.Evaluate(Random.value);

        mat.SetColor("_GlowColor", randomGlow);

        // Core stays white hot (optional)
        mat.SetColor("_CoreColor", Color.white * 2f);

        // Randomize intensity and pulse
        mat.SetFloat("_GlowIntensity", Random.Range(glowIntensityRange.x, glowIntensityRange.y));
        mat.SetFloat("_PulseSpeed", Random.Range(pulseSpeedRange.x, pulseSpeedRange.y));
        mat.SetFloat("_PulseStrength", Random.Range(pulseStrengthRange.x, pulseStrengthRange.y));
    }
}
