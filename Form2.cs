using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Web;
using System.Json;
using System.Media;

namespace Jukaela_Social
{
    public partial class Form2 : Form
    {
        public HttpWebRequest request;
        public CookieContainer cookies;
        public JsonArray feed;
        public String userID;

        private BindingSource bindingSource1 = new BindingSource();

        public Form2()
        {
            InitializeComponent();

            this.Load += new System.EventHandler(Form2_Load);

        }

        private void Form2_Load(object sender, System.EventArgs e)
        {
            getFeed();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.R))
            {
                getFeed();

                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void getFeed ()
        {
            request = (HttpWebRequest)WebRequest.Create("http://cold-planet-7717.herokuapp.com/home.json");
            request.Method = "POST";
            request.AllowAutoRedirect = true;
            request.CookieContainer = cookies;

            JsonObject tempObject = new JsonObject();

            tempObject.Add(new KeyValuePair<string, JsonValue>("first", (JsonValue)"0"));
            tempObject.Add(new KeyValuePair<string, JsonValue>("last", (JsonValue)"20"));

            byte[] byteArray = Encoding.UTF8.GetBytes(tempObject.ToString());

            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();

            if (responseFromServer != null)
            {
                feed = (JsonArray)JsonValue.Parse(responseFromServer);

                if (bindingSource1.Count > 0)
                {
                    bindingSource1 = null;

                    bindingSource1 = new BindingSource();
                }
                foreach (var post in feed)
                {
                    Post tempPost = new Post((string)post["content"], (string)post["name"], (string)post["username"]);

                    bindingSource1.Add(tempPost);
                }

                reader.Close();
                dataStream.Close();
                response.Close();

                dataGridView1.AutoGenerateColumns = false;
                dataGridView1.AutoSize = true;
                dataGridView1.DataSource = bindingSource1;
              
                if (dataGridView1.Columns.Count > 0)
                {
                    return;
                }
                else
                {
                    DataGridViewColumn column = new DataGridViewTextBoxColumn();
                    column.DataPropertyName = "name";
                    column.Name = "Name";
                    dataGridView1.Columns.Add(column);

                    DataGridViewColumn column2 = new DataGridViewTextBoxColumn();
                    column2.DataPropertyName = "content";
                    column2.Name = "Content";
                    dataGridView1.Columns.Add(column2);

                    dataGridView1.Columns["name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                    dataGridView1.Columns["content"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dataGridView1.Columns["content"].Width = 183;
                    dataGridView1.Columns["content"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            getFeed();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            request = (HttpWebRequest)WebRequest.Create("http://cold-planet-7717.herokuapp.com/microposts.json");
            request.Method = "POST";
            request.AllowAutoRedirect = true;
            request.CookieContainer = cookies;

            JsonObject tempObject = new JsonObject();

            tempObject.Add(new KeyValuePair<string, JsonValue>("content", (JsonValue)postTextBox.Text));
            tempObject.Add(new KeyValuePair<string, JsonValue>("user_id", (JsonValue)userID));

            byte[] byteArray = Encoding.UTF8.GetBytes(tempObject.ToString());

            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();

            if (responseFromServer != null)
            {
                SystemSounds.Exclamation.Play();

                postTextBox.Text = null;

                getFeed();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Unimplemented item", null);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 box = new AboutBox1();
            box.ShowDialog();

        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("Are you sure?", "Sure?", MessageBoxButtons.YesNo);

            if (r.ToString() == "Yes")
            {
                Properties.Settings.Default.AutoLogin = false;
                Properties.Settings.Default.Save();

                Form1 loginDialog = new Form1();

                loginDialog.Show();

                this.Close();
            }
        }

        private void refreshFeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getFeed();
        }
    }
}
