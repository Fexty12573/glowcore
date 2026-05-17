using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlowCore.UI.Menus
{
    public class SettingsRowFactory
    {
        private readonly Dictionary<SettingControlKind, Func<Transform, ISettingControl>> m_builders = new();

        public void Register(SettingControlKind kind, Func<Transform, ISettingControl> builder)
        {
            if (builder != null)
                m_builders[kind] = builder;
        }

        public ISettingControl Create(SettingDescriptor descriptor, Transform parent)
        {
            if (descriptor == null || !m_builders.TryGetValue(descriptor.Kind, out Func<Transform, ISettingControl> builder))
                return null;

            ISettingControl control = builder(parent);
            control?.Bind(descriptor);
            return control;
        }
    }
}
