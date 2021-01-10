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
    public PlayerAction Jump;
    public PlayerAction TargetEnemy;
    public PlayerAction TargetPlayerOne;
    public PlayerAction TargetPlayerTwo;
    public PlayerAction TargetPlayerThree;
    public PlayerAction TargetPlayerFour;
    public PlayerAction Start;
    public PlayerAction Pause;
    public PlayerAction Restart;
    public PlayerAction ToggleInfo;
    public PlayerAction ToggleUIText;
    public PlayerAction NumpadOne;
    public PlayerAction NumpadTwo;
    public PlayerAction NumpadThree;
    public PlayerAction NumpadFour;
    public PlayerTwoAxisAction Movement;

    public bool myIsController = false;


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
        Jump = CreatePlayerAction("Jump");
        TargetEnemy = CreatePlayerAction("TargetEnemy");
        TargetPlayerOne = CreatePlayerAction("TargetPlayerOne");
        TargetPlayerTwo = CreatePlayerAction("TargetPlayerTwo");
        TargetPlayerThree = CreatePlayerAction("TargetPlayerThree");
        TargetPlayerFour = CreatePlayerAction("TargetPlayerFour");
        Start = CreatePlayerAction("Start");
        Pause = CreatePlayerAction("Pause");
        Restart = CreatePlayerAction("Restart");
        ToggleInfo = CreatePlayerAction("ToggleSpellInfo");
        ToggleUIText = CreatePlayerAction("ToggleUIText");
        NumpadOne = CreatePlayerAction("NumpadOne");
        NumpadTwo = CreatePlayerAction("NumpadTwo");
        NumpadThree = CreatePlayerAction("NumpadThree");
        NumpadFour = CreatePlayerAction("NumpadFour");
        Movement = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
    }


    public static PlayerControls CreateWithKeyboardBindings()
    {
        var actions = new PlayerControls();

        actions.Action1.AddDefaultBinding(Key.DownArrow);
        actions.Action2.AddDefaultBinding(Key.RightArrow);
        actions.Action2.AddDefaultBinding(Key.Backspace);
        actions.Action3.AddDefaultBinding(Key.LeftArrow);
        actions.Action4.AddDefaultBinding(Key.UpArrow);

        actions.Action3.AddDefaultBinding(Key.Backspace);

        actions.Up.AddDefaultBinding(Key.W);
        actions.Down.AddDefaultBinding(Key.S);
        actions.Left.AddDefaultBinding(Key.A);
        actions.Right.AddDefaultBinding(Key.D);

        actions.Jump.AddDefaultBinding(Key.Space);

        actions.TargetEnemy.AddDefaultBinding(Key.Tab);
        actions.TargetPlayerOne.AddDefaultBinding(Key.Key1);
        actions.TargetPlayerTwo.AddDefaultBinding(Key.Key2);
        actions.TargetPlayerThree.AddDefaultBinding(Key.Key3);
        actions.TargetPlayerFour.AddDefaultBinding(Key.Key4);

        actions.Start.AddDefaultBinding(Key.Return);
        actions.Pause.AddDefaultBinding(Key.Escape);
        actions.Restart.AddDefaultBinding(Key.R);

        actions.ToggleInfo.AddDefaultBinding(Key.I);
        actions.ToggleUIText.AddDefaultBinding(Key.U);

        actions.NumpadOne.AddDefaultBinding(Key.Pad1);
        actions.NumpadTwo.AddDefaultBinding(Key.Pad2);
        actions.NumpadThree.AddDefaultBinding(Key.Pad3);
        actions.NumpadFour.AddDefaultBinding(Key.Pad4);

        actions.myIsController = false;

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

        actions.TargetEnemy.AddDefaultBinding(InputControlType.LeftTrigger);
        actions.TargetEnemy.AddDefaultBinding(InputControlType.LeftBumper);
        actions.TargetPlayerOne.AddDefaultBinding(InputControlType.RightStickLeft);
        actions.TargetPlayerTwo.AddDefaultBinding(InputControlType.RightStickUp);
        actions.TargetPlayerThree.AddDefaultBinding(InputControlType.RightStickDown);
        actions.TargetPlayerFour.AddDefaultBinding(InputControlType.RightStickRight);

        actions.Jump.AddDefaultBinding(InputControlType.RightTrigger);
        actions.Jump.AddDefaultBinding(InputControlType.RightBumper);

        actions.Start.AddDefaultBinding(InputControlType.Start);
        actions.Pause.AddDefaultBinding(InputControlType.Start);

        actions.Restart.AddDefaultBinding(InputControlType.Back);

        actions.ToggleInfo.AddDefaultBinding(InputControlType.DPadUp);
        actions.ToggleUIText.AddDefaultBinding(InputControlType.DPadRight);

        actions.NumpadFour.AddDefaultBinding(InputControlType.DPadLeft);

        actions.myIsController = true;

        return actions;
    }

    public void Vibrate(float anIntensity)
    {
        if (myIsController)
            Device.Vibrate(anIntensity, anIntensity);
    }

    public void Vibrate(float aLeftIntensity, float aRightIntensity)
    {
        if (myIsController)
            Device.Vibrate(aLeftIntensity, aRightIntensity);
    }

    public void StopVibrating()
    {
        if (myIsController)
            Device.StopVibration();
    }
}