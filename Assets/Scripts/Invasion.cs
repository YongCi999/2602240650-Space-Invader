using UnityEngine;
using UnityEngine.SceneManagement;

public class Invasion : MonoBehaviour
{
    public Invader[] prefabs;
    public int rows = 5;
    public int columns = 5;
    public AnimationCurve speed;
    public Projectile missilePrefab;
    public float missileAttackRate = 1.0f;
    public int amountKilled{get; private set;}
    public int amountAlive => this.totalEnemy - this.amountKilled;
    public int totalEnemy => this.rows * this.columns;
    public float percentKilled => (float)this.amountKilled / (float)this.totalEnemy;

    private Vector3 direction = Vector2.right;

    private void Awake()
    {
        for (int row = 0; row < this.rows; row++)
        {
            float width = 1.5f * (this.columns - 1);
            float height = 1.5f * (this.rows - 1);
            Vector2 centering = new Vector2(-width / 1.5f, -height / 1.5f);
            Vector3 rowPosition = new Vector3(centering.x, centering.y + (row * 1.5f), 0.0f);

            for (int col = 0; col < this.columns; col++)
            {
                Invader enemy = Instantiate(this.prefabs[row], this.transform);
                enemy.killed += EnemyKilled;
                Vector3 position = rowPosition;
                position.x += col * 1.5f;
                enemy.transform.localPosition = position;
            }
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(MissileAttack), this.missileAttackRate, this.missileAttackRate);
    }

    private void Update()
    {
        this.transform.position += direction * this.speed.Evaluate(this.percentKilled) * Time.deltaTime;

        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        foreach (Transform enemy in this.transform)
        {
            if (!enemy.gameObject.activeInHierarchy)
            {
                continue;
            }
            if (direction == Vector3.right && enemy.position.x > rightEdge.x)
            {
                AdvanceRow();
            } else if (direction == Vector3.left && enemy.position.x < leftEdge.x)
            {
                AdvanceRow();
            }
        }
    }

    private void AdvanceRow()
    {
        direction.x *= -1.0f;
        Vector3 position = this.transform.position;
        position.y -= 1.0f;
        this.transform.position = position;
    }

    private void MissileAttack()
    {
        foreach (Transform enemy in this.transform)
        {
            if (!enemy.gameObject.activeInHierarchy)
            {
                continue;
            }
            if (Random.value < (1.0f / (float)this.amountAlive))
            {
                Instantiate(this.missilePrefab, enemy.position, Quaternion.identity);
                break;
            }
        }
    }
    private void EnemyKilled()
    {
        this.amountKilled++;
        if(this.amountKilled >= this.totalEnemy)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

}
