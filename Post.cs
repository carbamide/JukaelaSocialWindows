using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jukaela_Social
{
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
