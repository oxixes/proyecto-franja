using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DialogueDataTests
{
    [Test]
    public void DialogueDataWithEmptyJSONPasses()
    {
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>("{}");
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(0, dialogueData.lines.Length);
    }

    [Test]
    public void DialogueDataWithEmptyLinesPasses()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("Dialogues/TestData/DialogueDataTest1");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(0, dialogueData.lines.Length);
    }

    [Test]
    public void DialogueDataWithSimpleLinesPasses()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("Dialogues/TestData/DialogueDataTest2");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(2, dialogueData.lines.Length);
        Assert.IsTrue(dialogueData.lines[0].CheckFormatOk());
        Assert.IsTrue(dialogueData.lines[1].CheckFormatOk());

        Assert.AreEqual("text", dialogueData.lines[0].type);
        Assert.AreEqual("TestCharacterA", dialogueData.lines[0].characterName);

        Assert.AreEqual("text", dialogueData.lines[1].type);
        Assert.AreEqual("TestCharacterB", dialogueData.lines[1].characterName);
    }

    [Test]
    public void DialogueDataWithImagesPasses()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("Dialogues/TestData/DialogueDataTest3");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsTrue(dialogueData.lines[0].CheckFormatOk());

        Assert.AreEqual("text", dialogueData.lines[0].type);
        Assert.AreEqual("example", dialogueData.lines[0].imageAssetId);
    }

    [Test]
    public void DialogueDataWithOptionsPasses()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("Dialogues/TestData/DialogueDataTest4");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsTrue(dialogueData.lines[0].CheckFormatOk());

        Assert.AreEqual("options", dialogueData.lines[0].type);
        Assert.AreEqual(2, dialogueData.lines[0].options.Length);
        
        Assert.AreEqual("Option A", dialogueData.lines[0].options[0].text);
        Assert.AreEqual("TestData/DialogueDataTest3", dialogueData.lines[0].options[0].diversion);

        Assert.AreEqual("Option B", dialogueData.lines[0].options[1].text);
        Assert.AreEqual("TestDialogueB", dialogueData.lines[0].options[1].diversion);
    }

    [Test]
    public void DialogueDataWithDiversionsPasses()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("Dialogues/TestData/DialogueDataTest5");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsTrue(dialogueData.lines[0].CheckFormatOk());

        Assert.AreEqual("diversion", dialogueData.lines[0].type);
        Assert.AreEqual("TestDialogueA", dialogueData.lines[0].diversion);
    }

    [Test]
    public void DialogueDataWithMissingCharacterNamePasses()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("Dialogues/TestData/DialogueDataTest7");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsTrue(dialogueData.lines[0].CheckFormatOk());
    }

    [Test]
    public void DialogueDataWithNotificationsPasses()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("Dialogues/TestData/DialogueDataTest16");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsTrue(dialogueData.lines[0].CheckFormatOk());

        Assert.AreEqual("notification", dialogueData.lines[0].type);
        Assert.AreEqual("TestNotification", dialogueData.lines[0].notificationId);
    }

    [Test]
    public void DialogueDataWithInvalidTypeFails()
    {
        LogAssert.Expect(LogType.Error, "Invalid dialogue line type: blabla");
        TextAsset dialogueJSON = Resources.Load<TextAsset>("Dialogues/TestData/DialogueDataTest6");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsFalse(dialogueData.lines[0].CheckFormatOk());
    }

    [Test]
    public void DialogueDataWithMissingTextFails()
    {
        LogAssert.Expect(LogType.Error, "Dialogue line type is text or notification but no text is defined");
        TextAsset dialogueJSON = Resources.Load<TextAsset>("Dialogues/TestData/DialogueDataTest8");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsFalse(dialogueData.lines[0].CheckFormatOk());
    }

    [Test]
    public void DialogueDataWithMissingOptionsFails()
    {
        LogAssert.Expect(LogType.Error, "Dialogue line type is options but no options are defined");
        TextAsset dialogueJSON = Resources.Load<TextAsset>("Dialogues/TestData/DialogueDataTest9");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsFalse(dialogueData.lines[0].CheckFormatOk());
    }

    [Test]
    public void DialogueDataWithTooManyOptionsFails()
    {
        LogAssert.Expect(LogType.Error, "Dialogue line type is options but more than 3 options are defined");
        TextAsset dialogueJSON = Resources.Load<TextAsset>("Dialogues/TestData/DialogueDataTest10");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsFalse(dialogueData.lines[0].CheckFormatOk());
    }

    [Test]
    public void DialogueDataWithMissingDiversionFails()
    {
        LogAssert.Expect(LogType.Error, "Dialogue line type is diversion but no diversion is defined");
        TextAsset dialogueJSON = Resources.Load<TextAsset>("Dialogues/TestData/DialogueDataTest11");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsFalse(dialogueData.lines[0].CheckFormatOk());
    }

    [Test]
    public void DialogueDataWithMissingNotificationIdFails()
    {
        LogAssert.Expect(LogType.Error, "Dialogue line type is notification but no notificationId is defined");
        TextAsset dialogueJSON = Resources.Load<TextAsset>("Dialogues/TestData/DialogueDataTest14");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsFalse(dialogueData.lines[0].CheckFormatOk());
    }

    [Test]
    public void DialogueDataWithMissingNotificationTextFails()
    {
        LogAssert.Expect(LogType.Error, "Dialogue line type is text or notification but no text is defined");
        TextAsset dialogueJSON = Resources.Load<TextAsset>("Dialogues/TestData/DialogueDataTest15");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsFalse(dialogueData.lines[0].CheckFormatOk());
    }

    [Test]
    public void DialogueDataCantGetTextInfoFromNonTextType()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("Dialogues/TestData/DialogueDataTest12");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsTrue(dialogueData.lines[0].CheckFormatOk());

        LogAssert.Expect(LogType.Error, "Trying to get text char length for a non-text dialogue line");
        Assert.AreEqual(-1, dialogueData.lines[0].GetTextCharLength());
        LogAssert.Expect(LogType.Error, "Trying to get text up until a char count for a non-text dialogue line");
        Assert.IsNull(dialogueData.lines[0].GetTextUpUntil(10));
    }

    [Test]
    public void DialogueDataLengthDoesNotCountTags()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("Dialogues/TestData/DialogueDataTest13");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(2, dialogueData.lines.Length);

        Assert.AreEqual(18, dialogueData.lines[0].GetTextCharLength());
        Assert.AreEqual(18, dialogueData.lines[1].GetTextCharLength());
    }

    [Test]
    public void DialogueDataGetTextUpUntilWorks()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("Dialogues/TestData/DialogueDataTest13");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(2, dialogueData.lines.Length);

        Assert.AreEqual("Te", dialogueData.lines[0].GetTextUpUntil(2));
        Assert.AreEqual("Test ", dialogueData.lines[0].GetTextUpUntil(5));
        Assert.AreEqual("Test <b>d</b>", dialogueData.lines[0].GetTextUpUntil(6));
        Assert.AreEqual("Test <b>dialogue line</b>", dialogueData.lines[0].GetTextUpUntil(18));
        Assert.AreEqual("Test <b>dialogue line</b>", dialogueData.lines[0].GetTextUpUntil(20));

        Assert.AreEqual("Test <b><color=yellow>dialogue</color> line</b>", dialogueData.lines[1].GetTextUpUntil(18));
        Assert.AreEqual("Test <b><color=yellow>d</color></b>", dialogueData.lines[1].GetTextUpUntil(6));
        Assert.AreEqual("Test <b><color=yellow>dialogue</color> li</b>", dialogueData.lines[1].GetTextUpUntil(16));
    }
}
