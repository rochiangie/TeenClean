using NUnit.Framework;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

public class Tests
{
    private GameObject jugadorGO;
    private InteraccionJugador interaccionJugador;

    [SetUp]
    public void SetUp()
    {
        jugadorGO = new GameObject("Jugador");
        jugadorGO.tag = "Player";
        interaccionJugador = jugadorGO.AddComponent<InteraccionJugador>();

        // Inicializamos campos necesarios
        interaccionJugador.teclaInteraccion = KeyCode.E;
        interaccionJugador.rango = 1.5f;
        interaccionJugador.capaInteractuable = LayerMask.GetMask("Interactuable");

        // Punto de carga simulado
        var puntoCarga = new GameObject("PuntoCarga").transform;
        interaccionJugador.puntoDeCarga = puntoCarga;

        // Lista de tags válidos para recoger
        typeof(InteraccionJugador)
            .GetField("tagsRecogibles", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(interaccionJugador, new string[] { "Platos", "Ropa", "Tarea" });
    }

    [Test]
    public void Jugador_RecogeObjetoConTagValido()
    {
        // Arrange
        var objeto = new GameObject("Plato");
        objeto.tag = "Platos";
        objeto.transform.position = jugadorGO.transform.position;

        objeto.AddComponent<BoxCollider2D>();

        // Simular que el jugador no lleva nada
        var campo = typeof(InteraccionJugador).GetField("objetoTransportado", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        campo.SetValue(interaccionJugador, null);

        // Act - Simulamos que está en rango (no usamos input real en tests unitarios)
        bool esRecogible = interaccionJugador.EsTagRecogible(objeto.tag);

        // Assert
        Assert.IsTrue(esRecogible);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(jugadorGO);
    }
}
