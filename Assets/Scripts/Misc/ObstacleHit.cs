using UnityEngine;

public class ObstacleHit : MonoBehaviour
{
    private bool hasHit = false;


    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player"))
        {
            hasHit = true;

            SoundManager.Instance.Play("damage");
           
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
