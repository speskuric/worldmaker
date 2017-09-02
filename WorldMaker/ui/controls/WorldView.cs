using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorldMaker.ui.enums;
using WorldMaker._2d;

namespace WorldMaker.ui
{
    public partial class WorldView : UserControl
    {
        public MouseMode Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                switch (value)
                {
                    case MouseMode.FreeDraw: Cursor = Cursors.Default; break;
                    case MouseMode.Select: Cursor = Cursors.Default; break;
                    case MouseMode.View: Cursor = Cursors.SizeAll; break;
                    case MouseMode.Zoom: Cursor = Cursors.Cross; break;
                }
            }
        }
        private MouseMode mode = MouseMode.FreeDraw;
        private const double MOUSE_WHEEL_SENSITIVITY = 1.1;

        LineGraph currentLineGraph;

        private World world = null;

        ActionType currentAction = ActionType.None;

        //Current View
        private double viewX = -10;
        private double viewY = -10;
        private double zoomPercent = 100;

        //Mouse Utilities
        private int mouseDownX = -1;
        private int mouseDownY = -1;
        private double mouseDownWorldX = -1;
        private double mouseDownWorldY = -1;
        private int mouseX = -1;
        private int mouseY = -1;
        private double mouseWorldX = -1;
        private double mouseWorldY = -1;

        //Mouse View Action
        private double viewBeforeMoveX = -10;
        private double viewBeforeMoveY = -10;

        //Selection
        Brush selectedBrush = Brushes.Red;
        private const double selectionPixelDistance = 5;
        private List<WorldPoint> selection = new List<WorldPoint>();
        private List<LineGraph> linesWhichHaveSelectedPoints = new List<LineGraph>();

        private List<WorldAction> actionList = new List<WorldAction>();
        private int actionIndex = 0;

        public WorldView()
        {
            InitializeComponent();
            DoubleBuffered = true;
            ResizeRedraw = true;
            world = new World(640,480);
            currentLineGraph = null;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            if (world != null)
            {
                double worldWest = -viewX * zoomPercent / 100;
                double worldEast = (world.Width - viewX) * zoomPercent / 100;
                double worldNorth = -viewY * zoomPercent / 100;
                double worldSouth = (world.Height - viewY) * zoomPercent / 100;

                if (worldWest > 0) e.Graphics.FillRectangle(Brushes.LightGray, 0, 0, (int)worldWest, Height);
                if (worldEast < Width) e.Graphics.FillRectangle(Brushes.LightGray, (int)worldEast, 0, (int)(Width - worldEast), Height);
                if (worldNorth > 0) e.Graphics.FillRectangle(Brushes.LightGray, 0, 0, Width, (int)worldNorth);
                if (worldSouth < Height) e.Graphics.FillRectangle(Brushes.LightGray, 0, (int)worldSouth, Width, (int)(Height - worldSouth));

                foreach (LineGraph line in world.Lines)
                {
                    drawLineGraph(e.Graphics, line, false, Pens.Black);
                }

                if (currentAction == ActionType.FreeDraw || currentAction == ActionType.PointDraw)
                {
                    drawLineGraph(e.Graphics, currentLineGraph, true, Pens.Gray);
                    if(currentLineGraph != null && currentLineGraph.Count >= 1)
                    {
                        WorldPoint lastPoint = currentLineGraph[currentLineGraph.Count - 1];
                        int x = worldToScreenX(lastPoint.X);
                        int y = worldToScreenY(lastPoint.Y);
                        Pen pen = new Pen(Color.Gray);
                        pen.DashPattern = new float[]{2,2 };
                        e.Graphics.DrawLine(pen, x, y, mouseX, mouseY);
                    }
                }
                else if (currentAction == ActionType.Select)
                {
                    Pen pen = new Pen(Color.Gray);
                    pen.DashPattern = new float[] { 5, 2 };
                    int x1 = worldToScreenX(mouseDownWorldX);
                    int y1 = worldToScreenY(mouseDownWorldY);
                    int x2 = worldToScreenX(mouseWorldX);
                    int y2 = worldToScreenY(mouseWorldY);
                    e.Graphics.DrawRectangle(pen, Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(x1 - x2), Math.Abs(y1 - y2));
                }
                else if (currentAction == ActionType.Zoom)
                {
                    Pen pen = new Pen(Color.Gray);
                    pen.DashPattern = new float[] { 2, 8 };
                    int x1 = worldToScreenX(mouseDownWorldX);
                    int y1 = worldToScreenY(mouseDownWorldY);
                    int x2 = worldToScreenX(mouseWorldX);
                    int y2 = worldToScreenY(mouseWorldY);
                    e.Graphics.DrawRectangle(pen, Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(x1 - x2), Math.Abs(y1 - y2));
                }
            }
            
            
        }



        private void drawLineGraph(Graphics g, LineGraph lineGraph, bool highlightDots, Pen pen)
        {
            SolidBrush brush = new SolidBrush(pen.Color);

            WorldRectangle bounds = lineGraph.Bounds;
            Rectangle screenRect = worldToScreen(bounds);
            g.DrawRectangle(Pens.White, screenRect);

            WorldPoint previous = null;
            int previousScreenX = -1;
            int previousScreenY = -1;
            foreach (WorldPoint point in lineGraph)
            {
                double x = point.X;
                double y = point.Y;
                if(currentAction == ActionType.MoveSelection && selection.Contains(point))
                {
                    x += mouseWorldX - mouseDownWorldX;
                    y += mouseWorldY - mouseDownWorldY;
                }
                int screenX = worldToScreenX(x);
                int screenY = worldToScreenY(y);
                bool selected = selection.Contains(point);
                try
                {
                    if (previous != null) g.DrawLine(pen, previousScreenX, previousScreenY, screenX, screenY);
                }
                catch (OverflowException) { }
                if (selected) g.FillRectangle(selectedBrush, screenX - 1, screenY - 1, 3, 3);
                else if (highlightDots) g.FillRectangle(brush, screenX - 1, screenY - 1, 3, 3);

                previousScreenX = screenX;
                previousScreenY = screenY;
                previous = point;
            }
            if (lineGraph.IsConnected && lineGraph.Count > 2)
            {
                double x = lineGraph[0].X;
                double y = lineGraph[0].Y;
                if (currentAction == ActionType.MoveSelection && selection.Contains(lineGraph[0]))
                {
                    x += mouseWorldX - mouseDownWorldX;
                    y += mouseWorldY - mouseDownWorldY;
                }
                int screenX = worldToScreenX(x);
                int screenY = worldToScreenY(y);
                g.DrawLine(pen, previousScreenX, previousScreenY, screenX, screenY);
            }
        }

        private void add(WorldAction action)
        {
            if(actionIndex < actionList.Count)
            {
                actionList.RemoveRange(actionIndex, actionList.Count - actionIndex);
            }
            actionList.Add(action);
            actionIndex++;
        }

        public void Undo()
        {
            if(actionIndex > 0)
            {
                WorldAction action = actionList[actionIndex - 1];
                switch (action.Type)
                {
                    case ActionType.FreeDraw:
                    case ActionType.PointDraw:
                        foreach(LineGraph line in action.Lines){
                            world.Lines.Remove(line);
                        }
                        
                        break;
                    case ActionType.Delete:
                        //TODO OMG make deletes remember the line and index of the point
                            //world.Lines.AddRange(action.Points);
                        break;
                    case ActionType.MoveSelection:
                        foreach (WorldPoint point in action.Points)
                        {
                            point.X -= action.Dx;
                            point.Y -= action.Dy;
                        }
                        break;
                }
                actionIndex--;
            }
            Invalidate();
        }
        public void Redo()
        {
            if (actionIndex < actionList.Count)
            {
                WorldAction action = actionList[actionIndex];
                switch (action.Type)
                {
                    case ActionType.FreeDraw:
                    case ActionType.PointDraw:
                        world.Lines.AddRange(action.Lines);
                        break;
                    case ActionType.Delete:
                        foreach (LineGraph line in action.Lines)
                        {
                            world.Lines.Remove(line);
                        }
                        break;
                    case ActionType.MoveSelection:
                        foreach (WorldPoint point in action.Points)
                        {
                            point.X += action.Dx;
                            point.Y += action.Dy;
                        }
                        break;
                }
                actionIndex++;
            }
            Invalidate();
        }

        private double mouseToWorldX(int x)
        {
            return viewX + x / zoomPercent * 100;
        }
        private double mouseToWorldY(int y)
        {
            return viewY + y / zoomPercent * 100;
        }

        private int worldToScreenX(double x)
        {
            return (int)((x - viewX) * zoomPercent / 100);
        }
        private int worldToScreenY(double y)
        {
            return (int)((y - viewY) * zoomPercent / 100);
        }
        private Rectangle worldToScreen(WorldRectangle rectangle)
        {
            int x1 = worldToScreenX(rectangle.Left);
            int y1 = worldToScreenY(rectangle.Top);
            int x2 = worldToScreenX(rectangle.Right);
            int y2 = worldToScreenY(rectangle.Bottom);
            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            mouseX = e.X;
            mouseY = e.Y;
            mouseWorldX = mouseToWorldX(e.X);
            mouseWorldY = mouseToWorldY(e.Y);

            if (currentAction == ActionType.FreeDraw) drawActionMouseMove(e);
            else if (currentAction == ActionType.MoveView) moveActionMouseMove(e);
            else if (currentAction == ActionType.Select) selectActionMouseMove(e);
            else if (currentAction == ActionType.Zoom) zoomActionMouseMove(e);
            else if (currentAction == ActionType.MoveSelection) moveSelectionActionMouseMove(e);
            else if (currentAction == ActionType.PointDraw) pointDrawActionMouseMove(e);

            else
            {
                
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mouseDownX = e.X;
            mouseDownY = e.Y;
            mouseDownWorldX = mouseToWorldX(e.X);
            mouseDownWorldY = mouseToWorldY(e.Y);

            if (currentAction == ActionType.None)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (Mode == MouseMode.FreeDraw) drawActionMouseDown(e);
                    else if (Mode == MouseMode.View) moveActionMouseDown(e);
                    else if (Mode == MouseMode.Select) selectActionMouseDown(e);
                    else if (Mode == MouseMode.Zoom) zoomActionMouseDown(e);
                    else if (Mode == MouseMode.PointDraw) pointDrawActionMouseLeftDown(e);
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    moveActionMouseDown(e);
                }
                else
                {
                    if(Mode == MouseMode.PointDraw) pointDrawActionMouseRightDown(e);
                    currentAction = ActionType.None;
                }
            } else
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (Mode == MouseMode.PointDraw) pointDrawActionMouseLeftDown(e);
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    
                }
                else
                {
                    if (Mode == MouseMode.PointDraw) pointDrawActionMouseRightDown(e);
                    currentAction = ActionType.None;
                }
            }
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (currentAction == ActionType.FreeDraw) drawActionMouseUp(e);
            else if (currentAction == ActionType.MoveView) moveActionMouseUp(e);
            else if (currentAction == ActionType.Select) selectActionMouseUp(e);
            else if (currentAction == ActionType.Zoom) zoomActionMouseUp(e);
            else if (currentAction == ActionType.MoveSelection) moveSelectionActionMouseUp(e);
            else if (currentAction == ActionType.PointDraw) pointDrawActionMouseUp(e);
            
            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            double worldX = mouseToWorldX(e.X);
            double worldY = mouseToWorldY(e.Y);

            if (e.Delta > 0)
            {
                // Zoom In
                viewX = worldX - (worldX - viewX) / MOUSE_WHEEL_SENSITIVITY;
                viewY = worldY - (worldY - viewY) / MOUSE_WHEEL_SENSITIVITY;

                zoomPercent *= MOUSE_WHEEL_SENSITIVITY;
            }
            else
            {
                viewX = (viewX - worldX) * MOUSE_WHEEL_SENSITIVITY + worldX;
                viewY = (viewY - worldY) * MOUSE_WHEEL_SENSITIVITY + worldY;


                // Zoom Out 
                zoomPercent /= MOUSE_WHEEL_SENSITIVITY;
            }

            Invalidate();

        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Alt && !e.Control && !e.Shift)
            {
                if (e.KeyCode == Keys.Delete) deleteSelection();
            }
            if (e.KeyCode == Keys.ShiftKey && currentLineGraph != null)
            {
                currentLineGraph.IsConnected = true;
            }
            if (e.Control && e.KeyCode == Keys.Z) Undo();
            if (e.Control && e.KeyCode == Keys.Y) Redo();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.ShiftKey && currentLineGraph != null)
            {
                currentLineGraph.IsConnected = false;
                Invalidate();
            }
        }

        private void deleteSelection()
        {
            if (selection.Count > 0)
            {
                WorldAction action = new WorldAction(ActionType.Delete, new List<LineGraph>(), selection);
                add(action);
                for (int i = 0; i < world.Lines.Count; i++)
                {
                    LineGraph line = world.Lines[i];
                    for (int j = 0; j < selection.Count; j++)
                    {
                        WorldPoint point = selection[j];
                        if (line.Remove(point))
                        {
                            selection.RemoveAt(j--);
                        }
                        
                    }
                    if (line.Count <= 1) world.Lines.RemoveAt(i--);
                    Invalidate();
                }
            }
        }

        private void moveActionMouseDown(MouseEventArgs e)
        {
            currentAction = ActionType.MoveView;
            viewBeforeMoveX = viewX;
            viewBeforeMoveY = viewY;
        }
        private void moveActionMouseUp(MouseEventArgs e)
        {
            currentAction = ActionType.None;
        }
        private void moveActionMouseMove(MouseEventArgs e)
        {
            viewX = viewBeforeMoveX - ((e.X - mouseDownX) / zoomPercent * 100);
            viewY = viewBeforeMoveY - ((e.Y - mouseDownY) / zoomPercent * 100);
            Invalidate();
        }

        private void selectActionMouseDown(MouseEventArgs e)
        {
            currentAction = ActionType.Select;
            double selectionWorldDistance = selectionPixelDistance / (zoomPercent / 100);
            double pointDistance = selectionWorldDistance;
            LineGraph closestPointsLine = null;
            WorldPoint closestPoint = null;
            double lineDistance = selectionWorldDistance;
            LineGraph closestLine = null;
            foreach (LineGraph line in world.Lines)
            {
                bool isLine;
                double distance;
                int index = line.Grab(mouseDownWorldX, mouseDownWorldY, selectionWorldDistance, out distance, out isLine);
                if(index != -1)
                {
                    if (isLine)
                    {
                        if(distance < lineDistance)
                        {
                            lineDistance = distance;
                            closestLine = line;
                        }
                    }
                    else
                    {
                        if (distance < pointDistance)
                        {
                            pointDistance = distance;
                            closestPoint = line[index];
                            closestPointsLine = line;
                        }
                    }
                }
            }
            if(closestPoint != null)
            {
                if (!selection.Contains(closestPoint))
                {
                    selection.Clear();
                    linesWhichHaveSelectedPoints.Clear();
                    selection.Add(closestPoint);
                    linesWhichHaveSelectedPoints.Add(closestPointsLine);
                }
                currentAction = ActionType.MoveSelection;
            }
            else if(closestLine != null)
            {
                selection.Clear();
                linesWhichHaveSelectedPoints.Clear();
                foreach (WorldPoint point in closestLine) selection.Add(point);
                linesWhichHaveSelectedPoints.Add(closestLine);
                currentAction = ActionType.MoveSelection;
            }
        }
        private void selectActionMouseUp(MouseEventArgs e)
        {
            selection.Clear();
            linesWhichHaveSelectedPoints.Clear();
            double x1 = Math.Min(mouseDownWorldX, mouseWorldX);
            double x2 = Math.Max(mouseDownWorldX, mouseWorldX);
            double y1 = Math.Min(mouseDownWorldY, mouseWorldY);
            double y2 = Math.Max(mouseDownWorldY, mouseWorldY);

            foreach (LineGraph graph in world.Lines)
            {
                foreach (WorldPoint point in graph)
                {
                    if (point.X >= x1 && point.X <= x2 && point.Y >= y1 && point.Y <= y2)
                    {
                        selection.Add(point);
                        if (!linesWhichHaveSelectedPoints.Contains(graph))
                            linesWhichHaveSelectedPoints.Add(graph);
                    }
                }
            }

            currentAction = ActionType.None;
        }
        private void selectActionMouseMove(MouseEventArgs e)
        {
            Invalidate();
        }

        private void moveSelectionActionMouseUp(MouseEventArgs e)
        {
            double dx = mouseWorldX - mouseDownWorldX;
            double dy = mouseWorldY - mouseDownWorldY;
            foreach (WorldPoint point in selection)
            {
                point.X += dx;
                point.Y += dy;
            }
            foreach (LineGraph line in linesWhichHaveSelectedPoints) line.RecalculateBounds();
            WorldAction action = new WorldAction(ActionType.MoveSelection, new List<LineGraph>(), selection, dx, dy);
            add(action);
            currentAction = ActionType.None;
        }
        private void moveSelectionActionMouseMove(MouseEventArgs e)
        {
            Invalidate();
        }

        private void zoomActionMouseDown(MouseEventArgs e)
        {
            currentAction = ActionType.Zoom;

        }
        private void zoomActionMouseUp(MouseEventArgs e)
        {
            double x1 = Math.Min(mouseDownWorldX, mouseWorldX);
            double x2 = Math.Max(mouseDownWorldX, mouseWorldX);
            double y1 = Math.Min(mouseDownWorldY, mouseWorldY);
            double y2 = Math.Max(mouseDownWorldY, mouseWorldY);
            
            double xZoom = 100 * Width / (x2 - x1);
            double yZoom = 100 * Height / (y2 - y1);
            viewX = x1;
            viewY = y1;
            zoomPercent = Math.Min(xZoom, yZoom);
            currentAction = ActionType.None;
        }
        private void zoomActionMouseMove(MouseEventArgs e)
        {
            Invalidate();
        }

        private void drawActionMouseDown(MouseEventArgs e)
        {
            currentAction = ActionType.FreeDraw;
            double worldX = mouseToWorldX(e.X);
            double worldY = mouseToWorldY(e.Y);

            currentLineGraph = new LineGraph(ModifierKeys == Keys.Shift);
            currentLineGraph.Add(worldX, worldY);
        }
        private void drawActionMouseUp(MouseEventArgs e)
        {
            if (currentLineGraph != null && currentLineGraph.Count > 1)
            {
                world.Lines.Add(currentLineGraph);
                WorldAction action = new WorldAction(ActionType.FreeDraw, currentLineGraph);
                add(action);
            }
            currentLineGraph = null;
            currentAction = ActionType.None;
        }
        private void drawActionMouseMove(MouseEventArgs e)
        {
            double worldX = mouseToWorldX(e.X);
            double worldY = mouseToWorldY(e.Y);

            currentLineGraph.Add(worldX, worldY);
            Invalidate();
        }

        private void pointDrawActionMouseLeftDown(MouseEventArgs e)
        {
            if (currentAction != ActionType.PointDraw)
            {
                currentAction = ActionType.PointDraw;
                currentLineGraph = new LineGraph(ModifierKeys == Keys.Shift);
            }
                
            double worldX = mouseToWorldX(e.X);
            double worldY = mouseToWorldY(e.Y);

            currentLineGraph.Add(worldX, worldY);
        }
        private void pointDrawActionMouseRightDown(MouseEventArgs e)
        {
            if (currentLineGraph != null && currentLineGraph.Count > 1) {
                world.Lines.Add(currentLineGraph);
                WorldAction action = new WorldAction(ActionType.PointDraw, currentLineGraph);
                add(action);
            }
            currentLineGraph = null;
        }
        private void pointDrawActionMouseUp(MouseEventArgs e)
        {
        }
        private void pointDrawActionMouseMove(MouseEventArgs e)
        {
            Invalidate();
        }
    }
}
