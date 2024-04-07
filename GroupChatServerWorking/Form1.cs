namespace GroupChatServerWorking
{
    public partial class Form1 : Form
    {
        NetworkOps ops;
        Object logBoxlock;
        Task NetworkTask;
        CancellationToken cancellationToken;
        CancellationTokenSource cancellationTokenSource;
        Object keyLock;
        public Form1()
        {
            InitializeComponent();
            KeyBox.PasswordChar = '*';
            logBoxlock = new Object();
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            keyLock = new Object();

        }
        public void AppendLogBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendLogBox), new object[] { value });
                return;
            }
            textBox2.Text += value;
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ops = NetworkOps.GetInstance(PortBox.Text, AddressBox.Text, textBox2, logBoxlock, KeyBox.Text, keyLock, UsernameBox.Text, this);
            NetworkTask = Task.Run(() => { ops.Run(cancellationToken); });
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

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (cancellationToken.CanBeCanceled)
                {
                    cancellationTokenSource.Cancel();
                }
                else
                {
                    MessageBox.Show("unable to cancel thread");
                }
                ops.CloseConnection();
            }
            catch (Exception ex)
            {
                textBox2.Text = ex.Message;
            }
        }

        private void UsernameBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ops.SendToAll(new Messages.Message(UsernameBox.Text, textBox1.Text, DateTime.Now));
            textBox1.Clear();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter) 
            {
                button1_Click(sender, e);
            }
        }
    }
}
