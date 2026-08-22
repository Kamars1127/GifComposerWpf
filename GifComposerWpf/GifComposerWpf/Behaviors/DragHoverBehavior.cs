using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace GifComposerWpf.Behaviors
{
    /// <summary>
    /// 提供拖曳 Hover 狀態的附加行為。
    /// 當檔案拖曳進入、離開或放置於指定 UI 元素時，
    /// 自動更新 <see cref="IsDragOverProperty"/>，讓 ViewModel 或 XAML 可以根據拖曳狀態變更 UI。
    /// </summary>
    public static class DragHoverBehavior
    {
        /// <summary>
        /// 表示目前是否有檔案拖曳至指定 UI 元素上方。
        /// 支援 TwoWay Binding，可與 ViewModel 的屬性雙向同步。
        /// 自動更新 <see cref="IsDragOverProperty"/>，讓 ViewModel 或 XAML 可以根據拖曳狀態變更 UI。
        /// </summary>
        public static readonly DependencyProperty IsDragOverProperty =
            DependencyProperty.RegisterAttached(
                "IsDragOver",
                typeof(bool),
                typeof(DragHoverBehavior),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 取得指定 UI 元素目前的拖曳 Hover 狀態。
        /// </summary>
        /// <param name="obj">要取得狀態的 DependencyObject。</param>
        /// <returns>若目前有檔案拖曳至元素上方則為 <see langword="true"/>；否則為 <see langword="false"/>。</returns>
        public static bool GetIsDragOver(DependencyObject obj)
            => (bool)obj.GetValue(IsDragOverProperty);

        /// <summary>
        /// 設定指定 UI 元素目前的拖曳 Hover 狀態。
        /// </summary>
        /// <param name="obj">要設定狀態的 DependencyObject。</param>
        /// <param name="value">是否處於拖曳 Hover 狀態。</param>
        public static void SetIsDragOver(DependencyObject obj, bool value)
            => obj.SetValue(IsDragOverProperty, value);

        /// <summary>
        /// 控制是否啟用拖曳 Hover 行為。
        /// 啟用後會自動監聽 DragEnter、DragLeave 與 Drop 事件。
        /// </summary>
        public static readonly DependencyProperty EnableProperty =
            DependencyProperty.RegisterAttached(
                "Enable",
                typeof(bool),
                typeof(DragHoverBehavior),
                new PropertyMetadata(false, OnEnableChanged));

        /// <summary>
        /// 取得指定 UI 元素是否已啟用拖曳 Hover 行為。
        /// </summary>
        /// <param name="obj">要取得設定的 DependencyObject。</param>
        /// <returns>若已啟用則為 <see langword="true"/>；否則為 <see langword="false"/>。</returns>
        public static bool GetEnable(DependencyObject obj)
            => (bool)obj.GetValue (EnableProperty);

        /// <summary>
        /// 設定指定 UI 元素是否啟用拖曳 Hover 行為。
        /// </summary>
        /// <param name="obj">要設定的 DependencyObject。</param>
        /// <param name="value">是否啟用拖曳 Hover 行為。</param>
        public static void SetEnable(DependencyObject obj, bool value)
            => obj.SetValue(EnableProperty, value);

        /// <summary>
        /// 當 Enable 附加屬性的值發生變更時執行，負責註冊或移除 UI 元素的拖曳事件。
        /// </summary>
        /// <param name="d">套用此附加行為的 DependencyObject。</param>
        /// <param name="e">Enable 屬性的變更資訊。</param>
        private static void OnEnableChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            // 只有 UIElement 才具有 DragEnter、DragLeave、Drop 等拖曳事件。
            if (d is not UIElement element) return;

            if ((bool)e.NewValue)
            {
                //啟用 Behavior 時註冊拖曳事件。
                element.DragEnter += OnDragEnter;
                element.DragLeave += OnDragLeave;
                element.Drop += OnDrop;
            }
            else
            {
                // 關閉 Behavior 時移除事件，避免重複註冊或不必要的事件參考。
                element.DragEnter -= OnDragEnter;
                element.DragLeave -= OnDragLeave;
                element.Drop -= OnDrop;
            }
        }

        /// <summary>
        /// 當拖曳資料進入 UI 元素時執行。若拖曳內容包含檔案，將 IsDragOver 設為 true。
        /// </summary>
        /// <param name="sender">觸發事件的 UI 元素。</param>
        /// <param name="e">拖曳事件資訊。</param>
        private static void OnDragEnter(object sender, DragEventArgs e)
        {
            if (sender is not DependencyObject element) return;

            //只針對從檔案總管拖入的檔案啟用 Hover 狀態。
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            SetIsDragOver(element, true);
        }

        /// <summary>
        /// 當拖曳資料離開 UI 元素時執行，並取消拖曳 Hover 狀態。
        /// </summary>
        /// <param name="sender">觸發事件的 UI 元素。</param>
        /// <param name="e">拖曳事件資訊。</param>
        private static void OnDragLeave(object sender, DragEventArgs e)
        {
            if(sender is DependencyObject element)
            {
                SetIsDragOver(element, false);
            }
        }

        /// <summary>
        /// 當拖曳資料放置至 UI 元素時執行，並在 Drop 完成後取消拖曳 Hover 狀態。
        /// </summary>
        /// <param name="sender">觸發事件的 UI 元素。<</param>
        /// <param name="e">拖曳事件資訊。</param>
        private static void OnDrop(object sender, DragEventArgs e)
        {
            if(sender is DependencyObject element)
            {
                SetIsDragOver(element, false);
            }
        }
    }
}
