using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float growthFactor = 0.01f;
    [SerializeField] private float platformRadius = 10f; // Platformun yarıçapı
    [SerializeField] private float centerAttractionStrength = 2f; // Merkeze çekim gücü
    [SerializeField] private float tensionDistance = 2f; // Gerilme mesafesi
    [SerializeField] private float accelerationFactor = 2f; // Hızlanma katsayısı
    [SerializeField] private float baseCollisionRepelForce = 500f; // Çarpışma sonrası geri tepme kuvveti için temel değer

    private Rigidbody rb;
    private Vector3 movement;
    private Vector3 previousPosition;
    private Transform target;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        previousPosition = transform.position;
    }

    void FixedUpdate()
    {
        FindClosestTarget();  // En yakın hedefi bul
        MoveTowardsTarget();  // Hedefe doğru ilerle
        StayOnPlatform();     // Platformda kalmayı sağla
        Growing();            // Hareket ettikçe büyü
        previousPosition = transform.position;
    }

    void FindClosestTarget()
    {
        float closestDistance = Mathf.Infinity;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.transform != transform)
            {
                // Oyuncunun platformun içinde olup olmadığını kontrol et
                if (player.transform.position.magnitude <= platformRadius)
                {
                    float distance = Vector3.Distance(transform.position, player.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        target = player.transform;
                    }
                }
            }
        }
    }

    void MoveTowardsTarget()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;

            // Platformun merkezine doğru bir çekim ekleyin
            Vector3 centerDirection = (-transform.position).normalized;
            direction += centerDirection * centerAttractionStrength;

            direction = direction.normalized; // Normalleştirerek hareket yönünü belirleyin

            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget > tensionDistance)
            {
                // Gerilme mesafesinden uzakta ise normal hızda hareket et
                movement = direction * speed;
            }
            else
            {
                // Hedefe yaklaşınca yavaşla, ardından hızlanarak çarp
                movement = direction * speed * accelerationFactor;
            }

            rb.AddForce(movement);
        }
    }

    void StayOnPlatform()
    {
        if (transform.position.magnitude > platformRadius)
        {
            Vector3 directionToCenter = -transform.position.normalized;
            movement = directionToCenter * speed;
            rb.AddForce(movement);
        }
    }

    void Growing()
    {
        float distanceMoved = Vector3.Distance(previousPosition, transform.position);

        if (distanceMoved > 0)
        {
            Vector3 newScale = transform.localScale + Vector3.one * (distanceMoved * growthFactor);
            transform.localScale = newScale;
        }
    }

    // Çarpışma sonrası geri tepme kuvvetini ölçeğe göre ayarlayın
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Mevcut ölçek değerini al
            float scaleMultiplier = transform.localScale.magnitude;

            // Çarpışma sonrası geri tepme kuvvetini ölçeğe göre ayarla
            float scaledRepelForce = baseCollisionRepelForce * scaleMultiplier;

            Vector3 repelDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(repelDirection * scaledRepelForce);
        }
    }
}
