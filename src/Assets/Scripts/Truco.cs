using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truco {

    private Player _player1;
    private Player _player2;
    
    // La mano que se está jugando actualmente
    private Mano _manoActiva;

    // La cantidad de partidas que se van a jugar
    private int _partidas;

    // La interfaz de resultado
    private Resultado _resultado;
    
    public Truco(string p1n, BotConnection p1b, InterfazPlayer p1i, string p2n, BotConnection p2b, InterfazPlayer p2i, int p, Resultado r) {
        _partidas = p;
        _resultado = r;
        _player1 = new Player(p1n, this, p1b, p1i);
        _player2 = new Player(p2n, this, p2b, p2i);
        iniciarPartida();
    }

    private void iniciarPartida() {
        _partidas--;

        // Si todavía quedan partidas por ganar, o se terminaron y están empatados, se sigue jugando.
        if (_partidas >= 0 || (_partidas < 0 && _player1.getPartidasGanadas() == _player2.getPartidasGanadas())) {

            // Se resetean los puntajes de los jugadores
            _player1.reseterPuntaje();
            _player2.reseterPuntaje();

            // Se decide quien es mano antes de iniciar la primer mano
            int jugadorMano = Random.Range(1, 2);
            _player1.setEsMano((jugadorMano == 1));
            _player2.setEsMano((jugadorMano == 2));

            _iniciarMano();
        } else {
            _mostrarGanador();
        }
    }

    private void _iniciarMano() {

        // Si alguno de los jugadores alcanzó el máximo de puntos inicio una nueva partida
        if (_player1.getPuntos() >= 30) {
            _player1.ganoPartida();
            _player2.perdioPartida();
            iniciarPartida();
        } else {
            if (_player2.getPuntos() >= 30) {
                _player2.ganoPartida();
                _player1.perdioPartida();
                iniciarPartida();
            } else {
                _manoActiva = new Mano(_player1, _player2, _iniciarMano);
            }
        }
    }

    public void realizarJugada(Jugada jugada) {
        _manoActiva.recibirJugada(jugada);
    }

    private void _mostrarGanador() {
        if (_player1.getPartidasGanadas() > _player2.getPartidasGanadas()) {
            _resultado.setGanador(_player1.nombre, _player1.getPartidasGanadas());
            _resultado.setPerdedor(_player2.nombre, _player2.getPartidasGanadas());
        } else {
            _resultado.setGanador(_player2.nombre, _player2.getPartidasGanadas());
            _resultado.setPerdedor(_player1.nombre, _player1.getPartidasGanadas());
        }
        _resultado.mostrar();
    }
    
}


