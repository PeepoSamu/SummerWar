using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float damage;
    private float gravityMultiplier;
    private Rigidbody rb;

    public void Initialize(float damage, float velocity, float gravity, float lifeTime)
    {
        this.damage = damage;
        this.gravityMultiplier = gravity;

        rb = GetComponent<Rigidbody>();

        rb.linearVelocity = transform.forward * velocity;

        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        rb.AddForce(
            Physics.gravity * gravityMultiplier,
            ForceMode.Acceleration
        );
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Hit {collision.collider.name} | Damage: {damage}");
        Destroy(gameObject);
    }
}