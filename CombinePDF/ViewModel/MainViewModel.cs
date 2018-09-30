namespace CombinePDF.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text.RegularExpressions;

    using CombinePDF.Model;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.CommandWpf;
    using GalaSoft.MvvmLight.Messaging;
    using Microsoft.Win32;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.IO;

    /// <summary>
    ///     This class contains properties that the main View can data bind to.
    ///     <para>
    ///         See http://www.mvvmlight.net
    ///     </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        private PdfDocument selectedPdf;

        public PdfDocument SelectedPDF
        {
            get
            {
                return this.selectedPdf;
            }
            set
            {
                this.selectedPdf = value;
                this.RaisePropertyChanged();

            }
        }
        private int totalPageCount;

        public string TotalPageCount {
        get
            {
                if(PDFs.Any()) {
                    return PDFs?.Select(x => x.PageCount)?.Aggregate((a, b) => a + b).ToString();
          
                }
                return "0";
            }
            
        }
        //{
        //    get
        //    {
        //        return this.totalPageCount;
        //    }
        //    set
        //    {
        //        this.totalPageCount = value;
        //    }
        //}

        private ObservableCollection<PdfDocument> pdfs = new ObservableCollection<PdfDocument>
                                                             {
                                                                 /*PdfReader.Open(
                                                                     "C:\\Users\\sf9398\\Desktop\\PDFCombineTest\\2.pdf",
                                                                     PdfDocumentOpenMode.Import),
                                                                 PdfReader.Open(
                                                                     "C:\\Users\\sf9398\\Desktop\\PDFCombineTest\\3.pdf",
                                                                     PdfDocumentOpenMode.Import)
                                                                     */
                                                             };

       

        public RelayCommand<PdfDocument> RemovePdfCommand { get; private set; }
        public RelayCommand<PdfDocument> MovePdfUpCommandCommand { get; private set; }
        public RelayCommand<PdfDocument> MovePdfDownCommandCommand { get; private set; }

        public RelayCommand CombinePdfCommand { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            this._dataService = dataService;
            RemovePdfCommand = new RelayCommand<PdfDocument>((s) => RemovePdf(s));
            MovePdfDownCommandCommand = new RelayCommand<PdfDocument>((s) => this.MovePdfdown(s));
            MovePdfUpCommandCommand = new RelayCommand<PdfDocument>((s) => this.MovePdfUp(s));
            CombinePdfCommand = new RelayCommand((CombinePDFs));
            // _dataService.GetData(
            // (item, error) =>
            // {
            // if (error != null)
            // {
            // // Report error here
            // return;
            // }

            // WelcomeTitle = item.Title;
            // });
            Messenger.Default.Register<List<string>>(this, this.ReceiveDropFiles);
            PDFs.CollectionChanged += (sender, args) => RaisePropertyChanged("TotalPageCount");
        }

        private void CombinePDFs()
        {
            PdfDocument CombinedPDf = new PdfDocument();
            var totalPages = PDFs.Select(x => x.PageCount).Aggregate((a, b) => a + b);

            foreach (var pdfDocument in this.PDFs)
            {
                foreach (var page in pdfDocument.Pages)
                {
                    CombinedPDf.AddPage(page);
                }
               
            }
            

            CombinedPDf.Save("CombinedPDF.pdf");
        }

        private void RemovePdf(PdfDocument pdfDocument)
        {
            PDFs.Remove(pdfDocument);
        }

        private void MovePdfUp(PdfDocument pdfDocument)
        {
            var index = PDFs.IndexOf(pdfDocument);
            if (index != 0)
            {
                PDFs.Move(index, index - 1);
            }
            

        }

        private void MovePdfdown(PdfDocument pdfDocument)
        {
            
            var index = PDFs.IndexOf(pdfDocument);
            if (index != PDFs.Count -1)
            {
                PDFs.Move(index, index + 1);
            }
            
        }

        /// <summary>
        ///     Gets the WelcomeTitle property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public ObservableCollection<PdfDocument> PDFs
        {
            get
            {
                return this.pdfs;
            }

            set
            {
                this.Set(ref this.pdfs, value);
                this.RaisePropertyChanged();
            }
        }

        private void ReceiveDropFiles(List<string> action)
        {
            foreach (var file in action)
            {
                Console.WriteLine(file);
                var pdf = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                var title = new Regex("(\\w?\\:?\\\\?[\\w\\-_ \\\\]*\\\\+)?([\\w-_ ]+)?(\\.[\\w-_ ]+)?").Match(file).Groups[2].Value;
                pdf.Info.Title = title;

                // PDFs.Add(pdf);
                this.PDFs.Add(pdf);
            }
        }

        ////}

        ////    base.Cleanup();
        ////    // Clean up if needed
        ////{

        ////public override void Cleanup()
    }
}