using Godot;
using System;

public partial class GrappleHook : Node3D
{
	public override void _Ready() {
	}

	public override void _PhysicsProcess(double delta) {
		Vector3 position = GlobalTransform.Origin;
		Vector3 direction = GlobalTransform.Basis.Z;
		var spaceState = GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(position, position + direction);
		var result = spaceState.IntersectRay(query);
		GD.Print(result);
	}
}
