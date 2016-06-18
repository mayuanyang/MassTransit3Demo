namespace MassTransit3Demo.Messages.Commands
{
    public class PrintToConsoleCommand
    {
        public string Text { get; set; }

        public PrintToConsoleCommand(string text)
        {
            Text = text;
        }
    }
}
