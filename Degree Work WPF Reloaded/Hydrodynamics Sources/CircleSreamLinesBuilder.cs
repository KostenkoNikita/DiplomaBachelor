﻿using System;
using System.Collections.Generic;
using MathCore_2_0;
using static MathCore_2_0.complex;
using static MathCore_2_0.math;
using OxyPlot;

namespace Degree_Work.Hydrodynamics_Sources
{
    class StreamLinesBuilderCircle : CircularStreamLinesBuilder
    {
        delegate void IniFillAsync(List<DataPoint> BasePlus, List<DataPoint> ListMinus, double y);
        delegate void TransformationAsync(List<DataPoint> Base, List<DataPoint> Lines, complex angleMult);
        delegate void FullBuildAsync(List<DataPoint> BasePlus, List<DataPoint> BaseMinus, List<DataPoint> LinesPlus, List<DataPoint> LinesMinus, complex angleMult, double y);
        IniFillAsync async_base;
        TransformationAsync async_transform;
        FullBuildAsync async_full;

        public StreamLinesBuilderCircle(Potential w, PlotWindowModel g)
        {
            this.w = w;
            this.g = g;
            h = Settings.PlotGeomParams.hVertical;
            h_mrk = Settings.PlotGeomParams.MRKh;
            x_min = Settings.PlotGeomParams.XMin;
            x_max = Settings.PlotGeomParams.XMax;
            y_min = Settings.PlotGeomParams.YMin;
            y_max = Settings.PlotGeomParams.YMax;
            domain = CanonicalDomain.Circular;
            function = this.w.f;
            InitialBuild();
        }
        public override void ChangeParams(double? x_min, double? x_max, double? y_max, double? h_horizontal, double? h_vertical)
        {
            this.x_min = x_min ?? this.x_min;
            this.x_max = x_max ?? this.x_max;
            this.y_max = y_max ?? this.y_max;
            this.h_mrk = h_horizontal ?? this.h_mrk;
            this.h = h_vertical ?? this.h;
            FullRebuild();
        }
        void InitialBuild()
        {
            g.Clear();
            FindBaseStreamLines();
        }
        public override void Rebuild()
        {
            g.Clear();
            g.DrawBorder(this);
            Transform();
        }
        void FullRebuild()
        {
            g.Clear();
            g.DrawBorder(this);
            FindAllStreamLines();
        }

        void FindInitSpecial() 
        {
            LeftSpecialStreamLineBase = new List<DataPoint>();
            RightSpecialStreamLineBase = new List<DataPoint>();
            LeftStagnationPointBase = -w.R;
            RightStagnationPointBase = w.R;
            double x;
            for (x = RightStagnationPointBase.Re + (h_mrk / 100.0); x <= x_max; x++)
            {
                LeftSpecialStreamLineBase.Add(new DataPoint(-x, 0));
                RightSpecialStreamLineBase.Add(new DataPoint(x, 0));
            }
        }
        void FindBaseStreamLines()
        {
            FindInitSpecial();
            StreamLinesBase = new List<List<DataPoint>>();
            async_base = new IniFillAsync(AsyncIniFill);
            res_list = new List<IAsyncResult>();
            for (double y = h; y <= y_max - h; y += h)
            {
                StreamLinesBase.Add(new List<DataPoint>());
                StreamLinesBase.Add(new List<DataPoint>());
                res_list.Add(async_base.BeginInvoke(StreamLinesBase[StreamLinesBase.Count - 2], StreamLinesBase[StreamLinesBase.Count-1], y, null, null));
            }
            while (!res_list.IsAllThreadsCompleted()) { }
            res_list = null;
        }
        void Transform()
        {
            StreamLines = new List<List<DataPoint>>();
            LeftSpecialStreamLine = new List<DataPoint>();
            RightSpecialStreamLine = new List<DataPoint>();
            async_transform = new TransformationAsync(AsyncTransform);
            res_list = new List<IAsyncResult>();
            foreach (List<DataPoint> sllb in StreamLinesBase)
            {
                StreamLines.Add(new List<DataPoint>());
                res_list.Add(async_transform.BeginInvoke(sllb, StreamLines[StreamLines.Count - 1], exp(i * w.AlphaRadians), null, null));
            }
            LeftStagnationPoint = w.f.z(LeftStagnationPointBase * exp(i * w.AlphaRadians));
            RightStagnationPoint = w.f.z(RightStagnationPointBase * exp(i * w.AlphaRadians));
            g.DrawStagnationPoints(RightStagnationPoint, LeftStagnationPoint);
            res_list.Add(async_transform.BeginInvoke(LeftSpecialStreamLineBase,LeftSpecialStreamLine, exp(i * w.AlphaRadians), null,null));
            res_list.Add(async_transform.BeginInvoke(RightSpecialStreamLineBase, RightSpecialStreamLine, exp(i * w.AlphaRadians), null, null));
            while (!res_list.IsAllThreadsCompleted()) { }
            res_list = null;
        }
        void FindAllStreamLines()
        {
            FindInitSpecial();
            double tmp = w.AlphaRadians;
            w.AlphaDegrees = 0;
            StreamLines = new List<List<DataPoint>>();
            StreamLinesBase = new List<List<DataPoint>>();
            async_full = new FullBuildAsync(AsyncFullBuild);
            res_list = new List<IAsyncResult>();
            for (double y = h; y <= y_max - h; y += h)
            {
                StreamLines.Add(new List<DataPoint>());
                StreamLines.Add(new List<DataPoint>());
                StreamLinesBase.Add(new List<DataPoint>());
                StreamLinesBase.Add(new List<DataPoint>());
                res_list.Add(async_full.BeginInvoke(StreamLinesBase[StreamLinesBase.Count - 2], StreamLinesBase[StreamLinesBase.Count - 1], StreamLines[StreamLines.Count - 2], StreamLines[StreamLines.Count - 1], exp(i * tmp), y, null, null));
            }
            LeftStagnationPoint = w.f.z(LeftStagnationPointBase * exp(i * tmp));
            RightStagnationPoint = w.f.z(RightStagnationPointBase * exp(i * tmp));
            g.DrawStagnationPoints(RightStagnationPoint, LeftStagnationPoint);
            LeftSpecialStreamLine = new List<DataPoint>();
            RightSpecialStreamLine = new List<DataPoint>();
            res_list.Add(async_transform.BeginInvoke(LeftSpecialStreamLineBase, LeftSpecialStreamLine, exp(i * tmp), null, null));
            res_list.Add(async_transform.BeginInvoke(RightSpecialStreamLineBase, RightSpecialStreamLine, exp(i * tmp), null, null));
            while (!res_list.IsAllThreadsCompleted()) { }
            w.AlphaRadians = tmp;
            res_list = null;
        }


