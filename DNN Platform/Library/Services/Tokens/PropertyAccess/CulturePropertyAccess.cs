#region Copyright
// 
// DotNetNuke® - https://www.dnnsoftware.com
// Copyright (c) 2002-2018
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
#region Usings

using System;
using System.Globalization;

using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.Services.Tokens
{
    public class CulturePropertyAccess : IPropertyAccess
    {
        #region IPropertyAccess Members

        public string GetProperty(string propertyName, string format, CultureInfo formatProvider, UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
        {
            CultureInfo ci = formatProvider;
            if (propertyName.Equals(CultureDropDownTypes.EnglishName.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return PropertyAccess.FormatString(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ci.EnglishName), format);
            }
            if (propertyName.Equals(CultureDropDownTypes.Lcid.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return ci.LCID.ToString();
            }
            if (propertyName.Equals(CultureDropDownTypes.Name.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return PropertyAccess.FormatString(ci.Name, format);
            }
            if (propertyName.Equals(CultureDropDownTypes.NativeName.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return PropertyAccess.FormatString(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ci.NativeName), format);
            }
            if (propertyName.Equals(CultureDropDownTypes.TwoLetterIsoCode.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return PropertyAccess.FormatString(ci.TwoLetterISOLanguageName, format);
            }
            if (propertyName.Equals(CultureDropDownTypes.ThreeLetterIsoCode.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return PropertyAccess.FormatString(ci.ThreeLetterISOLanguageName, format);
            }
            if (propertyName.Equals(CultureDropDownTypes.DisplayName.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return PropertyAccess.FormatString(ci.DisplayName, format);
            }
            if (propertyName.Equals("languagename", StringComparison.InvariantCultureIgnoreCase))
            {
                if(ci.IsNeutralCulture)
                {
                    return PropertyAccess.FormatString(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ci.EnglishName), format);
                }
                else
                {
                    return PropertyAccess.FormatString(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ci.Parent.EnglishName), format);
                }
            }
            if (propertyName.Equals("languagenativename", StringComparison.InvariantCultureIgnoreCase))
            {
                if(ci.IsNeutralCulture)
                {
                    return PropertyAccess.FormatString(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ci.NativeName), format);
                }
                else
                {
                    return PropertyAccess.FormatString(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ci.Parent.NativeName), format);
                }
            }
            if (propertyName.Equals("countryname", StringComparison.InvariantCultureIgnoreCase))
            {
                if(ci.IsNeutralCulture)
                {
                    //Neutral culture do not include region information
                    return "";
                }
                else
                {
                    RegionInfo country = new RegionInfo(new CultureInfo(ci.Name, false).LCID);
                    return PropertyAccess.FormatString(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(country.EnglishName), format);
                }
            }
            if (propertyName.Equals("countrynativename", StringComparison.InvariantCultureIgnoreCase))
            {
                if(ci.IsNeutralCulture)
                {
                    //Neutral culture do not include region information
                    return "";
                }
                else
                {
                    RegionInfo country = new RegionInfo(new CultureInfo(ci.Name, false).LCID);
                    return PropertyAccess.FormatString(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(country.NativeName), format);
                }

                
            }
            PropertyNotFound = true;
            return string.Empty;
        }

        public CacheLevel Cacheability
        {
            get
            {
                return CacheLevel.fullyCacheable;
            }
        }

        #endregion
    }
}