using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Catalogs;
using TSS = Tekla.Structures.Solid;
using SK.Tekla.Drawing.Automation.Support;

namespace SK.Tekla.Drawing.Automation.Handlers
{
    public class SKAngleHandler
    {
        private readonly BoltMatrixHandler boltMatrixHandler;

        public SKAngleHandler(BoltMatrixHandler boltMatrixHandler) { 
            this.boltMatrixHandler = boltMatrixHandler;
        }


        public void angle_place_check_for_hole_locking(TSD.PointList my_pt_of_angle, out TSD.PointList rd_point_list, TSM.ModelObjectEnumerator enum_for_bolt, TSD.View current_view, ref List<Guid> PARTMARK_TO_RETAIN, ref List<Guid> POSBOLTMARK_TO_RETAIN, ref List<Guid> NEGBOLTMARK_TO_RETAIN)
        {
            rd_point_list = new TSD.PointList();

            if ((my_pt_of_angle[0].Y < 0) && (my_pt_of_angle[1].Y < 0))
            {

                while (enum_for_bolt.MoveNext())
                {
                    //TSD.Bolt drgbolt = enum_for_bolt.Current as TSD.Bolt;
                    TSM.BoltGroup drgbolt = enum_for_bolt.Current as TSM.BoltGroup;
                    //TSG.Point[,] POINT_FOR_BOLT_MATRIX = boltMatrixHandler.Get_Bolt_properties_matrix_input_as_modelobject(drgbolt, current_view);
                    TSG.Point[,] POINT_FOR_BOLT_MATRIX = boltMatrixHandler.GetBoltMatrix(drgbolt, current_view);


                    ///////////////////////////////////////////////////condition for depth adjustment////////////////////////////////////////////////////////////////////////////////////////////
                    //////////////////////////////////////////////////// filtering bolts which lies within the section depth/////////////////////////////////////////////////////////////////////                       
                    double upper_limit = current_view.RestrictionBox.MaxPoint.Z;
                    double lower_limit = current_view.RestrictionBox.MinPoint.Z;
                    if (POINT_FOR_BOLT_MATRIX != null)
                    {
                        if ((POINT_FOR_BOLT_MATRIX[0, 0].Z > lower_limit) && (POINT_FOR_BOLT_MATRIX[0, 0].Z < upper_limit))
                        {
                            int y = POINT_FOR_BOLT_MATRIX.GetLength(0);
                            int x = POINT_FOR_BOLT_MATRIX.GetLength(1);
                            for (int i = 0; i < x; i++)
                            {
                                //////////////////////////////////////////// condition for getting last row of bolts//////////////////////////////////////////////////////////////////////////////////////////////
                                rd_point_list.Add(POINT_FOR_BOLT_MATRIX[0, i]);
                            }
                            if ((POINT_FOR_BOLT_MATRIX[0, 0].X) > 0)
                            {
                                POSBOLTMARK_TO_RETAIN.Add(drgbolt.Identifier.GUID);
                            }
                            else if ((POINT_FOR_BOLT_MATRIX[0, 0].X) < 0)
                            {
                                NEGBOLTMARK_TO_RETAIN.Add(drgbolt.Identifier.GUID);
                            }

                            break;
                        }
                    }
                }
            }
            else
            {

                while (enum_for_bolt.MoveNext())
                {
                    //TSD.Bolt drgbolt = enum_for_bolt.Current as TSD.Bolt;
                    TSM.BoltGroup drgbolt = enum_for_bolt.Current as TSM.BoltGroup;
                    //TSG.Point[,] POINT_FOR_BOLT_MATRIX = boltMatrixHandler.Get_Bolt_properties_matrix_input_as_modelobject(drgbolt, current_view);
                    TSG.Point[,] POINT_FOR_BOLT_MATRIX = boltMatrixHandler.GetBoltMatrix(drgbolt, current_view);


                    ///////////////////////////////////////////////////condition for depth adjustment////////////////////////////////////////////////////////////////////////////////////////////
                    //////////////////////////////////////////////////// filtering bolts which lies within the section depth/////////////////////////////////////////////////////////////////////                       
                    double upper_limit = current_view.RestrictionBox.MaxPoint.Z;
                    double lower_limit = current_view.RestrictionBox.MinPoint.Z;
                    if (POINT_FOR_BOLT_MATRIX != null)
                    {
                        if ((POINT_FOR_BOLT_MATRIX[0, 0].Z > lower_limit) && (POINT_FOR_BOLT_MATRIX[0, 0].Z < upper_limit))
                        {
                            int y = POINT_FOR_BOLT_MATRIX.GetLength(0);
                            int x = POINT_FOR_BOLT_MATRIX.GetLength(1);
                            for (int i = 0; i < x; i++)
                            {
                                //////////////////////////////////////////// condition for getting last row of bolts//////////////////////////////////////////////////////////////////////////////////////////////
                                rd_point_list.Add(POINT_FOR_BOLT_MATRIX[y - 1, i]);
                            }
                            if ((POINT_FOR_BOLT_MATRIX[0, 0].X) > 0)
                            {
                                POSBOLTMARK_TO_RETAIN.Add(drgbolt.Identifier.GUID);
                            }
                            else if ((POINT_FOR_BOLT_MATRIX[0, 0].X) < 0)
                            {
                                NEGBOLTMARK_TO_RETAIN.Add(drgbolt.Identifier.GUID);
                            }
                        }
                    }
                }
            }


        }



