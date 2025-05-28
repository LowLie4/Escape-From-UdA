# 🏆 RankingManager - Sistema de Ranking

Aquest script gestiona el sistema de ranking per mostrar els millors temps i la posició del jugador actual.

## 📋 Funcionalitats

- **GET de Firebase**: Carrega totes les puntuacions de la base de dades
- **Ordenació**: Ordena els temps de menor a major (millor temps primer)
- **TOP 3**: Mostra els 3 millors jugadors amb medalles 🥇🥈🥉
- **Posició del jugador**: Calcula i mostra la posició del jugador actual
- **Detecció de podio**: Detecta si el jugador ha entrat al TOP 3
- **Missatge personalitzat**: Demana introduir nom si està en el podio

## 🛠️ Configuració a Unity

### 1. Crear GameObject per Ranking
```
1. Crear un GameObject buit anomenat "RankingManager"
2. Afegir el script RankingManager.cs
3. Configurar les referències UI al inspector
```

### 2. Crear TextMeshPro per UI
Necessites crear 3 TextMeshPro:

#### A) Text Ranking TOP 3
```
- Nom: "TextRankingTop3"
- Tipus: TextMeshPro (3D) o TextMeshProUGUI (Canvas)
- Posició: On vulguis mostrar el ranking
- Mida de font: 24-36
- Alineació: Center
```

#### B) Text Posició Jugador
```
- Nom: "TextPosicioJugador"
- Tipus: TextMeshPro (3D) o TextMeshProUGUI (Canvas)
- Posició: Sota el ranking
- Mida de font: 20-28
- Alineació: Center
```

#### C) Text Introduir Nom
```
- Nom: "TextIntroduirNom"
- Tipus: TextMeshPro (3D) o TextMeshProUGUI (Canvas)
- Posició: Sota la posició del jugador
- Mida de font: 18-24
- Alineació: Center
- Color: Groc o verd per destacar
```

### 3. Assignar Referències
Al inspector del RankingManager:
- **Text Ranking Top3**: Arrossegar el TextMeshPro del TOP 3
- **Text Posicio Jugador**: Arrossegar el TextMeshPro de la posició
- **Text Introduir Nom**: Arrossegar el TextMeshPro del missatge

## 🔄 Integració amb el Sistema Existent

### Modificació del Final.cs
El script `Final.cs` ja està modificat per:
- Buscar automàticament el RankingManager
- Passar el temps final quan el joc acaba
- Activar el càlcul de ranking

### Flux Complet
```
1. Jugador completa el joc (Final.cs)
2. S'atura el temporizador (Temporizador.cs)
3. Es puja la puntuació a Firebase (NovaPuntuació.cs)
4. Es passa el temps al RankingManager (Final.cs)
5. RankingManager carrega totes les puntuacions
6. Calcula la posició del jugador
7. Mostra el TOP 3 i la posició del jugador
8. Si està en podio, demana introduir nom
```

## 📊 Estructura de Dades

### Classe PuntuacioJugador
```csharp
public class PuntuacioJugador
{
    public string nom;           // Nom del jugador
    public float temps;          // Temps en segons
    public string tempsFormatat; // Temps formatat (MM:SS)
    public long timestamp;       // Timestamp de quan es va crear
}
```

### Base de Dades Firebase
```
puntuacions/
├── puntuacio1/
│   ├── nom: "Jugador01"
│   ├── temps: 125.5
│   ├── tempsFormatat: "02:05"
│   └── timestamp: 1234567890
├── puntuacio2/
│   ├── nom: "Jugador02"
│   ├── temps: 98.2
│   ├── tempsFormatat: "01:38"
│   └── timestamp: 1234567891
└── ...
```

## 🎮 Mètodes Públics

### SetTempsJugadorActual(float temps)
Assigna el temps del jugador actual per calcular la seva posició.

### EstaEnPodio()
Retorna `true` si el jugador està en el TOP 3.

### GetPosicioJugador()
Retorna la posició del jugador (1, 2, 3, etc.).

## 🧪 Testing

### Context Menu Options
- **Recarregar Ranking**: Torna a carregar les puntuacions de Firebase
- **Test amb Temps Aleatori**: Genera un temps aleatori per provar

### Debug
El script inclou logs detallats:
- 🔥 Inicialització de Firebase
- 📊 Càrrega de puntuacions
- 🏆 Mostrar TOP 3
- 🎯 Posició del jugador
- ⚠️ Warnings i errors

## 🎨 Personalització UI

### Medalles i Emojis
```csharp
🥇 Primer lloc
🥈 Segon lloc  
🥉 Tercer lloc
🎯 Posició
⏱️ Temps
🎉 Podio
✏️ Introduir nom
```

### Exemple de Text Mostrat

#### TOP 3:
```
🏆 TOP 3 MILLORS TEMPS 🏆

🥇 Jugador15 - 01:23
🥈 Jugador08 - 01:45
🥉 Jugador22 - 02:01
```

#### Posició Jugador (dins podio):
```
🥈 POSICIÓ: 2
⏱️ TEMPS: 01:45
🎉 HAS ENTRAT AL PODIO!
```

#### Posició Jugador (fora podio):
```
🎯 POSICIÓ: 7
⏱️ TEMPS: 02:34
```

#### Missatge Introduir Nom:
```
✏️ INTRODUEIX EL TEU NOM
PER APARÈIXER AL RANKING
```

## 🔧 Resolució de Problemes

### Firebase no connecta
- Verificar que Firebase està configurat correctament
- Comprovar les dependències de Firebase
- Revisar la configuració de la base de dades

### No es mostren puntuacions
- Verificar que hi ha dades a Firebase
- Comprovar els logs de debug
- Usar "Recarregar Ranking" del context menu

### TextMeshPro no es mostra
- Verificar que les referències estan assignades
- Comprovar que els GameObjects estan actius
- Revisar la posició i mida dels texts

## 🚀 Futures Millores

- [ ] Sistema d'input per introduir nom personalitzat
- [ ] Animacions per mostrar el ranking
- [ ] Filtres per dates o categories
- [ ] Paginació per més de 3 jugadors
- [ ] Efectes visuals per podio
- [ ] Sons per celebrar el podio 