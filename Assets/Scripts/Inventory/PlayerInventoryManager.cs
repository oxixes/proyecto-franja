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

    //cuidado con guardar las cosas en el json durante la partida porque si cojo el colgante, guardo en json, cojo otra cosa y guardo en json, habrá 2 colgantes
    //el enfoque es guardar en el json en algún punto que aún no he pensado (¿cuándo cambie de pantalla?)
    public void CollectItem(ItemData item)
    {
        Debug.Log("Calling fun of Player Inventory");
        playerInventory.AddItem(item);
    }

    // Nueva función para recoger InformationData
    public void CollectInformation(InformationData info)
    {
            playerInventory.AddInformation(info);
            Debug.Log($"Información '{info.infoName}' ha sido llevada a Inventory");
        
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
