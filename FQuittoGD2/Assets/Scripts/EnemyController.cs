using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum EnemyStates { Idle, Patrol, Chase, Attack, Hit, Death }
    public EnemyStates state = EnemyStates.Patrol;

    public bool patrol = false;

    private float distance;
    private PlayerController player;

    public float chaseDistance = 80;
    public float attackDistance = 30;
    public float attackDuration = 3.0f;
    public float attackDelay = 0.5f;

    public float hitDuration = 0.5f;
    public float deathDuration = 3.0f;

    public Transform turret;
    public Transform cannon;

    public NavMeshAgent nav;
    public Transform bulletPoint;
    public GameObject shootFX;
    public GameObject hitFX;

    public GameObject bulletPrefab;
    public float bulletSpeed = 30;

    public SpriteRenderer hpBar;
    public float hp = 5;
    private float maxHp;

    private bool locked = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (locked) return;

        distance = Vector3.Distance(player.transform.position, transform.position);

        switch (state)
        {
            case EnemyStates.Idle:
                IdleUpdate();
                break;

            case EnemyStates.Patrol:
                PatrolUpdate();
                break;

            case EnemyStates.Chase:
                ChaseUpdate();
                break;

            case EnemyStates.Attack:
                AttackUpdate();
                break;

            case EnemyStates.Hit:
                HitUpdate();
                break;

            case EnemyStates.Death:
                DeathUpdate();
                break;
        }
    }

    void IdleUpdate()
    {
        if (distance < chaseDistance)
        {
            if (distance < attackDistance)
            {
                EnterAttack();
            }
            else
            {
                state = EnemyStates.Chase;
            }
        }
    }

    void PatrolUpdate()
    {

    }

    void ChaseUpdate()
    {
        if (distance > chaseDistance)
        {
            state = EnemyStates.Idle;
            nav.isStopped = true;
        }
        else if (distance < attackDistance)
        {
            EnterAttack();
        }
        else
        {
            nav.isStopped = false;
            nav.SetDestination(player.transform.position);
        }
    }

    void AttackUpdate()
    {
        if (distance < attackDistance)
        {
            EnterAttack();
        }
        else if (distance < chaseDistance)
        {
            state = EnemyStates.Chase;
        }
        else
        {
            nav.isStopped = true;
            state = EnemyStates.Idle;
        }
    }

    void HitUpdate()
    {

    }

    void DeathUpdate()
    {

    }

    void Shoot()
    {
        if (shootFX) Instantiate(shootFX, bulletPoint.position, bulletPoint.rotation);

        GameObject bullet = Instantiate(bulletPrefab, bulletPoint.position, bulletPoint.rotation);
        bullet.GetComponent<Rigidbody>().linearVelocity = bulletPoint.forward * bulletSpeed;
    }

    void Unlock() => locked = false;

    public void GetHit(float damage)
    {
        if (hp > 0)
        {
            CancelInvoke("Unlock");
            CancelInvoke("Shoot");
            locked = true;
            nav.isStopped = true;

            if (hitFX) Instantiate(hitFX, transform.position, transform.rotation);

            hp -= damage;
            hpBar.transform.localScale = hp / maxHp * Vector3.one;
            if (hp > 0)
            {
                //Leva hit
                Invoke("Unlock", hitDuration);
            }
            else
            {
                //Morre
                Destroy(gameObject, deathDuration);
            }
        }
    }

    void EnterAttack()
    {
        LookAtTarget();
        locked = true;
        CancelInvoke("Unlock");
        Invoke("Unlock", attackDuration);
        CancelInvoke("Shoot");
        Invoke("Shoot", attackDelay);
        state = EnemyStates.Attack;
        nav.isStopped = true;
    }

    void LookAtTarget()
    {
        Vector3 targetPosition = player.transform.position;
        targetPosition.y = transform.position.y;
        turret.LookAt(targetPosition);

        //cannon.LookAt(player.transform);
        //cannon.localEulerAngles = new Vector3(cannon.localEulerAngles.x, 0, 0);

        //bulletPoint.LookAt(player.transform);




        // 1. Encontra a distância e altura em relação ao pivot de rotação do canhão
        Vector3 targetPos = player.transform.position;
        Vector3 lowTarget = new Vector3(targetPos.x, cannon.position.y, targetPos.z);

        float xPivot = Vector3.Distance(cannon.position, lowTarget);
        float yPivot = targetPos.y - cannon.position.y;

        // 2. Calcula o offset local do bulletPoint em relação ao canhão
        // Isso nos dá exatamente o comprimento do cano (zOffset) e a altura dele (yOffset)
        Vector3 localMuzzleOffset = cannon.InverseTransformPoint(bulletPoint.position);
        float zOffset = localMuzzleOffset.z; // Distância para a frente do pivot
        float yOffset = localMuzzleOffset.y; // Distância para cima/baixo do pivot

        // 3. Ajustamos o alvo real subtraindo o avanço do cano do cálculo de distância horizontal
        float x = xPivot - zOffset;
        float y = yPivot - yOffset;

        float v = bulletSpeed;
        float g = Physics.gravity.magnitude;

        // 4. Fórmula da Trajetória Balística pura
        float discriminant = (v * v * v * v) - g * (g * (x * x) + 2 * y * (v * v));

        if (discriminant >= 0)
        {
            float sqrtRoot = Mathf.Sqrt(discriminant);

            // Ângulo da trajetória em radianos
            float angleRad = Mathf.Atan2((v * v) - sqrtRoot, g * x);
            float angleDeg = angleRad * Mathf.Rad2Deg;

            // 5. Aplica estritamente no eixo X local do canhão (invertido para a Unity)
            cannon.localRotation = Quaternion.Euler(-angleDeg, 0, 0);
        }
        else
        {
            // Fora de alcance: busca o maior ângulo
            cannon.localRotation = Quaternion.Euler(-45f, 0, 0);
            Debug.Log("out of range");
        }
    }
}