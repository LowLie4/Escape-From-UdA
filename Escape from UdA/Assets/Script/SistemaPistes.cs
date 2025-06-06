using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;

public class SistemaPistes : MonoBehaviour
{
    public XRSimpleInteractable boto;
    public TMP_Text textMostrar;
    public GameObject panellText;

    public AudioClip[] pistes;

    public AudioSource font;
    private int indice = 0;

    public Animator animacioMontse;

    [Header("Configuració USB")]
    public GameObject usbGris; // Referència al USB_Gris
    
    [Header("Configuració Ordinadors")]
    public GameObject[] ordenadorsAmbImatges; // Els 4 ordinadors amb imatges
    
    [Header("Referències Scripts")]
    public PuzzleOrdinadorUSB puzzleOrdinadorUSB; // Per detectar USB inserida
    public CajaPuzzleColores cajaPuzzleColores; // Per detectar puzle completat
    public PanellLletres panellLletres; // Per detectar puzle morse completat
    public PortaSortida portaSortida; // Per detectar puzle fusibles completat
    
    [Header("Audios de Pistes")]
    public AudioClip[] audiosUSBAntes;      // 3 audios para pistes abans d'agafar USB
    public AudioClip[] audiosUSBDespres;    // 3 audios para pistes després d'agafar USB
    public AudioClip[] audiosOrdenadors;    // 3 audios para pistes d'ordenadors
    public AudioClip[] audiosRelacionar;    // 3 audios para pistes de relacionar
    public AudioClip[] audiosMorse;         // 4 audios para pistes morse
    public AudioClip[] audiosFusibles;      // 4 audios para pistes fusibles
    
    [Header("Audios de Completat")]
    public AudioClip audioPuzleCompletat;   // Audio per puzle completat
    public AudioClip audioMorseCompletat;   // Audio per morse completat
    public AudioClip audioJocCompletat;     // Audio per joc completat
    
    // Estats del puzle
    public enum EstadoPuzzle
    {
        BuscarUSB,          // Buscar l'USB
        USBTrobada,         // USB agafada però no inserida
        USBInserida,        // USB inserida, projecció activa
        BuscarOrdenadors,   // Buscar els 4 ordinadors
        OrdenadorsTrobats,  // 4 ordinadors trobats
        PuzleCompletat,     // Caixa de colors resolta
        MorseIniciat,       // Puzle morse iniciat
        MorseCompletat,     // Puzle morse acabat
        FusiblesIniciat,    // Puzle fusibles iniciat
        JocCompletat        // Tots els puzles completats, porta oberta
    }
    
    // Estat intern
    private EstadoPuzzle estatActual = EstadoPuzzle.BuscarUSB;
    private bool usbTrobat = false;
    private bool usbInserida = false;
    private bool[] ordenadorsAgafats; // Array per fer seguiment dels ordinadors agafats
    private bool puzleCompletat = false;
    private bool morseCompletat = false;
    private bool fusiblesCompletat = false;
    private float rotacioInicialTapa = 0f; // Per detectar canvis a la rotació
    private float rotacioInicialPorta = 0f; // Per detectar canvis a la porta morse
    
    // Pistes de text per a cada estat
    private string[] pistesUSBAntes = {
        "Busca per la sala el que et demana l'ordinador del professor.",
        "Els alumnes solen utilitzar molt aquests dispositius i se'ls obliden.",
        "Busca en una de les torres de la tercera fila de la sala."
    };
    
    private string[] pistesUSBDespres = {
        "Ara connecta l'USB a l'ordinador del professor.",
        "Ves a l'ordinador principal i connecta el dispositiu.",
        "Insereix l'USB al socket de l'ordinador del professor."
    };
    
    private string[] pistesOrdenadors = {
        "Interactua amb tots els objectes que puguis a la sala.",
        "Als PCs s'hi amaga alguna cosa, intenta treure-la dels llocs.",
        "A cada fila hi ha un ordinador amb icones especials."
    };
    
    private string[] pistesRelacionar = {
        "Intenta relacionar les imatges amb els ODS de la pissarra.",
        "Introdueix els números al panell per ordre numèric.",
        "La solució és: 4, 10, 15, 16"
    };
    
