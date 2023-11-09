using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
namespace AutoCadCommands
{
    public class MyCommands
    {
        [assembly: CommandClass(typeof(MyCommands))]

        [CommandMethod("VectorLine")]
        public void VectorLine()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            double arrowPointX = 0;
            double arrowPointY = 0;
            double length = 0;
            double arrowWidth = 7;
            double vectorWidth = 1;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    // prompt to select first point
                    PromptPointOptions ppo = new PromptPointOptions("\nSelect first point: ");
                    PromptPointResult ppr = ed.GetPoint(ppo);
                    if (ppr.Status != PromptStatus.OK)
                    {
                        ed.WriteMessage("\nError in getting first point");
                        return;
                    }
                    Point3d startPoint = ppr.Value;

                    // prompt to select second point
                    PromptPointOptions ppo2 = new PromptPointOptions("\nSelect first point: ");
                    PromptPointResult ppr2 = ed.GetPoint(ppo2);
                    if (ppr2.Status != PromptStatus.OK)
                    {
                        ed.WriteMessage("\nError in getting first point");
                        return;
                    }
                    Point3d endPoint = ppr2.Value;
                    
                    // Vector and arrow parameters
                    length = Math.Sqrt(Math.Pow((endPoint.X - startPoint.X), 2) + Math.Pow((endPoint.Y - startPoint.Y), 2));
                    vectorWidth = length * 0.005;
                    arrowWidth = vectorWidth*2;
                    

                    //Create a point from which the arrowhead is drawn
                    arrowPointX = endPoint.X - 0.05*(endPoint.X - startPoint.X);
                    arrowPointY = endPoint.Y - 0.05*(endPoint.Y - startPoint.Y);

                    //Create a new polyline
                    Polyline pl = new Polyline();
                    pl.AddVertexAt(0, new Point2d(startPoint.X, startPoint.Y), 0, vectorWidth, vectorWidth);
                    pl.AddVertexAt(1, new Point2d(arrowPointX, arrowPointY), 0, arrowWidth, 0);          
                    // Draw the arrowhead
                    pl.AddVertexAt(2, new Point2d(endPoint.X, endPoint.Y), 0, 0, 0);

                    //Add the polyline to the current model space
                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    btr.AppendEntity(pl);
                    tr.AddNewlyCreatedDBObject(pl, true);

                    tr.Commit();
                }
                catch (System.Exception ex)
                {
                    ed.WriteMessage("Error: " + ex.Message);
                }
            }
        }
    }
}
