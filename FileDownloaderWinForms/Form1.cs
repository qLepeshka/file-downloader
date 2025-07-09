using System;
using System.Net;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FileDownloaderWinForms
{
    public partial class Form1 : Form
    {
        private WebClient webClient;

        public Form1()
        {
            InitializeComponent();

            webClient = new WebClient();
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
            this.AllowDrop = true;
            this.DragEnter += Form1_DragEnter;
            this.DragDrop += Form1_DragDrop;

            lblStatus.Text = "Перетащите ссылку на файл сюда.";


            this.FormClosing += MainForm_FormClosing;
            if (string.IsNullOrWhiteSpace(txtSavePath.Text))
            {
                txtSavePath.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            }
        }
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text) ||
                (e.Data.GetDataPresent(DataFormats.Text) && IsValidUrl((string)e.Data.GetData(DataFormats.Text))))
            {
                e.Effect = DragDropEffects.Link;
                lblStatus.Text = "Отпустите, чтобы начать скачивание...";
            }
            else
            {
                e.Effect = DragDropEffects.None;
                lblStatus.Text = "Неверный формат данных. Ожидается ссылка на файл.";
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string url = "";

            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                url = (string)e.Data.GetData(DataFormats.Text);
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                url = (string)e.Data.GetData(DataFormats.Text);
            }

            if (!string.IsNullOrWhiteSpace(url))
            {
                if (!IsValidUrl(url))
                {
                    MessageBox.Show("Перетащенная ссылка не является корректным URL.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    lblStatus.Text = "Ошибка: некорректный URL.";
                    return;
                }

                string saveFilePath = txtSavePath.Text;
                string fileName = "";

                try
                {
                    Uri uri = new Uri(url);
                    fileName = Path.GetFileName(uri.LocalPath);

                    if (string.IsNullOrWhiteSpace(fileName) || !Path.HasExtension(fileName))
                    {

                        fileName = "downloaded_file_" + DateTime.Now.ToFileTimeUtc() + ".bin";
                    }
                }
                catch (UriFormatException)
                {
                    MessageBox.Show("Некорректная ссылка.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    lblStatus.Text = "Ошибка: Некорректная ссылка.";
                    return;
                }

                if (Directory.Exists(saveFilePath))
                {
                    saveFilePath = Path.Combine(saveFilePath, fileName);
                }
                else
                {
                    string directory = Path.GetDirectoryName(saveFilePath);
                    if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
                    {
                        try
                        {
                            Directory.CreateDirectory(directory);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка создания директории: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                txtSavePath.Text = saveFilePath;
                StartDownload(url, saveFilePath);
            }
            else
            {
                lblStatus.Text = "Не удалось получить ссылку из перетащенных данных.";
            }
        }

        private bool IsValidUrl(string text)
        {
            return Uri.TryCreate(text, UriKind.Absolute, out Uri uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
        private void StartDownload(string url, string saveFilePath)
        {
            if (webClient.IsBusy)
            {
                MessageBox.Show("Уже идет скачивание. Дождитесь завершения или отмените текущее.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string directory = Path.GetDirectoryName(saveFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка создания директории: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            try
            {
                lblStatus.Text = "Начинаю скачивание...";
                progressBar1.Value = 0;
                txtSavePath.Enabled = false;

                webClient.DownloadFileAsync(new Uri(url), saveFilePath);
            }
            catch (UriFormatException)
            {
                MessageBox.Show("Некорректный URL. Убедитесь, что он начинается с http:// или https://", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Ошибка: Некорректный URL.";
                txtSavePath.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подготовке скачивания: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Ошибка: " + ex.Message;
                txtSavePath.Enabled = true;
            }
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (progressBar1 != null)
            {
                progressBar1.Value = e.ProgressPercentage;
            }
            if (lblStatus != null)
            {
                lblStatus.Text = $"Скачиваю... {e.ProgressPercentage}% ({e.BytesReceived / 1024} KB / {e.TotalBytesToReceive / 1024} KB)";
            }
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (txtSavePath != null) txtSavePath.Enabled = true;

            if (e.Cancelled)
            {
                if (lblStatus != null) lblStatus.Text = "Скачивание отменено.";
                MessageBox.Show("Скачивание отменено пользователем.", "Отмена", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (e.Error != null)
            {
                if (lblStatus != null) lblStatus.Text = $"Ошибка скачивания: {e.Error.Message}";
                MessageBox.Show($"Ошибка при скачивании: {e.Error.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (lblStatus != null) lblStatus.Text = "Скачивание завершено!";
                if (progressBar1 != null) progressBar1.Value = 100;
                MessageBox.Show("Файл успешно скачан!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (webClient != null)
            {
                webClient.Dispose();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblStatus_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}



