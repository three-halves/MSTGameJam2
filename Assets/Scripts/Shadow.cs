using UnityEngine;

public class Shadow : MonoBehaviour
{
    // for shadow scaling
    [SerializeField] private bool scaleWithHitDistance = false;
    [SerializeField] private float scalingFactor = 1f;
    [SerializeField] private LayerMask mask;
    [SerializeField] private Transform shadowCasterTransform;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        RaycastHit hit = new();
        transform.position = shadowCasterTransform.position;
        Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, mask);
        if (hit.collider == null) return;
        transform.position = new Vector3(transform.position.x, hit.point.y + 0.001f, transform.position.z);
        if (scaleWithHitDistance && hit.distance > 0 && hit.distance != Mathf.Infinity)
        {
            transform.localScale = originalScale * Mathf.Min(1f / (hit.distance * scalingFactor), 1f);
        }
    }
}
