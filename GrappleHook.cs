using Godot;
using System;

using JP = Godot.Generic6DofJoint3D.Param;

public partial class GrappleHook : Node3D
{
	public const float GrappleDistance = 100f;
	private Vector3 GrappleTarget;
	private bool Grappling = false;
	public RigidBody3D Player;

	public float RestLength = 10f;
	public const float Stiffness = 0.05f;
	public const float Damping = 1f;

	public override void _Ready() {
		Player = (RigidBody3D) this.GetNode("../..");
	}

	private void StartGrapple() {
		Vector3 position = GlobalTransform.Origin;
		Vector3 direction = -1 * GlobalTransform.Basis.Z;
		var spaceState = GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(position, position + direction * GrappleDistance);
		var result = spaceState.IntersectRay(query);

		if (result.Count != 0)
		{
			Grappling = true;
			GD.Print(result);
			GrappleTarget = ((Vector3) result["position"]);
			RestLength = (GrappleTarget - Player.Transform.Origin).Length() * 0.85f;
		}
	}

	private void UpdateGrapple() {
		Vector3 GrapplePath = GrappleTarget - Player.Transform.Origin;
		Vector3 GrapplePathNormal = GrapplePath.Normalized();
		float GrappleDist = GrapplePath.Length();

		float SpringDist = GrappleDist - RestLength;
		Vector3 SpringForce = Vector3.Zero;
		if (SpringDist > 0) {
			SpringForce = GrapplePathNormal * SpringDist * Stiffness;

			Vector3 VelDot = Player.LinearVelocity.Dot(GrapplePathNormal);

		}

		Player.LinearVelocity = Player.LinearVelocity + SpringForce;
		GD.Print(SpringDist);
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseButton clickEvent) {
			if (clickEvent.ButtonIndex == MouseButton.Left) {
				if (clickEvent.Pressed) {
					StartGrapple();
				} else {
					Grappling = false;
				}
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Grappling) {
			UpdateGrapple();
		}
	}
}
