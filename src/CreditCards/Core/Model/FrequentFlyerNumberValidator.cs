﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditCards.Core.Model
{
    /// <summary>
    /// A frequent flyer number consists of 2 parts separated by a '-':<br/>
    /// [member number]-[scheme identifier]<br/><br/>
    /// Member numbers consist of 6 numeric digits<br/><br/>
    /// Scheme identifiers are a single uppercase alphabetic character
    /// </summary>
    public class FrequentFlyerNumberValidator : IFrequentFlyerNumberValidator
    {
        private readonly char[] _validSchemeIdentifiers = { 'A', 'Q', 'Y' };
        private const int ExpectedTotalLength = 8;//MemberNumberLength + 1 + SchemeIdentifierLength(1) == 8
        private const int ExpectedMemberNumberLength = 6;

        public bool IsValid(string frequentFlyerNumber)
        {
            if (frequentFlyerNumber == null)
            {
                throw new ArgumentNullException(nameof(frequentFlyerNumber));
            }

            if (frequentFlyerNumber.Length != ExpectedTotalLength)
            {
                return false;
            }

            var memberNumberPart = frequentFlyerNumber.Substring(0, ExpectedMemberNumberLength);

            if (!int.TryParse(memberNumberPart, NumberStyles.None, null, out int _))
            {
                return false;
            }

            var schemeIdentifier = frequentFlyerNumber.Last();
            return _validSchemeIdentifiers.Contains(schemeIdentifier);
        }
    }
}
