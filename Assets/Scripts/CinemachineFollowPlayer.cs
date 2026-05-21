using UnityEngine;
using Unity.Cinemachine;

public class CinemachineFollowPlayer : MonoBehaviour
{
    void Start()
    {
        // Find the player object in the current scene using the "Player" tag
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            // Get the Virtual Camera component on this object
            var vcam = GetComponent<CinemachineCamera>();
            if (vcam != null)
            {
                // Assign the player's transform to the Follow property
                vcam.Follow = player.transform;
                Debug.Log("Cinemachine successfully linked to Player in this scene.");
            }
        }
        else
        {
            Debug.LogWarning("CinemachineFollowPlayer: No GameObject with tag 'Player' found in this scene!");
        }
    }
}