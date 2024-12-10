using Godot;
using System;

public partial class RigidBody3d : RigidBody3D
{
	public const float GroundSpeed = 100f;
	public const float GroundSpeedLimit = 6f;
	public const float JumpVelocity = 270f * 2;
	public const float GravityStrength = 0.1f * 3;
	public const float Sensitivity = 0.001f;
	public float framesGrounded = 0f;
	// Normal start position: 0, 0.768, 0

	private float direction = 0;

	public override void _Ready() {
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public bool IsOnFloor() {
		return true;
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
		Vector2 flatVelocity = new Vector2(LinearVelocity.X, LinearVelocity.Z);
		if (acceleration.Length() > 0) {
			projectedVelocity = flatAcceleration.Dot(flatVelocity) / flatAcceleration.Length();
			GD.Print(projectedVelocity);
		}
		if (projectedVelocity >= speedLimit) {
			// Too fast, do nothing
			GD.Print("Maxed");
		} else if (projectedVelocity >= speedLimit - flatAcceleration.Length() * delta && projectedVelocity < speedLimit) {
			flatVelocity = flatVelocity + (speedLimit - projectedVelocity) * flatAcceleration.Normalized();
			//GD.Print((speedLimit - projectedVelocity) * flatAcceleration.Normalized());
		} else if (projectedVelocity < speedLimit - flatAcceleration.Length() * delta) {
			flatVelocity = flatVelocity + flatAcceleration * (float)delta;
			//GD.Print(flatAcceleration * (float)delta);
		}
		if (IsOnFloor()) {
			framesGrounded += 1;
			//GD.Print("Floor");
		} else {
			framesGrounded = 0;
			//GD.Print("Air");
		}
		if (framesGrounded > 1) {
			float deceleration = 0.001f;
			deceleration = Mathf.Pow(deceleration, (float)delta);
			flatVelocity *= deceleration;
		}

		LinearVelocity = new Vector3(flatVelocity.X, LinearVelocity.Y + acceleration.Y * (float)delta, flatVelocity.Y);
	}
}
