using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    private Animator animator;
    private bool isCollected = false;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void PlayPotionAnimation()
    {
        if (animator != null)
        {
           animator.Play("Potion");
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            HealthManager playerHealth = other.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                    playerHealth.HPPotion();
                    CollectPotion();
            }
        }
    }
    private void CollectPotion()
    {
        isCollected = true;
        if (animator != null)
        {
            animator.Play("inscription");
            StartCoroutine(DestroyAfterDelay(8f));
        }
    }
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

}
