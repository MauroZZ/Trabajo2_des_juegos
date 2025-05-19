# Trabajo2_desarrollo_juegos

Este proyecto de desarrollo de juegos en Unity tiene como objetivo crear un juego donde el jugador debe llegar a una base y escapar de enemigos dentro de un límite de tiempo de 15 segundos, aplicando los conocimientos adquiridos en clase.

**Progreso Actual:**

- **1. Modificación del Escenario:** El escenario base ha sido modificado y se le han agregado obstáculos utilizando ProBuilder. Esto proporciona un entorno más desafiante para la navegación y la persecución de los enemigos.

- **2. Creación Básica del Personaje Principal:** Se ha creado un personaje principal rudimentario, listo para ser controlado e interactuar con el entorno.

- **3. Movimiento del Personaje con Respecto a la Cámara:** Se ha implementado un sistema de movimiento para el personaje principal que está vinculado a la perspectiva de la cámara. Esto permite un control intuitivo en tercera persona, donde el personaje se mueve en la dirección en la que el jugador está mirando a través de la cámara.

- **4. El Personaje Voltea Hacia Donde se Mueve:** El personaje ahora tiene la capacidad de rotar y orientarse automáticamente en la dirección en la que se está moviendo. Esto mejora la respuesta visual y la inmersión del jugador.

- **5. Implementación de la Función de Salto:** Se ha añadido la funcionalidad de salto para el personaje principal, permitiendo superar pequeños obstáculos o añadir dinamismo al movimiento. El salto se activa mediante la entrada designada (por defecto, la barra espaciadora).

- **6. Implementación del Seguimiento de Cámara en Tercera Persona (ELIMINADO):** Se ha implementado un sistema para que la cámara siga al personaje principal desde una perspectiva en tercera persona, proporcionando una vista clara del jugador y su entorno. La distancia, altura y suavidad del seguimiento han sido configuradas.


- **8. Desarrollo de la Estructura del Menú Principal y Ajustes (18 de Mayo de 2025 - Jonnathan):** Se ha implementado la estructura inicial de la interfaz de usuario del juego, incluyendo:

Escena del Menú Principal: Contiene botones para "Jugar" (carga la escena principal), "Ajustes" (navega a la escena de ajustes) y "Salir" (muestra un mensaje en consola).
Escena de Ajustes: Contiene un botón de "Volver" que permite regresar al menú principal.


- **9. Creacion de nuevo mapa mas sencillo y modificacionde camra en una escena nueva:** Se ha creado un mapa mas sencillo acorde a lo requerido y se modifico la vista de la camara para que se vea desde arriba y siga al personaje desde arriba 

- **10. creacion de enemigos, y que te puedan seguir con navmesh:** se hen creado 5 enemigos (esferas) que pueden seguir al jugador por el mapa utilizando navmesh

**Próximos Pasos:**

- Creación de la meta a la que el jugador debe llegar.
- Desarrollo del sistema de penalizaciones por colisión con los enemigos.
- Implementación del límite de tiempo de 15 segundos y la condición de derrota por tiempo agotado.
- Adición de efectos de sonido y música ambiental.
- Implementación de partículas para la colisión inicial con los enemigos.
- Desarrollo de la interfaz de usuario (menú interactivo, pantallas de GameOver, información final).
- Guardado del nombre del jugador, selección de dificultad y control de volumen desde el menú.
- Utilización de los datos del menú en la escena del juego.
- Mostrar información al finalizar el juego (tiempo utilizado y nombre del jugador).



*---------------------------------------------------------------------------------------------------------------------*



**Desarrollador:** Mauro.
**Desarrollador:** Jonnathan.


