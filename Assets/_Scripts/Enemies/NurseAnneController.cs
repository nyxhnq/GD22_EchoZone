using UnityEngine;

public class NurseAnneController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;

    [Header("Distance thresholds")]
    [Tooltip("<= closeDistance Ч считаетс€ \"в притык\" (Anne_Walk_01)")]
    public float closeDistance = 1.0f;
    [Tooltip("<= nearDistance Ч считаетс€ \"близко, но есть рассто€ние\" (Anne_Idle_01 при неподвижном игроке)")]
    public float nearDistance = 3.0f;

    [Header("Chase (aggro) settings")]
    [Tooltip("≈сли игрок подойдет ближе или равно этому значению Ч NurseAnne начнЄт преследование")]
    public float chaseTriggerDistance = 0.8f;
    [Tooltip("≈сли игрок удалитс€ дальше этого значени€ Ч преследование прекратитс€")]
    public float chaseStopDistance = 4.0f;
    [Tooltip("—корость при преследовании (м/с)")]
    public float chaseSpeed = 2.0f;

    [Header("Movement detection")]
    [Tooltip("ѕорог перемещени€ дл€ определени€, что объект движетс€ (в метрах)")]
    public float movementThreshold = 0.02f;
    [Tooltip(" ак часто провер€ем позицию (сек) Ч уменьшите дл€ более точной детекции")]
    public float positionCheckInterval = 0.12f;

    // »мена состо€ний в Animator (точно как в вашем контроллере)
    private const string FumbleState = "NurseAnne_01|Anne_Fumble_01";
    private const string IdleState = "NurseAnne_01|Anne_Idle_01";
    private const string WalkState = "NurseAnne_01|Anne_Walk_01";

    private Vector3 lastAnnePos;
    private Vector3 lastPlayerPos;
    private float checkTimer;
    private bool anneIsMoving;
    private bool playerIsMoving;
    private float sqrMovementThreshold;

    // флаг преследовани€
    private bool isChasing;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (player == null)
        {
            var found = GameObject.FindWithTag("Player");
            if (found != null) player = found.transform;
        }

        lastAnnePos = transform.position;
        lastPlayerPos = player != null ? player.position : Vector3.zero;
        checkTimer = 0f;
        sqrMovementThreshold = movementThreshold * movementThreshold;
        isChasing = false;
    }

    private void Update()
    {
        if (player == null || animator == null) return;

        float dist = Vector3.Distance(player.position, transform.position);

        // ”правление началом/окончанием преследовани€
        if (!isChasing && dist <= chaseTriggerDistance)
        {
            isChasing = true;
        }
        else if (isChasing && dist > chaseStopDistance)
        {
            isChasing = false;
        }

        // ≈сли в режиме преследовани€ Ч двигаемс€ к игроку и играем Walk
        if (isChasing)
        {
            // передвинутьс€ к игроку без NavMesh
            Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
            Vector3 newPos = Vector3.MoveTowards(transform.position, targetPos, chaseSpeed * Time.deltaTime);
            transform.position = newPos;

            // помечаем как движущийс€, чтобы логика анимаций не переключала на Fumble
            anneIsMoving = true;

            PlayIfNotCurrent(WalkState);

            // обновл€ем lastAnnePos дл€ корректной детекции движени€ в следующем шаге
            lastAnnePos = transform.position;
            lastPlayerPos = player.position;
            checkTimer = 0f;
            return;
        }

        // ѕериодическа€ проверка перемещений (чтобы не делать вычислени€ каждый кадр)
        checkTimer += Time.deltaTime;
        if (checkTimer >= positionCheckInterval)
        {
            float anneMoved = (transform.position - lastAnnePos).sqrMagnitude;
            float playerMoved = (player.position - lastPlayerPos).sqrMagnitude;

            anneIsMoving = anneMoved > sqrMovementThreshold;
            playerIsMoving = playerMoved > sqrMovementThreshold;

            lastAnnePos = transform.position;
            lastPlayerPos = player.position;
            checkTimer = 0f;
        }
        else
        {
            // ƒл€ более отзывчивой логики используем текущее смещение относительно последней сохранЄнной позиции
            anneIsMoving = (transform.position - lastAnnePos).sqrMagnitude > sqrMovementThreshold;
            playerIsMoving = (player.position - lastPlayerPos).sqrMagnitude > sqrMovementThreshold;
        }

        // ѕравила переключени€ анимаций (когда не преследуем):
        // 1) ≈сли игрок впритык (<= closeDistance) Ч Anne_Walk_01
        // 2) »наче если игрок в пределах nearDistance и игрок неподвижен Ч Anne_Idle_01
        // 3) »наче если NurseAnne неподвижна Ч Anne_Fumble_01
        if (dist <= closeDistance)
        {
            PlayIfNotCurrent(WalkState);
        }
        else if (dist <= nearDistance && !playerIsMoving)
        {
            PlayIfNotCurrent(IdleState);
        }
        else if (!anneIsMoving)
        {
            PlayIfNotCurrent(FumbleState);
        }
    }

    private void PlayIfNotCurrent(string stateName)
    {
        var state = animator.GetCurrentAnimatorStateInfo(0);
        if (!state.IsName(stateName))
        {
            animator.Play(stateName);
        }
    }
}