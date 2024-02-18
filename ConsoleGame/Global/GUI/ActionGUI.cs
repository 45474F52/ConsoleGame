using System;

namespace ConsoleGame.Global.GUI
{
    internal sealed class ActionGUI
    {
        public string title;
        public readonly Action action;

        public ActionGUI(string title, Action action)
        {
            this.title = title;
            this.action = action;
        }
    }
}
