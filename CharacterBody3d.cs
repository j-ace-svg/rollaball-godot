using Godot;
using System;

public partial class CharacterBody3d : CharacterBody3D
{
	public const float GroundSpeed = 60f;
	public const float GroundSpeedLimit = 30f;
	public const float JumpVelocity = 270f;
	public const float GravityStrength = 0.1f;
	public const float Sensitivity = 0.001f;
	public const float MaxGroundVelocity = 5f;
	public const float MaxAirVelocity = 7f;

	private float direction = 0;

	public override void _Ready() {
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseMotion motionEvent) {
			float deltaRotation = - motionEvent.Relative.X * Sensitivity;
			direction += deltaRotation;
			Transform3D transform = Transform;
			transform.Basis = transform.Basis.Rotated(Vector3.Up, deltaRotation);

			Transform = transform;
		}
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

		float speed = GroundSpeed;
		float speedLimit = GroundSpeedLimit;
		if (!IsOnFloor()) {
			speed *= 10;
			speedLimit *= 10;
		}

		if (Input.IsActionPressed("move_right")) {
			acceleration.X += speed;
		}
		if (Input.IsActionPressed("move_left")) {
			acceleration.X -= speed;
		}
		if (Input.IsActionPressed("move_up")) {
			acceleration.Z -= speed;
		}
		if (Input.IsActionPressed("move_down")) {
			acceleration.Z += speed;
		}
		if (Input.IsActionPressed("jump") && IsOnFloor()) {
			acceleration.Y += JumpVelocity;
		}

		/*float deceleration = 0.1f;
		if (IsOnFloor()) {
			deceleration = 0.01f;
		}
		deceleration = Mathf.Pow(deceleration, (float)delta);*/
		float projectedVelocity = 0;
		if (acceleration.Length() > 0) {
			projectedVelocity = acceleration.Dot(Velocity) / acceleration.Length();
		}
		projectedVelocity = Mathf.Min(projectedVelocity, speedLimit);

		acceleration = acceleration.Rotated(Vector3.Up, direction);

		Velocity = new Vector3(Velocity.X + acceleration.X * (float)delta, Velocity.Y - GravityStrength + acceleration.Y * (float)delta, Velocity.Z + acceleration.Z * (float)delta);
		MoveAndSlide();
	}
}
