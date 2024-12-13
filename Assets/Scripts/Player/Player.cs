using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5.0f;
    public float runningSpeed = 10.0f;
    private Animator animator;
    //PlayerInventoryManager playerInventory;

    // Allow tests to simulate key presses
    [HideInInspector] public bool upPressed = false;
    [HideInInspector] public bool downPressed = false;
    [HideInInspector] public bool leftPressed = false;
    [HideInInspector] public bool rightPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        //playerInventory = gameObject.AddComponent<PlayerInventoryManager>();
        Debug.Log("Inventory Created for the player");

        if (SaveManager.GetInstance().Get<int>("InitialSpawn") != 0)
        {
            gameObject.transform.position = new Vector3(SaveManager.GetInstance().Get<float>("PlayerX"), SaveManager.GetInstance().Get<float>("PlayerY"), 0);
        }
        else
        {
            SaveManager.GetInstance().Set("InitialSpawn", 1);
            SaveManager.GetInstance().Set("PlayerX", transform.position.x);
            SaveManager.GetInstance().Set("PlayerY", transform.position.y);
        }

        StartCoroutine(SavePeriodically());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SaveManager.GetInstance().DeleteAll();
            Application.Quit();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!DialogueSystem.GetInstance().IsDialogueActive() && !Minigame.isInMinigame)
        {
            int horizontalDir = Input.GetAxisRaw("Horizontal") > 0 ? 1 : Input.GetAxisRaw("Horizontal") < 0 ? -1 : 0;
            int verticalDir = Input.GetAxisRaw("Vertical") > 0 ? 1 : Input.GetAxisRaw("Vertical") < 0 ? -1 : 0;

            if (upPressed) verticalDir = 1;
            if (downPressed) verticalDir = -1;
            if (leftPressed) horizontalDir = -1;
            if (rightPressed) horizontalDir = 1;

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

    // Save the player's position when the game closes
    private void OnApplicationQuit()
    {
        SaveManager.GetInstance().Set("PlayerX", transform.position.x);
        SaveManager.GetInstance().Set("PlayerY", transform.position.y);
    }

    private IEnumerator SavePeriodically()
    {
        while (true)
        {
            SaveManager.GetInstance().Set("PlayerX", transform.position.x);
            SaveManager.GetInstance().Set("PlayerY", transform.position.y);
            yield return new WaitForSeconds(5);
        }
    }
}
