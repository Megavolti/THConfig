using System.Collections.Generic;
using System.Globalization;

namespace Turbo.Plugins.Default
{

    // this is not a plugin, just a helper class to display labels on the ground
    public class GroundLabelDecorator: IWorldDecorator
    {

        public bool Enabled { get; set; }
        public WorldLayer Layer { get; private set; }
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

        public int CountDownFrom { get; set; }

        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public bool CenterBaseLine { get; set; }
        public bool ForceOnScreen { get; set; }

        public GroundLabelDecorator(IController hud)
        {
            Enabled = true;
            Layer = WorldLayer.Ground;
            Hud = hud;

            CenterBaseLine = true;
            ForceOnScreen = true;
        }

        public void Paint(IActor actor, IWorldCoordinate coord, string text)
        {
            if (!Enabled) return;

            var painter = Hud.GetPlugin<GroundLabelDecoratorPainterPlugin>();
            if (painter == null) return;

            if ((CountDownFrom > 0) && (actor != null))
            {
                var remaining = CountDownFrom - ((Hud.Game.CurrentGameTick - actor.CreatedAtInGameTick) / 60.0f);
                if (remaining < 0) remaining = 0;

                var vf = (remaining > 1.0f) ? "F0" : "F1";
                text = remaining.ToString(vf, CultureInfo.InvariantCulture);
            }

            painter.EnqueLabelForPaint(this, coord, text);
        }

        public IEnumerable<ITransparent> GetTransparents()
        {
            yield return BackgroundBrush;
            yield return BorderBrush;
            yield return TextFont;
        }

    }

}