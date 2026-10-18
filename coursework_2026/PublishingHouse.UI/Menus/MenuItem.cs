namespace PublishingHouse.UI.Menus
{
    /// <summary>
    /// Пункт меню: текст і дія, що виконується при виборі.
    /// </summary>
    internal sealed class MenuItem
    {
        public MenuItem(string text, Action action)
        {
            Text = text;
            Action = action;
        }

        public string Text { get; }

        public Action Action { get; }
    }
}
