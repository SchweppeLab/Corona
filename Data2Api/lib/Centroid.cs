using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.Interfaces.SpectrumFormat_V1;

namespace Data2Api.lib
{
    /// <summary>
    /// for pulling spectral information from API scans
    /// </summary>
    public struct Centroid : ICentroid
    {
        public Centroid(double mz, double intensity, double baseline = 0, double noise = 0)
        {
            Mz = mz;
            Intensity = intensity;
            Baseline = baseline;
            Noise = noise;
        }

        /// <summary>
        /// Centroid m/z
        /// </summary>
        public double Mz { get; set; }

        /// <summary>
        /// Baseline
        /// </summary>
        public double Baseline { get; set; }

        /// <summary>
        /// Centroid intensity
        /// </summary>
        public double Intensity { get; set; }

        /// <summary>
        /// Source noise level
        /// </summary>
        public double Noise { get; set; }

        public bool? IsExceptional => throw new NotImplementedException();

        public bool? IsReferenced => throw new NotImplementedException();

        public bool? IsMerged => throw new NotImplementedException();

        public bool? IsFragmented => throw new NotImplementedException();

        public int? Charge => throw new NotImplementedException();

        public IMassIntensity[] Profile => throw new NotImplementedException();

        public double? Resolution => throw new NotImplementedException();

        public int? ChargeEnvelopeIndex => throw new NotImplementedException();

        public bool? IsMonoisotopic => throw new NotImplementedException();

        public bool? IsClusterTop => throw new NotImplementedException();
    }
}
