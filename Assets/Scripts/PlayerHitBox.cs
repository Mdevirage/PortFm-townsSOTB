using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            BugEnemy bugEnemy = other.GetComponent<BugEnemy>();
            Sword sword = other.GetComponent<Sword>();
            Sphere sphere = other.GetComponent<Sphere>();
            Axe axe = other.GetComponent<Axe>();

            if (bugEnemy != null) bugEnemy.TakeDamage();
            if (sword != null) sword.TakeDamage();
            if (sphere != null) sphere.TakeDamage();
            if (axe != null) axe.TakeDamage();
        }
    }
}
