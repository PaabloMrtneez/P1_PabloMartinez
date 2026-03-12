# Proyecto Unity - Plataformas 3D

Este proyecto es un juego de plataformas 3D hecho en Unity. El jugador controla un personaje que avanza por un circuito con plataformas, recoge monedas, evita zonas de daño y trata de llegar a la meta final sin quedarse sin vidas.

## Resumen

- El flujo del juego empieza en un menu principal.
- La partida principal ocurre en la escena `juego`.
- El jugador dispone de un sistema de vidas y respawn.
- Al perder todas las vidas se carga la escena `muerte`.
- Al alcanzar la meta se carga la escena `victoria`.

## Caracteristicas principales

- Movimiento del personaje con avance, retroceso y giro por pasos.
- Sprint para recorrer el nivel mas rapido.
- Salto y doble salto.
- Sistema de checkpoints para actualizar el punto de respawn.
- Plataformas moviles que hacen recorridos de ida y vuelta.
- Plataformas que caen cuando el jugador las pisa y luego regresan.
- Coleccion de monedas con contador en pantalla.
- Zonas de dano que restan vidas al jugador.
- Interfaz que muestra monedas y estado de vida.

## Controles

| Accion | Teclas |
| --- | --- |
| Avanzar | `W` o `Flecha Arriba` |
| Retroceder | `S` o `Flecha Abajo` |
| Girar a la izquierda | `A` o `Flecha Izquierda` |
| Girar a la derecha | `D` o `Flecha Derecha` |
| Sprint | `Shift` |
| Saltar / doble salto | `Espacio` |

## Como funciona la partida

El personaje se mueve usando fisicas con `Rigidbody`, y la rotacion se hace de forma progresiva hacia una orientacion objetivo. Mientras gira, el movimiento hacia delante se bloquea para mantener un control mas preciso.

El jugador empieza con 6 vidas. Cada vez que entra en una zona de dano pierde una vida. Si todavia le quedan vidas, reaparece en el ultimo checkpoint activado. Si llega a 0 vidas, la partida cambia a la escena `muerte`.

Las monedas recogidas se acumulan durante la partida actual y se muestran en la interfaz. Si el jugador reintenta desde la escena de derrota o vuelve al menu principal, el progreso de monedas y vidas se reinicia.

## Escenas incluidas

- `Assets/Scenes/MainMenu.unity`: menu principal y boton para empezar la partida.
- `Assets/Scenes/juego.unity`: nivel principal con plataformas, hazards, coleccionables y checkpoints.
- `Assets/Scenes/muerte.unity`: pantalla de derrota con opciones para reintentar o volver al menu.
- `Assets/Scenes/victoria.unity`: pantalla final al completar el nivel.

## Scripts principales

- `PlayerController.cs`: movimiento, giro, sprint, salto, doble salto y respawn del personaje.
- `GameManager.cs`: control global de vidas, monedas, reinicio de partida y cambio de escenas.
- `UI.cs`: actualizacion de la interfaz de vidas.
- `SceneLoader.cs`: carga de la escena principal desde el menu.
- `Checkpoint.cs`: activacion de puntos de control.
- `Coleccionable.cs`: recogida de monedas.
- `DamageZone.cs`: deteccion de dano al entrar en contacto con obstaculos.
- `MovingPlatform.cs`: plataformas que se desplazan entre dos puntos.
- `FallingPlatform.cs`: plataformas que caen tras un pequeno retardo cuando el jugador las pisa.
- `Goal.cs`: deteccion de llegada a la meta y carga de la escena de victoria.

## Estructura del proyecto

```text
Assets/
  Prefabs/         Prefabs de plataformas, coleccionables, hazards y GameManager
  Scenes/          MainMenu, juego, muerte y victoria
  Scripts/         Logica principal del juego
  Models/          Modelos 3D, materiales y recursos visuales
Packages/          Dependencias del proyecto
ProjectSettings/   Configuracion de Unity
```

