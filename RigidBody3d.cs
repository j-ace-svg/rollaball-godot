using Godot;
using System;
using System.Collections.Generic;

public partial class RigidBody3d : RigidBody3D
{
	public const float GroundSpeed = 100f;
	public const float GroundSpeedLimit = 6f;
	public const float JumpVelocity = 300f;
	public const float GravityStrength = 18f;
	public const float Sensitivity = 0.001f;
	public float framesGrounded = 0f;

	private Vector3 StartPoint;

	private float direction = Mathf.Pi;
	private float oldDirection = 0;
	private bool isGrounded = false;

	public override void _Ready() {
		Input.MouseMode = Input.MouseModeEnum.Captured;
		StartPoint = GlobalTransform.Origin;
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseMotion motionEvent) {
			float deltaRotation = - motionEvent.Relative.X * Sensitivity;
			direction += deltaRotation;
		}
	}

	public override void _IntegrateForces(PhysicsDirectBodyState3D state) {
		Transform3D newTransform = state.GetTransform();
		RotateObjectLocal(Vector3.Up, direction - oldDirection);
		state.AngularVelocity = Vector3.Zero;
		//newTransform = newTransform.Rotated(Vector3.Up, direction - oldDirection); // Rotate the player

		int contactCount = state.GetContactCount();

		isGrounded = false;

		for (int i = 0; i < contactCount; i++)
		{
		    Vector3 normal = state.GetContactLocalNormal(i);

		    if (normal.Equals(Vector3.Up)) {
			    isGrounded = true;
			    break;
		    }
		}
		//GD.Print(isGrounded);

		oldDirection = direction;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 acceleration = Vector3.Zero;

		float speed = GroundSpeed;
		float speedLimit = GroundSpeedLimit;
		if (!isGrounded) {
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
		if (Input.IsActionPressed("jump") && isGrounded) {
			acceleration.Y += JumpVelocity;
		}

		acceleration.Y -= GravityStrength;

		float projectedVelocity = 0;
		Vector2 flatAcceleration = new Vector2(acceleration.X, acceleration.Z);
		flatAcceleration = flatAcceleration.Rotated(-direction);
		Vector2 flatVelocity = new Vector2(LinearVelocity.X, LinearVelocity.Z);
		if (acceleration.Length() > 0) {
			projectedVelocity = flatAcceleration.Dot(flatVelocity) / flatAcceleration.Length();
			//GD.Print(flatAcceleration);
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
		if (isGrounded) {
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

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("reset")) {
			Transform3D newTransform = GlobalTransform;
			newTransform.Origin = StartPoint;
			GlobalTransform = newTransform;
			LinearVelocity = Vector3.Zero;
			direction = Mathf.Pi;
		}
	}
}
