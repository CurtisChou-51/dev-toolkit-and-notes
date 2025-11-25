namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var result = SomethingSync();
            label1.Text = result;
        }

        private async void button2_ClickAsync(object sender, EventArgs e)
        {
            var result = await SomethingAsync();
            label1.Text = result;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // deadlock
            var result = SomethingAsync().GetAwaiter().GetResult();
            label1.Text = result;
        }

        private static string SomethingSync()
        {
            Thread.Sleep(3000);
            return "SomethingSync Complete";
        }

        private static async Task<string> SomethingAsync()
        {
            await Task.Delay(3000);
            return "SomethingAsync Complete";
        }
    }
}
