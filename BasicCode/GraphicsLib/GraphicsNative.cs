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
