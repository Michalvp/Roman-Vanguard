using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int health = 150;
    [SerializeField] protected int damage = 20;
    [SerializeField] private int armor = 10;
    protected GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setstats(int health, int damage, int armor)
    {
        this.health = health;
        this.damage = damage;
        this.armor = armor;
    }
    public void takedamage(int damage)
    {
        health -= (damage - armor);
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
