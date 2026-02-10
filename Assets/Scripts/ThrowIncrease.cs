using UnityEngine;
using Oculus.Interaction;
using System.Runtime.InteropServices;

public class ThrowIncrease : MonoBehaviour
{
    [Header("Ajustes de puntería")]
    [Tooltip("Multiplica la fuerza general")]
    public float strengthMultiplier = 2.5f;

    [Tooltip("Ayuda a que la bola vuele más lejos hacia arriba")]
    public float ayudaArco = 1.5f;

    [Header("Referencias")]
    [SerializeField] private Grabbable grabbable;

    [Header("Debug")]
    public bool mostrarVelocidad = true;

    private Rigidbody rb;
    private Vector3 lastPosition;
    private Vector3 throwVelocity;
    private const int velocityFrames = 5;
    private Vector3[] velocityBuffer;
    private int currentFrame = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        velocityBuffer = new Vector3[velocityFrames];

        if (grabbable == null)
        {
            grabbable = GetComponent<Grabbable>();
        }

        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    void OnEnable()
    {
        if (grabbable != null)
        {
            grabbable.WhenPointerEventRaised += HandlePointerEvent;
        }
    }

    void OnDisable()
    {
        if (grabbable != null)
        {
            grabbable.WhenPointerEventRaised -= HandlePointerEvent;
        }
    }

    private void HandlePointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Unselect)
        {
            ApplyThrowForce();
        }
    }
    
    void FixedUpdate()
    {
        if (grabbable != null && grabbable.SelectingPointsCount > 0)
        {
            Vector3 currentVelocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
            velocityBuffer[currentFrame] = currentVelocity;
            currentFrame = (currentFrame + 1) % velocityFrames;
            lastPosition = transform.position;
        }
    }

    private void ApplyThrowForce()
    {
        if (rb != null)
        {
            Vector3 avgVelocity = Vector3.zero;
            for (int i = 0; i < velocityFrames; i++)
            {
                avgVelocity += velocityBuffer[i];
            }

            avgVelocity /= velocityFrames;

            if (mostrarVelocidad)
            {
                Debug.Log($"Velocidad de lanzamiento: {avgVelocity.magnitude} m/s");
            }

            Vector3 nuevaVelocidad = avgVelocity * strengthMultiplier;

            if (avgVelocity.magnitude > 0.3f)
            {
                nuevaVelocidad.y = Mathf.Max(nuevaVelocidad.y * ayudaArco, nuevaVelocidad.y + 1f);
            }

            rb.linearVelocity = nuevaVelocidad;

            if (mostrarVelocidad)
            {
                Debug.Log($"Velocidad aplicada: {nuevaVelocidad.magnitude} m/s");
            }

            for (int i = 0; i < velocityFrames; i++)
            {
                velocityBuffer[i] = Vector3.zero;
            }

            currentFrame = 0;
        }
    }
}
