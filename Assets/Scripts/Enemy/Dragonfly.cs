using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragonfly : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float amplitude = 1f;
    public float frequency = 1f;
    public float minX = -5f;
    public float maxX = 5f;

    [Header("Attack Settings")]
    public float attackCooldown = 1f;
    private float lastAttackTime;

    [Header("Animation Settings")]
    public Animator animator;

    private float offset;
    private bool moveRight = true;
    private bool isTurning = false;

    void Start()
    {
        offset = transform.position.x;
    }

    void Update()
    {
        float waveMovement = amplitude * Mathf.Sin(Time.time * frequency + Time.deltaTime * speed); // Added speed to frequency
        float targetX = offset + waveMovement;

        if (moveRight)
        {
            if (targetX >= maxX)
            {
                StartTurnAnimation();
                moveRight = false;
            }
            else
            {
                PlayMoveAnimation();
            }
        }
        else
        {
            if (targetX <= minX)
            {
                StartTurnAnimation();
                moveRight = true;
            }
            else
            {
                PlayMoveAnimation();
            }
        }

        transform.position = new Vector2(targetX, transform.position.y); //Directly move the transform
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Time.time - lastAttackTime >= attackCooldown)
        {
            HealthManager playerHealth = collision.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
                lastAttackTime = Time.time;
            }
        }
    }

    private void PlayMoveAnimation()
    {
        if (animator && !isTurning)
        {
            animator.Play("WaveMove");
        }
    }

    private void StartTurnAnimation()
    {
        if (animator)
        {
            animator.Play("Turn");
            isTurning = true;
            Invoke("EndTurnAnimation", animator.GetCurrentAnimatorStateInfo(0).length);
        }
    }

    private void EndTurnAnimation()
    {
        isTurning = false;
    }
}