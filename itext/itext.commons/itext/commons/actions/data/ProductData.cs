/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;

namespace iText.Commons.Actions.Data {
    /// <summary>Class is used to describe used product information.</summary>
    public sealed class ProductData {
        private readonly String publicProductName;

        private readonly String productName;

        private readonly String version;

        private readonly String minimalCompatibleLicenseKeyVersion;

        private readonly int sinceCopyrightYear;

        private readonly int toCopyrightYear;

        /// <summary>Creates a new instance of product data.</summary>
        /// <param name="publicProductName">is a product name</param>
        /// <param name="productName">is a technical name of the product</param>
        /// <param name="version">is a version of the product</param>
        /// <param name="sinceCopyrightYear">is the first year of a product development</param>
        /// <param name="toCopyrightYear">is a last year of a product development</param>
        public ProductData(String publicProductName, String productName, String version, int sinceCopyrightYear, int
             toCopyrightYear)
            : this(publicProductName, productName, version, null, sinceCopyrightYear, toCopyrightYear) {
        }

        /// <summary>Creates a new instance of product data.</summary>
        /// <param name="publicProductName">is a product name</param>
        /// <param name="productName">is a technical name of the product</param>
        /// <param name="version">is a version of the product</param>
        /// <param name="minimalCompatibleLicenseKeyVersion">is a minimal compatible version of licensekey library</param>
        /// <param name="sinceCopyrightYear">is the first year of a product development</param>
        /// <param name="toCopyrightYear">is a last year of a product development</param>
        public ProductData(String publicProductName, String productName, String version, String minimalCompatibleLicenseKeyVersion
            , int sinceCopyrightYear, int toCopyrightYear) {
            this.publicProductName = publicProductName;
            this.productName = productName;
            this.version = version;
            this.minimalCompatibleLicenseKeyVersion = minimalCompatibleLicenseKeyVersion;
            this.sinceCopyrightYear = sinceCopyrightYear;
            this.toCopyrightYear = toCopyrightYear;
        }

        /// <summary>Getter for a product name.</summary>
        /// <returns>product name</returns>
        public String GetPublicProductName() {
            return publicProductName;
        }

        /// <summary>Getter for a technical name of the product.</summary>
        /// <returns>the technical name of the product</returns>
        public String GetProductName() {
            return productName;
        }

        /// <summary>Getter for a version of the product.</summary>
        /// <returns>version of the product</returns>
        public String GetVersion() {
            return version;
        }

        /// <summary>Getter for the first year of copyright period.</summary>
        /// <returns>the first year of copyright</returns>
        public int GetSinceCopyrightYear() {
            return sinceCopyrightYear;
        }

        /// <summary>Getter for the last year of copyright period.</summary>
        /// <returns>the last year of copyright</returns>
        public int GetToCopyrightYear() {
            return toCopyrightYear;
        }

        /// <summary>Getter for the minimal compatible licensekey version.</summary>
        /// <returns>minimal compatible version of licensekey library.</returns>
        public String GetMinCompatibleLicensingModuleVersion() {
            return minimalCompatibleLicenseKeyVersion;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Commons.Actions.Data.ProductData other = (iText.Commons.Actions.Data.ProductData)o;
            return Object.Equals(publicProductName, other.publicProductName) && Object.Equals(productName, other.productName
                ) && Object.Equals(version, other.version) && sinceCopyrightYear == other.sinceCopyrightYear && toCopyrightYear
                 == other.toCopyrightYear;
        }

        public override int GetHashCode() {
            int result = publicProductName != null ? publicProductName.GetHashCode() : 0;
            result += 31 * result + (productName != null ? productName.GetHashCode() : 0);
            result += 31 * result + (version != null ? version.GetHashCode() : 0);
            result += 31 * result + sinceCopyrightYear;
            result += 31 * result + toCopyrightYear;
            return result;
        }
    }
}
