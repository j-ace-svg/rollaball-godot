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
	public const float RestRatio = 0.5f;
	public const float Stiffness = 10f;
	public const float Damping = 1f;

	private Node3D TargetPoint;
	private PackedScene TargetPointTemplate = GD.Load<PackedScene>("res://grapple_target.tscn");

	public override void _Ready() {
		Player = (RigidBody3D) this.GetNode("../..");
	}

	private void StartGrapple() {
		Vector3 position = GlobalTransform.Origin;
		Vector3 direction = -1 * GlobalTransform.Basis.Z;
		var spaceState = GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(position, position + direction * GrappleDistance, 1);
		var result = spaceState.IntersectRay(query);

		if (result.Count != 0)
		{
			Grappling = true;
			GD.Print(result);
			GrappleTarget = ((Vector3) result["position"]);
			RestLength = (GrappleTarget - Player.Transform.Origin).Length() * RestRatio;

			TargetPoint = (Node3D) TargetPointTemplate.Instantiate();
			GetTree().GetRoot().AddChild(TargetPoint);
			Transform3D TargetTransform = TargetPoint.Transform;
			TargetTransform.Origin = GrappleTarget;
			TargetPoint.Transform = TargetTransform;
		}
	}

	private void UpdateGrapple(double delta) {
		Vector3 GrapplePath = GrappleTarget - Player.Transform.Origin;
		Vector3 GrapplePathNormal = GrapplePath.Normalized();
		float GrappleDist = GrapplePath.Length();

		float SpringDist = GrappleDist - RestLength;
		Vector3 SpringForce = Vector3.Zero;
		if (SpringDist > 0) {
			SpringForce = GrapplePathNormal * SpringDist * Stiffness;

			float VelDot = Player.LinearVelocity.Dot(GrapplePathNormal);
			Vector3 LocalDamping = -Damping * VelDot * GrapplePathNormal;

			SpringForce = SpringForce + LocalDamping;
		}

		SpringForce = (float)delta * SpringForce;
		Player.LinearVelocity = Player.LinearVelocity + SpringForce;
		GD.Print(SpringDist);
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseButton clickEvent) {
			if (clickEvent.ButtonIndex == MouseButton.Left) {
				if (clickEvent.Pressed) {
					StartGrapple();
				} else if (Grappling) {
					Grappling = false;
					TargetPoint.QueueFree();
				}
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Grappling) {
			UpdateGrapple(delta);
		}
	}
}
