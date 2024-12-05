using Godot;
using System;

public partial class CameraController : Camera3D
{
	public const float Sensitivity = 0.001f;
	public float verticalDirection = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseMotion motionEvent) {
			float deltaRotation = - motionEvent.Relative.Y * Sensitivity;
			if (verticalDirection + deltaRotation > Mathf.Pi/2) {
				deltaRotation = Mathf.Pi/2 - verticalDirection;
			} else if (verticalDirection + deltaRotation < -Mathf.Pi/2) {
				deltaRotation = -Mathf.Pi/2 - verticalDirection;
			}
			verticalDirection += deltaRotation;

			Transform3D transform = Transform;
			transform.Basis = transform.Basis.Rotated(Vector3.Right, deltaRotation);

			Transform = transform;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
