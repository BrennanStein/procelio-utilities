using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    public float MovementSpeed = 15;
    public float TurnMultiplier = 120;
    // Use this for initialization
    void Start()
    {

    }
    float xRot = 180;
    float yRot = 0;
    float zRot = 0;
    // Update is called once per frame
    void Update()
    {
        bool forward = Input.GetKey(KeyCode.W); //Settings.ControlsSettings.GetKey(Settings.ControlsSettings.GetInstanceCode(Settings.ControlsSettings.CONTROLS.Forward));
        bool backward = Input.GetKey(KeyCode.S);// Settings.ControlsSettings.GetKey(Settings.ControlsSettings.GetInstanceCode(Settings.ControlsSettings.CONTROLS.Backward));
        bool left = Input.GetKey(KeyCode.A);// Settings.ControlsSettings.GetKey(Settings.ControlsSettings.GetInstanceCode(Settings.ControlsSettings.CONTROLS.Left));
        bool right = Input.GetKey(KeyCode.D); //Settings.ControlsSettings.GetKey(Settings.ControlsSettings.GetInstanceCode(Settings.ControlsSettings.CONTROLS.Right));
        bool up = Input.GetKey(KeyCode.Space); //Settings.ControlsSettings.GetKey(Settings.ControlsSettings.GetInstanceCode(Settings.ControlsSettings.CONTROLS.Up));
        bool down = Input.GetKey(KeyCode.LeftShift);// Settings.ControlsSettings.GetKey(Settings.ControlsSettings.GetInstanceCode(Settings.ControlsSettings.CONTROLS.Down));
        Vector3 transFor = transform.forward;
        if (forward || backward)
        {
            Vector3 move = transFor * (forward ? 1 : -1) * MovementSpeed * Time.deltaTime;
            if (move.y < 0 ? up : down)
                move = Vector3.ProjectOnPlane(move, Vector3.up);
            else if (up != down) move += (up ? Vector3.up : Vector3.down) * MovementSpeed / 2 * Time.deltaTime;
            transform.position += move;
        }
        else
        {
            if (up != down)
                transform.position += (up ? Vector3.up : Vector3.down) * MovementSpeed / 2 * Time.deltaTime;
        }

        if (left)
        {
            transform.position += -transform.right * MovementSpeed * Time.deltaTime;
        }

        if (right)
        {
            transform.position += transform.right * MovementSpeed * Time.deltaTime;
        }

        float xrot = Time.deltaTime * TurnMultiplier * Input.GetAxis("Mouse X");
        float yrot = Time.deltaTime * -TurnMultiplier * Input.GetAxis("Mouse Y");

        xRot += xrot;
        if (xRot > 360) xRot -= 360;
        if (xRot < 0) xRot += 360;
        yRot += yrot;
        if (yRot < -89.999) yRot = -89.999f;
        if (yRot > 89.999) yRot = 89.999f;
        transform.rotation = Quaternion.Euler(yRot, xRot, zRot);

    }
}
