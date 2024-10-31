using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogueSystemTests
{
    private GameObject prefab;
    private DialogueSystem dialogueSystem;

    [SetUp]
    public void SetUp()
    {
        // Instantiate the DialogueSystem prefab
        GameObject dialogueSystemPrefab = Resources.Load<GameObject>("TestData/DialogueSystem");
        Assert.IsNotNull(dialogueSystemPrefab);

        // Instantiate the prefab
        prefab = GameObject.Instantiate(dialogueSystemPrefab);

        // Get the DialogueSystem component from the child DialoguePanel
        this.dialogueSystem = prefab.GetComponentInChildren<DialogueSystem>();
    }

    [UnityTest]
    public IEnumerator GetInstanceReturnsTheSameInstance()
    {
        DialogueSystem instance1 = DialogueSystem.GetInstance();
        DialogueSystem instance2 = DialogueSystem.GetInstance();

        Assert.AreEqual(instance1, dialogueSystem);
        Assert.AreEqual(instance1, instance2);

        yield return new WaitForEndOfFrame();
    }

    [UnityTest]
    public IEnumerator ThereIsNoActiveDialogueOnStart()
    {
        Assert.IsFalse(dialogueSystem.IsDialogueActive());

        yield return new WaitForEndOfFrame();
    }

    [UnityTest]
    public IEnumerator DialogueIsActivated()
    {
        dialogueSystem.StartDialogue("TestData/DialogueDataTest2");
        Assert.IsTrue(dialogueSystem.IsDialogueActive());

        yield return new WaitForEndOfFrame();
    }

    [UnityTest]
    public IEnumerator InvalidDialogueFails()
    {
        LogAssert.Expect(LogType.Error, "Dialogue with id: TestData/InvalidDialogueData not found.");
        dialogueSystem.StartDialogue("TestData/InvalidDialogueData");
        Assert.IsFalse(dialogueSystem.IsDialogueActive());

        LogAssert.Expect(LogType.Error, "Failed to parse dialogue with id: TestData/InvalidJson - JSON parse error: Invalid value.");
        dialogueSystem.StartDialogue("TestData/InvalidJson");
        Assert.IsFalse(dialogueSystem.IsDialogueActive());

        LogAssert.Expect(LogType.Error, "Dialogue line type is text or notification but no text is defined");
        dialogueSystem.StartDialogue("TestData/DialogueDataTest8");
        Assert.IsFalse(dialogueSystem.IsDialogueActive());

        yield return new WaitForEndOfFrame();
    }

    [UnityTest]
    public IEnumerator DialogueIsDeactivated()
    {
        dialogueSystem.textSpeed = 100000;

        dialogueSystem.StartDialogue("TestData/DialogueDataTest3");
        Assert.IsTrue(dialogueSystem.IsDialogueActive());

        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        dialogueSystem.spacePressed = true;
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(dialogueSystem.IsDialogueActive());

        yield return new WaitForEndOfFrame();
    }

    [UnityTest]
    public IEnumerator DialogueIsDisplayed()
    {
        dialogueSystem.textSpeed = 100000;
        dialogueSystem.StartDialogue("TestData/DialogueDataTest3");
        Assert.IsTrue(dialogueSystem.IsDialogueActive());

        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        TextMeshProUGUI text = GameObject.Find("DialogueText").GetComponent<TextMeshProUGUI>();

        Assert.AreEqual("Test dialogue line", text.text);

        TextMeshProUGUI talkerName = GameObject.Find("TalkerName").GetComponent<TextMeshProUGUI>();

        Assert.AreEqual("TestCharacterA", talkerName.text);

        yield return new WaitForEndOfFrame();
    }

    [UnityTest]
    public IEnumerator DialogueIsDiverted()
    {
        dialogueSystem.textSpeed = 100000;
        dialogueSystem.StartDialogue("TestData/DialogueSystemDiversionTest");
        Assert.IsTrue(dialogueSystem.IsDialogueActive());

        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        dialogueSystem.spacePressed = true;

        yield return new WaitForEndOfFrame();

        dialogueSystem.spacePressed = false;

        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        TextMeshProUGUI text = GameObject.Find("DialogueText").GetComponent<TextMeshProUGUI>();

        Assert.AreEqual("Test dialogue line", text.text);

        yield return new WaitForEndOfFrame();
    }

    [UnityTest]
    public IEnumerator DialogueWithImageIsDisplayed()
    {
        dialogueSystem.textSpeed = 100000;
        dialogueSystem.StartDialogue("TestData/DialogueDataTest3");
        Assert.IsTrue(dialogueSystem.IsDialogueActive());

        yield return new WaitForEndOfFrame();

        RawImage image = GameObject.Find("TalkerImage").GetComponent<RawImage>();

        // Check if the image is active
        Assert.IsTrue(image.gameObject.activeSelf);

        // Check if the image is the correct one
        Assert.AreEqual("example", image.texture.name);

        yield return new WaitForEndOfFrame();
    }

    [UnityTest]
    public IEnumerator DialogueWithOptionsIsDisplayed()
    {
        dialogueSystem.StartDialogue("TestData/DialogueDataTest4");
        Assert.IsTrue(dialogueSystem.IsDialogueActive());

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        GameObject option1 = GameObject.Find("Option1");
        GameObject option2 = GameObject.Find("Option2");

        // Check if the options are active
        Assert.IsTrue(option1.activeSelf);
        Assert.IsTrue(option2.activeSelf);

        // Check if the options have the correct text
        TextMeshProUGUI option1Text = option1.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI option2Text = option2.GetComponentInChildren<TextMeshProUGUI>();

        Assert.AreEqual("<color=yellow>> Option A</color>", option1Text.text);
        Assert.AreEqual("   Option B", option2Text.text);

        // Check the character name too
        TextMeshProUGUI talkerName = GameObject.Find("TalkerName").GetComponent<TextMeshProUGUI>();

        Assert.AreEqual("TestCharacterA", talkerName.text);

        yield return new WaitForEndOfFrame();
    }

    [UnityTest]
    public IEnumerator DialogueWithOptionsDiverts()
    {
        dialogueSystem.textSpeed = 100000;
        dialogueSystem.StartDialogue("TestData/DialogueDataTest4");
        Assert.IsTrue(dialogueSystem.IsDialogueActive());

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        dialogueSystem.spacePressed = true;

        yield return new WaitForEndOfFrame();

        dialogueSystem.spacePressed = false;

        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        TextMeshProUGUI text = GameObject.Find("DialogueText").GetComponent<TextMeshProUGUI>();

        Assert.AreEqual("Test dialogue line", text.text);

        yield return new WaitForEndOfFrame();
    }

    [UnityTest]
    public IEnumerator DialogueWithOptionsAllowsToChangeOption()
    {
        dialogueSystem.StartDialogue("TestData/DialogueDataTest4");
        Assert.IsTrue(dialogueSystem.IsDialogueActive());

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        dialogueSystem.downPressed = true;
        yield return new WaitForEndOfFrame();
        dialogueSystem.downPressed = false;
        yield return new WaitForEndOfFrame();

        GameObject option1 = GameObject.Find("Option1");
        GameObject option2 = GameObject.Find("Option2");

        // Get the text of the options
        TextMeshProUGUI option1Text = option1.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI option2Text = option2.GetComponentInChildren<TextMeshProUGUI>();

        // Check if the options have the correct text
        Assert.AreEqual("   Option A", option1Text.text);
        Assert.AreEqual("<color=yellow>> Option B</color>", option2Text.text);

        // Go up now
        dialogueSystem.upPressed = true;
        yield return new WaitForEndOfFrame();
        dialogueSystem.upPressed = false;
        yield return new WaitForEndOfFrame();

        // Check if the options have the correct text
        Assert.AreEqual("<color=yellow>> Option A</color>", option1Text.text);
        Assert.AreEqual("   Option B", option2Text.text);
    }

    [UnityTest]
    public IEnumerator DialogueAccelerates()
    {
        dialogueSystem.textSpeed = 1;
        dialogueSystem.accelerationOnInput = 10000f;
        dialogueSystem.StartDialogue("TestData/DialogueDataTest3");
        Assert.IsTrue(dialogueSystem.IsDialogueActive());

        yield return new WaitForSeconds(5);

        TextMeshProUGUI text = GameObject.Find("DialogueText").GetComponent<TextMeshProUGUI>();

        Assert.AreNotEqual("Test dialogue line", text.text);

        dialogueSystem.spacePressed = true;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        dialogueSystem.spacePressed = false;

        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        Assert.AreEqual("Test dialogue line", text.text);

        yield return new WaitForEndOfFrame();
    }

    [UnityTest]
    public IEnumerator DialogueNotifiesAndRemovesHandler()
    {
        bool notified = false;
        Action<string, string, string> handler = (string dialogueId, string notificationId, string notificationData) =>
        {
            Assert.AreEqual("TestData/DialogueDataTest16", dialogueId);
            Assert.AreEqual("TestNotification", notificationId);
            Assert.AreEqual("notificationData", notificationData);

            notified = true;
        };

        int handlerId = dialogueSystem.HandleNotification("TestNotification", handler);

        dialogueSystem.StartDialogue("TestData/DialogueDataTest16");
        Assert.IsFalse(dialogueSystem.IsDialogueActive());
        Assert.IsTrue(notified);

        notified = false;
        bool success = dialogueSystem.RemoveNotificationHandler(handlerId);
        Assert.IsTrue(success);

        dialogueSystem.StartDialogue("TestData/DialogueDataTest16");
        Assert.IsFalse(notified);

        yield return new WaitForEndOfFrame();
    }

    [UnityTest]
    public IEnumerator DialogueSystemFailsToRemoveNonExistentHandler()
    {
        LogAssert.Expect(LogType.Error, "Trying to remove a notification handler that does not exist");
        bool success = dialogueSystem.RemoveNotificationHandler(0);

        Assert.IsFalse(success);

        yield return new WaitForEndOfFrame();
    }

    [TearDown]
    public void TearDown()
    {
        // Destroy the DialogueSystem
        GameObject.Destroy(this.prefab);
        this.prefab = null;
        this.dialogueSystem = null;
    }
}
