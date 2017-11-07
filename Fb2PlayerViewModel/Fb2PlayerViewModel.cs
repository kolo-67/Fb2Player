using Fb2PlayerCommon;
using Fb2PlayerCommon.Contracts;
using Fb2PlayerModel;
using Fb2PlayerViewModel.Infrostructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;

namespace Fb2PlayerViewModel
{
    //----------------------------------------------------------------------------------------------------------------------    
    // class Fb2PlayerViewModel
    //----------------------------------------------------------------------------------------------------------------------    
    public class Fb2PlayerViewModel : ViewModelBase, IPlayerActionQueriable, ISavable, IScrollIntoViewAction
    {
        private const string PlayerDataFileName = "Fb2PlayerData.bin";
        public event Action PauseQuery;
        public event Action PlayQuery;
        public event Action StopQuery;
        public event Action<object, object> ListChangeQuery;
        public event Action<object> FolderDialogQuery;
        public event Action<object> ChangePathDialogQuery;
        public event Action<Object> MainGridScrollIntoView;
        public event Action SpeechPhraseScrollIntoView;
        private ObservableCollection<InstalledVoice> installedVoices;
        private InstalledVoice selectedInstalledVoice;
        private SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        private ObservableCollection<TrackInfoViewModel> traks;
        //----------------------------------------------------------------------------------------------------------------------    
        public ObservableCollection<TrackInfoViewModel> Traks
        {
            get { return traks; }
            set
            {
                traks = value;
                OnPropertyChanged("Traks");
            }
        }
        private TrackInfoViewModel selectedTrak;
        //----------------------------------------------------------------------------------------------------------------------    
        public TrackInfoViewModel SelectedTrak
        {
            get { return selectedTrak; }
            set
            {
                selectedTrak = value;
                if (selectedTrak != null)
                    selectedTrak.CreateParagraph();
                OnPropertyChanged("SelectedTrak");
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        public ObservableCollection<InstalledVoice> InstalledVoices
        {
            get
            {
                return installedVoices;
            }
            set
            {
                installedVoices = value;
                OnPropertyChanged("InstalledVoices");
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        public InstalledVoice SelectedInstalledVoice
        {
            get
            {
                return selectedInstalledVoice;
            }
            set
            {
                selectedInstalledVoice = value;
                OnPropertyChanged("SelectedInstalledVoice");
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        private double speedRatio;
        public double SpeedRatio
        {
            get
            {
                return speedRatio;
            }
            set
            {
                speedRatio = value;
                OnPropertyChanged("SpeedRatio");
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        private int speechPause;
        public int SpeechPause
        {
            get
            {
                return speechPause;
            }
            set
            {
                speechPause = value;
                OnPropertyChanged("SpeechPause");
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        private int paragraphPause;
        public int ParagraphPause
        {
            get
            {
                return paragraphPause;
            }
            set
            {
                paragraphPause = value;
                OnPropertyChanged("ParagraphPause");
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        private int sentencePause;
        public int SentencePause
        {
            get
            {
                return sentencePause;
            }
            set
            {
                sentencePause = value;
                OnPropertyChanged("SentencePause");
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        private int startToSavePosition;
        public int StartToSavePosition
        {
            get
            {
                return startToSavePosition;
            }
            set
            {
                startToSavePosition = value;
                OnPropertyChanged("StartToSavePosition");
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        public Fb2PlayerViewModel()
        {
            Traks = new ObservableCollection<TrackInfoViewModel>();
            if (File.Exists(PlayerDataFileName))
            {
                List<TrackInfo>  data = Read();
                Traks = TrackInfoViewModel.CreateTrackInfoViewModel(data);
            }
            InstalledVoices = new ObservableCollection<InstalledVoice>();
            foreach (var iv in synthesizer.GetInstalledVoices())
            {
                InstalledVoices.Add(iv);
            }
            if (InstalledVoices.Count > 0)
                SelectedInstalledVoice = InstalledVoices[0];

            PropertyChanged += Settings_PropertyChanged;

            //PropertyChanged += PlayerViewModel_PropertyChanged;
            //MediaData.CurrentTrackChanged += MediaData_CurrentTrackChanged;
            //MediaData.CurrentListChanged += MediaDataOnCurrentListChanged;

            //if (MediaData.CurrentList != null)
            //{
            //    //                SelectedIndex = 0;
            //    MediaData.SyncCurrentPlayTrack();
            //    OnMainGridScrollIntoView(MediaData.CurrentList.SelectedTrack);
            //}

        }
        //-------------------------------------------------------------------------------------------------------------------
        private List<TrackInfo> Read()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(PlayerDataFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            List<TrackInfo> data = (List<TrackInfo>)formatter.Deserialize(stream);
            stream.Close();
            return data;

        }

        //-------------------------------------------------------------------------------------------------------------------
        public void Save()
        {

            List<TrackInfo> data = TrackInfoViewModel.CreateTrackInfo(traks);
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(PlayerDataFileName, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, data);
            stream.Close();
            
        }
        //----------------------------------------------------------------------------------------------------------------------
        public void EndAction(bool isFailed)
        {

            //if (MediaData.CurrentList != null && MediaData.CurrentList.SelectedTrack != null)
            //{
            //    MediaData.CurrentList.SelectedTrack.IsFailed = isFailed;
            //    MediaData.GoToNextTrack();
            //    OnMainGridScrollIntoView(MediaData.CurrentList.SelectedTrack);
            //}
            //PlayAction();
        }
        //-------------------------------------------------------------------------------------------------------------------
        public int TrackByList(object pList)
        {
            //if (MediaData.CurrentList != null && object.ReferenceEquals(pList, MediaData.CurrentList))
            //{
            //    return MediaData.CurrentList.SelectedIndex;
            //}
            return -1;
        }
        //----------------------------------------------------------------------------------------------------------------------
        public void OnLoad()
        {
            //if (MediaData.CurrentList != null)
            //{
            //    OnMainGridScrollIntoView(MediaData.CurrentList.SelectedTrack);
            //}
        }
        //-------------------------------------------------------------------------------------------------------------------
        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedInstalledVoice")
            {
                synthesizer.SelectVoice(SelectedInstalledVoice.VoiceInfo.Name);
            }
            else if (e.PropertyName == "SpeedRatio")
            {
                synthesizer.Rate = (int)SpeedRatio;
            }
        }        
        //-------------------------------------------------------------------------------------------------------------------
        private ICommand addFilesCommand;
        //-------------------------------------------------------------------------------------------------------------------
        public ICommand AddFilesCommand
        {
            get
            {
                if (addFilesCommand == null)
                {
                    addFilesCommand = new DelegateCommand(AddFilesAction, CanAddFilesAction);
                }
                return addFilesCommand;
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        private void AddFilesAction()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = true;
            dlg.FileName = "Document"; // Default file name
                                       //            dlg.DefaultExt = ".mp3"; // Default file extension
            dlg.Filter = "Text documents (.*)|*.*"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                foreach (var f in dlg.FileNames)
                {
                    TrackInfo ti = new TrackInfo()
                    {
                        DirectoryName = Path.GetFileName(Path.GetDirectoryName(f)),
                        FileName = Path.GetFileName(f),
                        FullDirectoryName = Path.GetDirectoryName(f),
                        Position = 0
                    };
                    Traks.Add(new TrackInfoViewModel(ti));
                }
            }
            Save();
        }
        //-------------------------------------------------------------------------------------------------------------------
        private bool CanAddFilesAction()
        {
            return true;
        }
        //-------------------------------------------------------------------------------------------------------------------
        private ICommand playCommand;
        //-------------------------------------------------------------------------------------------------------------------
        public ICommand PlayCommand
        {
            get
            {
                if (playCommand == null)
                {
                    playCommand = new DelegateCommand(PlayAction, CanPlayAction);
                }
                return playCommand;
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        private async void PlayAction()
        {
            await StartPlay(false);
        }
        //-------------------------------------------------------------------------------------------------------------------
        private bool CanPlayAction()
        {
            return true;
        }
        //-------------------------------------------------------------------------------------------------------------------
        private ICommand createAudioFileCommand;
        //-------------------------------------------------------------------------------------------------------------------
        public ICommand CreateAudioFileCommand
        {
            get
            {
                if (createAudioFileCommand == null)
                {
                    createAudioFileCommand = new DelegateCommand(createAudioFileAction, CanAction);
                }
                return createAudioFileCommand;
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        private async void createAudioFileAction()
        {
            ctsPlay = new CancellationTokenSource();
            FileInfo fi = new FileInfo(SelectedTrak.FullName);
            string file = SelectedTrak.FileName.Replace(Path.GetExtension(SelectedTrak.FullName), ".wav");
            synthesizer.SetOutputToWaveFile(file);
            PromptBuilder promptBuilder = new PromptBuilder();
            ProcessDocument(promptBuilder, SelectedTrak.Paragraphs);

            Prompt prompt = new Prompt(promptBuilder);

            try
            {
                Prompt p = await speechSynthesizerAsTask.SpeakAsync(prompt, ctsPlay.Token);
            }
            catch (OperationCanceledException oce)
            {
                return;
            }
            catch (Exception ce)
            {
                MessageBox.Show(ce.Message);
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        private bool CanAction()
        {
            return true;
        }
        //-------------------------------------------------------------------------------------------------------------------
        private ICommand createAudioFilesByChaptersCommand;
        //-------------------------------------------------------------------------------------------------------------------
        public ICommand CreateAudioFilesByChaptersCommand
        {
            get
            {
                if (createAudioFilesByChaptersCommand == null)
                {
                    createAudioFilesByChaptersCommand = new DelegateCommand(CreateCreateAudioFilesByChaptersAction, CanCreateAudioFilesByChaptersAction);
                }
                return createAudioFilesByChaptersCommand;
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        private void CreateCreateAudioFilesByChaptersAction()
        {
            ctsPlay = new CancellationTokenSource();
            FileInfo fi = new FileInfo(SelectedTrak.FullName);
            List<List<string>> chapters = ReadDocXmlBychapters(fi);
            for (int i = StartToSavePosition; i < chapters.Count; i++)
            {
                string file =  SelectedTrak.FileName.Replace(Path.GetExtension(SelectedTrak.FullName), $"_Chapter{i+1}.wav");
                synthesizer.SetOutputToWaveFile(file);
                PromptBuilder promptBuilder = new PromptBuilder();
                ProcessDocument(promptBuilder, chapters[i]);

                Prompt prompt = new Prompt(promptBuilder);

                try
                {
                   synthesizer.Speak(prompt);
                }
                catch (OperationCanceledException oce)
                {
                    return;
                }
                catch (Exception ce)
                {
                    MessageBox.Show(ce.Message);
                }
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        private List<List<string>> ReadDocXmlBychapters(FileInfo fi)
        {
            List<List<string>> chapters = new List<List<string>>();

            XElement rootElement = XElement.Load(fi.FullName);
            XNamespace aw = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

            List<string> currentChapters = new List<string>();

            IEnumerable<XElement> paragraphElements = (from el in rootElement.Descendants(aw + "p") select el).ToList();
            foreach (var p in paragraphElements)
            {
                IEnumerable<XElement> bookmarkStartElements = (from el in p.Elements(aw + "bookmarkStart") select el).ToList();
                if (bookmarkStartElements.Count() > 0)
                {
                    if (currentChapters.Count > 0)
                        chapters.Add(currentChapters);
                    currentChapters = new List<string>();
                }
                IEnumerable<XElement> textElements = (from el in p.Descendants(aw + "t") select el).ToList();
                foreach (var t in textElements)
                    currentChapters.Add((string)t);
            }

            if (currentChapters.Count > 0)
                chapters.Add(currentChapters);

            return chapters;
        }

        //-------------------------------------------------------------------------------------------------------------------
        private bool CanCreateAudioFilesByChaptersAction()
        {
            return true;
        }
        //-------------------------------------------------------------------------------------------------------------------
        private DelegateCommand<System.Windows.IDataObject> playToStartCommand;
        //-------------------------------------------------------------------------------------------------------------------
        public ICommand PlayToStartCommand
        {
            get
            {
                if (playToStartCommand == null)
                {
                    playToStartCommand = new DelegateCommand<System.Windows.IDataObject>(PlayToStartAction, CanPlayToStartAction);
                }
                return playToStartCommand;
            }
        }
        private CancellationTokenSource ctsPlay = null;
        private SpeechSynthesizerAsTask speechSynthesizerAsTask = new SpeechSynthesizerAsTask();
        //-------------------------------------------------------------------------------------------------------------------
        private async void PlayToStartAction(IDataObject pData)
        {
            await StartPlay(true);

        }
        //-------------------------------------------------------------------------------------------------------------------
        private void ProcessPhrase(PromptBuilder promptBuilder, string speech)
        {
                string[] paragraphs = speech.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var paragraph in paragraphs)
                {
                    promptBuilder.StartParagraph();
                    string[] sentences = paragraph.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var sentence in sentences)
                    {
                        promptBuilder.StartSentence();
                        promptBuilder.AppendText(sentence);
                        promptBuilder.EndSentence();
                        promptBuilder.AppendBreak(new TimeSpan(0, 0, 0, 0, SentencePause));
                    }
                    promptBuilder.EndParagraph();
                    promptBuilder.AppendBreak(new TimeSpan(0, 0, 0, 0, ParagraphPause));
                }
                promptBuilder.AppendBreak(new TimeSpan(0, 0, 0, 0, SpeechPause));
            
        }
        //-------------------------------------------------------------------------------------------------------------------
        private void ProcessDocument(PromptBuilder promptBuilder, IEnumerable<string> list)
        {
            foreach (string speech in list)
            {
                ProcessPhrase(promptBuilder, speech);
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        protected virtual void OnSpeechPhraseScrollIntoView()
        {
            Action handler = SpeechPhraseScrollIntoView;
            if (handler != null)
                handler();
        }
        //-------------------------------------------------------------------------------------------------------------------
        private bool CanPlayToStartAction(IDataObject sender)
        {
            return SelectedTrak != null;
        }
        //-------------------------------------------------------------------------------------------------------------------
        private ICommand pauseCommand;
        //-------------------------------------------------------------------------------------------------------------------
        public ICommand PauseCommand
        {
            get
            {
                if (pauseCommand == null)
                {
                    pauseCommand = new DelegateCommand(PauseAction, CanPauseAction);
                }
                return pauseCommand;
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        private void PauseAction()
        {
            if (ctsPlay != null && ctsPlay.Token.CanBeCanceled)
                ctsPlay.Cancel();
            if (synthesizer.State == SynthesizerState.Speaking)
                synthesizer.SpeakAsyncCancelAll();
        }
        //-------------------------------------------------------------------------------------------------------------------
        private bool CanPauseAction()
        {
            return true;
        }
        //-------------------------------------------------------------------------------------------------------------------
        private ICommand stopCommand;
        //-------------------------------------------------------------------------------------------------------------------
        public ICommand StopCommand
        {
            get
            {
                if (stopCommand == null)
                {
                    stopCommand = new DelegateCommand(StopAction, CanStopAction);
                }
                return stopCommand;
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        private void StopAction()
        {
            if (ctsPlay != null && ctsPlay.Token.CanBeCanceled)
                ctsPlay.Cancel();
            if (synthesizer.State == SynthesizerState.Speaking)
                synthesizer.SpeakAsyncCancelAll();
        }
        //-------------------------------------------------------------------------------------------------------------------
        private bool CanStopAction()
        {
            return true;
        }
        //--------------------------------------------------------------------------------------------------------------------    
        public async Task StartPlay(bool isFromStart)
        {
            synthesizer.SetOutputToDefaultAudioDevice();
            ICollectionView view = CollectionViewSource.GetDefaultView(SelectedTrak.Paragraphs);
            view.MoveCurrentToFirst();

            ctsPlay = new CancellationTokenSource();
            if (isFromStart)
                SelectedTrak.Position = 0;

            Task task = new Task(async () =>
            {
                try
                {
                    string currentSpeech = "";
                    for (int i = SelectedTrak.Position; i < SelectedTrak.Paragraphs.Count; i++)
                    {
                        currentSpeech = SelectedTrak.Paragraphs[i];
                        if (ctsPlay.Token.IsCancellationRequested)
                            break;

                        PromptBuilder promptBuilder = new PromptBuilder();
                        ProcessPhrase(promptBuilder, currentSpeech);

                        try
                        {
                            Prompt p = await speechSynthesizerAsTask.SpeakAsync(new Prompt(promptBuilder), ctsPlay.Token);
                        }
                        catch (OperationCanceledException oce)
                        {
                            break;
                        }
                        catch (Exception ce)
                        {
                            MessageBox.Show(ce.Message);
                        }

                        Thread.Sleep(SpeechPause);

                        Application.Current.Dispatcher.Invoke(
                        (Action)(() =>
                        {
                            SelectedTrak.Position = i;
                            OnSpeechPhraseScrollIntoView();
                        }));

                    }
                }
                catch (OperationCanceledException oce)
                {

                }
                catch (Exception ce)
                {
                    MessageBox.Show(ce.Message);
                }
                ctsPlay = null;
            },
            ctsPlay.Token);

            task.Start();

            try
            {
                await task;
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Operation canceled");
            }
            // Check for other exceptions. 
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        //-------------------------------------------------------------------------------------------------------------------
        private ICommand deleteFileCommand;
        //-------------------------------------------------------------------------------------------------------------------
        public ICommand DeleteFileCommand
        {
            get
            {
                if (deleteFileCommand == null)
                {
                    deleteFileCommand = new DelegateCommand(DeleteFileAction, CanDeleteFileAction);
                }
                return deleteFileCommand;
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        private void DeleteFileAction()
        {
            if (SelectedTrak != null )
            {
                Traks.Remove(SelectedTrak);
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        private bool CanDeleteFileAction()
        {
            return true;
        }
        //-------------------------------------------------------------------------------------------------------------------
        private ICommand deleteAllFileCommand;
        //-------------------------------------------------------------------------------------------------------------------
        public ICommand DeleteAllFileCommand
        {
            get
            {
                if (deleteAllFileCommand == null)
                {
                    deleteAllFileCommand = new DelegateCommand(DeleteAllFileAction, CanDeleteAllFileAction);
                }
                return deleteAllFileCommand;
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        private void DeleteAllFileAction()
        {
            if (MessageBox.Show("Are you sure?", "All track will be deleted.", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                var deletedTracks = Traks.ToList();

                foreach (var track in deletedTracks)
                {
                    Traks.Remove(track);
                }
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        private bool CanDeleteAllFileAction()
        {
            return true;
        }
        //--------------------------------------------------------------------------------------------------------------------    
    }
    //----------------------------------------------------------------------------------------------------------------------    
}
