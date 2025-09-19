# Unity Activity Checklist: Third Person Controller with Invulnerability System

## ASSETS DISPONIBLES EN EL PROYECTO:
- **Player Controller**: `PlayerArmature.prefab` en `Assets/StarterAssets/ThirdPersonController/Prefabs/`
- **Cámara del Player**: `PlayerFollowCamera.prefab` en `Assets/StarterAssets/ThirdPersonController/Prefabs/`
- **Copa para Invulnerabilidad**: `Cup empty.fbx` en `Assets/Cups/Objects/`
- **Skybox**: Múltiples materiales en `Assets/Free Stylized Skybox/CubeMap/Materials/`
- **Terreno**: `New Terrain.asset` ya creado en `Assets/`
- **Escena Actual**: `SampleScene.unity` en `Assets/Scenes/`

---

## FASE 1: ✅ CONFIGURACIÓN COMPLETADA

### 1.1 Preparación del Proyecto
- [x] **[MANUAL - TÚ]** Abrir Unity Hub y crear nuevo proyecto 3D
- [x] **[MANUAL - TÚ]** Nombrar el proyecto
- [x] **[MANUAL - TÚ]** Abrir el proyecto en Unity Editor

### 1.2 Assets Importados
- [x] **[MANUAL - TÚ]** Third Person Controller (`StarterAssets/`)
- [x] **[MANUAL - TÚ]** Cup Asset (`Cups/`)
- [x] **[MANUAL - TÚ]** Skybox (`Free Stylized Skybox/`)
- [x] **[MANUAL - TÚ]** Terreno creado (`New Terrain.asset`)
- [x] **[MANUAL - TÚ]** Skybox configurado

## FASE 2: CONFIGURACIÓN DEL JUGADOR

### 2.1 Player Controller en Escena
- [x] **[MANUAL - TÚ]** `PlayerArmature.prefab` colocado en `SampleScene.unity`
- [x] **[MANUAL - TÚ]** Player posicionado sobre el terreno

### 2.2 Configurar Cámara
- [x] **[AUTOMÁTICO - MCP]** Instanciar `PlayerFollowCamera.prefab` en la escena
- [x] **[AUTOMÁTICO - MCP]** Configurar la cámara como hija del Player o asignar Target al PlayerArmature
- [x] **[AUTOMÁTICO - MCP]** Verificar configuración de Third Person Controller

## FASE 3: SISTEMAS DE JUEGO (AUTOMÁTICO - MCP)

### 3.1 Sistema de Vida del Jugador
- [x] **[AUTOMÁTICO - MCP]** Crear script `PlayerHealthSystem.cs` en `Assets/Scripts/` con:
  - Variable float currentHealth = 1.0f
  - Método TakeDamage(float damage)
  - Método IsAlive()
  - Logs por consola: "Vida actual: X.X"
  - Referencia al InvulnerabilitySystem

### 3.2 Sistema de Invulnerabilidad  
- [x] **[AUTOMÁTICO - MCP]** Crear script `InvulnerabilitySystem.cs` en `Assets/Scripts/` con:
  - bool isInvulnerable = false
  - float invulnerabilityDuration = 5.0f
  - Corrutina para manejar timer
  - Logs: "¡Invulnerabilidad activada!" y "Invulnerabilidad terminada"
  - Método público ActivateInvulnerability()

### 3.3 Ítem de Invulnerabilidad
- [x] **[AUTOMÁTICO - MCP]** Crear GameObject `InvulnerabilityPowerUp` en la escena
- [x] **[AUTOMÁTICO - MCP]** Asignar modelo `Cup empty.fbx` al GameObject
- [x] **[AUTOMÁTICO - MCP]** Agregar SphereCollider como Trigger
- [x] **[AUTOMÁTICO - MCP]** Crear script `InvulnerabilityPowerUp.cs` con:
  - OnTriggerEnter detecta tag "Player"
  - Encuentra PlayerHealthSystem en el jugador
  - Llama ActivateInvulnerability()
  - Desactiva el GameObject

## FASE 4: SISTEMA DE PARED TRAMPA (AUTOMÁTICO - MCP)

### 4.1 Crear Pared Trampa
- [x] **[AUTOMÁTICO - MCP]** Crear GameObject `TrapWall` en la escena
- [x] **[AUTOMÁTICO - MCP]** Agregar Cube primitivo, escalar (1, 3, 0.2)
- [x] **[AUTOMÁTICO - MCP]** Crear Material `TrapWallMaterial` color rojo
- [x] **[AUTOMÁTICO - MCP]** Crear GameObject hijo `TriggerZone` con BoxCollider (IsTrigger = true)

### 4.2 Sistema de Detección 
- [x] **[AUTOMÁTICO - MCP]** Crear script `TrapWallDetector.cs` con:
  - OnTriggerEnter detecta tag "Player"
  - Referencia al componente ArrowLauncher
  - Cooldown 2 segundos
  - Log: "¡Jugador detectado! Disparando flechas!"

### 4.3 Sistema de Flechas
- [x] **[AUTOMÁTICO - MCP]** Crear prefab `ArrowProjectile` en `Assets/Prefabs/`
- [x] **[AUTOMÁTICO - MCP]** Usar Capsule escalada (0.1, 0.5, 0.1) como modelo
- [x] **[AUTOMÁTICO - MCP]** Agregar Rigidbody y CapsuleCollider (IsTrigger = true)
- [x] **[AUTOMÁTICO - MCP]** Crear script `ArrowProjectile.cs` con:
  - float damage = 0.2f
  - OnTriggerEnter detecta tag "Player"
  - Verifica si player.GetComponent<InvulnerabilitySystem>().isInvulnerable
  - Si no invulnerable: aplica daño y log "Flecha impactó - Daño aplicado: 0.2"
  - Si invulnerable: log "Flecha impactó - Sin daño (invulnerable)"
  - Destroy(gameObject, 5f) para cleanup

