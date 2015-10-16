using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;

//a9t9's BaiDu OCR Test - http://a9t9.com 

namespace BaiduAPITest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            cmbLanguage.SelectedIndex = 0;
        }

        private string getSelectedLanguage()
        {
            string strLang = "";
            switch(cmbLanguage.SelectedIndex)
            {
                case 0:
                    strLang = "CHN_ENG";
                    break;
                case 1:
                    strLang = "ENG";
                    break;
                case 2:
                    strLang = "JAP";
                    break;
                case 3:
                    strLang = "KOR";
                    break;
             }
            return strLang;

        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "jpeg files|*.jpg;*.JPG";
            if(fileDlg.ShowDialog() == DialogResult.OK)
            {
                FileInfo fileInfo = new FileInfo(fileDlg.FileName);
                if(fileInfo.Length > 300 * 1024)
                {
                    MessageBox.Show("jpeg file's size can not be larger than 300kb");
                    return;
                }
                pictureBox.BackgroundImage = Image.FromFile(fileDlg.FileName);
            }
        }

        private byte[] ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                return imageBytes;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox.BackgroundImage == null)
                return;

            txtResult.Text = "";

            button1.Enabled = false;
            button2.Enabled = false;

            try
            {
                HttpClient httpClient = new HttpClient();

               //Enter your API key here (If you have problems signing up at BaiDu, email blog@a9t9.com)
               httpClient.DefaultRequestHeaders.TryAddWithoutValidation("apikey", "YOUR KEY HERE");
       
                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent("pc"), "fromdevice");
                form.Add(new StringContent("10.10.10.0"), "clientip");
                form.Add(new StringContent("LocateRecognize"), "detecttype");
                form.Add(new StringContent(getSelectedLanguage()), "languagetype");
                form.Add(new StringContent("2"), "imagetype");

                byte [] imageData = ImageToBase64(pictureBox.BackgroundImage, System.Drawing.Imaging.ImageFormat.Jpeg);
                form.Add(new ByteArrayContent(imageData, 0, imageData.Length), "image", "image.jpg");

                HttpResponseMessage response = await httpClient.PostAsync("http://apis.baidu.com/apistore/idlocr/ocr", form);
                string strContent = await response.Content.ReadAsStringAsync();

                OCRResult ocrResult = JsonConvert.DeserializeObject<OCRResult>(strContent);

                if(ocrResult.errMsg == "success")
                {
                    for (int i = 0; i < ocrResult.retData.Count; i++)
                    {
                        txtResult.Text = txtResult.Text + ocrResult.retData[i].word;
                    }
                }
                else
                {
                    MessageBox.Show(strContent);
                }
                
            }
            catch(Exception exception)
            {

            }

            button1.Enabled = true;
            button2.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
