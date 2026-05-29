using UnityEngine;

public class DokhuController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    [Header("Animation")]
    [Tooltip("Имя состояния в Animator (например: Dokhu_Sit_01)")]
    public string stateName = "Dokhu_Sit_01";
    [Tooltip("Имя слоя в Animator (по умолчанию Base Layer)")]
    public string layerName = "Base Layer";

    [Header("Movement detection (м/с)")]
    [Tooltip("Порог скорости ниже которого считается, что персонаж стоит")]
    public float movementThreshold = 0.01f;

    private Vector3 _lastPosition;
    private int _layerIndex;
    private int _stateHash;
    private float _sqrThreshold;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        _lastPosition = transform.position;
        _layerIndex = animator != null ? Mathf.Max(0, animator.GetLayerIndex(layerName)) : 0;
        _stateHash = Animator.StringToHash(stateName);
        _sqrThreshold = movementThreshold * movementThreshold;
    }

    private void Update()
    {
        if (animator == null) return;

        float dt = Time.deltaTime;
        if (dt <= 0f)
        {
            _lastPosition = transform.position;
            return;
        }

        Vector3 delta = transform.position - _lastPosition;
        float speedSqr = delta.sqrMagnitude / (dt * dt);

        bool isStanding = speedSqr <= _sqrThreshold;

        if (isStanding)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(_layerIndex);
            if (stateInfo.shortNameHash != _stateHash)
            {
                animator.Play(_stateHash, _layerIndex, 0f);
            }
        }

        _lastPosition = transform.position;
    }
}