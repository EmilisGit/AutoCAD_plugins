using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcServices = Autodesk.AutoCAD.ApplicationServices;
using AcDb = Autodesk.AutoCAD.DatabaseServices;
using AcEd = Autodesk.AutoCAD.EditorInput;
using AcGeom = Autodesk.AutoCAD.Geometry;
using AcRun = Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;

[assembly: CommandClass(typeof(Vector_Graph_Autocad.VectorCommands))]

namespace Vector_Graph_Autocad
{
    
    public class VectorCommands
    {
        private double scale = 100;
        public AcGeom.Point3d GetOrigin()
        {
            AcServices.Document doc = AcServices.Application.DocumentManager.MdiActiveDocument;
            AcDb.Database db = doc.Database;
            AcEd.Editor ed = doc.Editor;
            AcGeom.Point3d origin = new Point3d(0,0,0);
            try
                {
                    // prompt to select first point
                    AcEd.PromptPointOptions ppo = new AcEd.PromptPointOptions("\nSelect base point: ");
                    AcEd.PromptPointResult ppr = ed.GetPoint(ppo);
                    if (ppr.Status != AcEd.PromptStatus.OK)
                    {
                        ed.WriteMessage("\nError in getting first point");
                    }
                origin = ppr.Value;
                }
                catch (System.Exception ex)
                {
                    ed.WriteMessage("Error: " + ex.Message);
                }
            return origin;
        }
        public void DrawVector(AcGeom.Point3d origin, double x, double y)
        {
            AcServices.Document doc = AcServices.Application.DocumentManager.MdiActiveDocument;
            AcDb.Database db = doc.Database;
            AcEd.Editor ed = doc.Editor;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    double vectorWidth = scale * 0.005;
                    double arrowWidth = vectorWidth * 2.5;
                    double arrowPointX = 0;
                    double arrowPointY = 0;

                    //Create a point from which the arrowhead is drawn
                    arrowPointX = x - 0.05 * (x - origin.X);
                    arrowPointY = y - 0.05 * (y - origin.Y);

                    //Create a new polyline
                    AcDb.Polyline pl = new AcDb.Polyline();
                    pl.AddVertexAt(0, new Point2d(origin.X, origin.Y), 0, vectorWidth, vectorWidth);
                    pl.AddVertexAt(1, new Point2d(arrowPointX, arrowPointY), 0, arrowWidth, 0);
                    // Draw the arrowhead
                    pl.AddVertexAt(2, new Point2d(x, y), 0, 0, 0);

                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    btr.AppendEntity(pl);
                    tr.AddNewlyCreatedDBObject(pl, true);

                    tr.Commit();
                }
                catch(System.Exception ex)
                {
                    ed.WriteMessage("Error: " + ex.Message);
                    return;
                }
            }
        }
        [CommandMethod("DrawPositiveGraph")]
        public void DrawPositiveGraph()
        {
            AcServices.Document doc = AcServices.Application.DocumentManager.MdiActiveDocument;
            AcDb.Database db = doc.Database;
            AcEd.Editor ed = doc.Editor;
            try
            {
                AcGeom.Point3d origin = GetOrigin();

                // Calculate the endpoint coordinates for the X and Y axes
                double xAxisEnd = origin.X + scale;
                double yAxisEnd = origin.Y + scale;

                // Calculate the coordinates for the second vector, shifted by 10%
                double xOffset = origin.X - scale * 0.1;
                double yOffset = origin.Y - scale * 0.1;

                // Draw the first vector
                DrawVector(origin, xAxisEnd, origin.Y);
                DrawVector(origin, origin.X, yAxisEnd);

                // Draw the second vector with a 10% overlap
                DrawVector(origin, xOffset, origin.Y);
                DrawVector(origin, origin.X, yOffset);

            }
            catch (System.Exception ex)
            {
                ed.WriteMessage("Error: " + ex.Message);
                return;
            }
        }      
    }
}


/*

                    // Vector and arrow parameters
                    length = Math.Sqrt(Math.Pow((endPoint.X - startPoint.X), 2) + Math.Pow((endPoint.Y - startPoint.Y), 2));
                    vectorWidth = length * 0.005;
                    arrowWidth = vectorWidth * 2;


                    //Create a point from which the arrowhead is drawn
                    arrowPointX = endPoint.X - 0.05 * (endPoint.X - startPoint.X);
                    arrowPointY = endPoint.Y - 0.05 * (endPoint.Y - startPoint.Y);

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
                    */