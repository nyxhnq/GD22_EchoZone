using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    private CharacterController characterController;
    private float originalHeight;
    public float crouchHeight = 1.0f;
    public float standHeight = 2.0f;

    public float crouchSpeed = 5f;
    private bool isCrouching = false;

    public Transform cameraTransform;
    public float standingCamY = 0.5f; 
    public float crouchingCamY = -0.5f; 

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController not found!");
            return;
        }
        originalHeight = characterController.height;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = false;
        }

        float targetHeight = isCrouching ? crouchHeight : standHeight;
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, crouchSpeed * Time.deltaTime);

        Vector3 targetCenter = isCrouching ? new Vector3(0, crouchHeight / 2f, 0) : new Vector3(0, standHeight / 2f, 0);
        characterController.center = Vector3.Lerp(characterController.center, targetCenter, crouchSpeed * Time.deltaTime);

        if (cameraTransform != null)
        {
            float targetCamY = isCrouching ? crouchingCamY : standingCamY;
            Vector3 targetCamPos = new Vector3(cameraTransform.localPosition.x, targetCamY, cameraTransform.localPosition.z);
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetCamPos, crouchSpeed * Time.deltaTime);
        }
    }
}
