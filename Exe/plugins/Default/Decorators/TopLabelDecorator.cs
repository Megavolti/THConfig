using System.Collections.Generic;

namespace Turbo.Plugins.Default
{

    public delegate string StringGeneratorFunc();

    public enum HorizontalAlign { Left, Center, Right }

    // this is not a plugin, just a helper class to display fixed sized labels on the screen
    public class TopLabelDecorator: ITransparentCollection
    {

        public bool Enabled { get; set; }
        public IController Hud { get; set; }

        public IFont TextFont { get; set; }

        // Option #1: Use brushes for background and border
        public IBrush BackgroundBrush { get; set; }
        public IBrush BorderBrush { get; set; }

        // Option #2: Use textures for background
        public ITexture BackgroundTexture1 { get; set; }
        public ITexture BackgroundTexture2 { get; set; }
        public float BackgroundTextureOpacity1 { get; set; }
        public float BackgroundTextureOpacity2 { get; set; }

        public StringGeneratorFunc TextFunc { get; set; }
        public StringGeneratorFunc HintFunc { get; set; }

        public bool HideBackgroundWhenTextIsEmpty { get; set; }

        public List<TopLabelDecorator> ExpandUpLabels { get; set; }
        public List<TopLabelDecorator> ExpandDownLabels { get; set; }
        public List<TopLabelDecorator> ExpandRightLabels { get; set; }
        public List<TopLabelDecorator> ExpandLeftLabels { get; set; }

        public TopLabelDecorator(IController hud)
        {
            Enabled = true;
            Hud = hud;
            BackgroundTextureOpacity1 = 1.0f;
            BackgroundTextureOpacity2 = 1.0f;
            HideBackgroundWhenTextIsEmpty = false;
        }

        public void Paint(float x, float y, float w, float h, HorizontalAlign align)
        {
            if (!Enabled) return;
            if (TextFont == null) return;

            var text = TextFunc != null ? TextFunc.Invoke() : null;
            var hint = HintFunc != null ? HintFunc.Invoke() : null;

            if (string.IsNullOrEmpty(text) && HideBackgroundWhenTextIsEmpty) return;

            if (!string.IsNullOrEmpty(hint) && Hud.Window.CursorInsideRect(x, y, w, h))
            {
                var expanded = false;
                if (ExpandUpLabels != null)
                {
                    var ly = y - h;
                    foreach (var label in ExpandUpLabels)
                    {
                        label.Paint(x, ly, w, h, align);
                        ly -= h; 
                        expanded = true;
                    }
                }
                if (ExpandDownLabels != null)
                {
                    var ly = y + h;
                    foreach (var label in ExpandDownLabels)
                    {
                        label.Paint(x, ly, w, h, align);
                        ly -= h; 
                        expanded = true;
                    }
                }
                if (ExpandRightLabels != null)
                {
                    var lx = x + w;
                    foreach (var label in ExpandRightLabels)
                    {
                        label.Paint(lx, y, w, h, align);
                        lx += h; 
                        expanded = true;
                    }
                }
                if (ExpandLeftLabels != null)
                {
                    var lx = x - w;
                    foreach (var label in ExpandLeftLabels)
                    {
                        label.Paint(lx, y, w, h, align);
                        lx -= h; 
                        expanded = true;
                    }
                }

                if (!expanded)
                {
                    Hud.Render.SetHint(hint);
                }
            }

            if (BackgroundTexture1 != null)
            {
                BackgroundTexture1.Draw(x, y, w, h, BackgroundTextureOpacity1);
            }

            if (BackgroundTexture2 != null)
            {
                BackgroundTexture2.Draw(x, y, w, h, BackgroundTextureOpacity2);
            }

            if (BackgroundBrush != null)
            {
                BackgroundBrush.DrawRectangle(x, y, w, h);
            }

            if (!string.IsNullOrEmpty(text))
            {
                var layout = TextFont.GetTextLayout(text);
                switch (align)
                {
                    case HorizontalAlign.Left:
                        TextFont.DrawText(layout, x, y + (h - layout.Metrics.Height) / 2);
                        break;
                    case HorizontalAlign.Center:
                        TextFont.DrawText(layout, x + (w - layout.Metrics.Width) / 2, y + (h - layout.Metrics.Height) / 2);
                        break;
                    case HorizontalAlign.Right:
                        TextFont.DrawText(layout, x + w - layout.Metrics.Width, y + (h - layout.Metrics.Height) / 2);
                        break;
                }
            }

            if (BorderBrush != null)
            {
                BorderBrush.DrawRectangle(x, y, w, h);
            }
        }

        public IEnumerable<ITransparent> GetTransparents()
        {
            yield return TextFont;
            yield return BackgroundTexture1;
            yield return BackgroundTexture2;
        }

    }

}