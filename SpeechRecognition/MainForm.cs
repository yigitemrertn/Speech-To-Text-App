// MainForm.cs
using System;
using System.Windows.Forms;
using System.IO;
using Vosk;
using NAudio.Wave;
using System.Text;

namespace SpeechRecognition
{
    public partial class MainForm : Form
    {
        private VoskRecognizer recognizer;
        private WaveInEvent waveIn;
        private bool isListening = false;
        private const string MODEL_PATH = @"C:\vosk-model-small-tr-0.3";

        private Button btnStart;
        private Button btnStop;
        private Button btnClear;
        private Label lblStatus;
        private TextBox txtOutput;

        public MainForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            InitializeVosk();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "Vosk Speech to Text";
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            btnStart = new Button();
            btnStart.Location = new System.Drawing.Point(50, 30);
            btnStart.Size = new System.Drawing.Size(100, 40);
            btnStart.Text = "Dinlemeyi Başlat";
            btnStart.Click += BtnStart_Click;
            this.Controls.Add(btnStart);

            btnStop = new Button();
            btnStop.Location = new System.Drawing.Point(170, 30);
            btnStop.Size = new System.Drawing.Size(100, 40);
            btnStop.Text = "Durdur";
            btnStop.Enabled = false;
            btnStop.Click += BtnStop_Click;
            this.Controls.Add(btnStop);

            btnClear = new Button();
            btnClear.Location = new System.Drawing.Point(290, 30);
            btnClear.Size = new System.Drawing.Size(100, 40);
            btnClear.Text = "Temizle";
            btnClear.Click += BtnClear_Click;
            this.Controls.Add(btnClear);

            lblStatus = new Label();
            lblStatus.Location = new System.Drawing.Point(50, 80);
            lblStatus.Size = new System.Drawing.Size(500, 20);
            lblStatus.Text = "Durum: Hazır";
            this.Controls.Add(lblStatus);

            txtOutput = new TextBox();
            txtOutput.Location = new System.Drawing.Point(50, 110);
            txtOutput.Size = new System.Drawing.Size(500, 250);
            txtOutput.Multiline = true;
            txtOutput.ScrollBars = ScrollBars.Vertical;
            txtOutput.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Controls.Add(txtOutput);
        }

        private void InitializeVosk()
        {
            try
            {
                if (!Directory.Exists(MODEL_PATH))
                {
                    MessageBox.Show(
                        $"Model bulunamadı!\n\nBeklenen konum: {MODEL_PATH}\n\n" +
                        "Lütfen Vosk modelini indirin ve doğru konuma yerleştirin.",
                        "Model Hatası",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    btnStart.Enabled = false;
                    return;
                }

                Vosk.Vosk.SetLogLevel(0);
                Model model = new Model(MODEL_PATH);
                recognizer = new VoskRecognizer(model, 16000.0f);
                recognizer.SetMaxAlternatives(0);
                recognizer.SetWords(true);

                lblStatus.Text = "Durum: Hazır - Türkçe model yüklendi";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Vosk başlatma hatası: {ex.Message}\n\n" +
                    "Model dosyalarının doğru olduğundan emin olun.",
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                btnStart.Enabled = false;
            }
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            try
            {
                waveIn = new WaveInEvent();
                waveIn.WaveFormat = new WaveFormat(16000, 1);
                waveIn.DataAvailable += WaveIn_DataAvailable;
                waveIn.RecordingStopped += WaveIn_RecordingStopped;

                waveIn.StartRecording();
                isListening = true;

                btnStart.Enabled = false;
                btnStop.Enabled = true;
                lblStatus.Text = "Durum: Dinleniyor... (Konuşmaya başlayın)";
                lblStatus.ForeColor = System.Drawing.Color.Green;

                txtOutput.AppendText("[Dinleme başladı]\r\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Mikrofon başlatma hatası: {ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            StopListening();
        }

        private void StopListening()
        {
            if (waveIn != null)
            {
                waveIn.StopRecording();
                waveIn.Dispose();
                waveIn = null;
            }

            isListening = false;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            lblStatus.Text = "Durum: Durduruldu";
            lblStatus.ForeColor = System.Drawing.Color.Black;

            txtOutput.AppendText("\r\n[Dinleme durdu]\r\n");
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtOutput.Clear();
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (recognizer.AcceptWaveform(e.Buffer, e.BytesRecorded))
            {
                var result = recognizer.Result();
                ProcessResult(result);
            }
            else
            {
                var partialResult = recognizer.PartialResult();
                ProcessPartialResult(partialResult);
            }
        }

        private void WaveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                MessageBox.Show($"Kayıt hatası: {e.Exception.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessResult(string json)
        {
            try
            {
                if (json.Contains("\"text\" : \"\""))
                    return;

                int textStart = json.IndexOf("\"text\" : \"") + 10;
                int textEnd = json.IndexOf("\"", textStart);

                if (textEnd > textStart)
                {
                    string text = json.Substring(textStart, textEnd - textStart);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() => UpdateTextBox(text, true)));
                        }
                        else
                        {
                            UpdateTextBox(text, true);
                        }
                    }
                }
            }
            catch { }
        }

        private void ProcessPartialResult(string json)
        {
            try
            {
                int partialStart = json.IndexOf("\"partial\" : \"") + 13;
                int partialEnd = json.IndexOf("\"", partialStart);

                if (partialEnd > partialStart)
                {
                    string partial = json.Substring(partialStart, partialEnd - partialStart);
                    if (!string.IsNullOrWhiteSpace(partial))
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() => lblStatus.Text = $"Durum: Dinleniyor... [{partial}]"));
                        }
                        else
                        {
                            lblStatus.Text = $"Durum: Dinleniyor... [{partial}]";
                        }
                    }
                }
            }
            catch { }
        }

        private void UpdateTextBox(string text, bool isFinal)
        {
            if (isFinal)
            {
                txtOutput.AppendText(text + " ");
                txtOutput.ScrollToCaret();
                lblStatus.Text = "Durum: Dinleniyor...";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopListening();

            if (recognizer != null)
            {
                recognizer.Dispose();
            }

            base.OnFormClosing(e);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(MODEL_PATH))
            {
                WarnForm wrn = new WarnForm();
                wrn.Show();
            }
        }
    }
}

