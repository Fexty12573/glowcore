using System;
using System.Collections.Generic;

namespace GlowCore.UI.Menus
{
    public class SettingDescriptor
    {
        private SettingDescriptor(
            string key,
            string label,
            SettingControlKind kind,
            float minValue,
            float maxValue,
            IReadOnlyList<string> options,
            Func<object> read,
            Action<object> write)
        {
            Key = key;
            Label = label;
            Kind = kind;
            MinValue = minValue;
            MaxValue = maxValue;
            Options = options;
            Read = read;
            Write = write;
        }

        public string Key { get; }
        public string Label { get; }
        public SettingControlKind Kind { get; }
        public float MinValue { get; }
        public float MaxValue { get; }
        public IReadOnlyList<string> Options { get; }
        public Func<object> Read { get; }
        public Action<object> Write { get; }

        public static SettingDescriptor Slider(string key, string label, float min, float max, Func<float> read, Action<float> write)
        {
            return new SettingDescriptor(
                key, label, SettingControlKind.Slider, min, max, null,
                () => read(),
                v => write(System.Convert.ToSingle(v)));
        }

        public static SettingDescriptor Toggle(string key, string label, Func<bool> read, Action<bool> write)
        {
            return new SettingDescriptor(
                key, label, SettingControlKind.Toggle, 0f, 1f, null,
                () => read(),
                v => write(System.Convert.ToBoolean(v)));
        }

        public static SettingDescriptor Dropdown(string key, string label, IReadOnlyList<string> options, Func<int> read, Action<int> write)
        {
            return new SettingDescriptor(
                key, label, SettingControlKind.Dropdown, 0f, options == null ? 0f : options.Count - 1, options,
                () => read(),
                v => write(System.Convert.ToInt32(v)));
        }
    }
}
