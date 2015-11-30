using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ch.hsr.wpf.gadgeothek.service;
using System.Configuration;
using ch.hsr.wpf.gadgeothek.domain;
using MahApps.Metro.Controls;

namespace Gadgeothek_Admin_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        LibraryAdminService service = new LibraryAdminService(ConfigurationManager.AppSettings["server"]);
        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            
            List<Gadget> data = service.GetAllGadgets();
            ObservableCollection<Gadget> sourceCollection = new ObservableCollection<Gadget>(data);

            sourceCollection.CollectionChanged +=
                new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);
            gadgetsDataGridView.ItemsSource = sourceCollection;

        }

        void DataGrid_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool healthy = false;
            if (sender.GetType() == typeof (ObservableCollection<Gadget>))
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Remove:
                        foreach(Gadget gadgetToRemove in e.OldItems)
                        {
                            healthy = service.DeleteGadget(gadgetToRemove);
                        }
                        break;
                    case NotifyCollectionChangedAction.Add:
                        foreach (Gadget newGadget in e.NewItems)
                        {
                            healthy = newGadget.Manufacturer != null 
                                && newGadget.Name != null 
                                && service.AddGadget(newGadget);
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (!healthy)
            {
                // TODO prevent grid change
            }
        }
    }
}
