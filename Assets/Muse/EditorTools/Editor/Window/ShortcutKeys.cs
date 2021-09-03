namespace Muse
{
    public static class ShortcutKeys
    {
        public const string CTRL = "%";
        public const string ALT = "&";
        public const string SHIFT = "#";

        public const string SHORTCUT_KEYS_PREFIX = " " + ALT + SHIFT;

        public const string SHORTCUT_WINDOW_FOLDER = SHORTCUT_KEYS_PREFIX + "f";
        public const string SHORTCUT_WINDOW_PREFABVARIANT = SHORTCUT_KEYS_PREFIX + "v";
        public const string SHORTCUT_WINDOW_SCENEPROFILE = SHORTCUT_KEYS_PREFIX + "s";
        public const string SHORTCUT_WINDOW_REPLACER = SHORTCUT_KEYS_PREFIX + "r";
        public const string SHORTCUT_WINDOW_LOD = SHORTCUT_KEYS_PREFIX + "l";
        public const string SHORTCUT_WINDOW_MODELCHECK = SHORTCUT_KEYS_PREFIX + "m";
    }
}