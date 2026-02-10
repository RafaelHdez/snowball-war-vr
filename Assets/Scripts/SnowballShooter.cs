using UnityEngine;
using Oculus.Interaction;
using System.Collections;

public class SnowballShooter : MonoBehaviour
{
    [Header("Configuración de disparo")]
    public float velocidadDisparo = 20f;
    public float impulsoVertical = 0.5f;

    [Header("Botón de disparo")]
    public OVRInput.Button botonDisparo = OVRInput.Button.One; // Botón A

    [Header("Referencias")]
    [SerializeField] private Grabbable grabbable;

    [Header("Efectos")]
    public float cooldownDisparo = 0.5f;
    [Range(0f, 1f)] public float fuerzaVibracion = 1f;
    public float duracionVibracion = 0.2f;

    // Referencias internas
    private Rigidbody rb;
    private bool isGrabbed = false;
    private bool puedeDisparar = true;
    private OVRInput.Controller controladorActivo;
    
    // Esta es la referencia clave para corregir la dirección
    private Transform trackingSpace; 

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (grabbable == null) grabbable = GetComponent<Grabbable>();

        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    void Start()
    {
        // <--- CAMBIO IMPORTANTE 1: Encontrar el padre de la cámara
        // En Meta Building Blocks/OVRCameraRig, el padre de la cámara es el "TrackingSpace".
        // Este objeto es el que sabe hacia dónde está mirando el "jugador virtual" en el mundo.
        if (Camera.main != null)
        {
            trackingSpace = Camera.main.transform.parent;
        }
        else
        {
            Debug.LogError("¡No encuentro la Main Camera! Asegúrate de que tu cámara tenga el tag MainCamera.");
        }
    }

    void OnEnable()
    {
        if (grabbable != null) grabbable.WhenPointerEventRaised += HandlePointerEvent;
    }

    void OnDisable()
    {
        if (grabbable != null) grabbable.WhenPointerEventRaised -= HandlePointerEvent;
    }

    private void HandlePointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Select) OnGrabbed();
        else if (evt.Type == PointerEventType.Unselect) OnReleased();
    }

    private void OnGrabbed()
    {
        isGrabbed = true;
        puedeDisparar = true;
        DetectarControlador();
    }

    private void OnReleased()
    {
        isGrabbed = false;
    }

    void Update()
    {
        if (isGrabbed && puedeDisparar)
        {
            if (OVRInput.GetDown(botonDisparo, controladorActivo))
            {
                StartCoroutine(SecuenciaDisparo());
            }
        }
    }

    private void DetectarControlador()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
            controladorActivo = OVRInput.Controller.RTouch;
        else
            controladorActivo = OVRInput.Controller.LTouch;
    }

    private IEnumerator SecuenciaDisparo()
    {
        if (rb == null || trackingSpace == null) yield break; // Seguridad extra

        puedeDisparar = false;

        // <--- CAMBIO IMPORTANTE 2: CÁLCULO DE DIRECCIÓN MUNDIAL
        
        // A. Obtenemos la rotación local del control (como antes)
        Quaternion rotacionLocalControl = OVRInput.GetLocalControllerRotation(controladorActivo);
        Vector3 direccionLocal = rotacionLocalControl * Vector3.forward;

        // B. Transformamos esa dirección local a MUNDIAL usando el TrackingSpace
        // Esto corrige el problema de que el disparo salga "chueco" al girar el personaje
        Vector3 direccionMundial = trackingSpace.TransformDirection(direccionLocal);

        // C. Añadimos el arco hacia arriba (en espacio mundial, UP siempre es arriba)
        direccionMundial += Vector3.up * impulsoVertical * 0.1f;
        direccionMundial.Normalize();

        // ---------------------------------------------------------

        // Feedback Háptico
        OVRInput.SetControllerVibration(fuerzaVibracion, fuerzaVibracion, controladorActivo);

        // Desactivar Grabbable para soltar
        if (grabbable != null) grabbable.enabled = false;
        transform.SetParent(null);

        // Activar física
        rb.isKinematic = false;
        rb.useGravity = true;

        // Aplicar fuerza
        #if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = direccionMundial * velocidadDisparo;
        #else
            rb.velocity = direccionMundial * velocidadDisparo;
        #endif

        rb.angularVelocity = Random.insideUnitSphere * 5f;

        // Limpieza y reactivación
        yield return new WaitForSeconds(duracionVibracion);
        OVRInput.SetControllerVibration(0, 0, controladorActivo);

        yield return new WaitForSeconds(0.5f);
        if (grabbable != null) grabbable.enabled = true;
        
        isGrabbed = false;
        yield return new WaitForSeconds(cooldownDisparo);
        puedeDisparar = true;
    }

    // Dibujamos la línea de debug para ver si ahora apunta bien
    void OnDrawGizmos()
    {
        if (isGrabbed && Application.isPlaying && trackingSpace != null)
        {
            Quaternion rotacionLocal = OVRInput.GetLocalControllerRotation(controladorActivo);
            Vector3 dirLocal = rotacionLocal * Vector3.forward;
            Vector3 dirMundial = trackingSpace.TransformDirection(dirLocal);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, dirMundial * 5f);
        }
    }
}