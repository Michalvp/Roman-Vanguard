using UnityEngine;

public class Waterbehavior : MonoBehaviour
{
    [SerializeField] private float movementInterval = 0.5f;
    [SerializeField] private float movementStep = 0.5f;
    [SerializeField] private int riseSteps = 30;
    [SerializeField] private int waitSteps = 20;
    [SerializeField] private int destroySteps = 40;

    private float timepassed;
    private int waterrise;
    private int waterdrytime;

    private void Update()
    {
        timepassed += Time.deltaTime;

        if (timepassed <= movementInterval)
        {
            return;
        }

        if (waterrise < riseSteps)
        {
            waterrise++;

            transform.position = new Vector3(
                transform.position.x,
                transform.position.y + movementStep,
                transform.position.z
            );
        }
        else
        {
            waterdrytime++;

            if (waterdrytime > waitSteps)
            {
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y - movementStep,
                    transform.position.z
                );
            }

            if (waterdrytime > destroySteps)
            {
                Destroy(gameObject);
            }
        }

        timepassed = 0f;
    }
}
