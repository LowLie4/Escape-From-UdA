# 🎮 Escape from UdA

Un joc de sala d'escapada immersiu en **Realitat Virtual** desenvolupat amb Unity. Els jugadors han de resoldre una sèrie de trencaclosques interconnectats per escapar de la UdA (Universitat d'Alacant) abans que s'acabe el temps.

## 🎯 Descripció del Joc

**Escape from UdA** és una experiència de sala d'escapada en RV que et situa en una aula universitària plena d'enigmes. El teu objectiu és trobar la manera d'eixir resolent trencaclosques que inclouen:

- 🔍 **Cerca d'objectes**: Troba l'USB perdut a la sala
- 💻 **Trencaclosques informàtics**: Interactua amb ordinadors i desxifra codis
- 🎨 **Trencaclosques de colors**: Relaciona imatges amb els Objectius de Desenvolupament Sostenible (ODS)
- 📡 **Codi Morse**: Desxifra missatges ocults amb pistes lumíniques
- ⚡ **Fusibles**: Troba i col·loca els fusibles correctes per obrir la porta d'escapada

## 🚀 Característiques Principals

- **Realitat Virtual Completa**: Compatible amb dispositius RV (Oculus/Meta)
- **Sistema de Pistes Intel·ligent**: Ajuda contextual segons el teu progrés
- **Temporitzador Integrat**: Competeix contra el temps i millora el teu rècord
- **Rànquing Online**: Comparteix els teus millors temps amb Firebase
- **Múltiples Trencaclosques**: Cada trencaclosques desbloqueja el següent en una seqüència lògica
- **Detecció de Mans**: Interacció natural amb Oculus Hand Tracking
- **Àudio Immersiu**: Pistes d'àudio i efectes de so ambientals

## 🛠️ Tecnologies Utilitzades

- **Unity 2022.x** - Motor de joc principal
- **XR Interaction Toolkit** - Framework per a interaccions RV
- **Mixed Reality Toolkit (MRTK)** - Eines de realitat mixta
- **Oculus Integration** - Suport per a dispositius Meta/Oculus
- **Firebase** - Backend per a rànquing i puntuacions
- **TextMeshPro** - Sistema de text avançat
- **ProBuilder** - Eines de modelat 3D

## 🎮 Com Jugar

### Controls RV
- **Agafament**: Usa els controladors RV o el seguiment de mans per interactuar
- **Moviment**: Teletransport o moviment lliure (segons configuració)
- **Interacció**: Apunta i selecciona objectes per interactuar
- **Menú de Pistes**: Prem el botó designat per rebre ajuda

### Progressió del Joc

1. **Inici**: Cerca l'USB a l'aula de classe
2. **Connexió**: Connecta l'USB a l'ordinador del professor
3. **Exploració**: Troba i interactua amb els 4 ordinadors especials
4. **Trencaclosques de Colors**: Relaciona les imatges amb els ODS de la pissarra
5. **Codi Morse**: Desxifra el missatge usant les pistes lumíniques
6. **Fusibles**: Troba i col·loca els fusibles correctes
7. **Escapada**: Surt per la porta una vegada completats tots els trencaclosques!

## 📦 Instal·lació i Configuració

### Requisits del Sistema
- **Unity 2022.3 LTS** o superior
- **Windows 10/11** (64-bit)
- **Dispositiu RV** (Meta Quest, Oculus Rift, etc.)
- **8 GB RAM** mínim (16 GB recomanat)
- **GPU compatible amb RV** (NVIDIA GTX 1060 / AMD RX 580 o superior)

### Configuració del Projecte

1. **Clonar el repositori**:
   ```bash
   git clone https://github.com/el-teu-usuari/Escape-From-UdA.git
   cd "Escape from UdA"
   ```

2. **Obrir en Unity**:
   - Obri Unity Hub
   - Selecciona "Open Project"
   - Navega a la carpeta del projecte
   - Selecciona la carpeta arrel del projecte

3. **Configurar XR**:
   - Ves a **Edit > Project Settings > XR Plugin Management**
   - Activa el proveïdor d'XR per al teu dispositiu (Oculus, OpenXR, etc.)

