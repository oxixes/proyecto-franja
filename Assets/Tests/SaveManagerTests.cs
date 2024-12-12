using System.Collections;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
[TestFixture]
public class SaveManagerTests
{
    [CanBeNull] private SaveManager saveManager;

    [SetUp]
    public void SetUp()
    {
        // Crear una nueva instancia de SaveManager para cada test
        GameObject gameObject = new GameObject();
        saveManager = gameObject.AddComponent<SaveManager>();
    }

    [TearDown]
    public void TearDown()
    {
        // Asegurarse de eliminar todos los datos al final de cada test
        saveManager.DeleteAll();
    }

    [Test]
    public void Singleton_ReplaceInstance()
    {
        // Primera instancia
        SaveManager instance1 = SaveManager.GetInstance();
        Assert.NotNull(instance1, "La primera instancia debería crearse correctamente.");

        // Segunda instancia, debería reemplazar la primera
        GameObject newGameObject = new GameObject();
        SaveManager newInstance = newGameObject.AddComponent<SaveManager>();

        SaveManager instance2 = SaveManager.GetInstance();
        Assert.AreEqual(newInstance, instance2, "La segunda instancia debería reemplazar la primera.");
    }

    [Test]
    public void Singleton_GetInstanceErrorIfNotCreated()
    {
        // Destruir la instancia actual para simular un estado inicial sin instancia
        Object.DestroyImmediate(saveManager.gameObject);

        // Llamar a GetInstance debería causar un error, ya que no existe una instancia
        LogAssert.Expect(LogType.Error, "You called GetInstance of SaveManager before it was instantiated in the scene!");
        SaveManager instance = SaveManager.GetInstance();

        Assert.AreEqual("<" + instance+ ">", "<null>",  "GetInstance debería devolver null si no hay instancia.");
    }

    [Test]
    public void SaveAndRetrieve_Data()
    {
        // Probar guardar y cargar datos de diferentes tipos
        saveManager.Set("testString", "testValue");
        saveManager.Set("testInt", 123);
        saveManager.Set("testFloat", 3.14f);

        Assert.AreEqual("testValue", saveManager.Get<string>("testString"));
        Assert.AreEqual(123, saveManager.Get<int>("testInt"));
        Assert.AreEqual(3.14f, saveManager.Get<float>("testFloat"), 0.0001f);
    }

    [Test]
    public void DefaultValue_ForUnknownKey()
    {
        // Probar que devuelve el valor predeterminado para claves no existentes
        Assert.AreEqual(0, saveManager.Get<int>("unknownInt"));
        Assert.AreEqual(null, saveManager.Get<string>("unknownString"));
        Assert.AreEqual(0.0f, saveManager.Get<float>("unknownFloat"), 0.0001f);
    }

    [Test]
    public void PersistData_BetweenSessions()
    {
        // Guardar datos y simular persistencia entre sesiones
        saveManager.Set("persistedKey", "persistedValue");

        // Destruir y recrear SaveManager
        Object.DestroyImmediate(saveManager.gameObject);
        SetUp();

        Assert.AreEqual("persistedValue", saveManager.Get<string>("persistedKey"));
    }

    [Test]
    public void DeleteAll_RemovesAllData()
    {
        // Guardar algunos datos
        saveManager.Set("key1", "value1");
        saveManager.Set("key2", 123);

        // Borrar todos los datos
        saveManager.DeleteAll();

        Assert.AreEqual(null, saveManager.Get<string>("key1"));
        Assert.AreEqual(0, saveManager.Get<int>("key2"));
    }

    [Test]
    public void HandleUnsupportedTypes_BoolAndObjects()
    {
        // Probar guardar y recuperar un tipo no soportado directamente (como bool)
        saveManager.Set("testBool", true);
        string storedValue = saveManager.Get<string>("testBool");

        // Verificar que se guarda y recupera como string
        Assert.AreEqual("True", storedValue);

        // Intentar recuperar como bool
        bool retrievedValue = storedValue.ToLower() == "true";
        Assert.AreEqual(true, retrievedValue);

        // Probar con un objeto raro (por ejemplo, un Vector3 serializado como string)
        Vector3 vector = new Vector3(1, 2, 3);
        saveManager.Set("testVector", vector.ToString());
        string vectorStored = saveManager.Get<string>("testVector");

        Assert.AreEqual(vector.ToString(), vectorStored);
    }
}
