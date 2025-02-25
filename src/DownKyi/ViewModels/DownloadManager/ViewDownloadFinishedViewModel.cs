﻿using DownKyi.Core.Settings;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DownKyi.ViewModels.DownloadManager
{
    public class ViewDownloadFinishedViewModel : BaseViewModel
    {
        public const string Tag = "PageDownloadManagerDownloadFinished";

        #region 页面属性申明

        private ObservableCollection<DownloadedItem> downloadedList;
        public ObservableCollection<DownloadedItem> DownloadedList
        {
            get => downloadedList;
            set => SetProperty(ref downloadedList, value);
        }

        private int finishedSortBy;
        public int FinishedSortBy
        {
            get => finishedSortBy;
            set => SetProperty(ref finishedSortBy, value);
        }

        #endregion

        public ViewDownloadFinishedViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            // 初始化DownloadedList
            DownloadedList = App.DownloadedList;

            DownloadFinishedSort finishedSort = SettingsManager.GetInstance().GetDownloadFinishedSort();
            switch (finishedSort)
            {
                case DownloadFinishedSort.DOWNLOAD:
                    FinishedSortBy = 0;
                    break;
                case DownloadFinishedSort.NUMBER:
                    FinishedSortBy = 1;
                    break;
                default:
                    FinishedSortBy = 0;
                    break;
            }
            App.SortDownloadedList(finishedSort);
        }

        #region 命令申明

        // 下载完成列表排序事件
        private DelegateCommand<object> finishedSortCommand;
        public DelegateCommand<object> FinishedSortCommand => finishedSortCommand ?? (finishedSortCommand = new DelegateCommand<object>(ExecuteFinishedSortCommand));

        /// <summary>
        /// 下载完成列表排序事件
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteFinishedSortCommand(object parameter)
        {
            if (!(parameter is int index)) { return; }

            switch (index)
            {
                case 0:
                    App.SortDownloadedList(DownloadFinishedSort.DOWNLOAD);
                    // 更新设置
                    SettingsManager.GetInstance().SetDownloadFinishedSort(DownloadFinishedSort.DOWNLOAD);
                    break;
                case 1:
                    App.SortDownloadedList(DownloadFinishedSort.NUMBER);
                    // 更新设置
                    SettingsManager.GetInstance().SetDownloadFinishedSort(DownloadFinishedSort.NUMBER);
                    break;
                default:
                    App.SortDownloadedList(DownloadFinishedSort.DOWNLOAD);
                    // 更新设置
                    SettingsManager.GetInstance().SetDownloadFinishedSort(DownloadFinishedSort.DOWNLOAD);
                    break;
            }
        }

        // 清空下载完成列表事件
        private DelegateCommand clearAllDownloadedCommand;
        public DelegateCommand ClearAllDownloadedCommand => clearAllDownloadedCommand ?? (clearAllDownloadedCommand = new DelegateCommand(ExecuteClearAllDownloadedCommand));

        /// <summary>
        /// 清空下载完成列表事件
        /// </summary>
        private async void ExecuteClearAllDownloadedCommand()
        {
            // 使用Clear()不能触发NotifyCollectionChangedAction.Remove事件
            // 因此遍历删除
            // DownloadingList中元素被删除后不能继续遍历
            await Task.Run(() =>
            {
                List<DownloadedItem> list = DownloadedList.ToList();
                foreach (DownloadedItem item in list)
                {
                    App.PropertyChangeAsync(new Action(() =>
                    {
                        App.DownloadedList.Remove(item);
                    }));
                }
            });
        }

        #endregion

    }
}
