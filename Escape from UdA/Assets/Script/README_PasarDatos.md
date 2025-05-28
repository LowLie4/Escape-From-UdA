# 🔄 Pasar Datos Entre Escenas - Soluciones

El problema era que la puntuación del jugador no se pasaba correctamente a la escena del ranking. Aquí están las soluciones implementadas:

## 🚨 Problema Original

1. `RankingManager` se ejecutaba en `Start()` antes de recibir el tiempo del jugador
2. Al cambiar de escena, se perdían los datos del tiempo
3. No había comunicación entre escenas

## ✅ Soluciones Implementadas

### Solución 1: PlayerPrefs (Recomendada para datos simples)

**Cómo funciona:**
- `Final.cs` guarda el tiempo en PlayerPrefs antes de cambiar de escena
- `RankingManager.cs` carga el tiempo desde PlayerPrefs al iniciar
- Los datos persisten entre sesiones de Unity

**Ventajas:**
- Simple de implementar
- Funciona automáticamente
- Persiste entre sesiones

**Desventajas:**
- Solo para datos simples
- Puede acumular datos innecesarios

### Solución 2: GameDataManager Singleton (Recomendada para datos complejos)

**Cómo funciona:**
- `GameDataManager` usa DontDestroyOnLoad para persistir entre escenas
- Singleton pattern garantiza una sola instancia
- Mantiene todos los datos de la partida en memoria

**Ventajas:**
- Más flexible para datos complejos
- Control total sobre els datos
- Fácil de expandir

**Desventajas:**
- Más complejo de implementar
- Requiere gestión manual de memoria

## 🛠️ Configuración

### Opción A: Solo PlayerPrefs (Automática)
```
✅ Ya está implementado en Final.cs y RankingManager.cs
✅ No requiere configuración adicional
✅ Funciona automáticamente
```

### Opción B: Con GameDataManager
```
1. Crear un GameObject buit llamado "GameDataManager"
2. Afegir el script GameDataManager.cs
3. El sistema detectarà automàticament si està disponible
```

## 🔄 Flux de Dades

### Escena de Joc:
```
1. Jugador completa el joc
2. Final.cs obté el temps del Temporizador
3. Final.cs guarda a PlayerPrefs + GameDataManager
4. Final.cs canvia d'escena
```

### Escena de Ranking:
```
1. RankingManager.cs s'inicia
2. Carrega puntuacions de Firebase
3. Carrega temps del jugador (GameDataManager > PlayerPrefs)
4. Calcula posició i mostra ranking
```

## 📊 Dades Guardades

### PlayerPrefs:
```
- "UltimaPartidaTemps" (float): Temps en segons
- "UltimaPartidaTempsFormatat" (string): Temps formatat MM:SS
```

### GameDataManager:
```
- tempsUltimaPartida (float): Temps en segons
- tempsUltimaPartidaFormatat (string): Temps formatat
- hiHaDadesDisponibles (bool): Si hi ha dades vàlides
```

## 🧪 Testing

### PlayerPrefs:
```csharp
// Verificar dades guardades
Debug.Log(PlayerPrefs.GetFloat("UltimaPartidaTemps"));
Debug.Log(PlayerPrefs.GetString("UltimaPartidaTempsFormatat"));

// Esborrar dades
PlayerPrefs.DeleteKey("UltimaPartidaTemps");
PlayerPrefs.DeleteKey("UltimaPartidaTempsFormatat");
```

### GameDataManager:
```csharp
// Context Menu options disponibles:
- "Mostrar Dades": Mostra les dades actuals
- "Esborrar Dades": Esborra les dades guardades

// Codi:
GameDataManager.Instance.MostrarDades();
GameDataManager.Instance.EsborrarDades();
```

## 🔧 Resolució de Problemes

### El temps no es pasa:
1. Verificar que `Final.cs` s'executa correctament
2. Comprovar els logs de debug
3. Verificar que el canvi d'escena funciona

### RankingManager no troba dades:
1. Verificar que las dades se guarden antes del cambio de escena
2. Comprovar que `RankingManager` s'executa després de carregar l'escena
3. Revisar los logs de debug

### GameDataManager no funciona:
1. Verificar que el GameObject existeix a l'escena inicial
2. Comprovar que té el script assignat
3. Verificar que no es destrueix entre escenas

## 🚀 Millores Futures

### Expansió de Dades:
```csharp
public class GameDataManager : MonoBehaviour
{
    // Dades actuals
    public float tempsUltimaPartida;
    
    // Noves dades possibles:
    public int puntuacioTotal;
    public string nomJugador;
    public DateTime dataPartida;
    public List<string> objectiusComplerts;
    public Dictionary<string, float> estadistiques;
}
```

### Persistència Avançada:
- Guardar a fitxers JSON
- Encriptació de dades
- Backup automàtic
- Sincronització amb núvol

## 📝 Resum

**Ambdues solucions estan implementades y funcionen automáticamente:**

1. **PlayerPrefs**: Solución simple y automática
2. **GameDataManager**: Solución avanzada y flexible

El sistema detecta automáticamente cual está disponible y usa la mejor opción. Puedes usar una o ambas según tus necesidades. 