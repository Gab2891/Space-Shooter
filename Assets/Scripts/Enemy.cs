using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum MoveDirection { DOWN, RIGHT, LEFT};
    [SerializeField]
    private float _speed = 4.0f;
    private Player _player;
    private Animator _animator;
    [SerializeField]
    private AudioClip _explosionSoundClip;
    private AudioSource _audioSource;
    [SerializeField]
    private GameObject _laserPrefab;
    private float _fireRate = 3.0f;
    private float _canFIre = -1.0f;
    private bool _hitted = false;
    private Vector3 movDirection = Vector3.down;

    // Start is called before the first frame update
    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if(_player == null)
            Debug.LogError("The Player is null");

        _animator = GetComponent<Animator>();
        if (_animator == null)
            Debug.LogError("The Animator is null");

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            Debug.LogError("Audio source is null");
        else
            _audioSource.clip = _explosionSoundClip;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > _canFIre && !_hitted)
            FireLaser();
    }

    public void SetMovDirection(MoveDirection direction)
    {
        switch (direction) {
            case MoveDirection.DOWN:
                movDirection = Vector3.down;
                break;
            case MoveDirection.LEFT:
                movDirection = Vector3.left;
                break;
            case MoveDirection.RIGHT:
                movDirection = Vector3.right;
                break;
        }
    }

    private void FireLaser()
    {
        _fireRate = Random.Range(3.0f, 7.0f);
        _canFIre = Time.time + _fireRate;
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
            lasers[i].AssignEnemyLaser();
    }

    void CalculateMovement()
    {
        transform.Translate(movDirection * _speed * Time.deltaTime);

        if (movDirection == Vector3.down && transform.position.y < -5.5f)
        {
            transform.position = new Vector3(Random.Range(-10.0f, 10.0f), 7.0f, 0);
        }
        else if (movDirection == Vector3.right && transform.position.x > 12.5f)
        {
            transform.position = new Vector3(-13.0f, Random.Range(0.0f, 6.0f), 0);
        }
        else if (movDirection == Vector3.left && transform.position.x < -12.5f)
        {
            transform.position = new Vector3(13.0f, Random.Range(0.0f, 6.0f), 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _hitted = true;
            gameObject.tag = "Untagged";
            Player player = other.GetComponent<Player>();
            if (player != null)
                player.Damage(Player.DamageReceivedBy.ENEMY_COLLISION, this.gameObject);
            _animator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(this.gameObject, 2.8f);
        }
        else if (other.gameObject.CompareTag("Laser"))
        {
            _hitted = true;
            gameObject.tag = "Untagged";
            if (_player != null)
                _player.AddScore(10);
            Destroy(other.gameObject);
            _animator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }
    }

    public bool Hitted()
    {
        return _hitted;
    }
}
