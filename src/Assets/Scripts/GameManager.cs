using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public InterfazPlayer interfazPlayer1;
    public InterfazPlayer interfazPlayer2;

    public Resultado resultado;
    
    public Slider sliderVelocidad;

    private BotConnection _bot1;
    private BotConnection _bot2;

    private string _nombrePlayer1;
    private string _nombrePlayer2;

    private int _botsListos = 0;
    private int _partidas;
    
    public void botListo() {
        _botsListos++;
        if (_botsListos == 2) 
            UnityMainThreadDispatcher.Instance().Enqueue(_empezarJuego());
    }

    // Cuando los dos bots están conectados comienzo el juego instanciando un Truco.
    private IEnumerator _empezarJuego() {
        Truco truco = new Truco(_nombrePlayer1, _bot1, interfazPlayer1, _nombrePlayer2, _bot2, interfazPlayer2, _partidas, resultado);
        yield break;
    }

    public void empezar(string nombre1, string ip1, string puerto1, string nombre2, string ip2, string puerto2, int p) {
        _partidas = p;
        _nombrePlayer1 = nombre1;
        _nombrePlayer2 = nombre2;
        _bot1 = new BotConnection(ip1, puerto1, botListo, sliderVelocidad.value);
        _bot2 = new BotConnection(ip2, puerto2, botListo, sliderVelocidad.value);
    }

    void OnApplicationQuit() {
        if (_bot1 != null)
            _bot1.stop();
        if (_bot2 != null)
            _bot2.stop();
    }

    public void actualizarVelocidad() {
        _bot1.setVelocidad(sliderVelocidad.value);
        _bot2.setVelocidad(sliderVelocidad.value);
    }
}
