using Godot;
using System;

public partial class Slideshow : Node2D
{
	private bool eng = true;
	private bool _spaceWasPressed = false;
	private Vector2 viewportSize;
	private int screenWidth;
	private int screenHeight;
	private int slideNumber;
	
	[Export] private TextureRect _swipeScreen;
	[Export] private TextureRect _solarScreen;
	[Export] private TextureRect _windScreen;
	[Export] private TextureRect _waveScreen;
	[Export] private TextureRect _hydroScreen;
	[Export] private TextureRect _geoScreen;
	[Export] private TextureRect _resetScreen;
	
	[Export] private Texture2D swipeEng;
	[Export] private Texture2D swipeSpanish;
	[Export] private Texture2D solarEng;
	[Export] private Texture2D solarSpanish;
	[Export] private Texture2D windEng;
	[Export] private Texture2D windSpanish;
	[Export] private Texture2D waveEng;
	[Export] private Texture2D waveSpanish;
	[Export] private Texture2D hydroEng;
	[Export] private Texture2D hydroSpanish;
	[Export] private Texture2D geoEng;
	[Export] private Texture2D geoSpanish;
	[Export] private Texture2D resetEng;
	[Export] private Texture2D resetSpanish;
	
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
		
		// Get viewport size as a Vector2.
		viewportSize = GetViewport().GetVisibleRect().Size;
		
		screenWidth = (int)viewportSize.X;
		screenHeight = (int)viewportSize.Y;
		
		SetTexturesToEnglish();
		
		_resetScreen.Set("visible", false);
		_solarScreen.Set("visible", false);
		_windScreen.Set("visible", false);
		_waveScreen.Set("visible", false);
		_hydroScreen.Set("visible", false);
		_geoScreen.Set("visible", false);
		_swipeScreen.Set("visible", true);
		
