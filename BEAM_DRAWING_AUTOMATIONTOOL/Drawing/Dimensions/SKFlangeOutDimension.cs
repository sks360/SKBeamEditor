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
using SK.Tekla.Drawing.Automation.Utils;
using System.Collections;
using SK.Tekla.Drawing.Automation.Handlers;
using SK.Tekla.Drawing.Automation.Models;

namespace SK.Tekla.Drawing.Automation.Drawing.Dimensions
{
    public class SKFlangeOutDimension
    {
        private string client;

        private FontSizeSelector fontSize;

        private readonly SKCatalogHandler catalogHandler;

        public SKFlangeOutDimension(SKCatalogHandler catalogHandler, string client,
            FontSizeSelector fontSize)
        {
            this.catalogHandler = catalogHandler;
            this.client = client;
            this.fontSize = fontSize;
        }

        public void Create_FLANGE_CUT_dimensions_top(TSD.View current_view, TSM.Beam main_part, string drg_att)
        {

            TSD.StraightDimensionSet.StraightDimensionSetAttributes dim_font_height = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
            dim_font_height.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
            dim_font_height.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
            TSD.StraightDimensionSetHandler dimset = new TSD.StraightDimensionSetHandler();



            if (current_view != null)
            {

                List<double> values = catalogHandler.Getcatalog_values(main_part);
                double size1_m = Convert.ToDouble(values[0]);
                double size2_m = Convert.ToDouble(values[1]);
                TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.DisplayCoordinateSystem);
                Type type_for_contourplate = typeof(TSM.ContourPlate);
                double size = 0;
                double output = 0;
                main_part.GetReportProperty("LENGTH", ref output);
                double DEPTH = Convert.ToDouble(current_view.Width.Equals(current_view.RestrictionBox.MaxPoint.X));
                if ((current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView)))
                {
                    size = size1_m / 2;
                }
                else
                {
                    size = size2_m / 2;
                }

