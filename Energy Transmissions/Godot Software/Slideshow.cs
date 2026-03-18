using Godot;
using System;

public partial class Slideshow : Node2D
{
	private bool eng = true;
	private bool _spaceWasPressed = false;
	
	private TextureRect _swipeScreen;
	private TextureRect _solarScreen;
	private TextureRect _windScreen;
	private TextureRect _waveScreen;
	private TextureRect _hydroScreen;
	private TextureRect _geoScreen;
	private TextureRect _resetScreen;
	
	private Texture2D swipeEng;
	private Texture2D swipeSpanish;
	private Texture2D solarEng;
	private Texture2D solarSpanish;
	private Texture2D windEng;
	private Texture2D windSpanish;
	private Texture2D waveEng;
	private Texture2D waveSpanish;
	private Texture2D hydroEng;
	private Texture2D hydroSpanish;
	private Texture2D geoEng;
	private Texture2D geoSpanish;
	private Texture2D resetEng;
	private Texture2D resetSpanish;
	
	public override void _Ready() {
		_swipeScreen = GetNode<TextureRect>("SwipeScreen");
		_solarScreen = GetNode<TextureRect>("SolarScreen");
		_windScreen = GetNode<TextureRect>("WindScreen");
		_waveScreen = GetNode<TextureRect>("WaveScreen");
		_hydroScreen = GetNode<TextureRect>("HydroelectricScreen");
		_geoScreen = GetNode<TextureRect>("GeothermalScreen");
		_resetScreen = GetNode<TextureRect>("ResetScreen");
		
		resetEng = (Texture2D)GD.Load("res://Media/English/English1.jpg");
		swipeEng = (Texture2D)GD.Load("res://Media/English/English2.jpg");
		solarEng = (Texture2D)GD.Load("res://Media/English/English3.jpg");
		windEng = (Texture2D)GD.Load("res://Media/English/English4.jpg");
		waveEng = (Texture2D)GD.Load("res://Media/English/English5.jpg");
		hydroEng = (Texture2D)GD.Load("res://Media/English/English6.jpg");
		geoEng = (Texture2D)GD.Load("res://Media/English/English7.jpg");
		
		resetSpanish = (Texture2D)GD.Load("res://Media/Spanish/Spanish1.jpg");
		swipeSpanish = (Texture2D)GD.Load("res://Media/Spanish/Spanish2.jpg");
		solarSpanish = (Texture2D)GD.Load("res://Media/Spanish/Spanish3.jpg");
		windSpanish = (Texture2D)GD.Load("res://Media/Spanish/Spanish4.jpg");
		waveSpanish = (Texture2D)GD.Load("res://Media/Spanish/Spanish5.jpg");
		hydroSpanish = (Texture2D)GD.Load("res://Media/Spanish/Spanish6.jpg");
		geoSpanish = (Texture2D)GD.Load("res://Media/Spanish/Spanish7.jpg");
		
		SetTexturesToEnglish();
	}
	
	public override void _Process(double delta) {
		bool spaceDown = Input.IsKeyPressed(Key.Space);

		// Only fire once per key press, not every frame
		if (spaceDown && !_spaceWasPressed) {
			OnSpacePressed();
		}

		_spaceWasPressed = spaceDown;
	}
	
	private void OnSpacePressed() {
		if(eng) {
			// Change to spanish
			eng = false;
			SetTexturesToSpanish();
		} else if(!eng) {
			// Change to english
			eng = true;
			SetTexturesToEnglish();
		}
	}
	
	private void SetTexturesToSpanish() {
		_swipeScreen.Texture = swipeSpanish;
		_solarScreen.Texture = solarSpanish;
		_windScreen.Texture = windSpanish;
		_waveScreen.Texture = waveSpanish;
		_hydroScreen.Texture = hydroSpanish;
		_geoScreen.Texture = geoSpanish;
		_resetScreen.Texture = resetSpanish;
	}
	
	private void SetTexturesToEnglish() {
		_swipeScreen.Texture = swipeEng;
		_solarScreen.Texture = solarEng;
		_windScreen.Texture = windEng;
		_waveScreen.Texture = waveEng;
		_hydroScreen.Texture = hydroEng;
		_geoScreen.Texture = geoEng;
		_resetScreen.Texture = resetEng;
	}
}
