using UnityEngine;

public class NurseHeideltrautController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    [Header("Animation")]
    [Tooltip("»м€ состо€ни€ в Animator, точно как в Animator (например: Nurse_Punch_01)")]
    public string stateName = "Nurse_Punch_01";
    [Tooltip("»м€ сло€ в Animator (по умолчанию Base Layer)")]
    public string layerName = "Base Layer";

    [Header("Movement detection (м/с)")]
    [Tooltip("ѕорог скорости ниже которого считаетс€, что персонаж стоит")]
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
            // ≈сли текущее состо€ние не то, которое нужно Ч принудительно поставить его.
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(_layerIndex);
            if (stateInfo.shortNameHash != _stateHash)
            {
                // Play с указанием сло€ и сбросом времени воспроизведени€ на 0
                animator.Play(_stateHash, _layerIndex, 0f);
            }
        }

        _lastPosition = transform.position;
    }
}