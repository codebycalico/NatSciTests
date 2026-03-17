/* * * * * * * * * * *
* Calico Rose
* * * * * * * * * * */
using Godot;
using System;

public partial class MainGameplay : Node2D
{
	private int _gameSection = 0;
	private bool _spaceWasPressed = false;
	private bool _qrGenerated = false;

	public override void _Ready()
	{
		GetNode("ResetNode").Set("visible", false);
		GetNode("RunningNode").Set("visible", false);
		GetNode("LoadingNode").Set("visible", false);
		GetNode("QRNode").Set("visible", false);
	}

	public override void _Process(double delta)
	{
		bool spaceDown = Input.IsKeyPressed(Key.Space);

		// Only fire once per key press, not every frame
		if (spaceDown && !_spaceWasPressed)
			OnSpacePressed();

		_spaceWasPressed = spaceDown;
	}

	private void OnSpacePressed()
	{
		switch (_gameSection)
		{
			case 0:
				// Start of the game
				// Has the pieces being actively put in
				// This node changes the "glow" based on what pieces are set down
				GetNode("ResetNode").Set("visible", false);
				GetNode("QRNode").Set("visible", false);
				GetNode("LoadingNode").Set("visible", false);
				GetNode("RunningNode").Set("visible", true);
				break;

			case 1:
				// Loading results screen
				GetNode("RunningNode").Set("visible", false);
				GetNode("ResetNode").Set("visible", false);
				GetNode("QRNode").Set("visible", false);
				GetNode("LoadingNode").Set("visible", true);
				break;

			case 2:
				// QR Screen
				GetNode("RunningNode").Set("visible", false);
				GetNode("ResetNode").Set("visible", false);
				GetNode("LoadingNode").Set("visible", false);
				GetNode("QRNode").Set("visible", true);

				// Only generate QR once per round
				if (!_qrGenerated)
				{
					GetNode<QRCodeGenerator>("QRNode").GenerateQRsForID("2");
					_qrGenerated = true;
				}
				break;

			case 3:
				// Reset Screen
				GetNode("RunningNode").Set("visible", false);
				GetNode("QRNode").Set("visible", false);
				GetNode("LoadingNode").Set("visible", false);
				GetNode("ResetNode").Set("visible", true);
				break;
		}

		// Advance section, wrap back to 0 after 3
		_gameSection = (_gameSection < 3) ? _gameSection + 1 : 0;
	}
}
