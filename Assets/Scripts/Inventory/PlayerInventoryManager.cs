using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{
    public Inventory playerInventory;

    void Start()
    {
        playerInventory = new Inventory();
        playerInventory.LoadInventory();
    }

    //cuidado con guardar las cosas en el json durante la partida porque si cojo el colgante, guardo en json, cojo otra cosa y guardo en json, habr� 2 colgantes
    //el enfoque es guardar en el json en alg�n punto que a�n no he pensado (�cu�ndo cambie de pantalla?)
    public void CollectItem(ItemData item)
    {
        playerInventory.AddItem(item);
    }

    public void SaveOnCheckpoint()
    {
        playerInventory.SaveInventory();
        Debug.Log("Inventario guardado en el punto de control.");
    }

    void OnApplicationQuit()
    {
        playerInventory.SaveInventory();
    }
}
