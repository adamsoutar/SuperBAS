
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
        private static string NUM_string = "";
private static string IMAGE_string = "";
private static double NUM_number = 0.0;
private static double WIN_number = 0.0;
private static double TEX_number = 0.0;
private static double SPRITE_number = 0.0;
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

// In scaleFactor of original texture size
static double userFn_GFXSETSPRITESIZERELATIVE_number (double sprId, double x, double y) {
  sprites[(int)sprId].Scale = vec(x, y);
  return 1;
}

// In pixels
static double userFn_GFXSETSPRITESIZEABSOLUTE_number (double sprId, double x, double y) {
  var sp = sprites[(int)sprId];
  var vc = vec(x, y);
  var sfX = x / sp.GetLocalBounds().Width;
  var sfY = y / sp.GetLocalBounds().Height;
  sp.Scale = vec(sfX, sfY);
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

goto case 500;
case 500:
Console.WriteLine("What would you like to inspect?");
goto case 501;
case 501:
Console.WriteLine(" 1) Axe");
goto case 502;
case 502:
Console.WriteLine(" 2) Coin");
goto case 503;
case 503:
Console.WriteLine(" 3) Torch");
goto case 504;
case 504:
NUM_string = Console.ReadLine();
goto case 505;
case 505:
NUM_number = double.Parse(NUM_string);
goto case 506;
case 506:
if ((NUM_number > 3.0)) { lineNumber = 500.0;
 goto GosubStart;
 } else {
}
goto case 507;
case 507:
if ((NUM_number < 1.0)) { lineNumber = 500.0;
 goto GosubStart;
 } else {
}
goto case 508;
case 508:
Console.WriteLine("You're inspecting an object...");
goto case 509;
case 509:
IMAGE_string = (("/Users/adam/i" + NUM_number) + ".png");
goto case 510;
case 510:
lineNumber = 1000.0;
 goto GosubStart;
goto case 1000;
case 1000:
WIN_number = userFn_GFXNEWWINDOW_number(512.0,512.0);
goto case 1001;
case 1001:
TEX_number = userFn_GFXNEWTEXTURE_number(IMAGE_string);
goto case 1002;
case 1002:
SPRITE_number = userFn_GFXNEWSPRITE_number(TEX_number);
goto case 1004;
case 1004:
userFn_GFXCLEARWINDOW_number(WIN_number);
goto case 1005;
case 1005:
userFn_GFXDRAW_number(WIN_number,SPRITE_number);
goto case 1006;
case 1006:
userFn_GFXDISPLAYWINDOW_number(WIN_number);
goto case 1007;
case 1007:
if ((userFn_GFXWINDOWISOPEN_number(WIN_number) > 0.0)) { userFn_GFXHANDLEEVENTS_number();
lineNumber = 1007.0;
 goto GosubStart;
 } else {
}
goto case 1008;
case 1008:
Console.WriteLine("You closed the inspection window");
goto case 1009;
case 1009:
Console.WriteLine("");
goto case 1010;
case 1010:
lineNumber = 500.0;
 goto GosubStart;
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
        