using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mano {

    private enum ESTADO { NEGOCIANDO_ENVIDO, NEGOCIANDO_TRUCO, JUGANDO, TERMINADA };
    private enum ESTADOTRUCO { NULO, TRUCO, RETRUCO, VALE4, JUGADO };
    private enum ESTADOENVIDO { NULO, ENVIDO, REAL, FALTA, JUGADO };
    private enum ESTADOCARTAS { NOMATA, MATA, EMPARDA }

    private ESTADO _estado;
    private ESTADOTRUCO _estadoTruco;
    private ESTADOENVIDO _estadoEnvido;
    private ESTADOCARTAS _estadoCartas;

    private System.Action _callback; 

    private int _turnosJugados;

    private Player _player1;
    private Player _player2;

    private int _puntosEnvidoQuerido;
    private int _puntosEnvidoNoQuerido;
    private int _puntosTrucoQuerido;
    private int _puntosTrucoNoQuerido;

    private int _envidosCantados;
    
    public Mano(Player p1, Player p2, System.Action cb) {
        _callback = cb;
        _player1 = p1;
        _player2 = p2;
        _puntosEnvidoQuerido = 0;
        _puntosEnvidoNoQuerido = 0;
        _puntosTrucoQuerido = 1;
        _puntosTrucoNoQuerido = 0;
        _envidosCantados = 0;
        _estado = ESTADO.JUGANDO;
        _estadoTruco = ESTADOTRUCO.NULO;
        _estadoEnvido = ESTADOENVIDO.NULO;
        _estadoCartas = ESTADOCARTAS.NOMATA;
        _repartirCartas();
        _pedirJugadas(null);
    }

    // El juego reparte las cartas a ambos jugadores
    private void _repartirCartas() {
        Carta[] cartasRepartidas = new Carta[6];
        Carta[] cartasPlayer1 = new Carta[3];
        Carta[] cartasPlayer2 = new Carta[3];
        Carta carta;
        for (var i = 0; i < 6; i++) {
            carta = new Carta();
            carta.random();
            while (carta.estaEnLista(cartasRepartidas)) {
                carta = new Carta();
                carta.random();
            }
            cartasRepartidas[i] = carta;
            if (i < 3)
                cartasPlayer1[i] = carta;
            else
                cartasPlayer2[i - 3] = carta;
        }

        MensajeIniciarMano mensajeP1 = new MensajeIniciarMano(cartasPlayer1, _player1.esMano());
        _player1.iniciarMano(mensajeP1);
        MensajeIniciarMano mensajeP2 = new MensajeIniciarMano(cartasPlayer2, _player2.esMano());
        _player2.iniciarMano(mensajeP2);
    }

    private void _pedirJugadas(Jugada jugadaAnterior) {
        _pedirJugada(_player1, _player2, jugadaAnterior);
        _pedirJugada(_player2, _player1, jugadaAnterior);
    }

    private void _pedirJugada(Player player, Player oponente, Jugada jugadaAnterior) {
        if ((_estado != ESTADO.JUGANDO && player.tienePalabra()) || (_estado == ESTADO.JUGANDO && player.tieneTurno())) {
            MensajePedirJugada mensaje = new MensajePedirJugada();
            mensaje.jugadaAnteriorOponente = jugadaAnterior;
            mensaje.jugadasDisponibles = _calcularJugadasDisponibles(player);
            mensaje.cartasEnMesa = player.getCartasMesa();
            mensaje.cartasEnMesaOponente = oponente.getCartasMesa();
            player.pedirJugada(mensaje);
        }
    }

    private Jugada[] _calcularJugadasDisponibles(Player player) {
        List<Jugada> jugadasDisponibles = new List<Jugada>();

        if ((_estadoEnvido == ESTADOENVIDO.NULO || _estadoEnvido == ESTADOENVIDO.ENVIDO) && _envidosCantados < 2 && _turnosJugados < 2 && _estadoTruco == ESTADOTRUCO.NULO) {
            jugadasDisponibles.Add(new Jugada("canto", "envido", null));
        }

        if ((_estadoEnvido == ESTADOENVIDO.NULO || _estadoEnvido == ESTADOENVIDO.ENVIDO) && _turnosJugados < 2 && _estadoTruco == ESTADOTRUCO.NULO) {
            jugadasDisponibles.Add(new Jugada("canto", "real envido", null));
        }

        if ((_estadoEnvido == ESTADOENVIDO.NULO || _estadoEnvido == ESTADOENVIDO.ENVIDO || _estadoEnvido == ESTADOENVIDO.REAL) && _turnosJugados < 2 && _estadoTruco == ESTADOTRUCO.NULO) {
            jugadasDisponibles.Add(new Jugada("canto", "falta envido", null));
        }

        if (_estadoTruco == ESTADOTRUCO.NULO && _estado != ESTADO.NEGOCIANDO_ENVIDO) {
            jugadasDisponibles.Add(new Jugada("canto", "truco", null));
        }

        if (_estadoTruco == ESTADOTRUCO.TRUCO && player.tieneQuieroTruco()) {
            jugadasDisponibles.Add(new Jugada("canto", "retruco", null));
        }

        if (_estadoTruco == ESTADOTRUCO.RETRUCO && player.tieneQuieroTruco()) {
            jugadasDisponibles.Add(new Jugada("canto", "vale 4", null));
        }

        if (_estado == ESTADO.JUGANDO && player.getCartas().Length > 0) {
            jugadasDisponibles.Add(new Jugada("carta", "carta", null));
        }

        if (_estado != ESTADO.JUGANDO) {
            jugadasDisponibles.Add(new Jugada("decision", "quiero", null));
            jugadasDisponibles.Add(new Jugada("decision", "no quiero", null));
        }

        return jugadasDisponibles.ToArray();
    }

    public void recibirJugada(Jugada jugada) {
        if (jugada.mensaje == "envido") {
            _estado = ESTADO.NEGOCIANDO_ENVIDO;
            _estadoEnvido = ESTADOENVIDO.ENVIDO;
            _estadoTruco = ESTADOTRUCO.NULO;
            _envidosCantados++;
            _puntosEnvidoQuerido = 2 * _envidosCantados;
            _puntosEnvidoNoQuerido = _envidosCantados;
        }

        if (jugada.mensaje == "real envido") {
            _estado = ESTADO.NEGOCIANDO_ENVIDO;
            _estadoEnvido = ESTADOENVIDO.REAL;
            _estadoTruco = ESTADOTRUCO.NULO;
            _puntosEnvidoQuerido = 3 + (2 * _envidosCantados);
            _puntosEnvidoNoQuerido = 2 + _envidosCantados;
        }

        if (jugada.mensaje == "falta envido") {
            _estado = ESTADO.NEGOCIANDO_ENVIDO;
            _estadoEnvido = ESTADOENVIDO.FALTA;
            _estadoTruco = ESTADOTRUCO.NULO;
            _puntosEnvidoQuerido = _calcularTantosFaltaEnvido();
            _puntosEnvidoNoQuerido = 1 + ((_estadoEnvido == ESTADOENVIDO.REAL) ? 3 : 0) + _envidosCantados;
        }

        if (jugada.mensaje == "truco") {
            _estado = ESTADO.NEGOCIANDO_TRUCO;
            _estadoTruco = ESTADOTRUCO.TRUCO;
            _puntosTrucoQuerido = 2;
            _puntosTrucoNoQuerido = 1;
            _player2.setTieneQuieroTruco(_player1.tienePalabra());
            _player1.setTieneQuieroTruco(!_player1.tienePalabra());
        }

        if (jugada.mensaje == "retruco") {
            _estado = ESTADO.NEGOCIANDO_TRUCO;
            _estadoTruco = ESTADOTRUCO.RETRUCO;
            _puntosTrucoQuerido = 3;
            _puntosTrucoNoQuerido = 2;
            _player2.setTieneQuieroTruco(_player1.tienePalabra());
            _player1.setTieneQuieroTruco(!_player1.tienePalabra());
        }

        if (jugada.mensaje == "vale 4") {
            _estado = ESTADO.NEGOCIANDO_TRUCO;
            _estadoTruco = ESTADOTRUCO.VALE4;
            _puntosTrucoQuerido = 4;
            _puntosTrucoNoQuerido = 3;
            _player2.setTieneQuieroTruco(_player1.tienePalabra());
            _player1.setTieneQuieroTruco(!_player1.tienePalabra());
        }

        if (jugada.mensaje == "quiero") {
            if (_estado == ESTADO.NEGOCIANDO_ENVIDO) {
                _estadoEnvido = ESTADOENVIDO.JUGADO;
                _calcularEnvido();
            }
            if (_player1.getPuntos() >= 30 || _player2.getPuntos() >= 30)
                _estado = ESTADO.TERMINADA;
            else
                _estado = ESTADO.JUGANDO;
        }

        if (jugada.mensaje == "no quiero") {
            if (_estado == ESTADO.NEGOCIANDO_TRUCO) {
                _estado = ESTADO.TERMINADA;
                if (_player1.tienePalabra())
                    _player2.sumarPuntos(_puntosTrucoNoQuerido);
                else
                    _player1.sumarPuntos(_puntosTrucoNoQuerido);
            } else {
                // Si se está engociando envido
                _estadoEnvido = ESTADOENVIDO.JUGADO;
                _estado = ESTADO.JUGANDO;
                if (_player1.tienePalabra())
                    _player2.sumarPuntos(_puntosEnvidoNoQuerido);
                else
                    _player1.sumarPuntos(_puntosEnvidoNoQuerido);
            }
        }

        if (jugada.mensaje == "carta") {
            _player1.setJugoTurno(_player1.tieneTurno());
            _player2.setJugoTurno(_player2.tieneTurno());

            
            // Verifico si alguno de los jugadores ya ganó la mano
            if (_calcularTurnosGanados(_player1, _player2) >= 2) {
                _estado = ESTADO.TERMINADA;
                _player1.sumarPuntos(_puntosTrucoQuerido);
            } else {
                if (_calcularTurnosGanados(_player2, _player2) >= 2) {
                    _estado = ESTADO.TERMINADA;
                    _player2.sumarPuntos(_puntosTrucoQuerido);
                } else {

                    // Si ambos jugadores ya tiraron las 3 cartas, se termina la mano
                    // Este caso solo se da si empardaron las 3 manos, si es así se lleva los puntos el mano
                    if (_player1.getCantCartasMesa() == 3 && _player2.getCantCartasMesa() == 3) {
                        _estado = ESTADO.TERMINADA;
                        if (_player1.esMano())
                            _player1.sumarPuntos(_puntosTrucoQuerido);
                        else
                            _player2.sumarPuntos(_puntosTrucoQuerido);
                    } else {
                        // Se verifica si la carta tirada mata, no mata o emparda a la anterior del oponente
                        if ((_player1.getUltimaCartaJugada() != null && _player2.getUltimaCartaJugada() != null) && (_player1.getCantCartasMesa() == _player2.getCantCartasMesa())) {
                            if ((_player1.tieneTurno() && _player1.getUltimaCartaJugada().mata(_player2.getUltimaCartaJugada()) == 1) || (_player2.tieneTurno() && _player2.getUltimaCartaJugada().mata(_player1.getUltimaCartaJugada()) == 1)) {
                                _estadoCartas = ESTADOCARTAS.MATA;
                            } else {
                                if ((_player1.tieneTurno() && _player1.getUltimaCartaJugada().mata(_player2.getUltimaCartaJugada()) == -1) || (_player2.tieneTurno() && _player2.getUltimaCartaJugada().mata(_player1.getUltimaCartaJugada()) == -1)) {
                                    _estadoCartas = ESTADOCARTAS.NOMATA;
                                } else {
                                    _estadoCartas = ESTADOCARTAS.EMPARDA;
                                }
                            }
                        } else {
                            _estadoCartas = ESTADOCARTAS.NOMATA;
                        }
                    }

                }
            }
            
        }

        _siguienteJugada(jugada);

    }

    private void _siguienteJugada(Jugada jugadaAnterior) {
        if (_estado == ESTADO.TERMINADA) {
            _player1.terminarMano(_player2);
            _player2.terminarMano(_player1);
            _player1.setEsMano(!_player1.esMano());
            _player2.setEsMano(!_player2.esMano());
            _callback();
        } else {
            if (_estado == ESTADO.JUGANDO && ((_player1.tieneTurno() && _player1.jugoTurno()) || (_player2.tieneTurno() && _player2.jugoTurno()))) {
                _turnosJugados++;
                // Si la carta no mató se pasa el turno
                if (_estadoCartas == ESTADOCARTAS.NOMATA) {
                    _player1.setTieneTurno(!_player1.tieneTurno());
                    _player2.setTieneTurno(!_player2.tieneTurno());
                } else {
                    // Si la carta mató el jugador conserva su turno
                    if (_estadoCartas == ESTADOCARTAS.MATA) {
                        _player1.setTieneTurno(_player1.tieneTurno());
                        _player2.setTieneTurno(_player2.tieneTurno());
                    } else {
                        // Si la carta empardó, se le da turno al que es mano.
                        _player1.setTieneTurno(_player1.esMano());
                        _player2.setTieneTurno(_player2.esMano());
                    }
                }
            } else {
                _player1.setTienePalabra(!_player1.tienePalabra());
                _player2.setTienePalabra(!_player2.tienePalabra());
            }
            _pedirJugadas(jugadaAnterior);
        }
    }

    private void _calcularEnvido() {
        if (_player1.getTantosEnvido() > _player2.getTantosEnvido()) {
            _player1.sumarPuntos(_puntosEnvidoQuerido);
            _enviarResultadoEnvido(_player1, _player2);
        } else {
            if (_player1.getTantosEnvido() < _player2.getTantosEnvido()) {
                _player2.sumarPuntos(_puntosEnvidoQuerido);
                _enviarResultadoEnvido(_player2, _player1);
            } else {
                if (_player1.esMano()) {
                    _player1.sumarPuntos(_puntosEnvidoQuerido);
                    _enviarResultadoEnvido(_player1, _player2);
                } else {
                    _player2.sumarPuntos(_puntosEnvidoQuerido);
                    _enviarResultadoEnvido(_player2, _player1);
                }
            }
        }
    }

    private void _enviarResultadoEnvido(Player ganador, Player perdedor) {
        if (ganador.esMano()) {
            ganador.resultadoEnvido(new MensajeResultadoEnvido(true, 0));
        } else {
            ganador.resultadoEnvido(new MensajeResultadoEnvido(true, perdedor.getTantosEnvido()));
        }
        perdedor.resultadoEnvido(new MensajeResultadoEnvido(false, ganador.getTantosEnvido()));
    }

    private int _calcularTurnosGanados(Player player, Player oponente) {
        Carta[] cartasMesaPlayer = player.getCartasMesa();
        Carta[] cartasMesaOponente = oponente.getCartasMesa();
        int turnosJugados = Mathf.Min(player.getCantCartasMesa(), oponente.getCantCartasMesa());
        int turnosGanados = 0;
        for (var i = 0; i < turnosJugados; i++) {
            if (cartasMesaPlayer[i].mata(cartasMesaOponente[i]) == 1) {
                turnosGanados++;
            }
        }
        return turnosGanados;
    }

    private int _calcularTantosFaltaEnvido() {
        if (_player1.getPuntos() < 15 && _player2.getPuntos() < 15)
            return 30;
        else
            return (30 - Mathf.Max(_player1.getPuntos(), _player2.getPuntos()));
    }
}
