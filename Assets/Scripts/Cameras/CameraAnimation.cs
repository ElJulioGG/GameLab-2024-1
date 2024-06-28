using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    public Transform player;
    public AnimationCurve transitionCurve;
    public float animationDuration = 2f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 playerPosition;
    private Quaternion playerRotation;

    void Start()
    {
        // Guarda la posición y rotación inicial de la cámara
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Guarda la posición y rotación del jugador
        playerPosition = player.position + new Vector3(0, 1.5f, -3f); // Ajusta según sea necesario
        playerRotation = Quaternion.LookRotation(player.position - playerPosition);

        // Inicia la corrutina para la animación de la cámara
        StartCoroutine(AnimateCamera());
    }

    IEnumerator AnimateCamera()
    {
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = transitionCurve.Evaluate(elapsedTime / animationDuration);

            // Interpola la posición y rotación de la cámara
            transform.position = Vector3.Lerp(initialPosition, playerPosition, t);
            transform.rotation = Quaternion.Slerp(initialRotation, playerRotation, t);

            yield return null;
        }

        // Asegúrate de que la cámara esté exactamente en la posición y rotación final
        transform.position = playerPosition;
        transform.rotation = playerRotation;

        // Aquí puedes agregar cualquier lógica adicional que necesites cuando la cámara termine la animación
        Debug.Log("La cámara ha terminado la animación y está en la vista del jugador.");
    }
}

