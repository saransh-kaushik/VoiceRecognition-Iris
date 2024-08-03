using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.IO;
using AForge.Video;
using AForge.Video.DirectShow;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine();
        SpeechSynthesizer Iris = new SpeechSynthesizer();
        SpeechRecognitionEngine startlistening = new SpeechRecognitionEngine();
        Random rnd = new Random();
        int RecTimeOut = 0;
        DateTime timeNow = DateTime.Now;

        public Form1()
        {
            InitializeComponent();
        }
        //for webcam
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;
        private void Form1_Load(object sender, EventArgs e)
        {
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.LoadGrammarAsync(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines(@"DefaultCommands.txt")))));
            _recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Default_SpeechRecognized);
            _recognizer.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>(_recognizer_SpeechRecognized);
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);

            startlistening.SetInputToDefaultAudioDevice();
            startlistening.LoadGrammarAsync(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines(@"DefaultCommands.txt")))));
            startlistening.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(startlistening_SpeechRecognized);

            //for webcam
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);

        }

        

       
        private void Default_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            int ranNum;
            string speech = e.Result.Text;

            if (speech == "Hello" ) {
                Iris.SpeakAsync("greetings user");
            }
            if (speech == "What is your name")
            {
                Iris.SpeakAsync("My name is Eren, I am here to assist you");
            }

            if (speech == "How are you")
            {
                Iris.SpeakAsync("All my systems are running at optimal condition");

            }
            if (speech == "What time is it")
            {
                Iris.SpeakAsync(DateTime.Now.ToString("h mm tt"));
            }
            if (speech == "Stop talking")
            {
                Iris.SpeakAsyncCancelAll();
                ranNum = rnd.Next(1,5);
                if (ranNum == 1)
                {
                    Iris.SpeakAsync("Yes sir");

                }
                if (ranNum == 2)
                {
                    Iris.SpeakAsync("I will be quiet");
                }
            }
            if (speech == "Stop listening")
            {
                Iris.SpeakAsync("if you need me just ask");
                _recognizer.RecognizeAsyncCancel();
                startlistening.RecognizeAsync(RecognizeMode.Multiple);
            }
            if (speech == "Show commands")
            {
                string[] commands = (File.ReadAllLines(@"DefaultCommands.txt"));
                LstCommands.Items.Clear();
                LstCommands.SelectionMode = SelectionMode.None;
                LstCommands.Visible = true;
                foreach(string command in commands)
                {
                    LstCommands.Items.Add(command);
                }

            }
            if (speech == "hide commands")
            {
                LstCommands.Visible = false;
            }
        }
        private void _recognizer_SpeechRecognized(object sender, SpeechDetectedEventArgs e)
        {
            RecTimeOut = 0;
        }

        private void startlistening_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string speech = e.Result.Text;

            if (speech == "wake up")
            {
                startlistening.RecognizeAsyncCancel();
                Iris.SpeakAsync("Waking up");
                _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        private void TmrSpeaking_Tick(object sender, EventArgs e)
        {
            if (RecTimeOut == 10)
            {
                _recognizer.RecognizeAsyncCancel();

            }
            else if (RecTimeOut == 11) { 
                TmrSpeaking.Stop();
                startlistening.RecognizeAsync(RecognizeMode.Multiple);
                RecTimeOut = 0;
            }
        }
    }
}
