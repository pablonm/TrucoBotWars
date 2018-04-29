using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tests re chantas para ver que todo este andando piola. TODO: Aprender unit testing de verdad.

public class PlayerTest : MonoBehaviour {
    
	void Start () {
        _testearCalculoEnvido();
        _testearVerificacionCarta();
        _testearVerificacionJuegada();
    }

    private void _testearCalculoEnvido() {
        List<bool> asserts = new List<bool>();
        Carta[] cartas;
        Player player = new Player("test", null, null, null);

        // 33
        cartas = new Carta[3] { new Carta("espada", 6), new Carta("espada", 7), new Carta("basto", 4), };
        player.iniciarMano(new MensajeIniciarMano(cartas, true));
        asserts.Add((player.calcularTantosEnvido() == 33));

        // 20
        cartas = new Carta[3] { new Carta("espada", 10), new Carta("espada", 11), new Carta("basto", 4), };
        player.iniciarMano(new MensajeIniciarMano(cartas, true));
        asserts.Add((player.calcularTantosEnvido() == 20));

        // 7
        cartas = new Carta[3] { new Carta("espada", 7), new Carta("basto", 6), new Carta("copa", 5), };
        player.iniciarMano(new MensajeIniciarMano(cartas, true));
        asserts.Add((player.calcularTantosEnvido() == 7));

        // 25 con 3 cartas del mismo palo
        cartas = new Carta[3] { new Carta("espada", 1), new Carta("espada", 2), new Carta("espada", 3), };
        player.iniciarMano(new MensajeIniciarMano(cartas, true));
        asserts.Add((player.calcularTantosEnvido() == 25));

        // 0 con 3 sotas de distinto palo
        cartas = new Carta[3] { new Carta("espada", 10), new Carta("basto", 10), new Carta("oro", 10), };
        player.iniciarMano(new MensajeIniciarMano(cartas, true));
        asserts.Add((player.calcularTantosEnvido() == 0));

        _pasoTest("Player->calcularTantosEnvido", asserts);
    }

    private void _testearVerificacionCarta() {
        List<bool> asserts = new List<bool>();
        Carta[] cartas;
        Player player = new Player("test", null, null, null);

        cartas = new Carta[3] { new Carta("espada", 1), new Carta("espada", 2), new Carta("espada", 3), };
        player.iniciarMano(new MensajeIniciarMano(cartas, true));

        // Tiene la carta (primera posición)
        asserts.Add(player.verificarCarta(new Carta("espada", 1)));

        // Tiene la carta (segunda posición)
        asserts.Add(player.verificarCarta(new Carta("espada", 2)));

        // Tiene la carta (última posición)
        asserts.Add(player.verificarCarta(new Carta("espada", 3)));

        // No tiene la carta
        asserts.Add(!player.verificarCarta(new Carta("espada", 4)));

        _pasoTest("Player->verificarCarta", asserts);
    }

    private void _testearVerificacionJuegada() {
        List<bool> asserts = new List<bool>();
        MensajePedirJugada mensaje;
        Player player = new Player("test", null, null, null);
        
        // Puede jugar truco
        mensaje = new MensajePedirJugada();
        mensaje.jugadaAnteriorOponente = null;
        mensaje.cartasEnMesa = null;
        mensaje.cartasEnMesaOponente = null;
        mensaje.jugadasDisponibles = new Jugada[2] { new Jugada("truco", null), new Jugada("envido", null) };
        player.pedirJugada(mensaje);
        asserts.Add(player.verificarJugada(new Jugada("truco", null)));

        // Puede jugar envido
        mensaje = new MensajePedirJugada();
        mensaje.jugadaAnteriorOponente = null;
        mensaje.cartasEnMesa = null;
        mensaje.cartasEnMesaOponente = null;
        mensaje.jugadasDisponibles = new Jugada[2] { new Jugada("truco", null), new Jugada("envido", null) };
        player.pedirJugada(mensaje);
        asserts.Add(player.verificarJugada(new Jugada("envido", null)));

        // No puede jugar carta
        mensaje = new MensajePedirJugada();
        mensaje.jugadaAnteriorOponente = null;
        mensaje.cartasEnMesa = null;
        mensaje.cartasEnMesaOponente = null;
        mensaje.jugadasDisponibles = new Jugada[2] { new Jugada("truco", null), new Jugada("envido", null) };
        player.pedirJugada(mensaje);
        asserts.Add(!player.verificarJugada(new Jugada("carta", null)));

        // No puede real envido
        mensaje = new MensajePedirJugada();
        mensaje.jugadaAnteriorOponente = null;
        mensaje.cartasEnMesa = null;
        mensaje.cartasEnMesaOponente = null;
        mensaje.jugadasDisponibles = new Jugada[2] { new Jugada("truco", null), new Jugada("envido", null) };
        player.pedirJugada(mensaje);
        asserts.Add(!player.verificarJugada(new Jugada("real envido", null)));

        _pasoTest("Player->verificarJugada", asserts);
    }

    private void _pasoTest(string nombreTest, List<bool> asserts) {
        bool paso = true;
        for (var i = 0; i < asserts.Count; i++)
            paso = paso && asserts[i];
        if (paso)
            Debug.Log("Test [ " + nombreTest + " ] -> OK");
        else
            Debug.LogError("Test [ " + nombreTest + " ] -> FALLO");
    }
}
