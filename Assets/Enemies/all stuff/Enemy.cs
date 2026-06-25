using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int health = 150;
    [SerializeField] protected int damage = 20;
    [SerializeField] private int armor = 10;

    protected GameObject player;

    public void setstats(int health, int damage, int armor)
    {
        this.health = health;
        this.damage = damage;
        this.armor = armor;
    }

    public void takedamage(int incomingDamage)
    {
        int finalDamage = Mathf.Max(1, incomingDamage - armor);

        Debug.Log("Enemy took " + finalDamage + " damage");
        health -= finalDamage;

        if (health <= 0)
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }

            PlayerStats playerStats =
                player != null ? player.GetComponent<PlayerStats>() : null;

            if (playerStats != null)
            {
                playerStats.AddXP(10 * (Mappicker.completedLevels + 1));
            }

            Destroy(gameObject);
        }
    }
}
