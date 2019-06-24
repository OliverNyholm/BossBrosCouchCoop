using InControl;

public class PlayerControls : PlayerActionSet
{
    public PlayerAction Action1;
    public PlayerAction Action2;
    public PlayerAction Action3;
    public PlayerAction Action4;
    public PlayerAction Left;
    public PlayerAction Right;
    public PlayerAction Up;
    public PlayerAction Down;
    public PlayerAction Shift;
    public PlayerAction Jump;
    public PlayerAction PlayerOne;
    public PlayerAction PlayerTwo;
    public PlayerAction PlayerThree;
    public PlayerAction PlayerFour;
    public PlayerAction TargetEnemy;
    public PlayerAction Start;
    public PlayerAction Pause;
    public PlayerTwoAxisAction Movement;


    public PlayerControls()
    {
        Action1 = CreatePlayerAction("Action1");
        Action2 = CreatePlayerAction("Action2");
        Action3 = CreatePlayerAction("Action3");
        Action4 = CreatePlayerAction("Action4");
        Left = CreatePlayerAction("Left");
        Right = CreatePlayerAction("Right");
        Up = CreatePlayerAction("Up");
        Down = CreatePlayerAction("Down");
        Shift = CreatePlayerAction("Shift");
        Jump = CreatePlayerAction("Jump");
        PlayerOne = CreatePlayerAction("PlayerOne");
        PlayerTwo = CreatePlayerAction("PlayerTwo");
        PlayerThree = CreatePlayerAction("PlayerThree");
        PlayerFour = CreatePlayerAction("PlayerFour");
        TargetEnemy = CreatePlayerAction("TargetEnemy");
        Start = CreatePlayerAction("Start");
        Pause = CreatePlayerAction("Pause");
        Movement = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
    }


    public static PlayerControls CreateWithKeyboardBindings()
    {
        var actions = new PlayerControls();

        actions.Action1.AddDefaultBinding(Key.Key1);
        actions.Action2.AddDefaultBinding(Key.Key3);
        actions.Action3.AddDefaultBinding(Key.Key2);
        actions.Action4.AddDefaultBinding(Key.Key4);

        actions.Up.AddDefaultBinding(Key.W);
        actions.Down.AddDefaultBinding(Key.S);
        actions.Left.AddDefaultBinding(Key.A);
        actions.Right.AddDefaultBinding(Key.D);

        actions.Shift.AddDefaultBinding(Key.Shift);
        actions.Jump.AddDefaultBinding(Key.Space);

        actions.PlayerOne.AddDefaultBinding(Key.F1);
        actions.PlayerTwo.AddDefaultBinding(Key.F2);
        actions.PlayerThree.AddDefaultBinding(Key.F3);
        actions.PlayerFour.AddDefaultBinding(Key.F4);
        actions.TargetEnemy.AddDefaultBinding(Key.Tab);

        actions.Start.AddDefaultBinding(Key.Return);
        actions.Pause.AddDefaultBinding(Key.Escape);

        return actions;
    }


    public static PlayerControls CreateWithJoystickBindings()
    {
        var actions = new PlayerControls();

        actions.Action1.AddDefaultBinding(InputControlType.Action1);
        actions.Action2.AddDefaultBinding(InputControlType.Action2);
        actions.Action3.AddDefaultBinding(InputControlType.Action3);
        actions.Action4.AddDefaultBinding(InputControlType.Action4);

        actions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
        actions.Down.AddDefaultBinding(InputControlType.LeftStickDown);
        actions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
        actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);

        actions.Shift.AddDefaultBinding(InputControlType.LeftTrigger);
        actions.Shift.AddDefaultBinding(InputControlType.RightTrigger);

        actions.Jump.AddDefaultBinding(InputControlType.RightBumper);

        actions.PlayerOne.AddDefaultBinding(InputControlType.RightStickUp);
        actions.PlayerTwo.AddDefaultBinding(InputControlType.RightStickRight);
        actions.PlayerThree.AddDefaultBinding(InputControlType.RightStickDown);
        actions.PlayerFour.AddDefaultBinding(InputControlType.RightStickLeft);
        actions.TargetEnemy.AddDefaultBinding(InputControlType.LeftBumper);

        actions.Start.AddDefaultBinding(InputControlType.Command);
        actions.Pause.AddDefaultBinding(InputControlType.Command);

        return actions;
    }
}