    private string[] pistesMorse = {
        "Mira la finestra negra a través de la separació entre els pilars.",
        "Intenta desxifrar el codi amb les llums de la caixa i les indicacions que tens.",
        "Cada llum representa una lletra i el seu ordre.",
        "La paraula és XARXA"
    };
    
    private string[] pistesFusibles = {
        "Busca la caixa de fusibles i obre-la.",
        "Necessites trobar els fusibles correctes completant els puzles.",
        "Hi ha un fusible amagat a la taula del professor.",
        "Col·loca només els fusibles bons (verds) per obrir la porta de sortida."
    };

    void Awake()
    {
        if (font == null)
            Debug.LogError("CyclePistes: no hi ha cap AudioSource al GameObject!");
    }

    void Start()
    {
        panellText.SetActive(false);
        boto.selectEntered.AddListener((arg) => BotoPulsat(boto.gameObject));
        
        // Assegurar estat inicial correcte
        InicialitzarEstat();
        
        // Configurar detecció de l'USB
        ConfigurarDeteccioUSB();
        
        // Configurar detecció d'ordinadors
        ConfigurarDeteccioOrdenadors();
        
        Debug.Log("🎮 SistemaPistes iniciat - Estat inicial: BuscarUSB");
    }
    
    void Update()
    {
        // Verificar canvis d'estat cada frame
        ActualitzarEstat();
    }
    
    void InicialitzarEstat()
    {
        estatActual = EstadoPuzzle.BuscarUSB;
        usbTrobat = false;
        usbInserida = false;
        puzleCompletat = false;
        morseCompletat = false;
        fusiblesCompletat = false;
        indice = 0;
        
        // Inicialitzar array d'ordinadors
        if (ordenadorsAmbImatges != null)
        {
            ordenadorsAgafats = new bool[ordenadorsAmbImatges.Length];
        }
        
        // Capturar rotació inicial de la tapa per detectar canvis
        if (cajaPuzzleColores != null && cajaPuzzleColores.tapa != null)
        {
            rotacioInicialTapa = cajaPuzzleColores.tapa.transform.eulerAngles.x;
            Debug.Log($"🔧 Rotació inicial de la tapa: {rotacioInicialTapa:F2}°");
        }
        
        // Capturar rotació inicial de la porta morse
        if (panellLletres != null && panellLletres.porta != null)
        {
            rotacioInicialPorta = panellLletres.porta.transform.eulerAngles.y;
            Debug.Log($"🔧 Rotació inicial de la porta morse: {rotacioInicialPorta:F2}°");
        }
    }
    
