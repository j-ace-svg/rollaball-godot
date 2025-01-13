using Godot;
using System;

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
			AddChild(GrappleTarget);

			GrappleJoint.NodeA = this.GetNode("../..").GetPath();
			GrappleJoint.NodeB = GrappleTarget.GetPath();

			GrappleJoint.LinearLimitY__Enabled = false;
		}
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseButton clickEvent) {
			if (clickEvent.ButtonIndex == MouseButton.Left) {
				if (clickEvent.Pressed) {
					StartGrapple();
				} else {
				}
			}
		}
	}
}
