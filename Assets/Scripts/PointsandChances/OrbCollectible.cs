using UnityEngine;

public class OrbCollectible : MonoBehaviour
{
    public int points = 1;
    private ParticleSystem burstFX;

    private void Start()
    {
        burstFX = GetComponentInChildren<ParticleSystem>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Play burst
        if (burstFX != null)
        {
            burstFX.gameObject.SetActive(true);
            burstFX.Play();
        }

        // Hide orb
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        UIManager.Instance.FlyCoinToUI(transform.position, points);

        StartCoroutine(DisableAfterFX());
    }



    private System.Collections.IEnumerator DisableAfterFX()
    {
        yield return new WaitUntil(() => !burstFX.isPlaying);
        gameObject.SetActive(false);
    }
}