		slideNumber = 0;
	}
	
	public override void _Process(double delta) {
		bool spaceDown = Input.IsKeyPressed(Key.Space);

		// Only fire once per key press, not every frame
		if (spaceDown && !_spaceWasPressed) {
			OnSpacePressed();
		}
		_spaceWasPressed = spaceDown;
		
		// Check if the "click" action was just pressed
		if (Input.IsActionJustPressed("click"))
		{
			GD.Print("Action 'click' performed!");
			// Get mouse position (can be done here or via InputEvent if needed)
			Vector2 mousePosition = GetViewport().GetMousePosition();
			
			if(mousePosition.X > (screenWidth / 2)) {
				// Mouse was clicked on the right side of the screen, 
				// move slides to the right.
				// slideNumber 0 = _swipeScreen
				// slideNumber 1 = _solarScreen
				// slideNumber 2 = _windScreen
				// slideNumber 3 = _waveScreen
				// slideNumber 4 = _hydroScreen
				// slideNumber 5 = _geoScreen
				switch(slideNumber) {
					case 0:
						// Move from swipe screen to solar screen
						_resetScreen.Set("visible", false);
						_windScreen.Set("visible", false);
						_waveScreen.Set("visible", false);
						_hydroScreen.Set("visible", false);
						_geoScreen.Set("visible", false);
						_swipeScreen.Set("visible", false);
						_solarScreen.Set("visible", true);
						slideNumber++;
						break;
					case 1:
						// Move from solar screen to wind screen
						_resetScreen.Set("visible", false);
						_solarScreen.Set("visible", false);
						_waveScreen.Set("visible", false);
						_hydroScreen.Set("visible", false);
						_geoScreen.Set("visible", false);
						_swipeScreen.Set("visible", false);
						_windScreen.Set("visible", true);
						slideNumber++;
						break;
					case 2:
						// Move from wind screen to wave screen
						_resetScreen.Set("visible", false);
						_solarScreen.Set("visible", false);
						_windScreen.Set("visible", false);
						_hydroScreen.Set("visible", false);
						_geoScreen.Set("visible", false);
						_swipeScreen.Set("visible", false);
						_waveScreen.Set("visible", true);
						slideNumber++;
						break;
					case 3:
						// Move from wave screen to hydro screen
						_resetScreen.Set("visible", false);
						_solarScreen.Set("visible", false);
						_windScreen.Set("visible", false);
						_waveScreen.Set("visible", false);
						_geoScreen.Set("visible", false);
						_swipeScreen.Set("visible", false);
						_hydroScreen.Set("visible", true);
						slideNumber++;
						break;
					case 4:
						// Move from hydro screen to geo screen
						_resetScreen.Set("visible", false);
						_solarScreen.Set("visible", false);
						_windScreen.Set("visible", false);
						_waveScreen.Set("visible", false);
						_hydroScreen.Set("visible", false);
						_swipeScreen.Set("visible", false);
						_geoScreen.Set("visible", true);
						slideNumber++;
						break;
					case 5:
						// On last page, geo page, don't do anything.
						break;
					default:
						break;
				}
			} else if (mousePosition.X < screenWidth) {
				// Mouse was clicked on the left side of the screen, 
				// move slides to the left.
				// slideNumber 0 = _swipeScreen
				// slideNumber 1 = _solarScreen
				// slideNumber 2 = _windScreen
				// slideNumber 3 = _waveScreen
				// slideNumber 4 = _hydroScreen
				// slideNumber 5 = _geoScreen
				switch(slideNumber) {
					case 0:
						// On swipe page, first page, don't do anything
						break;
					case 1:
						// Move from solar screen to swipe screen
						_resetScreen.Set("visible", false);
						_solarScreen.Set("visible", false);
						_waveScreen.Set("visible", false);
						_hydroScreen.Set("visible", false);
						_geoScreen.Set("visible", false);
						_windScreen.Set("visible", false);
						_swipeScreen.Set("visible", true);
						slideNumber--;
						break;
					case 2:
						// Move from wind screen to solar screen
						_resetScreen.Set("visible", false);
						_windScreen.Set("visible", false);
						_hydroScreen.Set("visible", false);
						_geoScreen.Set("visible", false);
						_swipeScreen.Set("visible", false);
						_waveScreen.Set("visible", false);
						_solarScreen.Set("visible", true);
						slideNumber--;
						break;
					case 3:
						// Move from wave screen to wind screen
						_resetScreen.Set("visible", false);
						_solarScreen.Set("visible", false);
						_waveScreen.Set("visible", false);
						_geoScreen.Set("visible", false);
						_swipeScreen.Set("visible", false);
						_hydroScreen.Set("visible", false);
						_windScreen.Set("visible", true);
						slideNumber--;
						break;
					case 4:
						// Move from hydro screen to wave screen
						_resetScreen.Set("visible", false);
						_solarScreen.Set("visible", false);
						_windScreen.Set("visible", false);
						_hydroScreen.Set("visible", false);
						_swipeScreen.Set("visible", false);
						_geoScreen.Set("visible", false);
						_waveScreen.Set("visible", true);
						slideNumber--;
						break;
					case 5:
						// Move from geo screen to hydro screen
						_resetScreen.Set("visible", false);
						_solarScreen.Set("visible", false);
						_windScreen.Set("visible", false);
						_swipeScreen.Set("visible", false);
						_geoScreen.Set("visible", false);
						_waveScreen.Set("visible", false);
						_hydroScreen.Set("visible", false);
						slideNumber--;
						break;
					default:
						break;
				}	
			}
		}
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
		if(swipeSpanish == null || solarSpanish == null || windSpanish == null ||
			waveSpanish == null || hydroSpanish == null || geoSpanish == null ||
			resetSpanish == null) {
			return;
		} else {
			_swipeScreen.Texture = swipeSpanish;
			_solarScreen.Texture = solarSpanish;
			_windScreen.Texture = windSpanish;
			_waveScreen.Texture = waveSpanish;
			_hydroScreen.Texture = hydroSpanish;
			_geoScreen.Texture = geoSpanish;
			_resetScreen.Texture = resetSpanish;
			
			if (resetSpanish  == null) GD.PrintErr("Failed to load English1.jpg");
			if (swipeSpanish  == null) GD.PrintErr("Failed to load English2.jpg");
			if (solarSpanish  == null) GD.PrintErr("Failed to load English3.jpg");
			if (windSpanish   == null) GD.PrintErr("Failed to load English4.jpg");
			if (waveSpanish   == null) GD.PrintErr("Failed to load English5.jpg");
			if (hydroSpanish  == null) GD.PrintErr("Failed to load English6.jpg");
			if (geoSpanish    == null) GD.PrintErr("Failed to load English7.jpg");
		}
	}
	
	private void SetTexturesToEnglish() {
		if(swipeEng == null || solarEng == null || windEng == null || waveEng == null ||
			hydroEng == null || geoEng == null || resetEng == null) {
			return;
		} else {
			_swipeScreen.Texture = swipeEng;
			_solarScreen.Texture = solarEng;
			_windScreen.Texture = windEng;
			_waveScreen.Texture = waveEng;
			_hydroScreen.Texture = hydroEng;
			_geoScreen.Texture = geoEng;
			_resetScreen.Texture = resetEng;
			
			if (resetEng  == null) GD.PrintErr("Failed to load English1.jpg");
			if (swipeEng  == null) GD.PrintErr("Failed to load English2.jpg");
			if (solarEng  == null) GD.PrintErr("Failed to load English3.jpg");
			if (windEng   == null) GD.PrintErr("Failed to load English4.jpg");
			if (waveEng   == null) GD.PrintErr("Failed to load English5.jpg");
			if (hydroEng  == null) GD.PrintErr("Failed to load English6.jpg");
			if (geoEng    == null) GD.PrintErr("Failed to load English7.jpg");	
		}
	}
}
