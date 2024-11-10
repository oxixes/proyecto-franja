using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5.0f;
    public float runningSpeed = 10.0f;
    private Animator animator;
    PlayerInventoryManager playerInventory;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerInventory = gameObject.AddComponent<PlayerInventoryManager>();
        Debug.Log("Inventory Created for the player");

    }

    void Update()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!DialogueSystem.GetInstance().IsDialogueActive())
        {
            int horizontalDir = Input.GetAxisRaw("Horizontal") > 0 ? 1 : Input.GetAxisRaw("Horizontal") < 0 ? -1 : 0;
            int verticalDir = Input.GetAxisRaw("Vertical") > 0 ? 1 : Input.GetAxisRaw("Vertical") < 0 ? -1 : 0;

            animator.SetFloat("XDirection", horizontalDir);
            animator.SetFloat("YDirection", verticalDir);
            animator.SetBool("Moving", horizontalDir != 0 || verticalDir != 0);

            Vector2 direction = new Vector2(horizontalDir, verticalDir);

            bool running = Input.GetKey(KeyCode.LeftShift);

            transform.Translate(direction.normalized * (running ? runningSpeed : speed) * Time.deltaTime);
        } 
        else
        {
            animator.SetFloat("XDirection", 0);
            animator.SetFloat("YDirection", 0);
            animator.SetBool("Moving", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Trigger"))
        {
            TransitionManager tm = collision.gameObject.GetComponent<TransitionManager>();
            if (tm != null)
            {
                tm.Transition();
            }
        }
    }
}
