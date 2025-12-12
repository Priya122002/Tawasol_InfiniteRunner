using UnityEngine;

public class UIInputManager : MonoBehaviour
{
    public static UIInputManager Instance;

    private PlayerMovement player;

    void Awake()
    {
        Instance = this;
    }

    public void SetPlayer(PlayerMovement p)
    {
        player = p;
    }

    // UI Button → call this
    public void JumpButton()
    {
        if (player != null)
            player.Jump();
    }
}
