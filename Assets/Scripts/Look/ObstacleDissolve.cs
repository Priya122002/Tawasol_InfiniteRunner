using UnityEngine;
using System.Collections;

public class ObstacleDissolve : MonoBehaviour
{
    [Header("Dissolve Settings")]
    public float dissolveDuration = 4f;   // ✅ 4 seconds dissolve

    private Material[] mats;
    private bool isDissolving = false;

    private static readonly int DissolveID =
        Shader.PropertyToID("_DissolveAmount");

    void Awake()
    {
        // Works for parent OR child renderers
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        if (renderers == null || renderers.Length == 0)
        {
            Debug.LogError("No Renderer found on " + gameObject.name);
            return;
        }

        mats = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            mats[i] = renderers[i].material; // instance material
            mats[i].SetFloat(DissolveID, 0f);
        }
    }

    public void StartDissolve()
    {
        if (isDissolving || mats == null) return;

        isDissolving = true;
        StartCoroutine(DissolveRoutine());
    }

    IEnumerator DissolveRoutine()
    {
        float t = 0f;

        while (t < dissolveDuration)
        {
            Debug.Log("Dissolve");
            t += Time.deltaTime;

            float normalized = Mathf.Clamp01(t / dissolveDuration);

            // ✅ Smooth + visible dissolve
            float dissolve = Mathf.SmoothStep(0f, 1f, normalized);

            foreach (var mat in mats)
                mat.SetFloat(DissolveID, dissolve);

            yield return null;
        }

        foreach (var mat in mats)
            mat.SetFloat(DissolveID, 1f);

        // Disable obstacle AFTER dissolve fully finishes
        gameObject.SetActive(false);
    }

    // 🔁 Pooling reuse
    public void ResetDissolve()
    {
        isDissolving = false;

        if (mats == null) return;

        foreach (var mat in mats)
            mat.SetFloat(DissolveID, 0f);
    }
}
