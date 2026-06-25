using UnityEngine;

public class Swordscript : MonoBehaviour
{
    [SerializeField] private float delayBeforeFalling = 3f;
    [SerializeField] private float fallStep = 0.5f;
    [SerializeField] private float fallStepInterval = 0.01f;

    private float timepassed;
    private bool move;

    private void Update()
    {
        timepassed += Time.deltaTime;

        if (!move && timepassed > delayBeforeFalling)
        {
            move = true;
            timepassed = 0f;
        }

        if (move && timepassed > fallStepInterval)
        {
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y - fallStep,
                transform.position.z
            );

            timepassed = 0f;
        }
    }
}
