using System;
using System.Collections.Generic;
using UnityEngine;
using Il2CppInterop.Runtime.Injection;

namespace PhantomMenu
{
    public class MenuOverlay : MonoBehaviour
    {
        public MenuOverlay(IntPtr ptr) : base(ptr) { }

        // ─── STYLE ────────────────────────────────────────────────────────────
        private const KeyCode TOGGLE_KEY  = KeyCode.F1;
        private const float   MENU_X      = 20f;
        private const float   MENU_Y      = 20f;
        private const float   MENU_WIDTH  = 250f;
        private const float   ROW_HEIGHT  = 24f;
        private const float   ROW_PAD     = 4f;
        private const int     FONT_SIZE   = 15;
        private static Color  BG_COLOR    = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        private static Color  TEXT_COLOR  = Color.white;
        private static Color  BTN_COLOR   = new Color(0.25f, 0.25f, 0.25f, 1f);
        private static Color  TITLE_COLOR = new Color(0.15f, 0.15f, 0.15f, 1f);
        // ─────────────────────────────────────────────────────────────────────

        private static bool   _open  = false;
        private static string _title = "Menu";

        private static readonly List<Action<Rect>>    _items   = new();
        private static readonly Dictionary<int, bool>  _toggles = new();
        private static readonly Dictionary<int, float> _sliders = new();

        private static GUIStyle? _labelStyle;
        private static GUIStyle? _buttonStyle;
        private static GUIStyle? _toggleStyle;
        private static GUIStyle? _boxStyle;
        private static bool      _stylesBuilt = false;

        public static void Register() => ClassInjector.RegisterTypeInIl2Cpp<MenuOverlay>();

        public static void Init(string title)
        {
            _title = title;
            var go = new GameObject("PhantomMenuOverlay");
            DontDestroyOnLoad(go);
            go.AddComponent<MenuOverlay>();
        }

        // ─── ADD ITEMS ───────────────────────────────────────────────────────

        public static void AddLabel(string text)
            => _items.Add(r => GUI.Label(r, text, Style_Label()));

        public static void AddButton(string text, Action onClick)
            => _items.Add(r => { if (GUI.Button(r, text, Style_Button())) onClick?.Invoke(); });

        public static void AddSeparator()
            => _items.Add(r => GUI.Box(new Rect(r.x, r.y + r.height / 1f, r.width, 1f), ""));

        public static void AddToggle(string text, bool defaultValue, Action<bool> onChange)
        {
            int id = _items.Count;
            _toggles[id] = defaultValue;
            _items.Add(r =>
            {
                bool next = GUI.Toggle(r, _toggles[id], " " + text, Style_Toggle());
                if (next == _toggles[id]) return;
                _toggles[id] = next;
                onChange?.Invoke(next);
            });
        }

        public static void AddSlider(string text, float defaultValue, float min, float max, Action<float> onChange)
        {
            int id = _items.Count;
            _sliders[id] = defaultValue;
            _items.Add(r =>
            {
                GUI.Label(new Rect(r.x, r.y, r.width, ROW_HEIGHT), $"{text}: {_sliders[id]:F1}", Style_Label());
                float next = GUI.HorizontalSlider(new Rect(r.x, r.y + ROW_HEIGHT, r.width, ROW_HEIGHT), _sliders[id], min, max);
                if (Mathf.Approximately(next, _sliders[id])) return;
                _sliders[id] = next;
                onChange?.Invoke(next);
            });
        }

        // ─── UNITY CALLBACKS ─────────────────────────────────────────────────

        public void Update() { if (Input.GetKeyDown(TOGGLE_KEY)) _open = !_open; }

        public void OnGUI()
        {
            if (!_open) return;

            float rowStep  = ROW_HEIGHT + ROW_PAD;
            float totalH   = 40f + _items.Count * rowStep + 10f;
            Rect menuRect = new Rect(MENU_X, MENU_Y, MENU_WIDTH, totalH);

            GUI.Box(menuRect, _title, Style_Box());

            float y = MENU_Y + 40f;
            for (int i = 0; i < _items.Count; i++)
            {
                bool  isSlider = _sliders.ContainsKey(i);
                float h        = isSlider ? ROW_HEIGHT * 2f : ROW_HEIGHT;
                _items[i]?.Invoke(new Rect(MENU_X + 10f, y, MENU_WIDTH - 20f, h));
                y += h + ROW_PAD;
            }
        }

        // ─── HELPERS ─────────────────────────────────────────────────────────

        private static GUIStyle Style_Label()  { Build(); return _labelStyle!; }
        private static GUIStyle Style_Button() { Build(); return _buttonStyle!; }
        private static GUIStyle Style_Toggle() { Build(); return _toggleStyle!; }
        private static GUIStyle Style_Box()    { Build(); return _boxStyle!; }

        private static void Build()
        {
            if (_stylesBuilt) return;

            _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = FONT_SIZE };
            _labelStyle.normal.textColor = TEXT_COLOR;

            _buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = FONT_SIZE };
            _buttonStyle.normal.textColor  = TEXT_COLOR;
            _buttonStyle.hover.textColor   = TEXT_COLOR;
            _buttonStyle.active.textColor  = TEXT_COLOR;
            _buttonStyle.normal.background = Tex(BTN_COLOR);

            _toggleStyle = new GUIStyle(GUI.skin.toggle) { fontSize = FONT_SIZE };
            _toggleStyle.normal.textColor   = TEXT_COLOR;
            _toggleStyle.onNormal.textColor = TEXT_COLOR;
            _toggleStyle.hover.textColor    = TEXT_COLOR;
            _toggleStyle.onHover.textColor  = TEXT_COLOR;
            _toggleStyle.active.textColor   = TEXT_COLOR;
            _toggleStyle.onActive.textColor = TEXT_COLOR;

            _boxStyle = new GUIStyle(GUI.skin.box) { fontSize = FONT_SIZE };
            _boxStyle.normal.textColor  = TEXT_COLOR;
            _boxStyle.normal.background = Tex(BG_COLOR);

            _stylesBuilt = true;
        }

        private static Texture2D Tex(Color c)
        {
            var t = new Texture2D(1, 1);
            t.SetPixel(0, 0, c);
            t.Apply();
            return t;
        }
    }
}