                //////copedimesnison/////
                Type bolpart = typeof(TSM.BooleanPart);
                Type fit = typeof(TSM.Fitting);
                TSM.ModelObjectEnumerator test_bool = main_part.GetBooleans();
                ArrayList cuts = new ArrayList();
                TSG.Point fittrans_origin = new TSG.Point();
                double workpoint = output;
                TSG.Point point1 = new TSG.Point();
                TSG.Point point2 = new TSG.Point();
                TSG.Point point3 = new TSG.Point();
                TSG.Point point4 = new TSG.Point();
                while (test_bool.MoveNext())
                {
                    int d = 0;
                    ArrayList pts_in_viewco = new ArrayList();
                    var partcut = test_bool.Current;
                    if (partcut.GetType().Equals(bolpart))
                    {


                        TSM.BooleanPart fitobj = partcut as TSM.BooleanPart;

                        if (fitobj.OperativePart.GetType().Equals(type_for_contourplate))
                        {
                            TSM.ContourPlate platecut = fitobj.OperativePart as TSM.ContourPlate;
                            ArrayList pts = platecut.Contour.ContourPoints;
                            ArrayList x_list = new ArrayList();
                            ArrayList y_list = new ArrayList();
                            ArrayList z_list = new ArrayList();
                            foreach (TSG.Point bolpart_point in pts)
                            {
                                TSG.Point tran_point = toviewmatrix.Transform(bolpart_point);
                                pts_in_viewco.Add(tran_point);

                            }
                            if (Convert.ToInt16((pts_in_viewco[0] as TSG.Point).Y) != Convert.ToInt16((pts_in_viewco[2] as TSG.Point).Y))
                            {
                                foreach (TSG.Point pt in pts_in_viewco)
                                {


                                    x_list.Add(pt.X);
                                    y_list.Add(pt.Y);
                                    z_list.Add(pt.Z);

                                }

                                x_list.Sort();
                                y_list.Sort();
                                z_list.Sort();
                                ////////////////////Lower boolean dim///////////////////
                                TSG.Point pt1 = new TSG.Point();
                                TSG.Point pt2 = new TSG.Point();
                                TSG.Point pt3 = new TSG.Point();
                                TSG.Point pt4 = new TSG.Point();
                                TSG.Vector dim_vect_for_x_dim = new TSG.Vector();
                                TSG.Vector dim_vect_for_y_dim = new TSG.Vector();
                                #region top_VIEW_DIM
                                if (Convert.ToInt64(z_list[0]) > 0)
                                {



                                    if ((Convert.ToInt16(y_list[0]) <= 0) && ((Convert.ToInt16(x_list[0]) <= 0)))
                                    {

                                        pt1 = new TSG.Point(0, 0, 0);
                                        pt2 = new TSG.Point(0, Convert.ToInt16(y_list[y_list.Count - 1]), 0);
                                        pt3 = new TSG.Point(Convert.ToInt16(x_list[x_list.Count - 1]), -size, 0);
                                        pt4 = new TSG.Point(0, Convert.ToInt16(y_list[y_list.Count - 1]), 0);
                                        dim_vect_for_x_dim = new TSG.Vector(0, -1, 0);
                                        dim_vect_for_y_dim = new TSG.Vector(-1, 0, 0);

                                    }
                                    //////////Second quadrant//////////////////////////
                                    else if ((Convert.ToInt16(x_list[0]) <= 0) && ((Convert.ToInt16(y_list[0]) >= 0)))
                                    {
                                        pt1 = new TSG.Point(0, 0, 0);
                                        pt2 = new TSG.Point(0, Convert.ToInt16(y_list[0]), 0);
                                        pt3 = new TSG.Point(Convert.ToInt16(x_list[x_list.Count - 1]), size, 0);
                                        point2 = pt1;
                                        pt4 = new TSG.Point(0, Convert.ToInt16(y_list[0]), 0);
                                        dim_vect_for_x_dim = new TSG.Vector(0, 1, 0);
                                        dim_vect_for_y_dim = new TSG.Vector(-1, 0, 0);

                                    }
                                    /////////Fourth Quadrant////////////////////////////
                                    else if ((Convert.ToInt16(x_list[0]) >= 0) && ((Convert.ToInt16(y_list[0]) <= 0)))
                                    {
                                        pt1 = new TSG.Point(workpoint, 0, 0);
                                        pt3 = new TSG.Point(Convert.ToInt16(x_list[0]), Convert.ToInt16(y_list[0]), 0);
                                        pt2 = new TSG.Point(workpoint, Convert.ToInt16(y_list[y_list.Count - 1]), 0);
                                        pt4 = new TSG.Point(workpoint, Convert.ToInt16(y_list[y_list.Count - 1]), 0);
                                        dim_vect_for_x_dim = new TSG.Vector(0, -1, 0);
                                        dim_vect_for_y_dim = new TSG.Vector(1, 0, 0);


                                    }
                                    //////////////First Quadrant////////////////
                                    else if ((Convert.ToInt16(x_list[0]) >= 0) && ((Convert.ToInt16(y_list[0]) >= 0)))
                                    {
                                        pt1 = new TSG.Point(workpoint, 0, 0);
                                        pt3 = new TSG.Point(Convert.ToInt16(x_list[0]), size, 0);
                                        pt2 = new TSG.Point(workpoint, Convert.ToInt16(y_list[0]), 0);
                                        pt4 = new TSG.Point(workpoint, Convert.ToInt16(y_list[0]), 0);
                                        point3 = pt1;
                                        dim_vect_for_x_dim = new TSG.Vector(0, 1, 0);
                                        dim_vect_for_y_dim = new TSG.Vector(1, 0, 0);


                                    }

                                    if (Convert.ToInt64(z_list[0]) > 0)
                                    {
                                        if ((pt2.X < current_view.RestrictionBox.MaxPoint.X) && (pt1.X > current_view.RestrictionBox.MinPoint.X))
                                        {
                                            TSD.PointList mypt = new TSD.PointList();
                                            mypt.Add(pt2);
                                            mypt.Add(pt3);
                                            try
                                            {
                                                dimset.CreateDimensionSet(current_view, mypt, dim_vect_for_x_dim, 200 + d, dim_font_height);
                                            }
                                            catch
                                            {
                                            }
                                            TSD.PointList mypt1 = new TSD.PointList();
                                            mypt1.Add(pt1);
                                            mypt1.Add(pt4);
                                            try
                                            {
                                                dimset.CreateDimensionSet(current_view, mypt, dim_vect_for_y_dim, 200, dim_font_height);
                                            }
                                            catch
                                            {
                                            }



                                            //TSD.StraightDimension bool_dim_x = new TSD.StraightDimension(current_view as TSD.ViewBase, pt2, pt3, dim_vect_for_x_dim, 200 + d,dim_font_height);
                                            //bool_dim_x.Insert();




                                            //TSD.StraightDimension bool_dim_y = new TSD.StraightDimension(current_view as TSD.ViewBase, pt1, pt4, dim_vect_for_y_dim, 200,dim_font_height);
                                            //bool_dim_y.Insert();
                                        }
                                    }
                                    if (Convert.ToInt64(z_list[0]) < 0)
                                    {
                                        //if (pt2.X > output / 2)
                                        //{
                                        //    TSD.StraightDimension bool_dim_x = new TSD.StraightDimension(current_view as TSD.ViewBase, pt2, pt3, dim_vect_for_x_dim, 200 + d,dim_font_height);
                                        //    bool_dim_x.Insert();

                                        //    TSD.StraightDimension bool_dim_y = new TSD.StraightDimension(current_view as TSD.ViewBase, pt1, pt4, dim_vect_for_y_dim, 200,dim_font_height);
                                        //    bool_dim_y.Insert();
                                        //}


                                        TSD.PointList mypt = new TSD.PointList();
                                        mypt.Add(pt2);
                                        mypt.Add(pt3);
                                        try
                                        {
                                            dimset.CreateDimensionSet(current_view, mypt, dim_vect_for_x_dim, 200 + d, dim_font_height);
                                        }
                                        catch
                                        {
                                        }

                                        TSD.PointList mypt1 = new TSD.PointList();
                                        mypt1.Add(pt1);
                                        mypt1.Add(pt4);
                                        try
                                        {
                                            dimset.CreateDimensionSet(current_view, mypt, dim_vect_for_y_dim, 200, dim_font_height);
                                        }
                                        catch
                                        {
                                        }
                                        //TSD.StraightDimension bool_dim_x = new TSD.StraightDimension(current_view as TSD.ViewBase, pt2, pt3, dim_vect_for_x_dim, 200 + d,dim_font_height);
                                        //bool_dim_x.Insert();




                                        //TSD.StraightDimension bool_dim_y = new TSD.StraightDimension(current_view as TSD.ViewBase, pt1, pt4, dim_vect_for_y_dim, 200,dim_font_height);
                                        //bool_dim_y.Insert();

                                    }




                                    ////////////////
                                }
                                # endregion
                            }
                        }

                        cuts.Add(partcut);
                        TSG.Point fittrans_origin1 = toviewmatrix.Transform(main_part.EndPoint);
                    }
                    else
                    {
                        point2 = new TSG.Point(0, size1_m / 2, 0);
                        point3 = new TSG.Point(output, size1_m / 2, 0);
                    }
                    d = d + 100;
                }

            }


        }
        
        
        public void Create_FLANGE_CUT_dimensions_bottom(TSD.View current_view, TSM.Beam main_part, string drg_att)
        {

            TSD.StraightDimensionSetHandler dimset = new TSD.StraightDimensionSetHandler();


            List<double> values = catalogHandler.Getcatalog_values(main_part);
            double size1_m = Convert.ToDouble(values[0]);
            double size2_m = Convert.ToDouble(values[1]);
            double size3_m = Convert.ToDouble(values[2]);
            long half_flange = Convert.ToInt64(size2_m / 2);
            long half_s_value = Convert.ToInt64(size3_m / 2);
            long cut_chip_value = half_flange - half_s_value;
            long check_for_flange_cut_and_chip = half_flange - cut_chip_value;
            TSD.StraightDimensionSet.StraightDimensionSetAttributes rd_att = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
            rd_att.ShortDimension = TSD.DimensionSetBaseAttributes.ShortDimensionTypes.Inside;
            rd_att.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
            rd_att.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);


            TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.DisplayCoordinateSystem);
            Type type_for_contourplate = typeof(TSM.ContourPlate);
            double size = 0;
            double output = 0;
            main_part.GetReportProperty("LENGTH", ref output);
            double DEPTH = Convert.ToDouble(current_view.Width.Equals(current_view.RestrictionBox.MaxPoint.X));
            if ((current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView)))
            {
                size = size1_m / 2;
            }
            else
            {
                size = size2_m / 2;
            }

            //////copedimesnison/////
            Type bolpart = typeof(TSM.BooleanPart);
            Type fit = typeof(TSM.Fitting);
            TSM.ModelObjectEnumerator test_bool = main_part.GetBooleans();
            ArrayList cuts = new ArrayList();
            TSG.Point fittrans_origin = new TSG.Point();
            double workpoint = output;
            TSG.Point point1 = new TSG.Point();
            TSG.Point point2 = new TSG.Point();
            TSG.Point point3 = new TSG.Point();
            TSG.Point point4 = new TSG.Point();
            while (test_bool.MoveNext())
            {
                int d = 0;
                ArrayList pts_in_viewco = new ArrayList();
                var partcut = test_bool.Current;

                if (partcut.GetType().Equals(bolpart))
                {


                    TSM.BooleanPart fitobj = partcut as TSM.BooleanPart;

                    if (fitobj.OperativePart.GetType().Equals(type_for_contourplate))
                    {
                        TSM.ContourPlate platecut = fitobj.OperativePart as TSM.ContourPlate;
                        ArrayList pts = platecut.Contour.ContourPoints;
                        ArrayList x_list = new ArrayList();
                        ArrayList y_list = new ArrayList();
                        ArrayList z_list = new ArrayList();
                        foreach (TSG.Point bolpart_point in pts)
                        {
                            TSG.Point tran_point = toviewmatrix.Transform(bolpart_point);
                            pts_in_viewco.Add(tran_point);

                        }
                        if (Convert.ToInt16((pts_in_viewco[0] as TSG.Point).Y) != Convert.ToInt16((pts_in_viewco[2] as TSG.Point).Y))
                        {
                            foreach (TSG.Point pt in pts_in_viewco)
                            {


                                x_list.Add(pt.X);
                                y_list.Add(pt.Y);
                                z_list.Add(pt.Z);

                            }

                            x_list.Sort();
                            y_list.Sort();
                            z_list.Sort();
                            ////////////////////Lower boolean dim///////////////////
                            TSG.Point pt1 = new TSG.Point();
                            TSG.Point pt2 = new TSG.Point();
                            TSG.Point pt3 = new TSG.Point();
                            TSG.Point pt4 = new TSG.Point();
                            TSG.Vector dim_vect_for_x_dim = new TSG.Vector();
                            TSG.Vector dim_vect_for_y_dim = new TSG.Vector();

                            //////////Third quadrant/////////////////////////
                            #region BOTTOM_VIEW_DIM
                            if (Convert.ToInt16(z_list[0]) < 0)
                            {



                                if ((Convert.ToInt16(y_list[0]) <= 0) && ((Convert.ToInt16(x_list[0]) <= 0)))
                                {

                                    pt1 = new TSG.Point(0, 0, 0);
                                    pt2 = new TSG.Point(0, Convert.ToInt16(y_list[y_list.Count - 1]), 0);
                                    pt3 = new TSG.Point(Convert.ToInt16(x_list[x_list.Count - 1]), -size, 0);
                                    pt4 = new TSG.Point(0, Convert.ToInt16(y_list[y_list.Count - 1]), 0);
                                    dim_vect_for_x_dim = new TSG.Vector(0, -1, 0);
                                    dim_vect_for_y_dim = new TSG.Vector(-1, 0, 0);

                                }
                                //////////Second quadrant//////////////////////////
                                else if ((Convert.ToInt16(x_list[0]) <= 0) && ((Convert.ToInt16(y_list[0]) >= 0)))
                                {
                                    pt1 = new TSG.Point(0, 0, 0);
                                    pt2 = new TSG.Point(0, Convert.ToInt16(y_list[0]), 0);
                                    pt3 = new TSG.Point(Convert.ToInt16(x_list[x_list.Count - 1]), size, 0);
                                    point2 = pt1;
                                    pt4 = new TSG.Point(0, Convert.ToInt16(y_list[0]), 0);
                                    dim_vect_for_x_dim = new TSG.Vector(0, 1, 0);
                                    dim_vect_for_y_dim = new TSG.Vector(-1, 0, 0);

                                }
                                /////////Fourth Quadrant////////////////////////////
                                else if ((Convert.ToInt16(x_list[0]) >= 0) && ((Convert.ToInt16(y_list[0]) <= 0)))
                                {
                                    pt1 = new TSG.Point(workpoint, 0, 0);
                                    pt3 = new TSG.Point(Convert.ToInt16(x_list[0]), Convert.ToInt16(y_list[0]), 0);
                                    pt2 = new TSG.Point(workpoint, Convert.ToInt16(y_list[y_list.Count - 1]), 0);
                                    pt4 = new TSG.Point(workpoint, Convert.ToInt16(y_list[y_list.Count - 1]), 0);
                                    dim_vect_for_x_dim = new TSG.Vector(0, -1, 0);
                                    dim_vect_for_y_dim = new TSG.Vector(1, 0, 0);


                                }
                                //////////////First Quadrant////////////////
                                else if ((Convert.ToInt16(x_list[0]) >= 0) && ((Convert.ToInt16(y_list[0]) >= 0)))
                                {
                                    pt1 = new TSG.Point(workpoint, 0, 0);
                                    pt3 = new TSG.Point(Convert.ToInt16(x_list[0]), size, 0);
                                    pt2 = new TSG.Point(workpoint, Convert.ToInt16(y_list[0]), 0);
                                    pt4 = new TSG.Point(workpoint, Convert.ToInt16(y_list[0]), 0);
                                    point3 = pt1;
                                    dim_vect_for_x_dim = new TSG.Vector(0, 1, 0);
                                    dim_vect_for_y_dim = new TSG.Vector(1, 0, 0);


                                }

                                double pt_to_pt_value = TSG.Distance.PointToPoint(pt1, pt4);

                                bool result_for_cut_chip = check_for_flange_cut_and_chip.Equals(Convert.ToInt64(pt_to_pt_value));


                                if (Convert.ToInt16(z_list[0]) > 0)
                                {
                                    if ((pt2.X < current_view.RestrictionBox.MaxPoint.X) && (pt1.X > current_view.RestrictionBox.MinPoint.X))
                                    {
                                        if (result_for_cut_chip == true)
                                        {


                                            TSD.StraightDimensionSet.StraightDimensionSetAttributes myset = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                                            myset.IncludePartCountInTag = false;
                                            myset.RightUpperTag.Clear();
                                            myset.RightLowerTag.Clear();
                                            TSD.TextElement mytext = new TSD.TextElement("");
                                            mytext.Value = "FLANGECUT &";
                                            TSD.TextElement mytext1 = new TSD.TextElement("");
                                            mytext1.Value = "CHIP";

                                            myset.RightUpperTag.Add(mytext);
                                            myset.RightLowerTag.Add(mytext1);
                                            myset.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                                            mytext.Font.Color = TSD.DrawingColors.Green;
                                            mytext1.Font.Color = TSD.DrawingColors.Green;

                                            myset.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                                            mytext.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                                            mytext1.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                                            TSD.PointList mypt = new TSD.PointList();
                                            mypt.Add(pt2);
                                            mypt.Add(pt3);
                                            try
                                            {
                                                dimset.CreateDimensionSet(current_view, mypt, dim_vect_for_x_dim, 200 + d, myset);
                                            }
                                            catch
                                            {
                                            }

                                        }

                                        if (result_for_cut_chip == false)
                                        {


                                            TSD.StraightDimensionSet.StraightDimensionSetAttributes myset = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                                            myset.RightUpperTag.Clear();
                                            myset.IncludePartCountInTag = false;
                                            TSD.TextElement mytext = new TSD.TextElement("");
                                            mytext.Value = "FLANGECUT &";
                                            myset.RightUpperTag.Add(mytext);
                                            myset.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                                            myset.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                                            mytext.Font.Color = TSD.DrawingColors.Green;
                                            mytext.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                                            TSD.PointList mypt = new TSD.PointList();
                                            mypt.Add(pt2);
                                            mypt.Add(pt3);
                                            try
                                            {
                                                dimset.CreateDimensionSet(current_view, mypt, dim_vect_for_x_dim, 200 + d, myset);
                                            }
                                            catch
                                            {
                                            }

                                            TSD.StraightDimensionSet.StraightDimensionSetAttributes myset1 = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                                            myset1.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                                            myset1.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                                            TSD.PointList mypt1 = new TSD.PointList();
                                            mypt1.Add(pt1);
                                            mypt1.Add(pt4);
                                            try
                                            {
                                                dimset.CreateDimensionSet(current_view, mypt1, dim_vect_for_y_dim, 200, myset1);
                                            }
                                            catch
                                            {
                                            }

                                        }
                                    }
                                }
                                if (Convert.ToInt16(z_list[0]) < 0)
                                {

                                    if ((pt2.X < current_view.RestrictionBox.MaxPoint.X) && (pt1.X >= current_view.RestrictionBox.MinPoint.X))
                                    {
                                        if (result_for_cut_chip == true)
                                        {


                                            TSD.StraightDimensionSet.StraightDimensionSetAttributes myset = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                                            myset.IncludePartCountInTag = false;
                                            myset.RightUpperTag.Clear();
                                            myset.RightLowerTag.Clear();
                                            TSD.TextElement mytext = new TSD.TextElement("");
                                            mytext.Value = "FLANGECUT &";
                                            TSD.TextElement mytext1 = new TSD.TextElement("");
                                            mytext1.Value = "CHIP";
                                            myset.RightUpperTag.Add(mytext);
                                            myset.RightLowerTag.Add(mytext1);
                                           
                                            myset.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;

                                            mytext.Font.Color = TSD.DrawingColors.Green;
                                            mytext1.Font.Color = TSD.DrawingColors.Green;
                                            myset.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                                            mytext.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                                            mytext1.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                                            TSD.PointList mypt = new TSD.PointList();
                                            mypt.Add(pt2);
                                            mypt.Add(pt3);
                                            try
                                            {
                                                dimset.CreateDimensionSet(current_view, mypt, dim_vect_for_x_dim, 200 + d, myset);
                                            }
                                            catch
                                            {
                                            }
                                           
                                        }

                                        if (result_for_cut_chip == false)
                                        {

                                            TSD.StraightDimensionSet.StraightDimensionSetAttributes myset = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                                            myset.IncludePartCountInTag = false;
                                            myset.RightUpperTag.Clear();
                                            TSD.TextElement mytext = new TSD.TextElement("");
                                            mytext.Value = "FLANGECUT &";
                                            myset.RightUpperTag.Add(mytext);
                                            myset.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                                             mytext.Font.Color = TSD.DrawingColors.Green;
                                            myset.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                                            mytext.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                                            
                                            TSD.PointList mypt = new TSD.PointList();
                                            mypt.Add(pt2);
                                            mypt.Add(pt3);
                                            try
                                            {
                                                dimset.CreateDimensionSet(current_view, mypt, dim_vect_for_x_dim, 200 + d, myset);
                                            }
                                            catch
                                            {
                                            }


                                            TSD.StraightDimensionSet.StraightDimensionSetAttributes myset1 = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();

                                            myset1.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                                            myset1.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                                            

                                            TSD.PointList mypt1 = new TSD.PointList();
                                            mypt1.Add(pt1);
                                            mypt1.Add(pt4);
                                            try
                                            {
                                                dimset.CreateDimensionSet(current_view, mypt1, dim_vect_for_y_dim, 200, myset1);
                                            }
                                            catch
                                            {
                                            }
                                            
                                        }
                                    }
                                    else
                                    {
                                    }

                                }




                                ////////////////
                            }
                            # endregion






                        }
                    }

                    cuts.Add(partcut);
                    TSG.Point fittrans_origin1 = toviewmatrix.Transform(main_part.EndPoint);
                }
                else
                {
                    point2 = new TSG.Point(0, size1_m / 2, 0);
                    point3 = new TSG.Point(output, size1_m / 2, 0);
                }
                d = d + 100;
            }




        }


    }
}
