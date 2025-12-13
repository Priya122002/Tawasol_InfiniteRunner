using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SpeedVisualController : MonoBehaviour
{
    [Header("Player")]
    public PlayerMovement player;

    [Header("Trigger Settings")]
    public float triggerSpeed = 15f;     // speed at which effect starts
    public float effectTime = 1.5f;      // how long effect stays

    [Header("Motion Blur")]
    public float maxBlur = 0.6f;

    [Header("Bloom")]
    public float normalBloom = 1f;
    public float boostBloom = 2.5f;

    [Header("Camera FOV")]
    public float normalFOV = 60f;
    public float boostFOV = 72f;

    Volume volume;
    MotionBlur motionBlur;
    Bloom bloom;
    Camera cam;

    float timer;
    bool effectRunning;

    void Awake()
    {
        cam = GetComponent<Camera>();

        volume = FindObjectOfType<Volume>();
        volume.profile.TryGet(out motionBlur);
        volume.profile.TryGet(out bloom);
    }

    void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerMovement>();
            return;
        }

        // 🔥 Trigger effect ONCE
        if (!effectRunning && player.forwardSpeed >= triggerSpeed)
        {
            effectRunning = true;
            timer = effectTime;
        }

        // ⏱ Effect timer
        if (effectRunning)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
                effectRunning = false;
        }

        // 🎯 Effect strength (fade out)
        float t = effectRunning ? timer / effectTime : 0f;
        t = Mathf.SmoothStep(0f, 1f, t);

        // Motion Blur
        if (motionBlur != null)
            motionBlur.intensity.value = Mathf.Lerp(0f, maxBlur, t);

        // Bloom
        if (bloom != null)
            bloom.intensity.value = Mathf.Lerp(normalBloom, boostBloom, t);

        // FOV
        cam.fieldOfView = Mathf.Lerp(normalFOV, boostFOV, t);
    }
}
