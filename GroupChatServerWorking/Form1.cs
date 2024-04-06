namespace GroupChatServerWorking
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            KeyBox.PasswordChar = '*';

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                KeyBox.PasswordChar = '\0';
            else
                KeyBox.PasswordChar = '*';
        }

        private void KeyBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // clears log
            textBox2.Clear();
        }
    }
}
