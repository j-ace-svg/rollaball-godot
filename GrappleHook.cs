using Godot;
using System;

public partial class GrappleHook : Node3D
{
	public const float GrappleDistance = 30f;
	private Generic6DOFJoint GrappleJoint;
	private bool Grappling = false;

	public override void _Ready() {
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseButton clickEvent) {
			if (clickEvent.ButtonIndex == MouseButton.Left) {
				Vector3 position = GlobalTransform.Origin;
				Vector3 direction = -1 * GlobalTransform.Basis.Z;
				var spaceState = GetWorld3D().DirectSpaceState;
				var query = PhysicsRayQueryParameters3D.Create(position, position + direction * GrappleDistance);
				var result = spaceState.IntersectRay(query);
				GD.Print(result);

				GrappleJoint = new Generic6DOFJoint();
				AddChild(GrappleJoint);
			}
		}
	}
}
