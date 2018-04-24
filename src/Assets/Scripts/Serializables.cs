using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Carta {
    public string palo;
    public int numero;

    public Carta(string p, int n) {
        this.palo = p;
        this.numero = n;
    }

    public Carta() {
    }

    public Carta random() {
        string[] palos = { "oro", "basto", "espada", "copa" };
        string palo = palos[Random.Range(0, 3)];
        int numero = Random.Range(1, 12);
        Carta carta = new Carta(palo, numero);
        while (!carta.esValida()) {
            palo = palos[Random.Range(0, 3)];
            numero = Random.Range(1, 12);
            carta = new Carta(palo, numero);
        }
        this.palo = carta.palo;
        this.numero = carta.numero;
        return this;
    }

    public bool esValida() {
        return (this.numero != 8 && this.numero != 9 && this.numero >= 1 && this.numero <= 12 && (this.palo == "oro" || this.palo == "basto" || this.palo == "espada" || this.palo == "copa"));
    }

    public bool estaEnLista(Carta[] cartas) {
        for (var i = 0; i < cartas.Length; i++) {
            if (cartas[i] != null && cartas[i].palo == this.palo && cartas[i].numero == this.numero)
                return true;
        }
        return false;
    }

    public int valorEnvido() {
        return (this.numero <= 7 ? this.numero : 0);
    }

    public int mata(Carta carta) {
        Dictionary<string, int> valores = new Dictionary<string, int>();
        valores.Add("oro4", 1);
        valores.Add("basto4", 1);
        valores.Add("espada4", 1);
        valores.Add("copa4", 1);
        valores.Add("oro5", 2);
        valores.Add("basto5", 2);
        valores.Add("espada5", 2);
        valores.Add("copa5", 2);
        valores.Add("oro6", 3);
        valores.Add("basto6", 3);
        valores.Add("espada6", 3);
        valores.Add("copa6", 3);
        valores.Add("basto7", 4);
        valores.Add("copa7", 4);
        valores.Add("oro10", 5);
        valores.Add("basto10", 5);
        valores.Add("espada10", 5);
        valores.Add("copa10", 5);
        valores.Add("oro11", 6);
        valores.Add("basto11", 6);
        valores.Add("espada11", 6);
        valores.Add("copa11", 6);
        valores.Add("oro12", 7);
        valores.Add("basto12", 7);
        valores.Add("espada12", 7);
        valores.Add("copa12", 7);
        valores.Add("oro1", 8);
        valores.Add("copa1", 8);
        valores.Add("oro2", 9);
        valores.Add("basto2", 9);
        valores.Add("espada2", 9);
        valores.Add("copa2", 9);
        valores.Add("oro3", 10);
        valores.Add("basto3", 10);
        valores.Add("espada3", 10);
        valores.Add("copa3", 10);
        valores.Add("oro7", 11);
        valores.Add("espada7", 12);
        valores.Add("basto1", 13);
        valores.Add("espada1", 14);
        if (valores[this.palo + this.numero.ToString()] > valores[carta.palo + carta.numero.ToString()])
            return 1;
        else if (valores[this.palo + this.numero.ToString()] < valores[carta.palo + carta.numero.ToString()])
            return -1;
        else
            return 0;
    }
}

[System.Serializable]
public class Jugada {
    public string mensaje;
    public Carta carta;

    public Jugada(string m, Carta c) {
        this.mensaje = m;
        this.carta = c;
    }

    public Jugada() {
    }
}

[System.Serializable]
public class MensajeIniciarMano {
    public string mensaje = "iniciarMano";
    public Carta[] cartas;
    public bool esMano;

    public MensajeIniciarMano(Carta[] c, bool m) {
        this.cartas = c;
        this.esMano = m;
    }
}

[System.Serializable]
public class MensajePedirJugada {
    public string mensaje = "pedirJugada";
    public Carta[] cartasEnMesa;
    public Carta[] cartasEnMesaOponente;
    public Jugada jugadaAnteriorOponente;
    public Jugada[] jugadasDisponibles;
}

[System.Serializable]
public class MensajeResultadoMano {
    public string mensaje = "resultadoMano";
    public int puntos;
    public int puntosOponente;

    public MensajeResultadoMano(int p, int po) {
        this.puntos = p;
        this.puntosOponente = po;
    }
}

[System.Serializable]
public class MensajeResultadoPartida {
    public string mensaje = "resultadoPartida";
    public bool ganada;

    public MensajeResultadoPartida(bool g) {
        this.ganada = g;
    }
}

[System.Serializable]
public class MensajeResultadoEnvido {
    public string mensaje = "resultadoEnvido";
    public bool ganado;
    public int tantosOponente;

    public MensajeResultadoEnvido(bool g, int t) {
        this.ganado = g;
        this.tantosOponente = t;
    }
}

