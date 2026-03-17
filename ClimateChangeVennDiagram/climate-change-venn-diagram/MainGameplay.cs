using Godot;
using System;

public partial class MainGameplay : Node2D
{
	public override void _Ready()
	{
		GetNode<QRCodeGenerator>("QRNode").GenerateQRsForID("2");
	}
}
