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
           
            LifeSystem.Instance.PlayerHit();

            GetComponent<ObstacleDissolve>()?.StartDissolve();

            GetComponent<Collider>().enabled = false;
        }
    }

    public void ResetHit()
    {
        hasHit = false;
        GetComponent<Collider>().enabled = true;
    }
}
