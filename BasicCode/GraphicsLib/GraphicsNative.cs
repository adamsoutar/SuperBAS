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
