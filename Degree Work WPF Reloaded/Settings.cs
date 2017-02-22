﻿using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OxyPlot;


namespace Degree_Work
{
    static class Settings
    {
        public static StreamLinesPlotGeomParams PlotGeomParams;
        public static StreamLinesPlotVisualParams PlotVisualParams;
        public static UInt16 Precision
        {
            get
            {
                return _precision;
            }
            set
            {
                _precision = value;
                Format = "0.";
                for (int i = 1; i <= _precision; i++)
                {
                    Format += "#";
                }
            }
        }
        public static string Format = "0.####"; 
        static UInt16 _precision = 4;
        static double precisionf
        {
            get
            {
                return Math.Pow(10, -_precision);
            }
        }
        internal static ImageSource exitIcoSource;
        internal static ImageSource exitIcoSelectedSource;
        internal static ImageSource saveIcoSource;
        internal static ImageSource saveIcoSelectedSource;
        internal static ImageSource OKIcoSource;

        static Settings()
        {
            exitIcoSource = new BitmapImage(new Uri(@"Resources/exitIco.bmp", UriKind.Relative));
            exitIcoSelectedSource = new BitmapImage(new Uri(@"Resources/exitSelected.bmp", UriKind.Relative));
            saveIcoSource = new BitmapImage(new Uri(@"Resources/saveIco3.png", UriKind.Relative));
            saveIcoSelectedSource = new BitmapImage(new Uri(@"Resources/saveIco3Selected.png", UriKind.Relative));
            OKIcoSource = new BitmapImage(new Uri(@"Resources/okayIcon.png", UriKind.Relative));
            PlotGeomParams = new StreamLinesPlotGeomParams() { hVertical = 0.5, MRKh = 0.3, XMax = 20, XMin = -20, YMax = 20, YMin = -20 };
            PlotVisualParams = new StreamLinesPlotVisualParams()
            {
                LineColor = OxyColors.Blue,
                ArrowColor = OxyColors.Black,
                BorderFillColor = OxyColors.Gray,
                BorderStrokeColor = OxyColors.Black,
                BorderStrokeThickness = 1,
                LineStrokeThickness = 3,
                ArrowStokeThickness = 3
            };
        }

    }
}
