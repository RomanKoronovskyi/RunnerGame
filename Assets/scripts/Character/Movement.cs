using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    PlayerInput player_input;
    InputAction moveAction;

    [SerializeField] float speed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_input = GetComponent<PlayerInput>();

        moveAction = player_input.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(direction.x, direction.y) * speed * Time.deltaTime;
        transform.position = transform.position + movement;
    }
}
