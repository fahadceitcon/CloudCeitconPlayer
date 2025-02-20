using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Ceitcon_Designer.Utilities
{
    public class MoveThumb : Thumb
    {
        public MoveThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Control designerItem = this.DataContext as Control;
            if (designerItem != null)
            {
                double left = Canvas.GetLeft(designerItem);
                double top = Canvas.GetTop(designerItem);
                Canvas canvas = designerItem.Parent as Canvas;
                double maxLeft = canvas.ActualWidth - ActualWidth;
                double maxTop = canvas.ActualHeight - ActualHeight;

                if (left >= 0 && top >= 0 && left <= maxLeft && top <= maxTop)
                {
                    double shiftX = left + e.HorizontalChange;
                    double shiftY = top + e.VerticalChange;
                    if (SpanEffect.Enable)
                    {
                        shiftX = (int)shiftX / SpanEffect.Step * SpanEffect.Step;
                        shiftY = (int)shiftY / SpanEffect.Step * SpanEffect.Step;
                    }

                    Canvas.SetLeft(designerItem, shiftX < 0 ? 0 : (shiftX > maxLeft ? maxLeft : shiftX));
                    Canvas.SetTop(designerItem, shiftY < 0 ? 0 : (shiftY > maxTop ? maxTop : shiftY));
                }
            }
        }
    }

    public static class SpanEffect
    {
        public static bool Enable = false;
        public static int Step = 50;
    }
    
}
