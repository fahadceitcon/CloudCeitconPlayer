using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Ceitcon_Designer.Utilities
{
    public class ResizeThumb : Thumb
    {
        public ResizeThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Control designerItem = this.DataContext as Control;

            if (designerItem != null)
            {
                double deltaVertical, deltaHorizontal;
                Canvas canvas = designerItem.Parent as Canvas;
                double maxLeft = canvas.ActualWidth - designerItem.ActualWidth;
                double maxTop = canvas.ActualHeight - designerItem.ActualHeight;
                double left = Canvas.GetLeft(designerItem);
                double top = Canvas.GetTop(designerItem);

                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        //deltaVertical = Math.Min(-e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                        //designerItem.Height -= deltaVertical;
                        deltaVertical = Math.Min(-e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                        if (top >= 0 && top <= maxTop + deltaVertical)
                        {
                            double shiftY = top + deltaVertical;
                            designerItem.Height -= shiftY > maxTop + deltaVertical ? 0 : deltaVertical;
                        }
                        break;
                    case VerticalAlignment.Top:
                        deltaVertical = Math.Min(e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                        //if (top >= 0 && top <= maxTop + deltaVertical)
                        if (top >= 0 && top + deltaVertical >= 0)
                        {
                            //Canvas.SetTop(designerItem, top + deltaVertical);
                            //designerItem.Height -= deltaVertical;
                            double shiftY = top + deltaVertical;
                            Canvas.SetTop(designerItem, shiftY < 0 ? 0 : (shiftY > maxTop ? maxTop : shiftY));
                            designerItem.Height -= shiftY < 0 ? 0 : deltaVertical;
                        }
                        break;
                    default:
                        break;
                }

                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        //deltaHorizontal = Math.Min(e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                        //Canvas.SetLeft(designerItem, Canvas.GetLeft(designerItem) + deltaHorizontal);
                        //designerItem.Width -= deltaHorizontal;
                        deltaHorizontal = Math.Min(e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                        //if(left >= 0 && left <= maxLeft + deltaHorizontal)
                        if (left >= 0 && maxLeft + deltaHorizontal >= 0)
                        {
                            double shiftX = left + deltaHorizontal;
                            Canvas.SetLeft(designerItem, shiftX < 0 ? 0 : (shiftX > maxLeft ? maxLeft : shiftX));
                            designerItem.Width -= shiftX < 0 ? 0 : deltaHorizontal;
                        }
                        break;
                    case HorizontalAlignment.Right:
                        //deltaHorizontal = Math.Min(-e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                        //designerItem.Width -= deltaHorizontal;
                        deltaHorizontal = Math.Min(-e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                        if (left >= 0 && left <= maxLeft + deltaHorizontal)
                        {
                            double shiftX = left + deltaHorizontal;
                            designerItem.Width -= shiftX > maxLeft + deltaHorizontal ? 0 : deltaHorizontal;
                        }
                        break;
                    default:
                        break;
                }
            }

            e.Handled = true;
        }
    }
}
