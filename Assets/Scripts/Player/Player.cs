using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5.0f;
    public bool inDialogue = false;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!inDialogue)
        {
            int horizontalDir = Input.GetAxisRaw("Horizontal") > 0 ? 1 : Input.GetAxisRaw("Horizontal") < 0 ? -1 : 0;
            int verticalDir = Input.GetAxisRaw("Vertical") > 0 ? 1 : Input.GetAxisRaw("Vertical") < 0 ? -1 : 0;

            animator.SetFloat("XDirection", horizontalDir);
            animator.SetFloat("YDirection", verticalDir);
            animator.SetBool("Moving", horizontalDir != 0 || verticalDir != 0);

            Vector2 direction = new Vector2(horizontalDir, verticalDir);

            transform.Translate(direction.normalized * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Trigger"))
        {
            collision.gameObject.GetComponent<TransitionManager>().Transition();
            GameObject level = collision.gameObject.GetComponent<TransitionManager>().newLevel;
        }
    }
}
