using GifComposerWpf.ViewModels;
using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System.Windows;

namespace GifComposerWpf.Behaviors
{
    public class ImageItemDropHandler : IDropTarget
    {
        private readonly Action _updateImageOrder;

        public ImageItemDropHandler(Action updateImageOrder)
        {
            _updateImageOrder = updateImageOrder;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if(dropInfo.Data is ImageItemViewModel && dropInfo.TargetItem is ImageItemViewModel)
            {
                dropInfo.Effects = DragDropEffects.Move;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is not ImageItemViewModel sourceItem) return;

            if (dropInfo.TargetCollection is not IEnumerable<ImageItemViewModel> targetCollection) return;

            var images = dropInfo.TargetCollection as ObservableCollection<ImageItemViewModel>;
            if (images == null) return;

            int oldIndex = images.IndexOf(sourceItem);
            int newIndex = dropInfo.InsertIndex;

            // GongSolutions 的 InsertIndex 已考慮插入位置,但搬移後 index 需微調
            if (oldIndex < newIndex)
            {
                newIndex--;
            }

            if (oldIndex == newIndex) return;

            images.Move(oldIndex, newIndex);

            _updateImageOrder();
        }
    }
}
