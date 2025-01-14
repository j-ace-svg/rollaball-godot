using Godot;
using System;

using JP = Godot.Generic6DofJoint3D.Param;

public partial class GrappleHook : Node3D
{
	public const float GrappleDistance = 30f;
	private Generic6DofJoint3D GrappleJoint;
	private Node3D GrappleTarget;
	private bool Grappling = false;

	public override void _Ready() {
	}

	private void StartGrapple() {
		Vector3 position = GlobalTransform.Origin;
		Vector3 direction = -1 * GlobalTransform.Basis.Z;
		var spaceState = GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(position, position + direction * GrappleDistance);
		var result = spaceState.IntersectRay(query);

		if (result.Count != 0)
		{
			GD.Print(result);
			GrappleJoint = new Generic6DofJoint3D();
			AddChild(GrappleJoint);

			GrappleTarget = new Node3D();
			Transform3D GrappleTargetTransform = GrappleTarget.Transform;
			GrappleTargetTransform.Origin = ((Vector3) result["position"]);
			GrappleTarget.Transform = GrappleTargetTransform;
			GrappleTarget.Basis = new Basis();
			this.GetNode("/root").AddChild(GrappleTarget);

			GrappleJoint.NodeA = this.GetNode("../..").GetPath();
			GrappleJoint.NodeB = GrappleTarget.GetPath();

			GrappleJoint.Set("linear_limit_x/enabled", false);
			GrappleJoint.Set("linear_limit_y/enabled", false);
			GrappleJoint.Set("linear_limit_z/enabled", false);
			GrappleJoint.Set("angular_limit_x/enabled", false);
			GrappleJoint.Set("angular_limit_y/enabled", false);
			GrappleJoint.Set("angular_limit_z/enabled", false);

			GrappleJoint.Set("linear_spring_x/enabled", true);
			GrappleJoint.Set("linear_spring_y/enabled", true);
			GrappleJoint.Set("linear_spring_z/enabled", true);
			GrappleJoint.Set("linear_spring_x/stiffness", 1);
			GrappleJoint.Set("linear_spring_y/stiffness", 1);
			GrappleJoint.Set("linear_spring_z/stiffness", 1);
			GD.Print(GrappleJoint);
		}
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseButton clickEvent) {
			if (clickEvent.ButtonIndex == MouseButton.Left) {
				if (clickEvent.Pressed) {
					StartGrapple();
				} else {
					GD.Print(GrappleJoint.Get("linear_sprint_x/enabled"));
				}
			}
		}
	}
}
