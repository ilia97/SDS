using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Interfaces;
using Core.Services;

namespace Encryptor
{
    public partial class Form1 : Form
    {
        IEncryptionService encryptionService;

        public Form1()
        {
            encryptionService = new EncryptionService();

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var text = this.textBox1.Text;
            var key = this.textBox2.Text;

            var result = encryptionService.EncryptWithDES(text, key);

            this.textBox3.Text = result;
        }
    }
}
