namespace GameMessageIdChecker.MessageScript
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class LineModel
    {
        public LineType Type { get; private set; }

        public string Text { get; private set; }

        public LineModel(LineType type, string text)
        {
            this.Type = type;
            this.Text = text;
        }
    }
}
