namespace MultiplayerWithBindingsExample
{
    using InControl;


    public class PlayerActions : PlayerActionSet
    {
        public PlayerAction Green;
        public PlayerAction Red;
        public PlayerAction Blue;
        public PlayerAction Yellow;
        public PlayerAction Left;
        public PlayerAction Right;
        public PlayerAction Up;
        public PlayerAction Down;
        public PlayerAction Shift;
        public PlayerAction Jump;
        public PlayerTwoAxisAction Movement;


        public PlayerActions()
        {
            Green = CreatePlayerAction("Green");
            Red = CreatePlayerAction("Red");
            Blue = CreatePlayerAction("Blue");
            Yellow = CreatePlayerAction("Yellow");
            Left = CreatePlayerAction("Left");
            Right = CreatePlayerAction("Right");
            Up = CreatePlayerAction("Up");
            Down = CreatePlayerAction("Down");
            Shift = CreatePlayerAction("Shift");
            Movement = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
        }


        public static PlayerActions CreateWithKeyboardBindings()
        {
            var actions = new PlayerActions();

            actions.Green.AddDefaultBinding(Key.Key1);
            actions.Red.AddDefaultBinding(Key.Key2);
            actions.Blue.AddDefaultBinding(Key.Key3);
            actions.Yellow.AddDefaultBinding(Key.Key4);

            actions.Up.AddDefaultBinding(Key.W);
            actions.Down.AddDefaultBinding(Key.S);
            actions.Left.AddDefaultBinding(Key.A);
            actions.Right.AddDefaultBinding(Key.D);

            actions.Shift.AddDefaultBinding(Key.Shift);

            return actions;
        }


        public static PlayerActions CreateWithJoystickBindings()
        {
            var actions = new PlayerActions();

            actions.Green.AddDefaultBinding(InputControlType.Action1);
            actions.Red.AddDefaultBinding(InputControlType.Action2);
            actions.Blue.AddDefaultBinding(InputControlType.Action3);
            actions.Yellow.AddDefaultBinding(InputControlType.Action4);

            actions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
            actions.Down.AddDefaultBinding(InputControlType.LeftStickDown);
            actions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
            actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);

            actions.Shift.AddDefaultBinding(InputControlType.LeftTrigger);
            actions.Shift.AddDefaultBinding(InputControlType.RightTrigger);

            return actions;
        }
    }
}

