using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InventoryTests
{
    private void DeleteInventoryFile()
    {
        string filePath = Application.persistentDataPath + "/inventory.json";
        // Delete the file if it exists
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    [SetUp]
    public void Setup()
    {
        DeleteInventoryFile();
    }

    [Test]
    public void InventoryLoadsIfFileDoesNotExist()
    {
        Inventory inventory = new Inventory();

        Assert.DoesNotThrow(() => inventory.LoadInventory());
    }

    [Test]
    public void EmptyInventorySavesIfFileDoesNotExist()
    {
        Inventory inventory = new Inventory();

        Assert.DoesNotThrow(() => inventory.SaveInventory());
        Assert.IsTrue(File.Exists(Application.persistentDataPath + "/inventory.json"));
    }

    [Test]
    public void InventoryItemAddingWorksIfFileDoesNotExist()
    {
        ItemData item = ScriptableObject.CreateInstance<ItemData>();
        item.itemName = "Test Item";
        item.description = "This is a test item.";
        item.icon = null;
        item.isUsable = true;

        Inventory inventory = new Inventory();

        inventory.AddItem(item);

        Assert.AreEqual(1, inventory.items.Count);
        Assert.AreEqual("Test Item", inventory.items[0].itemName);
        Assert.IsTrue(File.Exists(Application.persistentDataPath + "/inventory.json"));
    }

    [Test]
    public void InventoryLoadingReplacesContents()
    {
        ItemData item = ScriptableObject.CreateInstance<ItemData>();
        item.itemName = "Test Item";
        item.description = "This is a test item.";
        item.icon = null;
        item.isUsable = true;

        Inventory inventory = new Inventory();
        inventory.AddItem(item);

        Assert.IsTrue(File.Exists(Application.persistentDataPath + "/inventory.json"));
        
        File.Delete(Application.persistentDataPath + "/inventory.json");

        inventory.LoadInventory();

        Assert.AreEqual(0, inventory.items.Count);
    }

    [Test]
    public void InventoryAddingItemDoesNotDuplicateItems()
    {
        ItemData item = ScriptableObject.CreateInstance<ItemData>();
        item.itemName = "Test Item";
        item.description = "This is a test item.";
        item.icon = null;
        item.isUsable = true;

        Inventory inventory = new Inventory();
        inventory.AddItem(item);
        inventory.AddItem(item);

        Assert.AreEqual(1, inventory.items.Count);
    }

    [Test]
    public void InventoryAddingInformationWorks()
    {
        InformationData info = ScriptableObject.CreateInstance<InformationData>();
        info.infoName = "Test Information";
        info.description = "This is a test information.";

        Inventory inventory = new Inventory();
        inventory.AddInformation(info);

        Assert.AreEqual(1, inventory.information.Count);
        Assert.AreEqual("Test Information", inventory.information[0].infoName);
    }

    [Test]
    public void InventoryAddingInformationDoesNotDuplicate()
    {
        InformationData info = ScriptableObject.CreateInstance<InformationData>();
        info.infoName = "Test Information";
        info.description = "This is a test information.";

        Inventory inventory = new Inventory();
        inventory.AddInformation(info);
        inventory.AddInformation(info);

        Assert.AreEqual(1, inventory.information.Count);
    }

    [Test]
    public void InventoryHasItemWorks()
    {
        ItemData item = ScriptableObject.CreateInstance<ItemData>();
        item.itemName = "Test Item";
        item.description = "This is a test item.";
        item.icon = null;
        item.isUsable = true;

        ItemData item2 = ScriptableObject.CreateInstance<ItemData>();
        item2.itemName = "Test Item 2";
        item2.description = "This is a test item.";
        item2.icon = null;
        item2.isUsable = true;

        Inventory inventory = new Inventory();
        inventory.AddItem(item);

        Assert.IsTrue(inventory.HasItem(item));
        Assert.IsFalse(inventory.HasItem(item2));
    }

    [Test]
    public void InventoryHasInformationWorks()
    {
        InformationData info = ScriptableObject.CreateInstance<InformationData>();
        info.infoName = "Test Information";
        info.description = "This is a test information.";

        InformationData info2 = ScriptableObject.CreateInstance<InformationData>();
        info2.infoName = "Test Information 2";
        info2.description = "This is a test information.";

        Inventory inventory = new Inventory();
        inventory.AddInformation(info);

        Assert.IsTrue(inventory.HasInformation(info));
        Assert.IsFalse(inventory.HasInformation(info2));
    }

    [Test]
    public void ManagerCreatesInventoryOnStart()
    {
        GameObject go = new GameObject();
        PlayerInventoryManager manager = go.AddComponent<PlayerInventoryManager>();
        manager.Start();

        Assert.IsNotNull(manager.playerInventory);
    }

    [Test]
    public void ManagerAddsItemToInventory()
    {
        GameObject go = new GameObject();
        PlayerInventoryManager manager = go.AddComponent<PlayerInventoryManager>();
        manager.Start();

        ItemData item = ScriptableObject.CreateInstance<ItemData>();
        item.itemName = "Test Item";
        item.description = "This is a test item.";
        item.icon = null;
        item.isUsable = true;

        manager.CollectItem(item);

        Assert.AreEqual(1, manager.playerInventory.items.Count);
        Assert.AreEqual("Test Item", manager.playerInventory.items[0].itemName);
    }

    [Test]
    public void ManagerAddsInformationToInventory()
    {
        GameObject go = new GameObject();
        PlayerInventoryManager manager = go.AddComponent<PlayerInventoryManager>();
        manager.Start();

        InformationData info = ScriptableObject.CreateInstance<InformationData>();
        info.infoName = "Test Information";
        info.description = "This is a test information.";

        manager.CollectInformation(info);

        Assert.AreEqual(1, manager.playerInventory.information.Count);
        Assert.AreEqual("Test Information", manager.playerInventory.information[0].infoName);
    }

    [Test]
    public void ManagerSavesInventoryOnCheckpoint()
    {
        GameObject go = new GameObject();
        PlayerInventoryManager manager = go.AddComponent<PlayerInventoryManager>();
        manager.Start();

        ItemData item = ScriptableObject.CreateInstance<ItemData>();
        item.itemName = "Test Item";
        item.description = "This is a test item.";
        item.icon = null;
        item.isUsable = true;

        manager.CollectItem(item);

        Assert.IsTrue(File.Exists(Application.persistentDataPath + "/inventory.json"));
        File.Delete(Application.persistentDataPath + "/inventory.json");

        manager.SaveOnCheckpoint();

        Assert.IsTrue(File.Exists(Application.persistentDataPath + "/inventory.json"));
    }

    [Test]
    public void ManagerHasItemWorks()
    {
        GameObject go = new GameObject();
        PlayerInventoryManager manager = go.AddComponent<PlayerInventoryManager>();
        manager.Start();

        ItemData item = ScriptableObject.CreateInstance<ItemData>();
        item.itemName = "Test Item";
        item.description = "This is a test item.";
        item.icon = null;
        item.isUsable = true;

        manager.CollectItem(item);

        Assert.IsTrue(manager.HasItem(item));
    }

    [Test]
    public void ManagerHasInformationWorks()
    {
        GameObject go = new GameObject();
        PlayerInventoryManager manager = go.AddComponent<PlayerInventoryManager>();
        manager.Start();

        InformationData info = ScriptableObject.CreateInstance<InformationData>();
        info.infoName = "Test Information";
        info.description = "This is a test information.";

        manager.CollectInformation(info);

        Assert.IsTrue(manager.HasInformation(info));
    }

    [Test]
    public void ManagerSavesInventoryOnQuit()
    {
        GameObject go = new GameObject();
        PlayerInventoryManager manager = go.AddComponent<PlayerInventoryManager>();
        manager.Start();

        ItemData item = ScriptableObject.CreateInstance<ItemData>();
        item.itemName = "Test Item";
        item.description = "This is a test item.";
        item.icon = null;
        item.isUsable = true;

        manager.CollectItem(item);

        Assert.IsTrue(File.Exists(Application.persistentDataPath + "/inventory.json"));
        File.Delete(Application.persistentDataPath + "/inventory.json");

        manager.OnApplicationQuit();

        Assert.IsTrue(File.Exists(Application.persistentDataPath + "/inventory.json"));
    }

    [Test]
    public void CollectibleItemAddsToInventoryOnCollect()
    {
        GameObject go = new GameObject();
        PlayerInventoryManager manager = go.AddComponent<PlayerInventoryManager>();
        manager.Start();

        GameObject collectible = new GameObject();
        CollectibleItem item = collectible.AddComponent<CollectibleItem>();

        ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
        itemData.itemName = "Test Item";
        itemData.description = "This is a test item.";
        itemData.icon = null;
        itemData.isUsable = true;

        item.itemData = itemData;
        item.playerInventory = manager;

        LogAssert.ignoreFailingMessages = true; // Destroy in edit mode
        item.CollectItem();
        LogAssert.ignoreFailingMessages = false;

        Assert.AreEqual(1, manager.playerInventory.items.Count);
        Assert.AreEqual("Test Item", manager.playerInventory.items[0].itemName);
    }

    [Test]
    public void InfoProviderAddsToInventoryOnInfoReceived()
    {
        GameObject go = new GameObject();
        PlayerInventoryManager manager = go.AddComponent<PlayerInventoryManager>();
        manager.Start();

        GameObject infoProvider = new GameObject();
        InfoProvider info = infoProvider.AddComponent<InfoProvider>();

        InformationData infoData = ScriptableObject.CreateInstance<InformationData>();
        infoData.infoName = "Test Information";
        infoData.description = "This is a test information.";

        info.infoData = infoData;
        info.playerInventory = manager;

        info.AddInfoToInventory();

        Assert.AreEqual(1, manager.playerInventory.information.Count);
        Assert.AreEqual("Test Information", manager.playerInventory.information[0].infoName);
    }

    [TearDown]
    public void Teardown()
    {
        DeleteInventoryFile();
    }
}