    void ConfigurarDeteccioUSB()
    {
        // Buscar l'USB per nom si no està assignat
        if (usbGris == null)
        {
            usbGris = GameObject.Find("USB_Gris");
            if (usbGris == null)
            {
                Debug.LogWarning("No s'ha trobat el GameObject 'USB_Gris'. Assigna la referència manualment.");
                return;
            }
        }
        
        // Buscar el component XRGrabInteractable a l'USB
        var grabInteractable = usbGris.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            // Subscriure's a l'event de grab
            grabInteractable.selectEntered.AddListener(OnusbTrobat);
            Debug.Log("🔗 Detecció d'USB configurada correctament");
        }
        else
        {
            Debug.LogWarning("L'USB_Gris no té component XRGrabInteractable. Afegeix-lo per detectar el grab.");
        }
    }
    
    void ConfigurarDeteccioOrdenadors()
    {
        if (ordenadorsAmbImatges == null || ordenadorsAmbImatges.Length == 0)
        {
            Debug.LogWarning("No s'han assignat ordinadors amb imatges.");
            return;
        }
        
        for (int i = 0; i < ordenadorsAmbImatges.Length; i++)
        {
            if (ordenadorsAmbImatges[i] != null)
            {
                var grabInteractable = ordenadorsAmbImatges[i].GetComponent<XRGrabInteractable>();
                if (grabInteractable != null)
                {
                    int index = i; // Captura local per al closure
                    grabInteractable.selectEntered.AddListener((args) => OnOrdenadorAgafat(index, args));
                }
                else
                {
                    Debug.LogWarning($"L'ordinador {i} no té XRGrabInteractable.");
                }
            }
        }
        
        Debug.Log($"🖥️ Configurats {ordenadorsAmbImatges.Length} ordinadors per detectar");
    }
    
    void ActualitzarEstat()
    {
        // Verificar si l'USB està inserida
        bool usbActualmentInserida = false;
        if (puzzleOrdinadorUSB != null)
        {
            // Aquí podries afegir lògica per detectar si està inserida
            // Per ara, assumim que si la projecció està activa, està inserida
            usbActualmentInserida = puzzleOrdinadorUSB.ProjeccioODS.activeInHierarchy;
        }
        
        // Verificar si el puzle està completat
        bool puzleActualmentCompletat = false;
        if (cajaPuzzleColores != null)
        {
            // Verificar si la tapa s'ha rotat respecte a la seva posició inicial
            if (cajaPuzzleColores.tapa != null)
            {
                float rotacioActual = cajaPuzzleColores.tapa.transform.eulerAngles.x;
                float diferencia = Mathf.Abs(rotacioActual - rotacioInicialTapa);
                
                // Si la diferència és major a 30°, el puzle està completat
                puzleActualmentCompletat = diferencia > 30f;
                
                // Debug per veure què està passant
                if (diferencia > 5f)
                {
                    Debug.Log($"🔧 Rotació actual: {rotacioActual:F2}° | Inicial: {rotacioInicialTapa:F2}° | Diferència: {diferencia:F2}°");
                }
            }
        }
        
        // Verificar si el puzle morse està completat
        bool morseActualmentCompletat = false;
        if (panellLletres != null && panellLletres.porta != null)
        {
            float rotacioActual = panellLletres.porta.transform.eulerAngles.y;
            float diferencia = Mathf.Abs(rotacioActual - rotacioInicialPorta);
            
            // Si la diferència és major a 30°, el puzle morse està completat
            morseActualmentCompletat = diferencia > 30f;
        }
        
        // Verificar si el puzle de fusibles està completat
        bool fusiblesActualmentCompletat = false;
        if (portaSortida != null)
        {
            // Comptar LEDs verds als sockets de fusibles
            int ledsVerds = 0;
            foreach (var socket in portaSortida.sockets)
            {
                if (socket.LEDAsignat != null)
                {
                    var renderer = socket.LEDAsignat.GetComponent<Renderer>();
                    if (renderer != null && renderer.material.color == Color.green)
                    {
                        ledsVerds++;
                    }
                }
            }
            
            // Si hi ha 4 LEDs verds, el puzle està completat
            fusiblesActualmentCompletat = ledsVerds >= 4;
        }
        
        // Actualitzar estats
        if (fusiblesActualmentCompletat && !fusiblesCompletat)
        {
            fusiblesCompletat = true;
            CanviarEstat(EstadoPuzzle.JocCompletat);
        }
        else if (morseActualmentCompletat && !morseCompletat)
        {
            morseCompletat = true;
            CanviarEstat(EstadoPuzzle.FusiblesIniciat); // Canviar directament a fusibles
        }
        else if (puzleActualmentCompletat && !puzleCompletat)
        {
            puzleCompletat = true;
            CanviarEstat(EstadoPuzzle.MorseIniciat); // Canviar directament al morse
        }
        else if (usbActualmentInserida && !usbInserida)
        {
            usbInserida = true;
            if (TotsOrdenadorsTrobats())
            {
                CanviarEstat(EstadoPuzzle.OrdenadorsTrobats);
            }
            else
            {
                CanviarEstat(EstadoPuzzle.BuscarOrdenadors);
            }
        }
        else if (TotsOrdenadorsTrobats() && estatActual == EstadoPuzzle.BuscarOrdenadors)
        {
            CanviarEstat(EstadoPuzzle.OrdenadorsTrobats);
        }
    }
    
    void CanviarEstat(EstadoPuzzle nouEstat)
    {
        if (estatActual != nouEstat)
        {
            Debug.Log($"🎯 Estat canviat: {estatActual} → {nouEstat}");
            estatActual = nouEstat;
            indice = 0; // Reiniciar pistes al canviar estat
        }
    }
    
    bool TotsOrdenadorsTrobats()
    {
        if (ordenadorsAgafats == null) return false;
        
        for (int i = 0; i < ordenadorsAgafats.Length; i++)
        {
            if (!ordenadorsAgafats[i]) return false;
        }
        return true;
    }
    
    int OrdenadorsTrobats()
    {
        if (ordenadorsAgafats == null) return 0;
        
        int comptador = 0;
        for (int i = 0; i < ordenadorsAgafats.Length; i++)
        {
            if (ordenadorsAgafats[i]) comptador++;
        }
        return comptador;
    }
    
    void OnusbTrobat(SelectEnterEventArgs args)
    {
        // Verificar que sigui el jugador qui agafa l'USB, no un socket
        var interactor = args.interactorObject;
        
        // Debug complet per veure què està agafant l'USB
        Debug.Log($"🔍 USB agafat per: '{interactor.transform.name}' (Tipus: {interactor.GetType().Name})");
        Debug.Log($"🔍 GameObject pare: '{interactor.transform.parent?.name}' ");
        
        // Buscar si és un controlador del jugador (més flexible)
        string nomInteractor = interactor.transform.name.ToLower();
        string nomPare = interactor.transform.parent != null ? interactor.transform.parent.name.ToLower() : "";
        
        if (nomInteractor.Contains("controller") || 
            nomInteractor.Contains("hand") ||
            nomInteractor.Contains("player") ||
            nomInteractor.Contains("direct") ||
            nomInteractor.Contains("ray") ||
            nomInteractor.Contains("near-far") ||
            nomPare.Contains("controller") ||
            nomPare.Contains("hand") ||
            nomPare.Contains("player"))
        {
            usbTrobat = true;
            CanviarEstat(EstadoPuzzle.USBTrobada);
            Debug.Log("✅ USB agafat pel jugador! Canviant a pistes d'inserció.");
        }
        else
        {
            Debug.Log($"❌ USB interactuant amb '{interactor.transform.name}' - No és el jugador, ignorant.");
            Debug.Log("💡 Si això hauria de ser el jugador, digue'm el nom exacte per afegir-lo a la detecció.");
        }
    }
    
    void OnOrdenadorAgafat(int indiceOrdenador, SelectEnterEventArgs args)
    {
        // Verificar que sigui el jugador
        var interactor = args.interactorObject;
        string nomInteractor = interactor.transform.name.ToLower();
        string nomPare = interactor.transform.parent != null ? interactor.transform.parent.name.ToLower() : "";
        
        if (nomInteractor.Contains("controller") || 
            nomInteractor.Contains("hand") ||
            nomInteractor.Contains("player") ||
            nomInteractor.Contains("direct") ||
            nomInteractor.Contains("ray") ||
            nomInteractor.Contains("near-far") ||
            nomPare.Contains("controller") ||
            nomPare.Contains("hand") ||
            nomPare.Contains("player"))
        {
            if (!ordenadorsAgafats[indiceOrdenador])
            {
                ordenadorsAgafats[indiceOrdenador] = true;
                int trobats = OrdenadorsTrobats();
                Debug.Log($"🖥️ Ordinador {indiceOrdenador + 1} trobat! Total: {trobats}/{ordenadorsAmbImatges.Length}");
                
                if (TotsOrdenadorsTrobats() && usbInserida)
                {
                    CanviarEstat(EstadoPuzzle.OrdenadorsTrobats);
                }
            }
        }
    }

    public void BotoPulsat(GameObject boto)
    {
        panellText.SetActive(true);

        ReproduirPista();
    }

    public void ReproduirPista()
    {
        animacioMontse.SetTrigger("EstaParlant");
        
        // Obtenir l'audio corresponent a l'estat actual
        AudioClip audioActual = ObtenirAudioActual();
        
        // Reproduir audio si està disponible
        if (audioActual != null)
        {
            font.clip = audioActual;
            font.Play();
        }
        else
        {
            Debug.LogWarning($"⚠️ No hi ha audio assignat per l'estat {estatActual}, índex {indice}");
        }
        
        // Mostrar text segons l'estat actual
        string[] pistesActuals = ObtenirPistesActuals();
        
        if (pistesActuals.Length > 0)
        {
            int textIndex = Mathf.Min(indice, pistesActuals.Length - 1);
            textMostrar.text = pistesActuals[textIndex];
        }

        // Incrementar índice
        indice = (indice + 1) % pistesActuals.Length;
    }
    
    AudioClip ObtenirAudioActual()
    {
        AudioClip[] audiosActuals = null;
        
        switch (estatActual)
        {
            case EstadoPuzzle.BuscarUSB:
                audiosActuals = audiosUSBAntes;
                break;
                
            case EstadoPuzzle.USBTrobada:
                audiosActuals = audiosUSBDespres;
                break;
                
            case EstadoPuzzle.USBInserida:
            case EstadoPuzzle.BuscarOrdenadors:
                audiosActuals = audiosOrdenadors;
                break;
                
            case EstadoPuzzle.OrdenadorsTrobats:
                audiosActuals = audiosRelacionar;
                break;
                
            case EstadoPuzzle.PuzleCompletat:
                return audioPuzleCompletat;
                
            case EstadoPuzzle.MorseIniciat:
                audiosActuals = audiosMorse;
                break;
                
            case EstadoPuzzle.MorseCompletat:
                return audioMorseCompletat;
                
            case EstadoPuzzle.FusiblesIniciat:
                audiosActuals = audiosFusibles;
                break;
                
            case EstadoPuzzle.JocCompletat:
                return audioJocCompletat;
        }
        
        // Retornar l'audio corresponent a l'índex actual
        if (audiosActuals != null && audiosActuals.Length > 0)
        {
            int audioIndex = Mathf.Min(indice, audiosActuals.Length - 1);
            return audiosActuals[audioIndex];
        }
        
        return null;
    }
    
    string[] ObtenirPistesActuals()
    {
        switch (estatActual)
        {
            case EstadoPuzzle.BuscarUSB:
                return pistesUSBAntes;
                
            case EstadoPuzzle.USBTrobada:
                return pistesUSBDespres;
                
            case EstadoPuzzle.USBInserida:
            case EstadoPuzzle.BuscarOrdenadors:
                return pistesOrdenadors;
                
            case EstadoPuzzle.OrdenadorsTrobats:
                return pistesRelacionar;
                
            case EstadoPuzzle.PuzleCompletat:
                return new string[] { "¡Puzle completat! Busca el següent desafiament." };
                
            case EstadoPuzzle.MorseIniciat:
                return pistesMorse;
                
            case EstadoPuzzle.MorseCompletat:
                return new string[] { "¡Puzle morse completat! Continua explorant..." };
                
            case EstadoPuzzle.FusiblesIniciat:
                return pistesFusibles;
                
            case EstadoPuzzle.JocCompletat:
                return new string[] { "¡Joc completat! La porta s'ha obert." };
                
            default:
                return pistesUSBAntes;
        }
    }
    
    // Mètodes públics per reiniciar i comprovar estat
    public void ReiniciarEstat()
    {
        InicialitzarEstat();
        Debug.Log("🔄 Estat del sistema de pistes reiniciat");
    }
    
    // Mètodes per avançar manualment el puzle morse (per testing o triggers específics)
    public void AvancarMorse()
    {
        switch (estatActual)
        {
            case EstadoPuzzle.MorseIniciat:
                CanviarEstat(EstadoPuzzle.FusiblesIniciat);
                break;
        }
    }
    
    public void SaltarAEstatMorse(EstadoPuzzle nouEstat)
    {
        if (nouEstat >= EstadoPuzzle.MorseIniciat && nouEstat <= EstadoPuzzle.JocCompletat)
        {
            CanviarEstat(nouEstat);
        }
    }
    
    // Mètodes per al puzle de fusibles
    public void IniciarFusibles()
    {
        CanviarEstat(EstadoPuzzle.FusiblesIniciat);
    }
    
    public int GetFusiblesVerds()
    {
        if (portaSortida == null) return 0;
        
        int ledsVerds = 0;
        foreach (var socket in portaSortida.sockets)
        {
            if (socket.LEDAsignat != null)
            {
                var renderer = socket.LEDAsignat.GetComponent<Renderer>();
                if (renderer != null && renderer.material.color == Color.green)
                {
                    ledsVerds++;
                }
            }
        }
        return ledsVerds;
    }
    
    public bool EstaUsbTrobat()
    {
        return usbTrobat;
    }
    
    public EstadoPuzzle GetEstatActual()
    {
        return estatActual;
    }
    
    public int GetOrdenadorsTrobats()
    {
        return OrdenadorsTrobats();
    }
}

