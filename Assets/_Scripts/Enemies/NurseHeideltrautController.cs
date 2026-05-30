using UnityEngine;

/// <summary>
/// Ћогика анимаций и преследовани€ дл€ NurseHeideltraut:
/// - когда персонаж неподвижен и игрок далеко -> Nurse_Punch_01
/// - когда игрок близко (<= nearDistance) и игрок стоит -> Nurse_Idle_01
/// - когда игрок впритык (<= closeDistance) -> Nurse_Walk_01 и преследование игрока до ухода за chaseStopDistance
/// —крипт не использует NavMesh Ч движение реализовано через MoveTowards.
/// Ќазначьте Animator и, опционально, Transform игрока (если не указан Ч ищем по тегу "Player").
/// </summary>
public class NurseHeideltrautController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform player; // если не задан Ч будет найден по тегу "Player"

    [Header("Distances (метры)")]
    [Tooltip("<= closeDistance Ч впритык: запуск преследовани€ и Nurse_Walk_01")]
    public float closeDistance = 1.0f;
    [Tooltip("<= nearDistance Ч близко: Nurse_Idle_01, если игрок стоит")]
    public float nearDistance = 3.0f;
    [Tooltip("≈сли игрок удалитс€ дальше, преследование прекратитс€")]
    public float chaseStopDistance = 4.0f;

    [Header("Chase")]
    public float chaseSpeed = 2.0f;

    [Header("Movement detection")]
    [Tooltip("ѕорог смещени€ (м) дл€ признани€ как движение")]
    public float movementThreshold = 0.02f;
    [Tooltip("»нтервал проверки позиций (с)")]
    public float positionCheckInterval = 0.12f;

    [Header("Animator state names")]
    [Tooltip("»м€ состо€ни€ удара (пример: Nurse_Punch_01)")]
    public string punchStateName = "Nurse_Punch_01";
    [Tooltip("»м€ состо€ни€ отдыха (пример: Nurse_Idle_01)")]
    public string idleStateName = "Nurse_Idle_01";
    [Tooltip("»м€ состо€ни€ ходьбы (пример: Nurse_Walk_01)")]
    public string walkStateName = "Nurse_Walk_01";
    [Tooltip("»м€ сло€ в Animator (обычно 'Base Layer')")]
    public string layerName = "Base Layer";

    private Vector3 _lastNursePos;
    private Vector3 _lastPlayerPos;
    private float _checkTimer;
    private float _sqrMoveThreshold;
    private int _layerIndex;

    private bool _isChasing;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (player == null)
        {
            var go = GameObject.FindWithTag("Player");
            if (go != null) player = go.transform;
        }

        _lastNursePos = transform.position;
        _lastPlayerPos = player != null ? player.position : Vector3.zero;
        _checkTimer = 0f;
        _sqrMoveThreshold = movementThreshold * movementThreshold;
        _layerIndex = animator != null ? Mathf.Max(0, animator.GetLayerIndex(layerName)) : 0;
    }

    private void Update()
    {
        if (animator == null || player == null) return;

        // ќбновл€ем детекторы движени€ периодически
        _checkTimer += Time.deltaTime;
        bool nurseIsMoving;
        bool playerIsMoving;

        if (_checkTimer >= positionCheckInterval)
        {
            nurseIsMoving = (transform.position - _lastNursePos).sqrMagnitude > _sqrMoveThreshold;
            playerIsMoving = (player.position - _lastPlayerPos).sqrMagnitude > _sqrMoveThreshold;

            _lastNursePos = transform.position;
            _lastPlayerPos = player.position;
            _checkTimer = 0f;
        }
        else
        {
            // между интервалами берЄм текущую оценку дл€ отзывчивости
            nurseIsMoving = (transform.position - _lastNursePos).sqrMagnitude > _sqrMoveThreshold;
            playerIsMoving = (player.position - _lastPlayerPos).sqrMagnitude > _sqrMoveThreshold;
        }

        float dist = Vector3.Distance(player.position, transform.position);

        // ”правление началом/окончанием преследовани€
        if (!_isChasing && dist <= closeDistance)
        {
            _isChasing = true;
        }
        else if (_isChasing && dist > chaseStopDistance)
        {
            _isChasing = false;
        }

        if (_isChasing)
        {
            // ѕреследование: движение к игроку и проигрывание walk состо€ни€
            Vector3 target = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.position = Vector3.MoveTowards(transform.position, target, chaseSpeed * Time.deltaTime);

            PlayStateIfNotCurrent(walkStateName);

            // ќбновл€ем last position, чтобы не считать персонажа неподвижным
            _lastNursePos = transform.position;
            _lastPlayerPos = player.position;
            _checkTimer = 0f;
            return;
        }

        // ≈сли не преследуем Ч выбираем состо€ние по правилам:
        // 1) ≈сли игрок впритык (<= closeDistance) Ч Walk (без преследовани€)
        // 2) »наче если игрок в пределах nearDistance и »√–ќ  неподвижен Ч Idle
        // 3) »наче если Nurse неподвижна Ч Punch
        if (dist <= closeDistance)
        {
            PlayStateIfNotCurrent(walkStateName);
            return;
        }

        if (dist <= nearDistance && !playerIsMoving)
        {
            PlayStateIfNotCurrent(idleStateName);
            return;
        }

        if (!nurseIsMoving)
        {
            PlayStateIfNotCurrent(punchStateName);
            return;
        }

        // ¬ остальных случа€х (nurse движетс€) Ч не вмешиваемс€, пусть другие системы управл€ют анимацией.
    }

    private void PlayStateIfNotCurrent(string stateName)
    {
        if (animator == null) return;
        string fullName = $"{layerName}.{stateName}";
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(_layerIndex);
        if (!info.IsName(fullName))
        {
            // ѕереключаем мгновенно (нормализуем врем€ на 0)
            animator.Play(fullName, _layerIndex, 0f);
        }
    }
}