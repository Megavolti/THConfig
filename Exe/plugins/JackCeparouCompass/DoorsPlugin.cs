using System;
using System.Collections.Generic;
using System.Linq;
using Turbo.Plugins.Default;

namespace Turbo.Plugins.JackCeparouCompass
{
    public class DoorsPlugin : BasePlugin
    {
        public WorldDecoratorCollection DoorsDecorators { get; set; }
        public WorldDecoratorCollection BreakablesDoorsDecorators { get; set; }
        public WorldDecoratorCollection BridgesDecorators { get; set; }

        public bool OutOfTownOnly { get; set; }
        public bool GroundLabelsOutOfScreenOnly { get; set; }

        private readonly uint[] bridgesIds = new uint[] { 309432, 54850, 404043, 198125 };
        private readonly HashSet<uint> breakableDoorsIds = new HashSet<uint> { 55325, 427495, 5792, 95481, 379048, 95481, 230324, };// 258064 };
        private readonly HashSet<uint> doorsIdsBlackList = new HashSet<uint>() {
            197939, 169502, 214333, 181195, 190236, // A2 to belial
            167185, // A2 Alcarnus
            200371, 5503, // A2 City
            198977, 52685, // A3 rakkis crossing
            112316, // A3 stonefort
            356879,
        };
        //170245
        private readonly HashSet<uint> doorsDebugWhiteList = new HashSet<uint>() {
            309222, 308241, 454, // ??
            309812, // X1 ??
            104888,
            258595,
            4267,
            362651,
            447673,
            162386,
            415665,
            4393,
            219702,
            250031,
            153752,
            //102711
            343582,
            230324,
        };

        public DoorsPlugin()
        {
            Enabled = true;
            GroundLabelsOutOfScreenOnly = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);

            DoorsDecorators = CreateDecorators(255, 216, 0);
            BreakablesDoorsDecorators = CreateDecorators(250, 0, 0);
            BridgesDecorators = CreateDecorators(0, 195, 255);
        }

        public override void PaintWorld(WorldLayer layer)
        {
            if (Hud.Game.IsInTown && OutOfTownOnly) return;

            Hud.Game.Actors
                .Where(a => a.GizmoType == GizmoType.Door || a.GizmoType == GizmoType.BreakableDoor)
                .ForEach(door =>
                {
                    if (breakableDoorsIds.Contains(door.SnoActor.Sno))
                    {
                        PaintActor(layer, door, BreakablesDoorsDecorators);
                    }
                    else if (door.GizmoType == GizmoType.Door && !door.IsOperated && !door.IsDisabled && !doorsIdsBlackList.Contains(door.SnoActor.Sno))
                    {
                        PaintActor(layer, door, bridgesIds.Contains(door.SnoActor.Sno) ? BridgesDecorators : DoorsDecorators);
                        //if (!doorsDebugWhiteList.Contains(door.SnoActor.Sno))
                            //Simon.Says.Debug(string.Format("DOOR?? {0} {1} {2} {3} {4} {5} {6}", door.SnoActor.Sno, door.SnoActor.NameLocalized, door.IsOperated, door.IsClickable, door.IsDisabled, door.SnoActor.Kind, door.SnoActor.Code));/**/
                    }

                    //if (!doorsDebugWhiteList.Contains(door.SnoActor.Sno))
                        //Simon.Says.Debug(string.Format("DOOR?? {0} {1} {2} {3} {4} {5} {6}", door.SnoActor.Sno, door.SnoActor.NameLocalized, door.IsOperated, door.IsClickable, door.IsDisabled, door.SnoActor.Kind, door.SnoActor.Code));/**/
                });
        }

        private WorldDecoratorCollection CreateDecorators(int r, int g, int b)
        {
            return new WorldDecoratorCollection(
                new GroundLabelDecorator(Hud)
                {
                    TextFont = Hud.Render.CreateFont("tahoma", 18, 200, r, g, b, false, false, true),
                    //BackgroundBrush = Hud.Render.CreateBrush(142, 0, 0, 0, 0),
                    //BorderBrush = Hud.Render.CreateBrush(178, r, g, b, 1),
                },
                new MapShapeDecorator(Hud)
                {
                    ShapePainter = new DoorShapePainter(Hud),
                    Radius = 6f,
                    Brush = Hud.Render.CreateBrush(200, r, g, b, 1),
                }
            );
        }

        private void PaintActor(WorldLayer layer, IActor actor, WorldDecoratorCollection decorator)
        {
            if (GroundLabelsOutOfScreenOnly)
                decorator.ToggleDecorators<GroundLabelDecorator>(!actor.IsOnScreen);

            //decorator.Paint(layer, actor, actor.FloorCoordinate, actor.SnoActor.NameEnglish);// "\uD83D\uDEAA");
            decorator.Paint(layer, actor, actor.FloorCoordinate, "\uD83D\uDEAA");
        }
    }
}