        public TSD.PointList angle_pts_for_section(List<AngleFaceArea> myreq, TSD.View current_view)
        {
            TSM.Model mymodel = new TSM.Model();
            TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
            myreq.RemoveAll(x => x.VectorType.Equals("X"));
            var myreq1 = (from vector in myreq
                          group vector by vector.VectorType into newlist
                          select new
                          {
                              vector_type = newlist.Key,
                              Face = newlist.ToList()
                          }).ToList();
            List<TSS.Face> myface_list = new List<TSS.Face>();

            for (int h = 0; h < myreq1.Count; h++)
            {
                myface_list.Add((myreq1[h].Face.Find(x => x.Area.Equals(myreq1[h].Face.Max(y => y.Area)))).Face);
            }
            List<TSG.Point> list1 = new List<TSG.Point>();
            List<TSG.Point> list2 = new List<TSG.Point>();

            for (int x = 0; x < myface_list.Count; x++)
            {
                if (x == 0)
                {

                    TSS.LoopEnumerator myloop_enum = myface_list[x].GetLoopEnumerator();
                    while (myloop_enum.MoveNext())
                    {
                        TSS.Loop myloop = myloop_enum.Current as TSS.Loop;
                        TSS.VertexEnumerator myvertex_enum = myloop.GetVertexEnumerator();
                        while (myvertex_enum.MoveNext())
                        {
                            TSG.Point myvertex = myvertex_enum.Current as TSG.Point;
                            list1.Add(myvertex);

                        }
                    }
                }
                else
                {
                    TSS.LoopEnumerator myloop_enum = myface_list[x].GetLoopEnumerator();
                    while (myloop_enum.MoveNext())
                    {
                        TSS.Loop myloop = myloop_enum.Current as TSS.Loop;
                        TSS.VertexEnumerator myvertex_enum = myloop.GetVertexEnumerator();
                        while (myvertex_enum.MoveNext())
                        {
                            TSG.Point myvertex = myvertex_enum.Current as TSG.Point;
                            list2.Add(myvertex);

                        }
                    }

                }

            }

            List<TSG.Point> myedge = list1.Intersect(list2).ToList();
            TSG.Matrix global_matrix = mymodel.GetWorkPlaneHandler().GetCurrentTransformationPlane().TransformationMatrixToGlobal;
            mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
            TSG.Point globalpt = global_matrix.Transform(myedge[0]);
            TSG.Point viewpt = toviewmatrix.Transform(globalpt);
            TSG.Point globalpt1 = global_matrix.Transform(myedge[1]);
            TSG.Point viewpt1 = toviewmatrix.Transform(globalpt1);
            TSD.PointList point = new TSD.PointList();
            return point;

        }


    }
}
