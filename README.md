# TrucoBotWars

TrucoBotWars es un juego hecho en Unity el cual se conecta a través de WebSockets con dos bots externos haciendolos jugar al truco. Básicamente sirve para hacer competir a dos bots, el bot más gauchezco gana.

## Los bots

Los bots deben implementar un servidor de WebSockets al cual se va a conectar Unity para enviar y recibir mensajes. [Acá](https://github.com/pablonm/TrucoBotWars-RandomBot) pueden encontrar un ejemplo de un bot que hace una jugada random entre las jugadas disponibles.

## Comunicación con los bots

La comunicación entre el juego y los bots se compone de los siguientes mensajes en formato JSON:

### Mensaje de inicio de mano

Se envía al bot cada vez que comienza una nueva mano y es el primer mensaje que recibe el bot al iniciarse el juego.

```
{
    mensaje: "iniciarMano",
    cartas: [
        {
            palo: "oro",
            numero: 1
        },
        {
            palo: "basto",
            numero: 7
        },
        {
            palo: "espada",
            numero: 3
        },
    ],
    esMano: true
}

```

### Mensaje de pedido de jugada

Se envía al bot cada vez que es su turno para realizar una jugada.

```
{
    mensaje: "pedirJugada",
    cartasEnMesa: [
        {
            palo: "oro",
            numero: 1
        }
    ],
    cartasEnMesaOponente: [
        {
            palo: "oro",
            numero: 7
        },
        {
            palo: "basto",
            numero: 4
        }
    ],
    jugadaAnteriorOponente: {
        mensaje: "carta"
        carta: {
            palo: "basto",
            numero: 4
        }
    },
    jugadasDisponibles: [ // Las posibles jugadas que puede hacer el bot.
        {
            mensaje: "carta" // Significa que se puede tirar una carta
            carta: null
        },
        {
            mensaje: "truco" // Significa que se puede cantar truco
            carta: null
        },
    ]
}

```


### Mensaje de jugada

Se envía desde el bot hacia el juego para indicar que se realiza una cierta jugada.

```
{
    mensaje: "carta" 
    carta: { // Si no se juega una carta, este campo debe ir en null
        palo: "espada",
        numero: 3
    }
}
```

### Mensaje de resultado de envido

Se envía al bot cada vez que una mano termina.

```
{
    mensaje: "resultadoEnvido",
    ganado: false,
    tantosOponente: 33, // Este valor viene en null si el ganador del envido es el jugador mano en esa mano.
}
```

### Mensaje de resultado de mano

Se envía al bot cada vez que una mano termina.

```
{
    mensaje: "resultadoMano",
    puntos: 10, // Puntos totales en la partida actual
    puntosOponente: 5, // Puntos totales del oponente en la partida actual
}
```

### Mensaje de resultado de partida

Se envía al bot cada vez que una partida se termina.

```
{
    mensaje: "resultadoPartida",
    ganada: true
}
```
