
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
private static double SIZE_number = 0.0;
private static double SPEED_number = 0.0;
private static double GOINGUP_number = 0.0;
private static double DELTATIME_number = 0.0;
private static double AMNT_number = 0.0;
private static double RECT_number = 0.0;
static List<RenderWindow> windows = new List<RenderWindow>();
static List<Clock> clocks = new List<Clock>();
static List<Drawable> drawables = new List<Drawable>();

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

// Drawables
static double userFn_GFXFREEDRAWABLE_number (double drwId) {
  drawables[(int)drwId] = null;
  return 1;
}

static double userFn_GFXNEWRECTANGLE_number (double width, double height, double r = 255, double g = 255, double b = 255, double alpha = 255) {
  var rect = new RectangleShape(new Vector2f((float)width, (float)height));
  rect.FillColor = new Color((byte)r, (byte)g, (byte)b, (byte)alpha);
  var drwId = drawables.Count;
  drawables.Add(rect);
  return drwId;
}

static double userFn_GFXDRAW_number (double winId, double drwId) {
  windows[(int)winId].Draw(drawables[(int)drwId]);
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

goto case 19;
case 19:
Console.WriteLine("Started");
goto case 20;
case 20:
WIN_number = userFn_GFXNEWWINDOW_number(1920.0,1080.0,"Window",0.0);
goto case 21;
case 21:
CLOCK_number = userFn_GFXNEWCLOCK_number();
goto case 22;
case 22:
SIZE_number = 0.0;
goto case 23;
case 23:
SPEED_number = 5000.0;
goto case 24;
case 24:
GOINGUP_number = 1.0;
goto case 32;
case 32:
DELTATIME_number = userFn_GFXRESTARTCLOCK_number(CLOCK_number);
goto case 32.5;
case 32.5:
userFn_GFXHANDLEEVENTS_number(WIN_number);
goto case 32.7;
case 32.7:
AMNT_number = (DELTATIME_number * SPEED_number);
goto case 33;
case 33:
if ((GOINGUP_number == 1.0)) { SIZE_number = (SIZE_number + AMNT_number);
 } else {
SIZE_number = (SIZE_number - AMNT_number);
}
goto case 34;
case 34:
RECT_number = userFn_GFXNEWRECTANGLE_number(SIZE_number,SIZE_number);
goto case 35;
case 35:
userFn_GFXCLEARWINDOW_number(WIN_number);
goto case 36;
case 36:
userFn_GFXDRAW_number(WIN_number,RECT_number);
goto case 38;
case 38:
userFn_GFXFREEDRAWABLE_number(RECT_number);
goto case 39;
case 39:
userFn_GFXDISPLAYWINDOW_number(WIN_number);
goto case 40;
case 40:
if ((GOINGUP_number == 1.0)) { if ((SIZE_number > 1080.0)) { GOINGUP_number = 0.0;
 } else {
}
 } else {
}
goto case 41;
case 41:
if ((GOINGUP_number == 0.0)) { if ((SIZE_number < 2.0)) { GOINGUP_number = 1.0;
 } else {
}
 } else {
}
goto case 50;
case 50:
if ((userFn_GFXWINDOWISOPEN_number(WIN_number) == 1.0)) { lineNumber = 32.0;
 goto GosubStart;
 } else {
}
goto case 51;
case 51:
Console.WriteLine("Window closed!");
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
        