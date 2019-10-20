using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField]
    private float _speed = 7.5f;
    GameObject _closesEnemy;
    Enemy _enemyScript;
    float _distance = 30f;

    // Start is called before the first frame update
    void Start()
    {
        SearchClosesEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        ChaseEnemy();
    }

    void SearchClosesEnemy()
    {
        _closesEnemy = null;
        _enemyScript = null;

        GameObject[] _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < _enemies.Length; ++i)
        {
            Vector3 enemyPosition = _enemies[i].transform.position;
            if (enemyPosition.y + 0.5f > transform.position.y)
            {
                if (Vector3.Distance(enemyPosition, transform.position) < _distance)
                    _closesEnemy = _enemies[i];
            }
        }

        if (_closesEnemy != null)
            _enemyScript = _closesEnemy.GetComponent<Enemy>();
    }

    void ChaseEnemy()
    {
        if (_closesEnemy == null)
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _closesEnemy.transform.position, _speed * Time.deltaTime);
            Vector3 norTar = (_closesEnemy.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(norTar.y, norTar.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, 0, angle - 90);

            if (_enemyScript.Hitted())
                SearchClosesEnemy();
        }

        ValidatePosition();
    }

    void ValidatePosition()
    {
        if (transform.position.y < -8.0f)
        {
            if (transform.parent != null)
                Destroy(this.transform.parent.gameObject);
            Destroy(this.gameObject);
        }
        else if (transform.position.y > 8.0f)
        {
            if (transform.parent != null)
                Destroy(this.transform.parent.gameObject);
            Destroy(this.gameObject);
        }

        if (transform.position.x < -11.0f)
        {
            if (transform.parent != null)
                Destroy(this.transform.parent.gameObject);
            Destroy(this.gameObject);
        }
        else if (transform.position.x > 11.0f)
        {
            if (transform.parent != null)
                Destroy(this.transform.parent.gameObject);
            Destroy(this.gameObject);
        }
    }
}
