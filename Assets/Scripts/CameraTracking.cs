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
        // Calcula la nueva posición de la cámara basada en el jugador
        //float targetX = player.position.x + offset.x;
        //float targetY = player.position.y + offset.y;

        // Limita la posición de la cámara dentro de los límites del nivel
        //float clampedX = Mathf.Clamp(targetX, minX, maxX);
        //float clampedY = Mathf.Clamp(targetY, minY, maxY);

        // Actualiza la posición de la cámara, manteniendo el offset en Z
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
    }
}
