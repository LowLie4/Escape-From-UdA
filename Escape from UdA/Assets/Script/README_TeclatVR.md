# 🎹 Integració Teclat VR (MRTK)

Aquest document explica com configurar i usar el teclat VR del MRTK amb el nostre sistema de ranking.

## 📋 Sobre el Teclat MRTK

El teclat que estàs usant és del repositori: [MRTK-Keyboard](https://github.com/Ayfel/MRTK-Keyboard)

**Característiques:**
- Versió standalone del NonNative Keyboard del MRTK
- Funciona amb Unity Input regular
- Tecles són botons UI de Unity
- Compatible amb VR i escenes 3D
- Fàcil d'integrar

## 🛠️ Configuració Pas a Pas

### 1. Verificar Instal·lació del Teclat
```
✅ Assegura't que tens els fitxers del MRTK-Keyboard a Assets/
✅ El prefab hauria d'estar a: MRTK/SDK/Experimental/NonNativeKeyboard/Prefabs/
✅ Verifica que el prefab està a l'escena
```

### 2. Configurar RankingManager
Al inspector del RankingManager:

#### Teclat VR (MRTK):
```
☑️ Usar Teclat VR: TRUE
📍 Posicio Teclat: Crear un GameObject buit on vols que aparegui el teclat
```

#### Botons:
```
🔘 Boto Mostrar Teclat: Crear un botó amb text "INTRODUIR NOM"
💾 Boto Guardar Nom: Deixar buit (no necessari amb VR)
📝 Input Field Nom: Deixar buit (no necessari amb VR)
```

### 3. Posicionar el Teclat
```
1. Crear un GameObject buit anomenat "PosicioTeclat"
2. Posicionar-lo davant del jugador (recomanat: 1-2 metres)
3. Assignar-lo al camp "Posicio Teclat" del RankingManager
```

## 🎮 Com Funciona

### Flux VR:
```
1. Jugador entra al podio (TOP 3)
2. Apareix missatge: "PREM EL BOTÓ PER INTRODUIR EL TEU NOM"
3. Apareix botó "INTRODUIR NOM"
4. Jugador fa clic al botó
5. Apareix el teclat VR a la posició configurada
6. Jugador escriu el nom amb el teclat VR
7. Jugador prem Enter al teclat
8. El teclat es tanca automàticament
9. El nom es guarda a Firebase
10. El ranking es recarrega amb el nou nom
```

### Events del Teclat:
```
📝 OnTextUpdated: Mentre l'usuari escriu (temps real)
✅ OnTextSubmitted: Quan l'usuari prem Enter (confirma)
```

## 🔧 Configuració Avançada

### Posicionament del Teclat:
```csharp
// Recomanacions per VR:
Position: (0, 1.2, 1.5) // Davant del jugador, a l'altura dels ulls
Rotation: (15, 0, 0)    // Lleugerament inclinat cap avall
Scale: (1, 1, 1)        // Mida normal
```

### Fallback a InputField:
```
Si el teclat VR no funciona:
1. Desmarcar "Usar Teclat VR" al inspector
2. Assignar un InputField normal
3. El sistema usarà la UI tradicional
```

## 🧪 Testing i Debug

### Verificar Integració:
```csharp
// Logs que hauríeu de veure:
🎹 Teclat VR configurat correctament
🎹 Teclat VR mostrat
📝 Text actualitzat: 'nom_usuari'
✅ Text enviat des del teclat VR: 'nom_usuari'
💾 Guardant nom personalitzat: 'nom_usuari'
```

### Problemes Comuns:

#### El teclat no apareix:
```
❌ Problema: NonNativeKeyboard.Instance és null
✅ Solució: Assegurar que el prefab està a l'escena i actiu
```

#### No es detecta el component:
```
❌ Problema: No s'ha trobat el component NonNativeKeyboard
✅ Solució: Verificar que el prefab té el script NonNativeKeyboard
```

#### El teclat apareix en mala posició:
```
❌ Problema: Teclat apareix lluny o en posició estranya
✅ Solució: Configurar correctament "Posicio Teclat"
```

## 🎨 Personalització UI

### Missatges per VR:
```csharp
// Podio amb VR
"PREM EL BOTÓ PER INTRODUIR\nEL TEU NOM"

// Mentre escriu
"INTRODUEIX EL TEU NOM:\n'text_actual'"

// Després de guardar
"✅ NOM GUARDAT CORRECTAMENT!"

// Errors
"❌ EL NOM NO POT ESTAR BUIT!"
"❌ NOM MASSA LLARG (MÀX 20)!"
```

### Botó Recomanat:
```
Text: "INTRODUIR NOM"
Mida: 200x80 pixels
Colors: Blau per destacar
Font: Bold, 18-24pt
```

## 🔄 Comparació Sistemes

### Teclat VR (Recomanat per VR):
```
✅ Immersiu per VR
✅ Fàcil d'usar amb controladors
✅ Integració automàtica
✅ No requereix InputField
❌ Més complex de configurar
```

### InputField Tradicional:
```
✅ Simple de configurar
✅ Funciona amb mouse/teclat
❌ Menys immersiu en VR
❌ Pot ser difícil d'usar amb controladors VR
```

## 📝 Codi d'Exemple

### Mostrar Teclat Manualment:
```csharp
// Des d'altre script:
var ranking = FindObjectOfType<RankingManager>();
ranking.MostrarTeclatVR();
```

### Configurar Posició Programàticament:
```csharp
void Start()
{
    // Posicionar davant de la càmera
    Transform camera = Camera.main.transform;
    Vector3 posicio = camera.position + camera.forward * 2f;
    posicio.y = camera.position.y; // Mantenir altura
    
    posicioTeclat.position = posicio;
    posicioTeclat.LookAt(camera);
}
```

### Subscriure's als Events:
```csharp
void Start()
{
    var keyboard = FindObjectOfType<Microsoft.MixedReality.Toolkit.Experimental.UI.NonNativeKeyboard>();
    if (keyboard != null)
    {
        keyboard.OnTextSubmitted += (text) => {
            Debug.Log($"Text rebut: {text}");
        };
    }
}
```

## 🚀 Millores Futures

### Funcionalitats Avançades:
```csharp
- Autocompletar noms
- Historial de noms usats
- Validació en temps real
- Efectes visuals al teclat
- Sons de tecleig
```

### Optimitzacions VR:
```csharp
- Posicionament automàtic segons la mirada
- Mida adaptativa segons la distància
- Haptic feedback als controladors
- Integració amb hand tracking
```

## 📋 Checklist de Configuració

```
☐ Teclat MRTK instal·lat a Assets/
☐ Prefab NonNativeKeyboard a l'escena
☐ RankingManager configurat amb "Usar Teclat VR" = true
☐ GameObject "PosicioTeclat" creat i assignat
☐ Botó "Introduir Nom" creat i assignat
☐ Testat en VR amb controladors
☐ Verificat que es guarda el nom a Firebase
```

El teclat MRTK és perfecte per VR! Només cal configurar-lo correctament seguint aquests passos. 