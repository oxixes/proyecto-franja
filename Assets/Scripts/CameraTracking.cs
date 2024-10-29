using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    // Limites del nivel
    //public float minX;
    //public float maxX;
    //public float minY;
    //public float maxY;

    // Update is called once per frame
    void Update()
    {
        // Calcula la nueva posici�n de la c�mara basada en el jugador
        //float targetX = player.position.x + offset.x;
        //float targetY = player.position.y + offset.y;

        // Limita la posici�n de la c�mara dentro de los l�mites del nivel
        //float clampedX = Mathf.Clamp(targetX, minX, maxX);
        //float clampedY = Mathf.Clamp(targetY, minY, maxY);

        // Actualiza la posici�n de la c�mara, manteniendo el offset en Z
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
    }
}
