﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ch.hsr.wpf.gadgeothek.service;

namespace Gadgeothek_Admin_App.ViewModels
{
    static class ViewModelCollectionFactory
    {
        public static ObservableCollection<T> GetObservableCollection<T, TV>(LibraryAdminService service, List<TV> data) where T : IInstanceableViewModel
        {
            if (data == null) return null;
            ObservableCollection<T> collection = new ObservableCollection<T>();
            data.ForEach(x => collection.Add((T)Activator.CreateInstance(typeof(T), service, x)));
            return collection;
        }
    }
}
