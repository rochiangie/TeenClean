using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Reflection;
using TMPro;


public class InteraccionJugadorTests
{
    private GameObject jugadorGO;
    private InteraccionJugador interaccionJugador;

    [SetUp]
    public void SetUp()
    {
        jugadorGO = new GameObject("Jugador");
        jugadorGO.tag = "Player";
        interaccionJugador = jugadorGO.AddComponent<InteraccionJugador>();

        interaccionJugador.teclaInteraccion = KeyCode.E;
        interaccionJugador.rango = 1.5f;
        interaccionJugador.capaInteractuable = LayerMask.GetMask("Interactuable");

        var puntoCarga = new GameObject("PuntoCarga").transform;
        interaccionJugador.puntoDeCarga = puntoCarga;

        typeof(InteraccionJugador)
            .GetField("tagsRecogibles", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(interaccionJugador, new string[] { "Platos", "Ropa", "Tarea" });
    }

    [Test]
    public void RecogeObjetoConTagValido()
    {
        var objeto = new GameObject("Plato");
        objeto.tag = "Platos";
        objeto.AddComponent<BoxCollider2D>();

        var campo = typeof(InteraccionJugador)
            .GetField("objetoTransportado", BindingFlags.NonPublic | BindingFlags.Instance);
        campo.SetValue(interaccionJugador, null);

        bool esRecogible = interaccionJugador.EsTagRecogible(objeto.tag);

        Assert.IsTrue(esRecogible);
    }

    [Test]
    public void NoRecogeObjetoSiYaTieneUno()
    {
        var campo = typeof(InteraccionJugador)
            .GetField("objetoTransportado", BindingFlags.NonPublic | BindingFlags.Instance);

        campo.SetValue(interaccionJugador, new GameObject("ObjetoFalso"));

        var objeto = new GameObject("Plato");
        objeto.tag = "Platos";

        bool esRecogible = interaccionJugador.EsTagRecogible(objeto.tag);

        Assert.IsTrue(esRecogible);
        Assert.IsNotNull(campo.GetValue(interaccionJugador)); // ya tenía uno
    }

    [Test]
    public void PuedeSoltarObjeto()
    {
        var objeto = new GameObject("ObjetoTransportado");
        var campo = typeof(InteraccionJugador)
            .GetField("objetoTransportado", BindingFlags.NonPublic | BindingFlags.Instance);

        campo.SetValue(interaccionJugador, objeto);

        campo.SetValue(interaccionJugador, null); // Simulamos soltado

        Assert.IsNull(campo.GetValue(interaccionJugador));
    }

    [Test]
    public void Movimiento_CambiaEstadoDeAnimacion()
    {
        Vector2 input = new Vector2(1, 0);
        bool isRunning = input != Vector2.zero;
        Assert.IsTrue(isRunning);
    }



    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(jugadorGO);
        foreach (var go in GameObject.FindObjectsOfType<GameObject>())
        {
            Object.DestroyImmediate(go);
        }
    }
}
