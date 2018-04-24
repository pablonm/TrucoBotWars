using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class BotConnection {

    private Player player;
    private WebSocket socket;
    private bool esperandoJugada;
    private float _velocidad;

    public BotConnection(string ip, string puerto, System.Action callback, float vel) {
        esperandoJugada = false;
        _velocidad = vel;
        socket = new WebSocket("ws://"+ ip +":" + puerto);

        socket.OnMessage += (sender, e) => {
            if (esperandoJugada)
                UnityMainThreadDispatcher.Instance().Enqueue(realizarJugada(JsonUtility.FromJson<Jugada>(e.Data)));
        };

        socket.OnOpen += (sender, e) => {
            callback();
        };

        socket.Connect();
        
    }

    public void stop() {
        socket.Close();
    }

    public void setPlayer(Player p) {
        player = p;
    }

    public void iniciarMano(MensajeIniciarMano mensaje) {
        socket.Send(JsonUtility.ToJson(mensaje));
    }

    public void pedirJugada(MensajePedirJugada mensaje) {
        esperandoJugada = true;
        socket.Send(JsonUtility.ToJson(mensaje));
    }

    private IEnumerator realizarJugada(Jugada jugada) {
        yield return new WaitForSeconds(_velocidad);
        esperandoJugada = false;
        player.realizarJugada(jugada);
        yield break;
    }

    public void terminarMano(MensajeResultadoMano mensaje) {
        socket.Send(JsonUtility.ToJson(mensaje));
    }

    public void terminarPartida(MensajeResultadoPartida mensaje) {
        socket.Send(JsonUtility.ToJson(mensaje));
    }

    public void resultadoEnvido(MensajeResultadoEnvido mensaje) {
        socket.Send(JsonUtility.ToJson(mensaje));
    }

    public void setVelocidad(float vel) {
        _velocidad = vel;
    }
}
