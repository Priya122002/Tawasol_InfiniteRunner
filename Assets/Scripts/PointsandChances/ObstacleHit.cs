using UnityEngine;

public class ObstacleHit : MonoBehaviour
{
    private bool hasHit = false;

    [Header("Hit Sound")]
    public AudioSource audioSource;
    public AudioClip hitSound;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player"))
        {
            hasHit = true;

            // 🔊 Play hit sound once
            if (audioSource != null && hitSound != null)
                audioSource.PlayOneShot(hitSound);

            // Damage player
            LifeSystem.Instance.PlayerHit();

            // 🔥 Dissolve THIS obstacle
            GetComponent<ObstacleDissolve>()?.StartDissolve();

            // Disable collider immediately
            GetComponent<Collider>().enabled = false;
        }
    }

    // 🔁 Reset when reused from pool
    public void ResetHit()
    {
        hasHit = false;
        GetComponent<Collider>().enabled = true;
    }
}
