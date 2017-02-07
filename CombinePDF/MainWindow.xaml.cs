using System.Windows;
using CombinePDF.ViewModel;

namespace CombinePDF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using GalaSoft.MvvmLight.Messaging;

    using MahApps.Metro.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public delegate void DropDelegat(string[] files, EventArgs e);
        public DropDelegat dropEvent;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            this.AllowDrop = true;

            this.Drop += this.UIElement_OnDrop;
        }

        private void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var pdfPaths = new List<string>();
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Note that you can have more than one file.
                foreach (var file in files)
                {
                    var isPDF = new Regex("\\.pdf$").IsMatch(file);
                    if (isPDF)
                    {
                        pdfPaths.Add(file);

                    }
                }
                if (pdfPaths.Any())
                {
                    Messenger.Default.Send<List<string>>(pdfPaths);
                }
            }
        }
        }
    }
