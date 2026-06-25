using UnityEngine;

public class Lightningbehavior : MonoBehaviour
{
    [SerializeField] private float lifetime = 1f;

    private float elapsedTime;

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
