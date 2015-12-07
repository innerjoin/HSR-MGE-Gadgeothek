using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ch.hsr.wpf.gadgeothek.service;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using ch.hsr.wpf.gadgeothek.domain;
using Gadgeothek_Admin_App.ViewModels;
using MahApps.Metro.Controls;

namespace Gadgeothek_Admin_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [SuppressMessage("ReSharper", "SuggestVarOrType_Elsewhere")]
    public partial class MainWindow : MetroWindow
    {
        readonly LibraryAdminService _service = new LibraryAdminService(ConfigurationManager.AppSettings["server"]);
        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            List<Gadget> data = _service.GetAllGadgets();

            ObservableCollection<GadgetViewModel> viewGadgets = GetViewGadgets(data);

            if (viewGadgets != null)
            {
                viewGadgets.CollectionChanged +=
                    new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);
                GadgetsDataGridView.ItemsSource = viewGadgets;
            }
            else
            {
                MessageBox.Show("server seems to be unavailable");
            }

        }

        private ObservableCollection<GadgetViewModel> GetViewGadgets(List<Gadget> data)
        {
            ObservableCollection <GadgetViewModel> obsGadgets = new ObservableCollection<GadgetViewModel>();
            if (data == null)
                return null;
            data.ForEach(x => obsGadgets.Add(new GadgetViewModel(_service, x)));
            return obsGadgets;
        }

        void DataGrid_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool healthy = false;
            if (sender.GetType() == typeof (ObservableCollection<GadgetViewModel>))
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Remove:
                        foreach(GadgetViewModel gadgetToRemove in e.OldItems)
                        {
                            healthy = gadgetToRemove.Remove();
                        }
                        break;
                    case NotifyCollectionChangedAction.Add:
                        foreach (Gadget newGadget in e.NewItems)
                        {
                            healthy = newGadget.Manufacturer != null 
                                && newGadget.Name != null 
                                && _service.AddGadget(newGadget);
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
