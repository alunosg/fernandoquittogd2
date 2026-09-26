using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody rig;

    public bool hitPlayer = true;
    public bool hitEnemy = true;

    public float damage = 1f;

    public GameObject hitFx;


    private void OnTriggerEnter(Collider other)
    {

        if (hitPlayer && other.CompareTag("Player"))
        {
            //other.getcomponent<PlayerContoller>().GetHit(damage);
        }

        if (hitPlayer && other.CompareTag("Enemy"))
        {
            //other.getcomponent<EnemyContoller>().GetHit(damage);
        }
        
        if (hitFx) Instantiate(hitFx, transform.position, transform.rotation);


    }

    private void LateUpdate()
    {
        if (rig.linearVelocity.sqrMagnitude > 0.1f) ;
        {
            transform.rotation = Quaternion.LookRotation(rig.linearVelocity);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
