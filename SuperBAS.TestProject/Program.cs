
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace UserProgram
{
    class Program
    {
        private static Random rand;
        private static int startX;
        private static int startY;
        private static double WIN_number = 0.0;
private static double CLOCK_number = 0.0;
private static double MARPNG_number = 0.0;
private static double MARIO_number = 0.0;
private static double GRAVITYSPEED_number = 0.0;
private static double FLOORHEIGHT_number = 0.0;
private static double MOVESPEED_number = 0.0;
private static double JUMPSPEED_number = 0.0;
private static double DELTATIME_number = 0.0;
private static double JUMPING_number = 0.0;
static List<RenderWindow> windows = new List<RenderWindow>();
static List<Clock> clocks = new List<Clock>();
static List<Sprite> sprites = new List<Sprite>();
static List<Texture> textures = new List<Texture>();

// Helpers
static Vector2f vec (double x, double y) {
  return new Vector2f((float)x, (float)y);
}
// End helpers

// Windows
// TODO: VSync option?
// NOTE: The _number means it isn't called with $
static double userFn_GFXNEWWINDOW_number (double width, double height, string title = "Graphics", double fullscreen = 0) {
  var winId = windows.Count;

  var win = new RenderWindow(
    new VideoMode((uint)width, (uint)height),
    title,
    (fullscreen > 0) ? Styles.Fullscreen : Styles.Default
  );
  win.SetVerticalSyncEnabled(true);
  win.Closed += (object sender, EventArgs e) => win.Close();

  windows.Add(win);

  return winId;
}

static double userFn_GFXDISPLAYWINDOW_number (double winId) {
  windows[(int)winId].Display();
  return 1;
}

static double userFn_GFXHANDLEEVENTS_number (double winId = -1) {
  if (winId < 0) {
    foreach (var win in windows) {
      win.DispatchEvents();
    }
  } else {
    windows[(int)winId].DispatchEvents();
  }

  // Functions must have a return value.
  // There's no concept of void/null in SuperBAS
  return 1;
}

static double userFn_GFXCLEARWINDOW_number (double winId) {
  windows[(int)winId].Clear();
  return 1;
}

static double userFn_GFXWINDOWISOPEN_number (double winId) {
  return windows[(int)winId].IsOpen ? 1 : 0;
}
// End Windows

// Clocks
static double userFn_GFXNEWCLOCK_number () {
  var clockId = clocks.Count;
  var clock = new Clock();
  clocks.Add(clock);
  return clockId;
}

static double userFn_GFXRESTARTCLOCK_number (double clockId) {
  var time = clocks[(int)clockId].Restart();
  return time.AsSeconds();
}
// End Clocks

// Keyboard
// TODO: Controllers
static double userFn_GFXISKEYPRESSED_number (string keyName) {
  Keyboard.Key k;
  Enum.TryParse(keyName, out k);
  return Keyboard.IsKeyPressed(k) ? 1 : 0;
}
// End Keyboard

// Drawables
static double userFn_GFXNEWTEXTURE_number (string filePath) {
  var texId = textures.Count;
  textures.Add(new Texture(filePath));
  return texId;
}

static double userFn_GFXNEWSPRITE_number (double texId) {
  var sprId = sprites.Count;
  sprites.Add(new Sprite(textures[(int)texId]));
  return sprId;
}

static double userFn_GFXSETSPRITEPOSITION_number (double sprId, double x, double y) {
  sprites[(int)sprId].Position = vec(x, y);
  return 1;
}

static double userFn_GFXMOVESPRITE_number (double sprId, double x, double y) {
  sprites[(int)sprId].Position += vec(x, y);
  return 1;
}

static double userFn_GFXROTATESPRITE_number (double sprId, double angle) {
  sprites[(int)sprId].Rotation = (float)angle;
  return 1;
}

static double userFn_GFXSCALESPRITE_number (double sprId, double x, double y) {
  sprites[(int)sprId].Scale = vec(x, y);
  return 1;
}

static double userFn_GFXGETSPRITEX_number (double sprId) {
  return (double)(sprites[(int)sprId].Position.X);
}

static double userFn_GFXGETSPRITEY_number (double sprId) {
  return (double)(sprites[(int)sprId].Position.Y);
}

static double userFn_GFXSETSPRITETEXTURE_number (double sprId, double texId) {
  sprites[(int)sprId].Texture = textures[(int)texId];
  return 1;
}

static double userFn_GFXDRAW_number (double winId, double sprId) {
  windows[(int)winId].Draw(sprites[(int)sprId]);
  return 1;
}
// End drawables


        static void Gosub(double lineNumber)
        {
        GosubStart:
            switch (lineNumber)
            {
                case -1:
                    return;
                case 0:

goto case 1;
case 1:
WIN_number = userFn_GFXNEWWINDOW_number(640.0,480.0);
goto case 2;
case 2:
CLOCK_number = userFn_GFXNEWCLOCK_number();
goto case 3;
case 3:
MARPNG_number = userFn_GFXNEWTEXTURE_number("/Users/adam/Documents/Mac Projects/SuperBAS/BasicCode/GraphicsLib/mario.png");
goto case 4;
case 4:
MARIO_number = userFn_GFXNEWSPRITE_number(MARPNG_number);
goto case 5;
case 5:
GRAVITYSPEED_number = 500.0;
goto case 6;
case 6:
userFn_GFXSETSPRITEPOSITION_number(MARIO_number,0.0,0.0);
goto case 7;
case 7:
userFn_GFXSCALESPRITE_number(MARIO_number,0.1,0.1);
goto case 8;
case 8:
FLOORHEIGHT_number = 100.0;
goto case 9;
case 9:
MOVESPEED_number = 400.0;
goto case 10;
case 10:
JUMPSPEED_number = 700.0;
goto case 100;
case 100:
DELTATIME_number = userFn_GFXRESTARTCLOCK_number(CLOCK_number);
goto case 101;
case 101:
userFn_GFXHANDLEEVENTS_number(WIN_number);
goto case 102;
case 102:
userFn_GFXCLEARWINDOW_number(WIN_number);
goto case 103;
case 103:
JUMPING_number = userFn_GFXISKEYPRESSED_number("Up");
goto case 104;
case 104:
if ((userFn_GFXGETSPRITEY_number(MARIO_number) < 390.0)) { if ((JUMPING_number < 1.0)) { userFn_GFXMOVESPRITE_number(MARIO_number,0.0,(GRAVITYSPEED_number * DELTATIME_number));
 } else {
}
 } else {
}
goto case 105;
case 105:
if ((userFn_GFXISKEYPRESSED_number("Right") > 0.0)) { userFn_GFXMOVESPRITE_number(MARIO_number,(MOVESPEED_number * DELTATIME_number),0.0);
 } else {
}
goto case 106;
case 106:
if ((userFn_GFXISKEYPRESSED_number("Left") > 0.0)) { userFn_GFXMOVESPRITE_number(MARIO_number,(0.0 - (MOVESPEED_number * DELTATIME_number)),0.0);
 } else {
}
goto case 107;
case 107:
if ((JUMPING_number > 0.0)) { userFn_GFXMOVESPRITE_number(MARIO_number,0.0,(0.0 - (JUMPSPEED_number * DELTATIME_number)));
 } else {
}
goto case 110;
case 110:
userFn_GFXDRAW_number(WIN_number,MARIO_number);
goto case 111;
case 111:
userFn_GFXDISPLAYWINDOW_number(WIN_number);
goto case 200;
case 200:
if ((userFn_GFXWINDOWISOPEN_number(WIN_number) > 0.0)) { lineNumber = 100.0;
 goto GosubStart;
 } else {
}
goto case 201;
case 201:
Console.WriteLine("Game Exited");
goto case -1;

                default:
                 throw new Exception($"Invalid GOTO { lineNumber } - Not a line");
            }
}

static void Main(string[] args)
{
    Console.Clear();
    startX = Console.CursorLeft;
    startY = Console.CursorTop;
    Gosub(0);
}

static void PrintAt(double x, double y, string text)
{
    /* I'm not sure this works. It certainly doesn't on macOS .NET Native */
    int oldx = Console.CursorLeft;
    int oldy = Console.CursorTop;
    Console.SetCursorPosition(startX + (int)x, startY + (int)y);
    Console.Write(text);
    Console.SetCursorPosition(oldx, oldy);
}

static string ReadAllFile (string flName) {
    // Read an entire file to a string
    // Put into a function for use in expressions
    var sr = new StreamReader(flName);
    var s = sr.ReadToEnd();
    sr.Close();
    return s;
}
    }
}
        