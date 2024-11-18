using Godot;
using System;

public partial class CharacterBody3d : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	public const float GravityStrength = 0.1f;
	public const float Sensitivity = 0.001f;

	private float direction = 0;

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseMotion motionEvent) {
			direction += motionEvent.Relative.X * Sensitivity;
		}

		// Now just need to rotate ball to match orientation so camera moves too. Also, lock + hide cursor.
	}

	public override void _PhysicsProcess(double delta)
	{
		/*Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();*/

		Vector3 acceleration = Vector3.Zero;

		if (Input.IsActionPressed("move_right")) {
			acceleration.X += 1;
		}
		if (Input.IsActionPressed("move_left")) {
			acceleration.X -= 1;
		}
		if (Input.IsActionPressed("move_up")) {
			acceleration.Z -= 1;
		}
		if (Input.IsActionPressed("move_down")) {
			acceleration.Z += 1;
		}

		acceleration = acceleration.Rotated(Vector3.Up, direction);

		Velocity = new Vector3(acceleration.X * Speed, Velocity.Y - GravityStrength, acceleration.Z * Speed);
		MoveAndSlide();
	}
}
