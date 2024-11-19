using System;
using UnityEngine;
using UnityEngine.InputSystem;
using StateMachine;

//Klasse, die Unity Input System Events einfacher konfigurierbar macht, indem Abkürzungen von den Unity Befehlen zur Verfügung gestellt werden
public class InputSystem
{
    private static GameInputActions _input = new();    

    public void Enable()
    {
        _input.Player.WASD.Enable();
        _input.Player.HorizontalAxis.Enable();
        _input.Player.MousePosition.Enable();
        _input.Player.Jump.Enable();
        _input.Player.JumpAccelerate.Enable();
        _input.Player.Dash.Enable();
        _input.Player.GetOffToRight.Enable();
        _input.Player.GetOffToLeft.Enable();
        _input.Player.Grapple.Enable();
        _input.Player.Inventory.Enable();
        _input.Player.Escape.Enable();
        _input.Player.Attack.Enable();
        _input.Player.Heal.Enable();
        _input.Player.PickUp.Enable();
        _input.Player.Pray.Enable();
    }

    public void Disable()
    {
        _input.Player.WASD.Disable();
        _input.Player.HorizontalAxis.Disable();
        _input.Player.MousePosition.Disable();
        _input.Player.Jump.Disable();
        _input.Player.JumpAccelerate.Disable();
        _input.Player.Dash.Disable();
        _input.Player.GetOffToRight.Disable();
        _input.Player.GetOffToLeft.Disable();
        _input.Player.Grapple.Disable();
        _input.Player.Inventory.Disable();
        _input.Player.Escape.Disable();
        _input.Player.Attack.Disable();
        _input.Player.Heal.Disable();
        _input.Player.PickUp.Disable();
        _input.Player.Pray.Disable();
    }

    

    //Values

    //Fasst die Methode zum Lesen des Wertes unter HorizontalAxis() zusammen
    public float HorizontalAxis()
    {
        return _input.Player.HorizontalAxis.ReadValue<float>();
    }

    public Vector2 MousePosition()
    {
        return _input.Player.MousePosition.ReadValue<Vector2>();
    }

    public Vector2 WASD()
    {
        return _input.Player.WASD.ReadValue<Vector2>();
    }

    public float VerticalAxis()
    {
        return _input.Player.WASD.ReadValue<Vector2>().y;
    }

    public float JumpAcceleration()
    {
        return _input.Player.JumpAccelerate.ReadValue<float>();
    }



    //Events

    //Stellt das performed event der Sprung-Action mit unter JumpPerformed zur Verfügung
    public event Action<InputAction.CallbackContext> JumpPerformed
    {
        add
        {
            _input.Player.Jump.performed += value;

        }

        remove
        {
            _input.Player.Jump.performed -= value;

        }
    }   

    public event Action<InputAction.CallbackContext> DashPerformed
    {
        add
        {
            _input.Player.Dash.performed += value;

        }

        remove
        {
            _input.Player.Dash.performed -= value;

        }

    }

    public event Action<InputAction.CallbackContext> GetOffRightPerformed
    {
        add
        {
            _input.Player.GetOffToRight.performed += value;
        }

        remove
        {
            _input.Player.GetOffToRight.performed -= value;
        }
    }

    public event Action<InputAction.CallbackContext> GetOffLeftPerformed
    {
        add
        {
            _input.Player.GetOffToLeft.performed += value;
        }

        remove
        {
            _input.Player.GetOffToLeft.performed -= value;
        }
    }

    public event Action<InputAction.CallbackContext> GrapplePerformed
    {
        add
        {
            _input.Player.Grapple.performed += value;
        }

        remove
        {
            _input.Player.Grapple.performed -= value;
        }
    }

    public event Action<InputAction.CallbackContext> InventoryPerformed
    {
        add
        {
            _input.Player.Inventory.performed += value;
        }

        remove
        {
            _input.Player.Inventory.performed -= value;
        }
    }

    public event Action<InputAction.CallbackContext> EscapePerformed
    {
        add
        {
            _input.Player.Escape.performed += value;
        }

        remove
        {
            _input.Player.Escape.performed -= value;
        }
    }

    public event Action<InputAction.CallbackContext> AttackPerformed
    {
        add
        {
            _input.Player.Attack.performed += value;
        }

        remove
        {
            _input.Player.Attack.performed -= value;
        }
    }

    public event Action<InputAction.CallbackContext> HealPerformed
    {
        add
        {
            _input.Player.Heal.performed += value;
        }
        remove
        {
            _input.Player.Heal.performed -= value;
        }
    }

    public event Action<InputAction.CallbackContext> PickUpPerformed
    {
        add
        {
            _input.Player.PickUp.performed += value;
        }

        remove
        {
            _input.Player.PickUp.performed -= value;
        }

    }

    public event Action<InputAction.CallbackContext> PrayPerformed
    {
        add
        {
            _input.Player.Pray.performed += value;
        }
        remove
        {
            _input.Player.Pray.performed -= value;
        }
    }
}
