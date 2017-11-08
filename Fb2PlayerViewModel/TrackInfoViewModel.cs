using Fb2PlayerCommon;
using Fb2PlayerModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Linq;

namespace Fb2PlayerViewModel
{
    //----------------------------------------------------------------------------------------------------------------------
    // class TrackInfoViewModel
    //----------------------------------------------------------------------------------------------------------------------    
    public class TrackInfoViewModel : ViewModelBase
    {
        private TrackInfo trackInfo;
        //----------------------------------------------------------------------------------------------------------------------    
        public TrackInfo Track
        {
            get { return trackInfo; }
        }
        private ObservableCollection<string> paragraphs;
        //----------------------------------------------------------------------------------------------------------------------    
        public ObservableCollection<string> Paragraphs
        {
            get
            {
                //if (paragraphs == null)
                //    paragraphs = GetParagraphs();
                return paragraphs;
            }
            set
            {
                paragraphs = value;
                OnPropertyChanged("Paragraphs");
            }
        }
        //----------------------------------------------------------------------------------------------------------------------    
        public string FileName
        {
            get { return trackInfo.FileName; }
            set
            {
                trackInfo.FileName = value;
                OnPropertyChanged("FileName");
            }
        }
        //----------------------------------------------------------------------------------------------------------------------    
        public string DirectoryName
        {
            get { return trackInfo.DirectoryName; }
            set
            {
                trackInfo.DirectoryName = value;
                OnPropertyChanged("DirectoryName");
            }
        }
        //----------------------------------------------------------------------------------------------------------------------    
        public string FullDirectoryName
        {
            get { return trackInfo.FullDirectoryName; }
            set
            {
                trackInfo.FullDirectoryName = value;
                OnPropertyChanged("FullDirectoryName");
                OnPropertyChanged("FullName");
            }
        }
        //----------------------------------------------------------------------------------------------------------------------    
        public string FullName
        {
            get
            {
                return trackInfo.FullDirectoryName + "\\" + trackInfo.FileName;
            }
        }

        //----------------------------------------------------------------------------------------------------------------------    
        public bool IsFailed
        {
            get { return trackInfo.IsFailed; }
            set
            {
                trackInfo.IsFailed = value;
                OnPropertyChanged("IsFailed");
            }
        }
        //----------------------------------------------------------------------------------------------------------------------    
        public int Position
        {
            get { return trackInfo.Position; }
            set
            {
                trackInfo.Position = value;
                OnPropertyChanged("Position");
            }
        }
        //----------------------------------------------------------------------------------------------------------------------    
        public double MaxPosition
        {
            get { return trackInfo.MaxPosition; }
            set
            {
                trackInfo.MaxPosition = value;
                OnPropertyChanged("MaxPosition");
            }
        }
        //----------------------------------------------------------------------------------------------------------------------    
        public TrackInfoViewModel(TrackInfo pTrackInfo)
        {
            trackInfo = pTrackInfo;
        }
        //----------------------------------------------------------------------------------------------------------------------
        private ObservableCollection<string> GetParagraphs()
        {
            ObservableCollection<string> result = new ObservableCollection<string>();
            FileInfo fi = new FileInfo(FullName);
            if (File.Exists(FullName))
            {
                if (fi.Extension.ToUpper() == ".FB2")
                {
                    XElement rootElement = XElement.Load(FullName);
                    IEnumerable<XElement> paragraphElements = (from el in rootElement.Descendants("p") select el).ToList();
                    foreach (var p in paragraphElements)
                        result.Add((string)p);
                }
                else
                {
                    XElement rootElement = XElement.Load(FullName);
                    XNamespace aw = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
                    IEnumerable<XElement> paragraphElements = (from el in rootElement.Descendants(aw + "t") select el).ToList();
                    foreach (var p in paragraphElements)
                        result.Add((string)p);
                }
            }
            return result;
        }
        //----------------------------------------------------------------------------------------------------------------------
        public void CreateParagraph()
        {
            if (Paragraphs == null)
                 Paragraphs = GetParagraphs();
       }
        //----------------------------------------------------------------------------------------------------------------------
        public override string ToString()
        {
            return FileName + " / " + Position.ToString() + (IsFailed ? " Failed" : "");
        }
        //----------------------------------------------------------------------------------------------------------------------
        public void Start()
        {

        }
        //----------------------------------------------------------------------------------------------------------------------
        public static ObservableCollection<TrackInfoViewModel> CreateTrackInfoViewModel(List<TrackInfo> list)
        {
            ObservableCollection<TrackInfoViewModel> result = new ObservableCollection<TrackInfoViewModel>();
            foreach (var t in list)
                result.Add(new TrackInfoViewModel(t));
            return result;
        }
        //----------------------------------------------------------------------------------------------------------------------
        public static List<TrackInfo> CreateTrackInfo(ObservableCollection<TrackInfoViewModel> list)
        {
            List<TrackInfo> result = new List<TrackInfo>();
            foreach (var t in list)
                result.Add(t.Track);
            return result;
        }
        //----------------------------------------------------------------------------------------------------------------------
    }
    //----------------------------------------------------------------------------------------------------------------------    

}