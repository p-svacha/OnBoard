using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class AffinityGlowFX : MonoBehaviour
{
    [Tooltip("Set from code to the token's affinity color.")]
    public Color GlowColor = Color.white;

    [Tooltip("How many pulses per second.")]
    public float Frequency = 0.4f;

    [Tooltip("Glow alpha oscillation range (0…1).")]
    public float Amplitude = 1f;

    [Tooltip("Base glow alpha offset.")]
    public float BaseAlpha = 0.2f;

    // Scale pulse range
    [Tooltip("Minimum scale multiplier.")]
    public float ScaleMin = 1.2f;
    [Tooltip("Maximum scale multiplier.")]
    public float ScaleMax = 1.3f;

    MaterialPropertyBlock _props;
    MeshRenderer _renderer;
    int _colID, _intensityID;

    void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _props = new MaterialPropertyBlock();
        _colID = Shader.PropertyToID("_Color");
        _intensityID = Shader.PropertyToID("_Intensity");
    }

    void Update()
    {
        // time-based sine wave in [-1,1]
        float sine = Mathf.Sin(Time.time * Frequency * 2f * Mathf.PI);

        // ——— Pulse Alpha ———
        float pulseAlpha = BaseAlpha + (sine * 0.5f * Amplitude);
        pulseAlpha = Mathf.Clamp01(pulseAlpha);

        _renderer.GetPropertyBlock(_props);
        _props.SetColor(_colID, GlowColor);
        _props.SetFloat(_intensityID, pulseAlpha);
        _renderer.SetPropertyBlock(_props);

        // ——— Pulse Scale ———
        // Map sine [-1,1] -> [0,1]
        float t = sine * 0.5f + 0.5f;
        // Interpolate between ScaleMin and ScaleMax
        float scale = Mathf.Lerp(ScaleMin, ScaleMax, t);
        transform.localScale = Vector3.one * scale;
    }
}
