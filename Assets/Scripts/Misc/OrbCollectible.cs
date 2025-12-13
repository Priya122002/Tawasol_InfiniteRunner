using UnityEngine;
using System.Collections;

public class OrbCollectible : MonoBehaviour
{
    public int points = 1;

    [Header("FX")]
    private ParticleSystem burstFX;



    private bool collected = false;

    private void Awake()
    {
        burstFX = GetComponentInChildren<ParticleSystem>(true);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        collected = true;

        SoundManager.Instance.Play("collect coin");
        // Play burst FX
        if (burstFX != null)
        {
            burstFX.gameObject.SetActive(true);
            burstFX.Play();
        }

        // Hide orb visuals + collider
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // Fly coin to UI + add score at end
        UIManager.Instance.FlyCoinToUI(transform.position, points);

        StartCoroutine(DisableAfterFX());
    }

    private IEnumerator DisableAfterFX()
    {
        if (burstFX != null)
            yield return new WaitUntil(() => !burstFX.isPlaying);
        else
            yield return null;

        gameObject.SetActive(false);
    }

    // 🔁 For pooling reuse
    public void ResetOrb()
    {
        collected = false;
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<Collider>().enabled = true;

        if (burstFX != null)
            burstFX.gameObject.SetActive(false);
    }
}
