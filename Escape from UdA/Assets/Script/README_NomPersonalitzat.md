# ✏️ Sistema de Nom Personalitzat

Aquest sistema permet als jugadors que entren al podio (TOP 3) introduir el seu nom personalitzat per reemplaçar el nom automàtic generat.

## 📋 Funcionalitats

- **Detecció de podio**: Només els jugadors del TOP 3 poden canviar el nom
- **Input field**: Camp de text per introduir el nom personalitzat
- **Validació**: Comprova que el nom no estigui buit i no sigui massa llarg
- **Actualització Firebase**: Actualitza el nom a la base de dades
- **Refresc automàtic**: Recarrega el ranking per mostrar els canvis
- **UI dinàmica**: Mostra/amaga elements segons la posició del jugador

## 🛠️ Configuració a Unity

### 1. Crear Input Field
```
1. Crear un Canvas si no en tens
2. Botó dret → UI → Input Field - TextMeshPro
3. Configurar:
   - Nom: "InputFieldNom"
   - Placeholder Text: "Introdueix el teu nom..."
   - Character Limit: 20
   - Content Type: Standard
```

### 2. Crear Botó
```
1. Botó dret → UI → Button - TextMeshPro
2. Configurar:
   - Nom: "BotoGuardarNom"
   - Text: "GUARDAR NOM"
   - Colors segons el teu disseny
```

### 3. Assignar Referències
Al inspector del RankingManager:
- **Input Field Nom**: Arrossegar l'InputField creat
- **Boto Guardar Nom**: Arrossegar el Button creat

### 4. Configuració Inicial (Opcional)
```
Per defecte, els elements estaran amagats.
Si vols que estiguin visibles des de l'inici:
- InputField: SetActive(true)
- Button: SetActive(true)

El sistema els mostrarà/amagarà automàticament.
```

## 🎮 Com Funciona

### Flux Normal:
```
1. Jugador completa el joc
2. RankingManager calcula la posició
3. Si està en TOP 3:
   - Mostra missatge "INTRODUEIX EL TEU NOM"
   - Activa InputField i Button
4. Jugador introdueix nom i fa clic al botó
5. Sistema valida el nom
6. Actualitza Firebase
7. Recarrega ranking amb el nou nom
8. Amaga InputField i Button
```

### Si no està en podio:
```
1. Amaga InputField i Button
2. No mostra missatge d'introduir nom
3. Només mostra la posició actual
```

## 📊 Validacions Implementades

### Nom buit:
```
⚠️ El nom no pot estar buit!
```

### Nom massa llarg:
```
⚠️ El nom és massa llarg (màxim 20 caràcters)!
```

### No està en podio:
```
⚠️ Només els jugadors del podio poden canviar el nom!
```

## 🔍 Cerca de Puntuació

El sistema busca la puntuació correcta a Firebase:
- **Temps**: Compara amb tolerància de 0.1 segons
- **Timestamp**: Agafa la més recent si hi ha múltiples coincidències
- **Clau única**: Usa la clau de Firebase per actualitzar

## 🎨 Personalització UI

### Missatges:
```csharp
// Podio
"INTRODUEIX EL TEU NOM\nPER APARÈIXER AL RANKING"

// Després de guardar
"✅ NOM GUARDAT CORRECTAMENT!"

// Fora de podio
"" (buit)
```

### Estils Recomanats:

#### InputField:
```
- Font Size: 18-24
- Placeholder Color: Gris clar
- Text Color: Blanc/Negre segons fons
- Background: Semi-transparent
```

#### Button:
```
- Font Size: 16-20
- Colors: Verd per confirmar
- Hover: Verd més clar
- Pressed: Verd més fosc
```

## 🧪 Testing

### Context Menu (RankingManager):
```
- "Test amb Temps Aleatori": Genera temps aleatori
- "Recarregar Ranking": Recarrega des de Firebase
```

### Debug Manual:
```csharp
// Simular podio
RankingManager.Instance.SetTempsJugadorActual(60f); // Temps que estigui en TOP 3

// Verificar estat
Debug.Log(RankingManager.Instance.EstaEnPodio());
Debug.Log(RankingManager.Instance.GetPosicioJugador());
```

## 🔧 Resolució de Problemes

### InputField no apareix:
1. Verificar que està assignat al inspector
2. Comprovar que el Canvas està actiu
3. Revisar que el jugador està en podio

### Botó no funciona:
1. Verificar que està assignat al inspector
2. Comprovar que té el component Button
3. Revisar que el listener s'ha afegit correctament

### No actualitza Firebase:
1. Verificar connexió a Firebase
2. Comprovar permisos de la base de dades
3. Revisar logs de debug per errors

### No troba la puntuació:
1. Verificar que la puntuació s'ha pujat correctament
2. Comprovar que el temps coincideix
3. Revisar el timestamp de la puntuació

## 🚀 Millores Futures

### Validacions Avançades:
```csharp
- Filtrar paraules ofensives
- Verificar noms únics
- Limitar caràcters especials
- Verificar longitud mínima
```

### UI Millorada:
```csharp
- Animacions d'entrada/sortida
- Efectes visuals per podio
- Sons de confirmació
- Feedback visual millor
```

### Funcionalitats Extra:
```csharp
- Editar nom després de guardar
- Historial de noms usats
- Avatars o icones
- Verificació per email
```

## 📝 Codi d'Exemple

### Ús des d'altre script:
```csharp
// Obtenir referència
RankingManager ranking = FindObjectOfType<RankingManager>();

// Verificar si pot canviar nom
if (ranking.EstaEnPodio())
{
    Debug.Log("El jugador pot canviar el nom!");
}

// Forçar actualització
ranking.GuardarNomPersonalitzat();
```

### Configuració programàtica:
```csharp
void Start()
{
    // Configurar InputField
    inputField.characterLimit = 20;
    inputField.placeholder.GetComponent<TextMeshProUGUI>().text = "El teu nom...";
    
    // Configurar Button
    button.onClick.AddListener(() => {
        rankingManager.GuardarNomPersonalitzat();
    });
}
```

El sistema està llest per usar! Només cal configurar l'InputField i el Button a Unity i assignar-los al RankingManager. 