using System.Collections;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] float _FadeOutWaitTimer = 0.5f;
    [SerializeField] float _FadeOutTimer = 0.5f;
    [SerializeField] float _FadeInWaitTimer = 0.5f;
    [SerializeField] float _FadeInTimer = 0.5f;
    [SerializeField] float _MoveTimer = 0.1f;
    [SerializeField] public SkinnedMeshRenderer Renderer;
    [SerializeField] Material PlayerMaterial;
    [SerializeField] Material PlayerFadeMaterial;
    private bool _isInAmination = false;
    private bool _isInMovement = false;
    public bool IsInAmination { get { return _isInAmination; } }
    private Vector3 _shadowPosition;
    public bool IsInMovement { get { return _isInMovement; } }
    public Vector3 ShadowPosition { get { return _shadowPosition; } set { _shadowPosition = value; } }

    public IEnumerator TransformToShadowAnim()
    {
        _isInAmination = true;
        yield return new WaitForSeconds(_FadeOutWaitTimer);
        Renderer.material = PlayerFadeMaterial;
        Color playerColor = PlayerFadeMaterial.color;
        for (float timer = _FadeOutTimer; timer > 0; timer -= Time.deltaTime)
        {
            playerColor.a = timer / _FadeOutTimer;
            PlayerFadeMaterial.color = playerColor;
            yield return Time.deltaTime;
        }
        Renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        gameObject.layer = LayerMask.NameToLayer("Shadows");
        _isInAmination = false;
        yield return null;
    }

    public IEnumerator TransformToPlayerAnim()
    {
        _isInAmination = true;
        Vector3 currentPos = transform.position;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        for (float timer = _FadeInWaitTimer; timer > 0; timer -= Time.deltaTime)
        {
            transform.position = Vector3.Lerp(_shadowPosition, currentPos, timer / _FadeInWaitTimer);
            yield return Time.deltaTime;
        }
        Vector3 targetPos = _shadowPosition + Vector3.back;
        Color playerColor = PlayerFadeMaterial.color;
        Renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        for (float timer = _FadeInTimer; timer > 0; timer -= Time.deltaTime)
        {
            transform.position = Vector3.Lerp(targetPos, _shadowPosition, timer / _FadeInTimer);
            playerColor.a = 1 - timer / _FadeInTimer;
            PlayerFadeMaterial.color = playerColor;
            yield return Time.deltaTime;
        }
        playerColor.a = 1.0f;
        PlayerFadeMaterial.color = playerColor;
        Renderer.material = PlayerMaterial;
        _isInAmination = false;
        yield return null;
    }

    public void MovePlayerDepthTo(Vector2 deltaPos)
    {
        _isInMovement = true;
        StartCoroutine(MovePlayerDepth(deltaPos));
    }
    private IEnumerator MovePlayerDepth(Vector2 deltaPos)
    {
        Vector2 currentPos = new Vector2(transform.position.y, transform.position.z);
        for (float timer = _MoveTimer; timer > 0; timer -= Time.deltaTime)
        {
            Vector2 local = Vector2.Lerp(deltaPos, currentPos, timer / _MoveTimer);
            transform.position = new Vector3(transform.position.x, local.x, local.y);
            yield return Time.deltaTime;
        }
        _isInMovement = false;
        yield return null;
    }
}
