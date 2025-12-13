using UnityEngine;

public class ObstacleDissolve : MonoBehaviour
{
    public Material mat;
    public float dissolveSpeed = 1f;

    private float amount = 0;
    private bool dissolving = false;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    public void StartDissolve()
    {
        dissolving = true;
    }

    void Update()
    {
        if (!dissolving) return;

        amount += Time.deltaTime * dissolveSpeed;
        mat.SetFloat("_DissolveAmount", amount);

        if (amount >= 1f)
            Destroy(gameObject);
    }
}
