using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Json;

namespace Jukaela_Social
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            if (Properties.Settings.Default.AutoLogin)
            {
                loginCheckbox.Checked = Properties.Settings.Default.AutoLogin;
                usernameTextBox.Text = Properties.Settings.Default.Username;
                passwordTextBox.Text = Properties.Settings.Default.Password;

                loginButton_Click(loginButton, null);
            }
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            saveSettings();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://cold-planet-7717.herokuapp.com/sessions.json");
            request.Method = "POST";
            request.CookieContainer = new CookieContainer();

            JsonObject sessionObject = new JsonObject();
            JsonObject infoObject = new JsonObject();

            infoObject.Add(new KeyValuePair<string, JsonValue>("email", usernameTextBox.Text));
            infoObject.Add(new KeyValuePair<string, JsonValue>("password", passwordTextBox.Text));
            infoObject.Add(new KeyValuePair<string, JsonValue>("apns", ""));

            sessionObject.Add(new KeyValuePair<string, JsonValue>("session", infoObject));

            byte[] byteArray = Encoding.UTF8.GetBytes(sessionObject.ToString());

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
                JsonObject responseObject = (JsonObject)JsonValue.Parse(responseFromServer);

                Form2 feed = new Form2();

                request.CookieContainer.Add(response.Cookies);

                feed.request = request;
                feed.cookies = request.CookieContainer;
                feed.userID = (string)responseObject["id"];

                feed.Show();
                feed.FormClosed += new FormClosedEventHandler(feedClosed);

                this.WindowState = FormWindowState.Minimized;
            }

            reader.Close();
            dataStream.Close();
            response.Close();
        }

        void saveSettings()
        {
            if (loginCheckbox.Checked)
            {
                Properties.Settings.Default.Username = usernameTextBox.Text;
                Properties.Settings.Default.Password = passwordTextBox.Text;
                Properties.Settings.Default.AutoLogin = loginCheckbox.Checked;

                Properties.Settings.Default.Save();
            }
        }
        void feedClosed(object sender, FormClosedEventArgs e)
        {
            Form1 login = new Form1();

            login.Show();
        }
    }
}
