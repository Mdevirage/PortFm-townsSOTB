using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    private Animator animator;
    private bool isCollected = false;
    public NumberStringDisplay numberDisplay;
    void Start()
    {
        animator = GetComponent<Animator>();
        if (numberDisplay == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                numberDisplay = playerObject.GetComponent<NumberStringDisplay>();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            CollectKey();

        }
    }
    private void CollectKey()
    {
        isCollected = true;
        if (animator != null)
        {
            animator.Play("DoorInscription");
        }
        if (numberDisplay != null)
        {
            numberDisplay.SetKey(true);
        }
        StartCoroutine(DestroyAfterDelay(8f));
    }
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

}