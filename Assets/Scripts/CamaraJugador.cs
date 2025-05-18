using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraJugador : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    public Transform objetivo;

    [Header("Offset")]
    public Vector3 offset = new Vector3(2f, 1f, -10f);

    [Header("Movimiento suave")]
    public float suavizado = 5f;

    private void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 posicionDeseada = objetivo.position + offset;
        Vector3 posicionSuavizada = Vector3.Lerp(transform.position, posicionDeseada, suavizado * Time.deltaTime);

        transform.position = posicionSuavizada;
    }
}
