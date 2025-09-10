
# PEC1-CARRERAS-3D


## ¿Cómo usar?

Abrir el projecto dentro del directorio Carrerinhas3D, utilizando Unity 2021.3.19f.

Controles por defecto:
 - Jugador 1:
	 - Acelerar: Tecla W / Tecla Arriba
	 - Girar: Teclas AD / Teclas Izquierda-Derecha
     - Freno/Reversa: Tecla S/ Tecla Abajo
     - Freno de Mano: Barra Espaciadora

## Estructura Lógica del Proyecto

### Gestion Carrera

Se definen una serie de Clases para repartir las responsabilidades dentro del flujo de juego de la carrrera y sus entidades asociades. 


- **RaceManager**: El GameManager será responsable tanto del control del flujo del juego (GameLoop), como de la instanciación de los vehiculos escogidos, manteniendo la espera cuando esta el conteo regresivo, habilitando la carrera mientras el jugador no haya completado 3 vueltas y finalmente controlar la finalización de la misma y mostrar los resultados. Tambien tendra en su conocimiento los waypoints y atajos existentes en la pista, los cuales le serán pasados a cada corredor para procesar su avance en la carrera.

- **CarManager**: Esta clase centraliza todo lo referente a la instancia de un corredor y coordina con sus diferentes componentes para brindar la experiencia de juego. En este manager se estará controlando la salud del auto, las colisiones y triggers (para detectar cuando el vehiculo ha salido de pista o se ha estrellado contra algo en escena), los sistemas de particulas de tierra la salir de pista o el humo para reflejar daños, la Cinemachine para poder cambiar el ángulo en el que se esté viendo al vehiculo y referencia al UIHelper para actualizar el estado del vehiculo en el HUD (tiempos de vuelta, velocidad y vueltas completadas). 

- **CarLapManager**: Clase que centraliza la gestión de Waypoints para controlar el recorrido de la carrera. Al subordinada por un CarManager, no necesita ser una clase MonoBehavior. Esta clase arma un Diccionario de transform y waypoint, de tal manera que cada que el CarManager detecta que ha pasado por un waypoint, este actualizará su estado a superado. El procesado de waypoints a través del circuito busca asegurar que el jugador recorra en un orden establecido la pista. Sin embargo, este componente tambien tiene en cuenta cuando se pasa por un atajo, el cual va a tener como definido desde cual waypoint se esta saltando el circuito hasta cual waypoint va a desembocar el jugador, con lo que al pasar por ahí, se marcan todos los demás waypoints en ese rango como superados, simulando que se ha recorrido la pista hasta ese punto.

- **CarPlayerSampler**: Clase encargada de realizar muestras del recorrido de un jugador durante la carrera para eventualmente utilizarlas como interpolación para el recorrido en modo replay o para un fantasma de la pista. Para el proyecto, se han tomado muestras cada 0.25 segundos. Adicionalmente, aquí se definen clases serializables RaceSample, SampleLap y CarTracePosition, las cuales ayudarán a representar los datos persistentes para ser guardados como record de pista y segmentar correctamente los recorridos que ha hecho el jugador en cada vuelta para alcanzar dicho record.

- **UIHandler**: Clase de enlace con los diferentes componentes de interfaz de la carrera, como es el caso del velocimetro, el cronometro, los marcadores de tiempo de vuelta y el contador de vueltas completadas, asi como exponer métodos para que los botones llamen al ser clickeados y puedan invocar funciones de otras clases que no son accesibles a nivel de UI.

- **CommonDataSingleton** : Una instancia única que va a servir de enlace entre otras clases así como de proveer y mantener datos y metodos de uso común (como es el de cargar y guardar muestras generadas por el CarPlayerSampler y manejadas a través de archivos JSON con la ruta de aplicación segun corresponda (A nivel de editor de Unity o a nivel del sistema operativo donde esté siendo alojado el build)). Se usa tambien para pasar datos entre escena, como es el caso de los datos de la pista escogida y el jugador escogido.

- **SceneLoader, PlayerSelector, TrackSelector**: Estas clases se utilizan particularmente para la transición entre escenas y exponer métodos para poder efectuar la elección de Corredor y Pista. Particularmente el TrackSelector se apoya en el Singleton para cargar los datos de cada pista y mostrar cual es el record actual en cada una. 

- **ReplayController**: Clase que gestiona el recorrido de una carro en una pista basado en un recorrido establecido. Se utiliza tanto para que el fantasma haga su recorrido como para hacer un replay de la carrera, la cual hará una rotación frecuente de diferentes ángulos del vehiculo escogido mientras repite el recorrido que el jugador acaba de efectuar. Incluye métodos que cambian los shaders y el color de todo un vehiculo para que sea visto como un fantasma. 

## Video de Demostración

[Video de YouTube aquí <----](https://www.youtube.com/watch?v=azF6AMt0ONA)


## Estructura de Escenas

1. Pantalla Principal 
2. Selección de Corredor
3. Seleccion de Pista
4. Pista 1 (Cuadrumbia)
5. Pista 2 (DesiertoPicudo)
6. Créditos

## Créditos

Banda Sonora
- Hot-air Skyway, Tiny Arena, Main Menu - MIDI Remix de Crash Team Racing (1999)
- Victory Theme - Jorge Velandia 

Assets Importados
- EasyRoads3Dv3 - AndaSoft --
- FreeLowPolyDesertPack - 23 Space Robots and Counitng --
- LowPolyEuropeanCityPack - Karboosx --
- CartoonRaceTrack - RCC Design--

Efectos de Sonido
- RPG Maker VX Ace Runtime Package.
- https://kronbits.itch.io/freesfx