### 4.4 Lanzador de Flechas
- [x] **[AUTOMÁTICO - MCP]** Crear script `ArrowLauncher.cs` con:
  - Transform[] launchPoints (3 posiciones)
  - GameObject arrowPrefab (referencia al ArrowProjectile)
  - Método LaunchArrows() que instancia 3 flechas
  - AddForce hacia el jugador
  - Log: "¡Flechas disparadas!"
- [x] **[AUTOMÁTICO - MCP]** Crear 3 Empty GameObjects como hijos de TrapWall para launch points

## FASE 5: INTEGRACIÓN Y CONFIGURACIÓN (AUTOMÁTICO - MCP)

### 5.1 Conectar Scripts a GameObjects
- [x] **[AUTOMÁTICO - MCP]** Asignar `PlayerHealthSystem.cs` al `PlayerArmature`
- [x] **[AUTOMÁTICO - MCP]** Asignar `InvulnerabilitySystem.cs` al `PlayerArmature`  
- [x] **[AUTOMÁTICO - MCP]** Asignar `TrapWallDetector.cs` al `TriggerZone`
- [x] **[AUTOMÁTICO - MCP]** Asignar `ArrowLauncher.cs` al `TrapWall`

### 5.2 Configurar Referencias en Inspector
- [x] **[AUTOMÁTICO - MCP]** TrapWallDetector: asignar ArrowLauncher del padre (auto-find)
- [x] **[AUTOMÁTICO - MCP]** ArrowLauncher: asignar ArrowProjectile prefab
- [x] **[AUTOMÁTICO - MCP]** ArrowLauncher: asignar los 3 LaunchPoints (auto-find)
- [x] **[AUTOMÁTICO - MCP]** Verificar que PlayerArmature tiene tag "Player"

## FASE 6: POSICIONAMIENTO Y TESTING (MANUAL - TÚ)

### 6.1 Posicionar Elementos
- [ ] **[MANUAL - TÚ]** Posicionar `InvulnerabilityPowerUp` (Copa) sobre el terreno
- [ ] **[MANUAL - TÚ]** Posicionar `TrapWall` en el camino del jugador
- [ ] **[MANUAL - TÚ]** Ajustar posición y tamaño del `TriggerZone`

### 6.2 Testing Básico
- [ ] **[MANUAL - TÚ]** Probar movimiento con PlayerArmature
- [ ] **[MANUAL - TÚ]** Recoger copa → Verificar log "¡Invulnerabilidad activada!"
- [ ] **[MANUAL - TÚ]** Pasar por TrapWall → Verificar disparo de flechas
- [ ] **[MANUAL - TÚ]** Verificar logs de vida en consola

### 6.3 Testing de Integración - 3 Escenarios
- [ ] **[MANUAL - TÚ]** **Escenario 1**: Pasar por TrapWall SIN copa → Recibir daño (vida 1.0 → 0.8)
- [ ] **[MANUAL - TÚ]** **Escenario 2**: Recoger copa → Pasar inmediatamente → Sin daño (vida 1.0)
- [ ] **[MANUAL - TÚ]** **Escenario 3**: Recoger copa → Esperar 6+ segundos → Pasar → Recibir daño
- [ ] **[MANUAL - TÚ]** Verificar todos los logs en consola

---

## NOTAS IMPORTANTES:

### 📋 NOMBRES EXACTOS DE ASSETS:
- **Player**: `PlayerArmature` (GameObject en escena)
- **Cámara**: `PlayerFollowCamera.prefab` 
- **Copa**: `Cup empty.fbx` → GameObject `InvulnerabilityPowerUp`
- **Pared Trampa**: GameObject `TrapWall` con hijo `TriggerZone`
- **Prefab Flecha**: `ArrowProjectile.prefab` en `Assets/Prefabs/`
- **Scripts**: Todos en `Assets/Scripts/`

### 🎯 LOGS REQUERIDOS POR CONSOLA:
   - `"Vida actual: X.X"`
   - `"¡Invulnerabilidad activada! Duración: 5 segundos"`
   - `"Invulnerabilidad terminada"`
   - `"¡Jugador detectado! Disparando flechas!"`
   - `"Flecha impactó - Daño aplicado: 0.2"`
   - `"Flecha impactó - Sin daño (invulnerable)"`

### ⚙️ CONFIGURACIÓN DE LAS 3 FLECHAS:
   - Crear 3 Empty GameObjects como hijos de `TrapWall`
   - Nombrarlos: `LaunchPoint_1`, `LaunchPoint_2`, `LaunchPoint_3`
   - Asignarlos al array `launchPoints[]` en `ArrowLauncher.cs`
   - Las 3 se instancian simultáneamente hacia el jugador

### 🔢 PARÁMETROS CLAVE:
   - **Vida inicial**: `1.0f`
   - **Daño por flecha**: `0.2f`  
   - **Duración invulnerabilidad**: `5.0f` segundos
   - **Cooldown trampa**: `2.0f` segundos
   - **3 flechas** por disparo

### 🚨 TROUBLESHOOTING:
   - **Flechas no impactan**: Verificar AddForce direction y velocity
   - **Trigger no funciona**: Verificar que `PlayerArmature` tiene tag "Player"
   - **No aparecen logs**: Verificar que scripts están asignados correctamente
   - **Cámara no sigue**: Verificar configuración de `PlayerFollowCamera.prefab`
