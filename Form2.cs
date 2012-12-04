﻿using System;
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
        public JsonArray tempObject;
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

        private void getFeed ()
        {
            request = (HttpWebRequest)WebRequest.Create("http://cold-planet-7717.herokuapp.com/home.json");
            request.Method = "POST";
            request.AllowAutoRedirect = true;
            request.CookieContainer = cookies;

            string postData = String.Format("{{\"first\" : \"{0}\", \"last\" : \"{1}\"}}", "0", "20");

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

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
                tempObject = (JsonArray)JsonValue.Parse(responseFromServer);

                if (bindingSource1.Count > 0)
                {
                    bindingSource1 = null;

                    bindingSource1 = new BindingSource();
                }
                foreach (var post in tempObject)
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

            string postData = String.Format("{{\"content\":\"{0}\",\"user_id\":{1}}}", postTextBox.Text, userID);

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

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
    }
    public class Post
    {
        public string content;
        public string name;
        public string username;

        public Post(string tempMicropost, string tempName, string tempUsername)
        {
            this.content = tempMicropost;
            this.name = tempName;
            this.username = tempUsername;
        }

        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                this.content = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                this.name = value;
            }
        }

        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                this.username = value;
            }
        }
    }
}