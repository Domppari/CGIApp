using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BusinessIdSpecification
{
    public class BusinessIdSpecification : ISpecification<string>
    {        
        private const int idLength = 9;
        private const int idPrefixLength = 7;
        private const int idVerificationNumberLength = 1;
        private const string idAllowedCharactersRegularExpression = "^[]0-9,[-]*$";
        private const string idDelimiterString = "-";
        private static readonly int[] prefixChecksumWeights = new int[idPrefixLength] {
            7, 9, 10, 5, 8, 4, 2
        };

        private Error<FailReason> failReasons = new Error<FailReason>();
        private string idPrefix = null;
        private string idVerificationNumber = null;

        public BusinessIdSpecification()
        {
            InitializeErrors();
        }

        public IEnumerable<string> ReasonsForDissatisfaction
        {
            get
            {
                return failReasons.GetReportedErrors();
            }
        }

        public bool IsSatisfiedBy(string businessId)
        {
            Reset();

            if (string.IsNullOrEmpty(businessId))
            {
                failReasons.ReportError(FailReason.INPUT_TOO_SHORT);
                return false;
            }

            var isSatisfied = true;            
            isSatisfied &= CheckInvalidCharacters(businessId);
            isSatisfied &= CheckDelimiter(businessId);
            isSatisfied &= CheckLengths(businessId);
            if (isSatisfied)
                isSatisfied &= CheckVerificationNumber();
            return isSatisfied;
        }

        private void Reset()
        {
            idPrefix = null;
            idVerificationNumber = null;
            failReasons.ClearReportedErrors();
        }

        private bool CheckVerificationNumber()
        {
            if (idPrefix == null || idVerificationNumber == null)
                return false;

            int checksum = 0;
            for (var i = 0; i < idPrefixLength; i++)
                checksum += prefixChecksumWeights[i] * (int)char.GetNumericValue(idPrefix[i]);

            var remainder = checksum % 11;
            if (remainder == 1)
            {
                failReasons.ReportError(FailReason.VERIFICATION_NUMBER_CHECKSUM);
                return false;
            }

            if (remainder == 0 && idVerificationNumber == "0")
                return true;

            if (11 - remainder == int.Parse(idVerificationNumber))
                return true;

            failReasons.ReportError(FailReason.VERIFICATION_NUMBER_CHECKSUM);
            return false;
        }

        private bool CheckInvalidCharacters(string businessId)
        {
            var regex = new Regex(idAllowedCharactersRegularExpression);
            if (!regex.Match(businessId).Success)
            {
                failReasons.ReportError(FailReason.CONTAINS_INVALID_CHARACTERS);
                return false;
            }

            return true;
        }

        private bool CheckDelimiter(string businessId)
        {
            if (!businessId.Contains(idDelimiterString))
            {
                failReasons.ReportError(FailReason.DELIMITER_MISSING);
                return false;
            }

            return true;
        }

        private bool CheckLengths(string businessId)
        {
            bool correctLength = true;

            if (businessId.Length > idLength)
            {
                failReasons.ReportError(FailReason.INPUT_TOO_LONG);
                correctLength = false;
            }

            if (businessId.Length < idLength)
            {
                failReasons.ReportError(FailReason.INPUT_TOO_SHORT);
                correctLength = false;
            }

            if (!DisassembleBusinessId(businessId))
                return false;

            if (idPrefix.Length > idPrefixLength)
            {
                failReasons.ReportError(FailReason.PREFIX_TOO_LONG);
                correctLength = false;
            }

            if (idPrefix.Length < idPrefixLength)
            {
                failReasons.ReportError(FailReason.PREFIX_TOO_SHORT);
                correctLength = false;
            }

            if (idVerificationNumber.Length > idVerificationNumberLength)
            {
                failReasons.ReportError(FailReason.VERIFICATION_NUMBER_TOO_LONG);
                correctLength = false;
            }

            if (idVerificationNumber.Length < idVerificationNumberLength)
            {
                failReasons.ReportError(FailReason.VERIFICATION_NUMBER_TOO_SHORT);
                correctLength = false;
            }

            return correctLength;
        }

        private bool DisassembleBusinessId(string businessId)
        {
            var splitted = businessId.Split(new string[] { idDelimiterString }, StringSplitOptions.None);
            try
            {
                idPrefix = splitted[0];
                idVerificationNumber = splitted[1];
                return true;
            }
            catch (Exception)
            {
                idPrefix = null;
                idVerificationNumber = null;
                return false;
            }
        }

        private void InitializeErrors()
        {
            // Texts should be moved to own "localization" class / resource manager
            failReasons.InitError(FailReason.CONTAINS_INVALID_CHARACTERS, "Contains incorrect character(s)");
            failReasons.InitError(FailReason.DELIMITER_MISSING, "Does not contain the required verification number delimiter: '" + idDelimiterString + "'");
            failReasons.InitError(FailReason.INPUT_TOO_LONG, "Too long, the required length is " + idLength);
            failReasons.InitError(FailReason.INPUT_TOO_SHORT, "Too short, the required length is " + idLength);
            failReasons.InitError(FailReason.PREFIX_TOO_LONG, "Prefix is too long, the required length is " + idPrefixLength);
            failReasons.InitError(FailReason.PREFIX_TOO_SHORT, "Prefix is too short, the required length is " + idPrefixLength);
            failReasons.InitError(FailReason.VERIFICATION_NUMBER_TOO_LONG, "Verification number is too long, the required length is " + idVerificationNumberLength);
            failReasons.InitError(FailReason.VERIFICATION_NUMBER_TOO_SHORT, "Verification number is too short, the required length is " + idVerificationNumberLength);
            failReasons.InitError(FailReason.VERIFICATION_NUMBER_CHECKSUM, "Verification number checksum failure, check business id and try again");
        }

        public enum FailReason
        {
            INPUT_TOO_LONG,
            INPUT_TOO_SHORT,
            DELIMITER_MISSING,
            CONTAINS_INVALID_CHARACTERS,
            PREFIX_TOO_LONG,
            PREFIX_TOO_SHORT,
            VERIFICATION_NUMBER_TOO_SHORT,
            VERIFICATION_NUMBER_TOO_LONG,
            VERIFICATION_NUMBER_CHECKSUM,
        }
    }
}
