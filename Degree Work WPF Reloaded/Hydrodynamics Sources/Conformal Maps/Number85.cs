﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Degree_Work.Mathematical_Sources.Complex;
using static Degree_Work.Mathematical_Sources.Functions.ElementaryFunctions;
using OxyPlot;

namespace Degree_Work.Hydrodynamics_Sources.Conformal_Maps
{
    class Number85 : IConformalMapFunction
    {
        public DataPoint this[DataPoint dzeta] => z(dzeta);

        public Complex this[Complex dzeta] => z(dzeta);

        double h;

        public double lambda { get { return h / Math.PI; } }

        public double H { get { return h; } set { if (value <= 0) { throw new ArgumentOutOfRangeException(); } h = value; } }

        public double X => lambda * Math.Log(lambda) - lambda;

        public Number85(double h)
        {
            if (h <= 0) { throw new ArgumentOutOfRangeException(); }
            this.h = h;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Complex dzeta(Complex Z)
        {
            return Mathematical_Sources.Equations.Solve(z, dz_ddzeta, Z); ;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public DataPoint dzeta(DataPoint Z)
        {
            return dzeta(Z.DataPointToComplex()).ComplexToDataPoint();
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Complex dz_ddzeta(Complex dzeta)
        {
            return 1 + lambda / dzeta;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public DataPoint dz_ddzeta(DataPoint dzeta)
        {
            return dz_ddzeta(dzeta.DataPointToComplex()).ComplexToDataPoint();
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Complex z(Complex dzeta)
        {
            return dzeta+lambda*Ln(dzeta);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public DataPoint z(DataPoint dzeta)
        {
            return z(dzeta.DataPointToComplex()).ComplexToDataPoint();
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is Number85 ? Equals((Number85)obj) : false;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Equals(Number85 other) => h == other.h;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return h.GetHashCode();
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return "Number85";
        }
    }
}