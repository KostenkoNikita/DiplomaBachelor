﻿//#define HELP_FOR_GROUP_LEADER
using System.Collections.Generic;
using OxyPlot;
using System;

namespace Degree_Work.Hydrodynamics_Sources
{
    abstract class StreamLinesBuilder
    {
        /// <summary>
        /// Графическая модель, на которой ведется рисование
        /// </summary>
        protected PlotWindowModel g;

        /// <summary>
        /// Ссылка на экземпляр класса потенциала
        /// </summary>
        protected Potential w;

        /// <summary>
        /// Тип канонической области для текущего построителя линий тока
        /// </summary>
        protected CanonicalDomain domain;

        /// <summary>
        /// Границы области
        /// </summary>
        protected double x_min, x_max, y_max, y_min;

        /// <summary>
        /// Расстояние между линиями тока в начале отсчета по вертикальной оси
        /// </summary>
        protected double h;

        /// <summary>
        /// Расстояние между линиями тока по горизонтальной оси
        /// </summary>
        protected double h_mrk;

        public CanonicalDomain Domain => domain;

        public Potential W => w;

        /// <summary>
        /// Функция консормного отображения
        /// </summary>
        protected IConformalMapFunction function => w.f;

        /// <summary>
        /// Линии тока в физической плоскости
        /// </summary>
        protected List<List<DataPoint>> StreamLines;

        /// <summary>
        /// Линии тока во вспомогательной плоскости
        /// </summary>
        protected List<List<DataPoint>> StreamLinesBase;

        /// <summary>
        /// Метод для перестроения линий тока. Переопределяется
        /// </summary>
        public virtual void Rebuild() { }

        /// <summary>
        /// Метод для изменения каких-то параметров построения. Измененные параметры передаются в аргументах, неизмененные передаются как null
        /// </summary>
        /// <param name="x_min">Левая граница</param>
        /// <param name="x_max">Правая граница</param>
        /// <param name="y_max">Верхняя граница по У</param>
        /// <param name="h_horizontal">Расстояние между точками линии тока</param>
        /// <param name="h_vertical">Расстояние между линиями тока</param>
        public virtual void ChangeParams(double? x_min, double? x_max, double? y_max, double? h_horizontal, double? h_vertical) { }
        protected List<IAsyncResult> res_list;
#if !HELP_FOR_GROUP_LEADER
        protected StreamLinesBuilder(Potential w, PlotWindowModel g, CanonicalDomain domain)
        {
            this.w = w;
            this.g = g;
            this.domain = domain;
            switch (domain)
            {
                case CanonicalDomain.HalfPlane:
                    x_min = Settings.PlotGeomParams.XMin;
                    x_max = Settings.PlotGeomParams.XMax;
                    y_max = Settings.PlotGeomParams.YMax;
                    y_min = 0;
                    break;
                case CanonicalDomain.Zone:
                    x_min = Settings.PlotGeomParams.XMin;
                    x_max = Settings.PlotGeomParams.XMax;
                    y_max = Math.PI;
                    y_min = -Settings.PlotGeomParams.hVertical;
                    break;
                case CanonicalDomain.Circular:
                    x_min = Settings.PlotGeomParams.XMin;
                    x_max = Settings.PlotGeomParams.XMax;
                    y_max = Settings.PlotGeomParams.YMax;
                    y_min = 0;
                    break;
                default: throw new ArgumentException();
            }
            h_mrk = Settings.PlotGeomParams.MRKh;
            h = Settings.PlotGeomParams.hVertical;
            StreamLinesBase = new List<List<DataPoint>>();
        }
#endif
    }
}
