using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorScript : MonoBehaviour
{
    [SerializeField] private float speed;
    private Material _mat;
    private PlayerMovement _player;
    private bool _playerHit = false;
    private bool _playerTouched = false;
    private bool _reverse = false;

    public bool IsReverse { get { return _reverse; } }

    private void Start()
    {
        _mat = gameObject.GetComponent<MeshRenderer>().material;
    }

    private void FixedUpdate()
    {
        _mat.mainTextureOffset = new Vector2(0, Time.realtimeSinceStartup * speed * -0.5f);
        if (!_playerHit && _playerTouched)
        {
            if (_player.IsGrounded || _player.GetComponent<PlayerStatus>().IsShadow)
            {
                _playerTouched = false;
            }
            else
            {
                _player.transform.position += transform.forward * (_reverse ? -speed : speed) * Time.deltaTime;
            }
        }
        _playerHit = false;
    }

    public void Reverse()
    {
        _reverse = !_reverse;
        _mat.mainTextureScale = new Vector2(_mat.mainTextureScale.x, -_mat.mainTextureScale.y);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.rigidbody)
        {
            Component anim = collision.gameObject.GetComponent(typeof(PlayerStatus));
            if (!anim)
            {
                collision.rigidbody.velocity = transform.forward * (_reverse ? -speed : speed);
                //collision.rigidbody.position += transform.forward * (_reverse ? -speed : speed) * Time.deltaTime;
            }
            else if (!((PlayerStatus)anim).IsShadow)
            {
                collision.rigidbody.position += transform.forward * (_reverse ? -speed : speed) * Time.deltaTime;
                _playerHit = true;
                _playerTouched = true;
                _player = collision.gameObject.GetComponent<PlayerMovement>();
            }
        }
    }
}
