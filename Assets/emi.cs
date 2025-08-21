using System.Collections;
using UnityEngine;

public class emi : MonoBehaviour
{
    [Header("Configuración de disparo")]
    public GameObject cajaPrefab;            // Prefab de la caja que vamos a disparar
    public int cantidadCajas = 20;           // Número total de cajas a disparar
    public float maxDistancia = 10f;         // Distancia máxima para detectar paredes (raycast)
    public float delayEntreDisparos = 0.2f;  // Tiempo entre disparos (segundos)
    public float fuerzaDisparo = 10f;        // Fuerza aplicada a las cajas para dispararlas
    public float anguloTotal = 180f;         // Ángulo total para disparar (medio círculo)

    void Start()
    {
        StartCoroutine(DispararCajas());
    }

    IEnumerator DispararCajas()
    {
        // Paso angular para repartir las cajas dentro del ánguloTotal (ej: 180 grados)
        float angleStep = 0;
        if (cantidadCajas > 1)
            angleStep = anguloTotal / (cantidadCajas - 1);
        else
            angleStep = 0;

        // El ángulo inicial para centrar el abanico hacia adelante (eje Z+)
        float anguloInicial = -anguloTotal / 2;

        for (int i = 0; i < cantidadCajas; i++)
        {
            // Calculamos el ángulo actual dentro del abanico
            float angle = anguloInicial + i * angleStep;

            // Calculamos la dirección en plano XZ usando seno para X y coseno para Z
            Vector3 direccion = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad));

            Vector3 origen = transform.position;

            // Raycast para detectar pared (opcional, se puede usar para posicionar o ajustar)
            RaycastHit hit;
            Vector3 posicionInstancia;

            if (Physics.Raycast(origen, direccion, out hit, maxDistancia))
            {
                posicionInstancia = origen + direccion * (hit.distance - 0.5f);
            }
            else
            {
                posicionInstancia = origen + direccion * maxDistancia;
            }

            // Instanciamos la caja en el origen
            GameObject caja = Instantiate(cajaPrefab, origen, Quaternion.identity);

            // Aplicamos fuerza para disparar la caja hacia la dirección calculada
            Rigidbody rb = caja.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(direccion * fuerzaDisparo, ForceMode.Impulse);
            }
            else
            {
                Debug.LogWarning("El prefab de la caja no tiene Rigidbody!");
            }

            yield return new WaitForSeconds(delayEntreDisparos);
        }
    }
}
