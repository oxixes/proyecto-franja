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
        TextAsset dialogueJSON = Resources.Load<TextAsset>("TestData/Dialogues/DialogueDataTest1");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(0, dialogueData.lines.Length);
    }

    [Test]
    public void DialogueDataWithSimpleLinesPasses()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("TestData/Dialogues/DialogueDataTest2");
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
        TextAsset dialogueJSON = Resources.Load<TextAsset>("TestData/Dialogues/DialogueDataTest3");
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
        TextAsset dialogueJSON = Resources.Load<TextAsset>("TestData/Dialogues/DialogueDataTest4");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsTrue(dialogueData.lines[0].CheckFormatOk());

        Assert.AreEqual("options", dialogueData.lines[0].type);
        Assert.AreEqual(2, dialogueData.lines[0].options.Length);
        
        Assert.AreEqual("Option A", dialogueData.lines[0].options[0].text);
        Assert.AreEqual("TestDialogueA", dialogueData.lines[0].options[0].diversion);

        Assert.AreEqual("Option B", dialogueData.lines[0].options[1].text);
        Assert.AreEqual("TestDialogueB", dialogueData.lines[0].options[1].diversion);
    }

    [Test]
    public void DialogueDataWithDiversionsPasses()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("TestData/Dialogues/DialogueDataTest5");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsTrue(dialogueData.lines[0].CheckFormatOk());

        Assert.AreEqual("diversion", dialogueData.lines[0].type);
        Assert.AreEqual("TestDialogueA", dialogueData.lines[0].diversion);
    }

    [Test]
    public void DialogueDataWithInvalidTypeFails()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("TestData/Dialogues/DialogueDataTest6");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsFalse(dialogueData.lines[0].CheckFormatOk());
    }

    [Test]
    public void DialogueDataWithMissingCharacterNameFails()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("TestData/Dialogues/DialogueDataTest7");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsFalse(dialogueData.lines[0].CheckFormatOk());
    }

    [Test]
    public void DialogueDataWithMissingTextFails()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("TestData/Dialogues/DialogueDataTest8");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsFalse(dialogueData.lines[0].CheckFormatOk());
    }

    [Test]
    public void DialogueDataWithMissingOptionsFails()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("TestData/Dialogues/DialogueDataTest9");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsFalse(dialogueData.lines[0].CheckFormatOk());
    }

    [Test]
    public void DialogueDataWithTooManyOptionsFails()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("TestData/Dialogues/DialogueDataTest10");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsFalse(dialogueData.lines[0].CheckFormatOk());
    }

    [Test]
    public void DialogueDataWithMissingDiversionFails()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("TestData/Dialogues/DialogueDataTest11");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsFalse(dialogueData.lines[0].CheckFormatOk());
    }

    [Test]
    public void DialogueDataCantGetTextInfoFromNonTextType()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("TestData/Dialogues/DialogueDataTest12");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(1, dialogueData.lines.Length);
        Assert.IsTrue(dialogueData.lines[0].CheckFormatOk());

        Assert.AreEqual(-1, dialogueData.lines[0].GetTextCharLength());
        Assert.IsNull(dialogueData.lines[0].GetTextUpUntil(10));
    }

    [Test]
    public void DialogueDataLengthDoesNotCountTags()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("TestData/Dialogues/DialogueDataTest13");
        DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJSON.text);
        Assert.IsNotNull(dialogueData);

        Assert.AreEqual(2, dialogueData.lines.Length);

        Assert.AreEqual(18, dialogueData.lines[0].GetTextCharLength());
        Assert.AreEqual(18, dialogueData.lines[1].GetTextCharLength());
    }

    [Test]
    public void DialogueDataGetTextUpUntilWorks()
    {
        TextAsset dialogueJSON = Resources.Load<TextAsset>("TestData/Dialogues/DialogueDataTest13");
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