        void AsyncIniFill(List<DataPoint> bp, List<DataPoint> bm, double y)
        {
            double x, x_new, y_new, k1, k2, k3, k4;
            x_new = x_min;
            y_new = y;
            bp.Add(new DataPoint(x_new, y_new));
            bm.Add(new DataPoint(x_new, -y_new));
            while (x_new < x_max)
            {
                x = x_new;
                y = y_new;
                k1 = f(x, y);
                k2 = f(x + h_mrk / 2, y + (h_mrk * k1) / 2);
                k3 = f(x + h_mrk / 2, y + (h_mrk * k2) / 2);
                k4 = f(x + h_mrk, y + (h_mrk * k3));
                y_new = y + (h_mrk / 6) * (k1 + 2 * k2 + 2 * k3 + k4);
                x_new = x + h_mrk;
                bp.Add(new DataPoint(x_new, y_new));
                bm.Add(new DataPoint(x_new, -y_new));
            }
        }
        void AsyncTransform(List<DataPoint> b, List<DataPoint> l, complex angleMult)
        {
            DataPoint tmp;
            foreach (DataPoint bp in b)
            {
                tmp = w.f.z((bp.DataPointToComplex()* angleMult).ComplexToDataPoint());
                if (tmp.Abs() < 20) { l.Add(tmp); }
            }
            g.DrawCurve(l);
        }
        void AsyncFullBuild(List<DataPoint> bp, List<DataPoint> bm, List<DataPoint> lp, List<DataPoint> lm, complex angleMult, double y)
        {
            DataPoint tmp;
            double x, x_new, y_new, k1, k2, k3, k4;
            x_new = x_min;
            y_new = y;
            bp.Add(new DataPoint(x_new, y_new));
            bm.Add(new DataPoint(x_new, -y_new));
            while (x_new < x_max)
            {
                x = x_new;
                y = y_new;
                k1 = f(x, y);
                k2 = f(x + h_mrk / 2, y + (h_mrk * k1) / 2);
                k3 = f(x + h_mrk / 2, y + (h_mrk * k2) / 2);
                k4 = f(x + h_mrk, y + (h_mrk * k3));
                y_new = y + (h_mrk / 6) * (k1 + 2 * k2 + 2 * k3 + k4);
                x_new = x + h_mrk;
                bp.Add(new DataPoint(x_new, y_new));
                bm.Add(new DataPoint(x_new, -y_new));
                tmp = w.f.z((bp[bp.Count - 1].DataPointToComplex() * angleMult).ComplexToDataPoint());
                if (tmp.Abs() < 20) { lp.Add(tmp); }
                tmp = w.f.z((bm[bm.Count - 1].DataPointToComplex() * angleMult).ComplexToDataPoint());
                if (tmp.Abs() < 20) { lm.Add(tmp); }
            }
            g.DrawCurve(lp);
            g.DrawCurve(lm);
        }
    }
}