4. **Configurar Firebase** (opcional per al rànquing):
   - Afig el teu arxiu `google-services.json` a la carpeta Assets
   - Configura les credencials de Firebase a l'inspector

### Configuració d'Escenes

El projecte inclou les següents escenes principals:
- **Menu Principal**: Menú d'inici del joc
- **Tutorial**: Escena d'aprenentatge de controls
- **Joc**: Escena principal del joc
- **Final**: Pantalla de resultats i puntuació

## 🎨 Estructura del Projecte

```
Assets/
├── Scenes/                 # Escenes del joc
├── Script/                 # Scripts principals
│   ├── Puzzles/           # Lògica de trencaclosques
│   ├── UI/                # Interfície d'usuari
│   └── Base de Dades/     # Gestió de dades
├── Audios/                # Arxius d'àudio
├── Animacions/            # Animacions
├── Objetos Propios/       # Models 3D personalitzats
└── Settings/              # Configuracions del projecte
```

### Scripts Principals

- **`SistemaPistes.cs`**: Gestiona el sistema de pistes contextual
- **`Final.cs`**: Controla la finalització del joc i puntuacions
- **`GameDataManager.cs`**: Gestiona les dades de la partida
- **`Temporizador.cs`**: Controla el temps de joc
- **`CajaPuzzleColores.cs`**: Lògica del trencaclosques de colors
- **`PanellLletres.cs`**: Gestiona el trencaclosques de codi Morse
- **`CaixaFusibles.cs`**: Controla el trencaclosques de fusibles

## 🏆 Sistema de Puntuació

El joc inclou un sistema de rànquing basat en el temps:
- **Temps mínim**: Millor temps possible
- **Rànquing local**: Guardat en PlayerPrefs
- **Rànquing online**: Sincronitzat amb Firebase
- **Format**: MM:SS (minuts:segons)

## 🔧 Desenvolupament i Personalització

### Afegir Nous Trencaclosques

1. Crea un nou script heretant de `MonoBehaviour`
2. Implementa la lògica del trencaclosques
3. Integra amb `SistemaPistes.cs` per afegir pistes contextuals
4. Actualitza l'enum `EstadoPuzzle` si és necessari

### Modificar Pistes

Les pistes es troben en `SistemaPistes.cs`:
- `pistesUSBAntes[]`: Pistes per trobar l'USB
- `pistesOrdenadors[]`: Pistes per als ordinadors
- `pistesMorse[]`: Pistes per al codi Morse
- `pistesFusibles[]`: Pistes per als fusibles

### Personalitzar Àudio

Els arxius d'àudio s'organitzen en `Assets/Audios/`:
- `Veu/`: Pistes de veu
- `Digital/`: Efectes sonors digitals
- `Resistencies/`: Sons de components electrònics

## 📱 Plataformes Suportades

- **PC RV** (Windows)
- **Meta Quest** (Android)
- **Oculus Rift/Rift S**
- **Valve Index**
- **HTC Vive**

## 🤝 Contribuir

Si vols contribuir al projecte:

1. Fes fork del repositori
2. Crea una branca per a la teua característica (`git checkout -b feature/AmazingFeature`)
3. Commit els teus canvis (`git commit -m 'Add some AmazingFeature'`)
4. Push a la branca (`git push origin feature/AmazingFeature`)
5. Obri un Pull Request

## 📄 Llicència

Aquest projecte està sota la Llicència MIT - consulta l'arxiu [LICENSE](LICENSE) per a més detalls.

## 👥 Crèdits

- **Desenvolupador Principal**: [El teu nom]
- **Institució**: Universitat d'Alacant (UdA)
- **Assets**: Diversos assets d'Unity Asset Store
- **Eines**: Unity, MRTK, XR Interaction Toolkit

## 🐛 Problemes Coneguts

- El seguiment de mans pot ser inestable en condicions de poca llum
- Alguns trencaclosques poden requerir recalibració després d'una pausa
- La sincronització amb Firebase pot fallar sense connexió a internet

## 📞 Suport

Si trobes algun problema o tens suggeriments:
- Obri un issue a GitHub
- Contacta amb el desenvolupador
- Consulta la documentació d'Unity RV

---

**Gaudeix escapant de la UdA!** 🎓✨ 