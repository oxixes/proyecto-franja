using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerTests
{
    private GameObject prefab;
    private Player player;

    private GameObject level1Prefab;
    private TransitionManager transitionManager;
    private GameObject level2Prefab;
    
    private GameObject dialogueSystemPrefab;
    private DialogueSystem dialogueSystem;

    private GameObject npc;
    private InteractableObject npcInteractableObject;

    [SetUp]
    public void SetUp()
    {
        // Instantiate the DialogueSystem (required by the Player)
        GameObject dialogueSystemPrefabResource = Resources.Load<GameObject>("TestData/DialogueSystem");
        Assert.IsNotNull(dialogueSystemPrefabResource);

        this.dialogueSystemPrefab = GameObject.Instantiate(dialogueSystemPrefabResource);
        this.dialogueSystem = dialogueSystemPrefab.GetComponentInChildren<DialogueSystem>();

        // Instantiate the Player prefab
        GameObject playerPrefabResource = Resources.Load<GameObject>("TestData/Player");
        Assert.IsNotNull(playerPrefabResource);

        this.prefab = GameObject.Instantiate(playerPrefabResource);

        // Get the Player component
        this.player = prefab.GetComponent<Player>();

        // Instantiate the levels prefab
        GameObject level1PrefabResource = Resources.Load<GameObject>("TestData/Level1");
        Assert.IsNotNull(level1PrefabResource);
        this.level1Prefab = GameObject.Instantiate(level1PrefabResource);

        // Get the "TriggerRight" children
        Transform triggerRight = this.level1Prefab.transform.Find("TriggerRight");
        Assert.IsNotNull(triggerRight);
        this.transitionManager = triggerRight.GetComponent<TransitionManager>();
        Assert.IsNotNull(transitionManager);

        GameObject level2PrefabResource = Resources.Load<GameObject>("TestData/Level2");
        Assert.IsNotNull(level2PrefabResource);
        this.level2Prefab = GameObject.Instantiate(level2PrefabResource);

        this.transitionManager.newLevel = this.level2Prefab;
        this.transitionManager.coordToChange = TransitionManager.Coordinate.X;
        this.transitionManager.newPlayerPosition = new Vector2(0, 0);

        this.level2Prefab.SetActive(false);

        // Instantiate the NPC prefab
        GameObject npcPrefabResource = Resources.Load<GameObject>("TestData/InteractableNPC");
        Assert.IsNotNull(npcPrefabResource);

        this.npc = GameObject.Instantiate(npcPrefabResource);
        this.npcInteractableObject = npc.GetComponent<InteractableObject>();

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    [UnityTest]
    public IEnumerator PlayerMoves()
    {
        float currentY = player.transform.position.y;
        player.upPressed = true;
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        Assert.Greater(player.transform.position.y, currentY);

        float currentX = player.transform.position.x;
        player.upPressed = false;
        player.leftPressed = true;

        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        Assert.Less(player.transform.position.x, currentX);

        yield return new WaitForEndOfFrame();
    }

    [UnityTest]
    public IEnumerator PlayerSwitchesLevels()
    {
        yield return new WaitForSeconds(.5f);

        player.transform.position = new Vector2(13.5f, 0);
        this.transitionManager.player = player.gameObject;
        yield return new WaitForSeconds(.5f);

        Assert.IsFalse(this.level1Prefab.activeSelf);
        Assert.IsTrue(this.level2Prefab.activeSelf);

        Assert.AreEqual(player.transform.position.x, 0f);

        yield return new WaitForEndOfFrame();
    }

    [UnityTest]
    public IEnumerator PlayerInteracts()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        npcInteractableObject.dialogueID = "TestData/DialogueDataTest2";
        npcInteractableObject.ePressed = true;
        npcInteractableObject.forcePlayerInRange = true;

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(this.dialogueSystem.IsDialogueActive());

        yield return new WaitForEndOfFrame();
    }


    [TearDown]
    public void TearDown()
    {
        // Destroy the Player
        GameObject.Destroy(prefab);
        this.prefab = null;
        this.player = null;

        // Destroy the levels
        GameObject.Destroy(level1Prefab);
        GameObject.Destroy(level2Prefab);
        this.level1Prefab = null;
        this.level2Prefab = null;
        this.transitionManager = null;

        // Destroy the DialogueSystem
        GameObject.Destroy(dialogueSystemPrefab);
        this.dialogueSystemPrefab = null;

        // Destroy the NPC
        GameObject.Destroy(npc);
        this.npc = null;
        this.npcInteractableObject = null;

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
