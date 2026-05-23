using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction pauseAction;
    bool isPaused = false;

    [SerializeField] GameObject Menu;

    public void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        pauseAction = playerInput.actions.FindAction("Pause");
        Menu.SetActive(false);

        pauseAction.performed += OnPausePressed;
    }
    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (isPaused)
        {
            SetGameUnPaused();
        }
        else
        {
            SetGamePaused();
        }
    }
    public void SetGamePaused()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Menu.SetActive(true);
    }
    public void SetGameUnPaused()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Menu.SetActive(false);
    }
}
