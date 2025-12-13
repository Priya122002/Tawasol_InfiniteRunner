using UnityEngine;

public class ObstacleHit : MonoBehaviour
{
    private bool hasHit = false;   // Prevents multiple hits

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player"))
        {
            hasHit = true;

            // Damage the player
            LifeSystem.Instance.PlayerHit();

            // Trigger dissolve effect
            GetComponent<ObstacleDissolve>()?.StartDissolve();

            // Optional: disable collider
            GetComponent<Collider>().enabled = false;
        }
    }

    void ResetHit()
    {
        hasHit = false;
    }
}
