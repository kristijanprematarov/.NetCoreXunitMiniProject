using CreditCards.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditCards.Core.Model
{
    public class CreditCardApplicationEvaluator
    {
        private const int AutoReferralMaxAge = 20;
        private const int LowIncomeThreshhold = 20_000;
        private const int HighIncomeThreshhold = 100_000;
        private readonly IFrequentFlyerNumberValidator _frequentFlyerNumberValidator;

        public CreditCardApplicationEvaluator(IFrequentFlyerNumberValidator frequentFlyerNumberValidator)
        {
            _frequentFlyerNumberValidator = frequentFlyerNumberValidator;
        }

        public CreditCardApplicationDecision Evaluate(CreditCardApplication application)
        {
            if (application.GrossAnnualIncome >= HighIncomeThreshhold)
            {
                return CreditCardApplicationDecision.AutoAccepted;
            }

            if (!_frequentFlyerNumberValidator.IsValid(application.FrequentFlyerNumber))
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.Age <= AutoReferralMaxAge)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.GrossAnnualIncome < LowIncomeThreshhold)
            {
                return CreditCardApplicationDecision.AutoDeclined;
            }

            return CreditCardApplicationDecision.ReferredToHuman;
        }
    }
}
