using System.Collections.Generic;

namespace GlowCore.UI.Menus
{
    public class EscapeRouter
    {
        private readonly List<IEscapeConsumer> m_consumers = new();

        public void Register(IEscapeConsumer consumer)
        {
            if (consumer == null)
                return;
            m_consumers.Add(consumer);
            // Higher Priority runs first.
            m_consumers.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        public void Unregister(IEscapeConsumer consumer)
        {
            if (consumer != null)
                m_consumers.Remove(consumer);
        }

        public bool Dispatch()
        {
            for (var i = 0; i < m_consumers.Count; i++)
            {
                if (m_consumers[i].TryConsumeEscape())
                    return true;
            }
            return false;
        }
    }
}
