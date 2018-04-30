using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InterfazPlayer : MonoBehaviour {
    
    public GameObject puntosPartida;
    public GameObject partidasGanadas;
    public Text tituloPuntos;
    public Text historial;
    

    // En vez de setear en null el sprite de una carta vacía, le asigno esto
    public Sprite nullcard;

    // Sprites de las cartas
    public List<Sprite> spritesCartasOro;
    public List<Sprite> spritesCartasEspada;
    public List<Sprite> spritesCartasBasto;
    public List<Sprite> spritesCartasCopa;
    private Dictionary<string, Sprite> _spritesCartas;

    // Nombre del jugador
    public Text _nombreJugador;

    // Cartas en la mano
    private List<Image> _cartasMano;

    // Cartas en mesa
    private List<Image> _cartasMesa;

    // Panel de cantos
    private Text _canto;

    // Fosforos para los puntos
    private List<Image> _fosforos;
    private int _fosforosMostrados;

    void Start() {

        // Inicializo el diccionario de sprites de cartas
        _spritesCartas = new Dictionary<string, Sprite>();
        for (var i = 0; i < spritesCartasOro.Count; i++)
            _spritesCartas.Add("oro" + (i + 1).ToString(), spritesCartasOro[i]);
        for (var i = 0; i < spritesCartasEspada.Count; i++)
            _spritesCartas.Add("espada" + (i + 1).ToString(), spritesCartasEspada[i]);
        for (var i = 0; i < spritesCartasBasto.Count; i++)
            _spritesCartas.Add("basto" + (i + 1).ToString(), spritesCartasBasto[i]);
        for (var i = 0; i < spritesCartasCopa.Count; i++)
            _spritesCartas.Add("copa" + (i + 1).ToString(), spritesCartasCopa[i]);

        // Busco el Text para el nombre del player
        _nombreJugador = transform.Find("Nombre").GetComponent<Text>();

        // Busco los componentes Image de las cartas en la mano
        _cartasMano = new List<Image>();
        Image[] imagesMano = transform.Find("Mano").GetComponentsInChildren<Image>();
        for (var i = 0; i < imagesMano.Length; i++)
            _cartasMano.Add(imagesMano[i]);

        // Busco los componentes Image de las cartas en la mesa
        _cartasMesa = new List<Image>();
        Image[] imagesMesa = transform.Find("Mesa").GetComponentsInChildren<Image>();
        for (var i = 0; i < imagesMesa.Length; i++)
            _cartasMesa.Add(imagesMesa[i]);

        // Busco el Text de los cantos
        _canto = transform.Find("Canto").GetComponent<Text>();

        // Busco todos los fosforos y los guardo en la lista
        _fosforosMostrados = 0;
        _fosforos = new List<Image>();
        Image[] imagenesFosforos = puntosPartida.transform.GetComponentsInChildren<Image>();
        for (var i = 0; i < imagenesFosforos.Length; i++)
            _fosforos.Add(imagenesFosforos[i]);

    }

    public void asignarNombre(string n) {
        tituloPuntos.text = n;
        _nombreJugador.text = n;
    }

    public void iniciarMano(Carta[] cartas) {
        // Limpiar la mesa
        for (var i = 0; i < _cartasMesa.Count; i++)
            _cartasMesa[i].sprite = nullcard;

        // Limpiar los cantos
        _canto.text = null;

        // Asignar cartas al jugador
        for (var i = 0; i < cartas.Length; i++) {
            _cartasMano[i].sprite = _spritesCartas[cartas[i].palo + cartas[i].numero.ToString()];
            _cartasMano[i].gameObject.name = cartas[i].palo + cartas[i].numero.ToString();
        }
            
    }

    public void realizarJugada(Jugada jugada) {
        if (jugada.mensaje == "carta") {

            // Elimiar carta de la mano
            for (var i = 0; i < _cartasMano.Count; i++) {
                if (_cartasMano[i].gameObject.name == jugada.carta.palo + jugada.carta.numero.ToString()) {
                    _cartasMano[i].sprite = nullcard;
                    _cartasMano[i].gameObject.name = "vacia";
                }
            }

            // Instanciar carta en la mesa
            for (var i = 0; i < _cartasMesa.Count; i++) {
                if (_cartasMesa[i].sprite == nullcard) {
                    _cartasMesa[i].sprite = _spritesCartas[jugada.carta.palo + jugada.carta.numero.ToString()];
                    break;
                }
            }
            
            // Eliminar canto resuelto
            _canto.text = "";

        } else {
            _canto.text = "¡" + jugada.mensaje + "!";
        }

    }

    public void sumarPuntos(int p) {
        for (var i = _fosforosMostrados; (i < (_fosforosMostrados + p) && i < _fosforos.Count); i++)
            _fosforos[i].enabled = true;
        _fosforosMostrados += p;
    }

    public void resetearPuntos() {
        _fosforosMostrados = 0;
        for (var i = 0; i < _fosforos.Count; i++)
            _fosforos[i].enabled = false;
    }

    public void ganoPartida() {
        Text texto = partidasGanadas.GetComponent<Text>();
        texto.text = ((int.Parse(texto.text)) + 1).ToString();
    }

    public void agregarAlHistorial(string mensaje)
    {
        string[] lines = historial.text.Split('\n');
        for (int i = 0; i < lines.Length - 1; i++)
        {
            lines[i] = lines[i + 1];
        }
        if (lines.Length > 15)
        {
            lines[lines.Length - 1] = string.Format("> {0}: {1}",_nombreJugador.text, mensaje);
            historial.text = String.Join("\n", lines);
        }
        else
        {
            historial.text = String.Join("\n", lines) + string.Format("> {0}: {1}\n",_nombreJugador.text, mensaje);
        }
    }
}
