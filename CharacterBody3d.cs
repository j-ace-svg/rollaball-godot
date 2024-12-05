using Godot;
using System;

public partial class CharacterBody3d : CharacterBody3D
{
	public const float GroundSpeed = 100f;
	public const float GroundSpeedLimit = 6f;
	public const float JumpVelocity = 270f;
	public const float GravityStrength = 0.1f;
	public const float Sensitivity = 0.001f;
	public float framesGrounded = 0f;

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
			speed *= 3;
			speedLimit /= 2;
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

		float projectedVelocity = 0;
		Vector2 flatAcceleration = new Vector2(acceleration.X, acceleration.Z);
		flatAcceleration = flatAcceleration.Rotated(-direction);
		Vector2 flatVelocity = new Vector2(Velocity.X, Velocity.Z);
		if (acceleration.Length() > 0) {
			projectedVelocity = flatAcceleration.Dot(flatVelocity) / flatAcceleration.Length();
		}
		if (projectedVelocity > speedLimit) {
			// Too fast, do nothing
		} else if (projectedVelocity > speedLimit - flatAcceleration.Length() * delta && projectedVelocity < speedLimit) {
			flatVelocity = flatVelocity + (speedLimit - projectedVelocity) * flatAcceleration.Normalized();
		} else if (projectedVelocity < speedLimit - flatAcceleration.Length() * delta) {
			flatVelocity = flatVelocity + flatAcceleration * (float)delta;
		}
		if (IsOnFloor()) {
			framesGrounded += 1;
			GD.Print("Floor");
		} else {
			framesGrounded = 0;
			GD.Print("Air");
		}
		if (framesGrounded > 1) {
			float deceleration = 0.001f;
			deceleration = Mathf.Pow(deceleration, (float)delta);
			flatVelocity *= deceleration;
		}

		Velocity = new Vector3(flatVelocity.X, Velocity.Y - GravityStrength + acceleration.Y * (float)delta, flatVelocity.Y);
		MoveAndSlide();
	}
}
