using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {

    // Cada player conoce el juego de truco en el que está participando
    private Truco _truco;

    // Cada player tiene una instancia de BotConnection para comunicarse con su bot
    private BotConnection _bot;

    // Cada player tiene una instancia de interfaz
    private InterfazPlayer _interfaz;

    // Variables del player
    public string nombre;
    private List<Carta> _cartas;
    private List<Carta> _cartasEnMesa;
    private int _puntaje;
    private int _partidasGanadas;

    // Variables de la mano
    private bool _esMano;
    private int _tantosEnvido;
    private bool _tieneQuieroTruco;
    private bool _tieneTurno;
    private bool _jugoTurno;
    private bool _tienePalabra;

    private Jugada[] _jugadasDisponibles;

    public Player(string n, Truco t, BotConnection b, InterfazPlayer i) {
        nombre = n;
        _truco = t;
        _bot = b;
        if (_bot != null)
            _bot.setPlayer(this);
        _interfaz = i;
        if (_interfaz != null)
            _interfaz.asignarNombre(n);
        _cartas = new List<Carta>();
        _cartasEnMesa = new List<Carta>();
        _puntaje = 0;
        _partidasGanadas = 0;
    }

    public void inicarPartida() {
        _puntaje = 0;
    }

    public void iniciarMano(MensajeIniciarMano mensaje) {
        _cartas = new List<Carta>();
        _cartas.Add(mensaje.cartas[0]);
        _cartas.Add(mensaje.cartas[1]);
        _cartas.Add(mensaje.cartas[2]);
        _esMano = mensaje.esMano;
        _cartasEnMesa = new List<Carta>();
        _tantosEnvido = calcularTantosEnvido();
        _tieneQuieroTruco = false;
        _tieneTurno = mensaje.esMano;
        _jugoTurno = false;
        _tienePalabra = mensaje.esMano;
        if (_interfaz != null)
            _interfaz.iniciarMano(mensaje.cartas);
        if (_bot != null)
            _bot.iniciarMano(mensaje);
    }

    public int calcularTantosEnvido() {
        int[] tantos = { 0, 0, 0 };
        tantos[0] = _tantosEntreDosCartas(_cartas[0], _cartas[1]);
        tantos[1] = _tantosEntreDosCartas(_cartas[0], _cartas[2]);
        tantos[2] = _tantosEntreDosCartas(_cartas[1], _cartas[2]);
        return Mathf.Max(tantos);
    }

    private int _tantosEntreDosCartas(Carta c1, Carta c2) {
        int v1 = c1.valorEnvido();
        int v2 = c2.valorEnvido();
        if (c1.palo == c2.palo)
            return 20 + v1 + v2;
        return Mathf.Max(v1, v2);
    }

    public void pedirJugada(MensajePedirJugada mensaje) {
        _jugadasDisponibles = mensaje.jugadasDisponibles;
        if (_bot != null)
            _bot.pedirJugada(mensaje);
    }

    public void realizarJugada(Jugada jugada) {

        if (verificarJugada(jugada)) {

            // Si la jugada es una carta, la muevo a la mesa
            if (jugada.mensaje == "carta") {
                int index = -1;
                for (var i = 0; i < _cartas.Count; i++) {
                    if (_cartas[i].palo == jugada.carta.palo && _cartas[i].numero == jugada.carta.numero)
                        index = i;
                }
                _cartasEnMesa.Add(jugada.carta);
                if (index > -1)
                    _cartas.RemoveAt(index);
            }

            if (_interfaz != null)
            {
                _interfaz.realizarJugada(jugada);
                if (jugada.mensaje == "carta")
                {
                    _interfaz.agregarAlHistorial(string.Format("{0} {1}", jugada.carta.palo, jugada.carta.numero));
                }
                else
                {
                    _interfaz.agregarAlHistorial(jugada.mensaje);
                }
            }
            if (_truco != null)
                _truco.realizarJugada(jugada);
        } else {
            string[] mensajesDisponibles = new string[_jugadasDisponibles.Length];
            for (int i = 0; i < _jugadasDisponibles.Length; i++)
            {
                mensajesDisponibles[i] = _jugadasDisponibles[i].mensaje;
            }
            string joinMensajes = string.Join(" ", mensajesDisponibles);
            // TODO: Hacer algo si la jugada no es válida (se intentó hacer trampa o el bot manqueó).
            if (jugada.mensaje == "carta")
                _interfaz.agregarAlHistorial(string.Format("{0}: Jugada invalida: {1} {2}; las jugadas disponibles eran: {3}", nombre, jugada.carta.palo, jugada.carta.numero, joinMensajes));
            else
                _interfaz.agregarAlHistorial(string.Format("{0} Jugada invalida: {1}; las jugadas disponibles eran: {2}", nombre, jugada.mensaje, joinMensajes));
        }
    }

    public bool verificarJugada(Jugada jugada) {
        bool ret = false;
        for (var i = 0; i < _jugadasDisponibles.Length; i++) {
            if (_jugadasDisponibles[i].mensaje == jugada.mensaje) {
                ret = true;
                break;
            }
        }
        if (jugada.mensaje == "carta")
            ret = ret && verificarCarta(jugada.carta);
        return ret;
    }

    public bool verificarCarta(Carta carta) {
        bool ret = false;
        for (var i = 0; i < _cartas.Count; i++) {
            if (_cartas[i].palo == carta.palo && _cartas[i].numero == carta.numero) {
                ret = true;
                break;
            }
        }
        return ret;
    }

    public void terminarMano(Player other) {
        MensajeResultadoMano mensaje = new MensajeResultadoMano(_puntaje, other.getPuntos());
        if (_bot != null)
            _bot.terminarMano(mensaje);
    }

    public Carta getUltimaCartaJugada() {
        if (_cartasEnMesa.Count > 0)
            return _cartasEnMesa[_cartasEnMesa.Count - 1];
        return null;
    }

    public void sumarPuntos(int p) {
        _puntaje += p;
        if (_interfaz != null)
            _interfaz.sumarPuntos(p);
    }

    public void reseterPuntaje() {
        _puntaje = 0;
        if (_interfaz != null)
            _interfaz.resetearPuntos();
    }

    public void ganoPartida() {
        _partidasGanadas++;
        if (_interfaz != null)
            _interfaz.ganoPartida();

        MensajeResultadoPartida mensaje = new MensajeResultadoPartida(true);
        if (_bot != null)
            _bot.terminarPartida(mensaje);
    }

    public void perdioPartida() {
        MensajeResultadoPartida mensaje = new MensajeResultadoPartida(false);
        if (_bot != null)
            _bot.terminarPartida(mensaje);
    }

    public void resultadoEnvido(MensajeResultadoEnvido mensaje) {
        if (_bot != null)
            _bot.resultadoEnvido(mensaje);
    }

    //  -------------------
    // | Getters y setters |
    //  -------------------

    public void setTieneTurno(bool b) {
        _tieneTurno = b;
        _tienePalabra = b;
        _jugoTurno = false;
    }

    public void setTienePalabra(bool b) {
        _tienePalabra = b;
    }

    public bool tieneTurno() {
        return _tieneTurno;
    }

    public bool tienePalabra() {
        return _tienePalabra;
    }

    public void setTieneQuieroTruco(bool b) {
        _tieneQuieroTruco = b;
    }

    public bool tieneQuieroTruco() {
        return _tieneQuieroTruco;
    }

    public bool esMano() {
        return _esMano;
    }

    public void setEsMano(bool mano) {
        _esMano = mano;
        setTieneTurno(mano);
    }

    public void setJugoTurno(bool b) {
        _jugoTurno = b;
    }

    public bool jugoTurno() {
        return _jugoTurno;
    }

    public Carta[] getCartas() {
        return _cartas.ToArray();
    }

    public Carta[] getCartasMesa() {
        return _cartasEnMesa.ToArray();
    }

    public int getCantCartasMesa() {
        return _cartasEnMesa.Count;
    }

    public int getTantosEnvido() {
        return _tantosEnvido;
    }

    public int getPuntos() {
        return _puntaje;
    }

    public int getPartidasGanadas() {
        return _partidasGanadas;
    }
    
}
