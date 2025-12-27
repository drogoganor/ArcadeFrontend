using XInput.Wrapper;

namespace ArcadeFrontend;

public enum XButton
{
    DPadLeft,
    DPadRight,
    DPadUp,
    DPadDown,
    A,
    B,
    X,
    Y,
    Back,
    Start,
    LBumper,
    RBumper
}

public static class XInputTracker
{
    private static readonly HashSet<XButton> _currentlyPressedButtons = [];
    private static readonly HashSet<XButton> _newButtonsThisFrame = [];

    private static readonly Dictionary<XButton, XStateButtonPairing> _buttonsState = new()
    {
        {
            XButton.DPadLeft,
            new()
        },
        {
            XButton.DPadRight,
            new()
        },
        {
            XButton.DPadUp,
            new()
        },
        {
            XButton.DPadDown,
            new()
        },
        {
            XButton.A,
            new()
        },
        {
            XButton.B,
            new()
        },
        {
            XButton.X,
            new()
        },
        {
            XButton.Y,
            new()
        },
        {
            XButton.Back,
            new()
        },
        {
            XButton.Start,
            new()
        },
        {
            XButton.LBumper,
            new()
        },
        {
            XButton.RBumper,
            new()
        }
    };

    private class XStateButtonPairing
    {
        public bool IsButtonDown { get; set; }
        public bool IsButtonUp { get; set; }
    }

    public static bool GetButton(XButton button)
    {
        return _currentlyPressedButtons.Contains(button);
    }

    public static bool GetButtonDown(XButton button)
    {
        return _newButtonsThisFrame.Contains(button);
    }

    public static void UpdateXInput()
    {
        _newButtonsThisFrame.Clear();

        if (!X.IsAvailable)
            return;

        var gamepad = X.Gamepad_1;
        if (!gamepad.Update())
            return;

        if (!gamepad.IsConnected)
            return;

        var state = _buttonsState[XButton.DPadLeft];
        state.IsButtonDown = gamepad.Dpad_Left_down;
        state.IsButtonUp = gamepad.Dpad_Left_up;

        state = _buttonsState[XButton.DPadRight];
        state.IsButtonDown = gamepad.Dpad_Right_down;
        state.IsButtonUp = gamepad.Dpad_Right_up;

        state = _buttonsState[XButton.DPadUp];
        state.IsButtonDown = gamepad.Dpad_Up_down;
        state.IsButtonUp = gamepad.Dpad_Up_up;

        state = _buttonsState[XButton.DPadDown];
        state.IsButtonDown = gamepad.Dpad_Down_down;
        state.IsButtonUp = gamepad.Dpad_Down_up;

        state = _buttonsState[XButton.A];
        state.IsButtonDown = gamepad.A_down;
        state.IsButtonUp = gamepad.A_up;

        state = _buttonsState[XButton.B];
        state.IsButtonDown = gamepad.B_down;
        state.IsButtonUp = gamepad.B_up;

        state = _buttonsState[XButton.X];
        state.IsButtonDown = gamepad.X_down;
        state.IsButtonUp = gamepad.X_up;

        state = _buttonsState[XButton.Y];
        state.IsButtonDown = gamepad.Y_down;
        state.IsButtonUp = gamepad.Y_up;

        state = _buttonsState[XButton.Back];
        state.IsButtonDown = gamepad.Back_down;
        state.IsButtonUp = gamepad.Back_up;

        state = _buttonsState[XButton.Start];
        state.IsButtonDown = gamepad.Start_down;
        state.IsButtonUp = gamepad.Start_up;

        state = _buttonsState[XButton.LBumper];
        state.IsButtonDown = gamepad.LBumper_down;
        state.IsButtonUp = gamepad.LBumper_up;

        state = _buttonsState[XButton.RBumper];
        state.IsButtonDown = gamepad.RBumper_down;
        state.IsButtonUp = gamepad.RBumper_up;

        foreach (var kvp in _buttonsState)
        {
            if (kvp.Value.IsButtonDown)
            {
                ButtonDown(kvp.Key);
            }
            else if (kvp.Value.IsButtonUp)
            {
                ButtonUp(kvp.Key);
            }
        }
    }

    private static void ButtonUp(XButton button)
    {
        _currentlyPressedButtons.Remove(button);
        _newButtonsThisFrame.Remove(button);
    }

    private static void ButtonDown(XButton button)
    {
        if (_currentlyPressedButtons.Add(button))
        {
            _newButtonsThisFrame.Add(button);
        }
    }
}
