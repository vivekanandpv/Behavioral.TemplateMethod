using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Behavioral.TemplateMethod
{
    //  Idea: Template method is a method of the superclass which
    //  lays out the foundation of certain critical operation as
    //  high-level steps. The subclasses implement these steps. 

    //  Steps are locked, but the details are left to the subclasses.

    //  Duplication of high-level process, standardization of the process,
    //  OCP are the prime concerns

    //  Subclasses don't call the template method, but the template method
    //  calls the subclass implementations.

    public abstract class LoanBase<T> where T: Loan, new()
    {
        public T Loan { get; private set; }

        //  Template method. Do not make it virtual!
        public T Process()
        {
            Loan = new T();

            List<ApprovalStatus> listOfStatus = new List<ApprovalStatus>();

            //  Template method calls
            //  Not the implementers
            listOfStatus.Add(VerifyDocuments());
            listOfStatus.Add(GetPreClearance());
            listOfStatus.Add(MakerApprove());
            listOfStatus.Add(CheckerApprove());
            listOfStatus.Add(GetPostClearance());

            var rejected = listOfStatus.Any(a => !a.Status);
            Loan.RejectionReasons.AddRange(
                listOfStatus
                    .Where(s => !s.Status)
                    .Select(a => a.Message)
                    .ToList());
            
            Loan.IsApproved = !rejected;
            return Loan;
        }

        protected abstract ApprovalStatus VerifyDocuments();

        //  Hook
        protected virtual ApprovalStatus GetPreClearance()
        {
            return new ApprovalStatus {Status = true};
        }

        protected abstract ApprovalStatus MakerApprove();

        protected abstract ApprovalStatus CheckerApprove();

        //  Hook
        protected virtual ApprovalStatus GetPostClearance()
        {
            return new ApprovalStatus {Status = true};
        }
    }

    //  For generic constraint only
    public abstract class Loan
    {
        public bool IsApproved { get; set; }
        public List<string> RejectionReasons { get; set; } = new List<string>();
    }

    public class PrimePersonalLoan: Loan
    {
        public int CreditScore { get; set; } = 730;
        public bool CityDweller { get; set; } = true;
        public int AnnualIncome { get; set; } = 2_000_000;
        
    }

    public class SubprimePersonalLoan: Loan
    {

    }

    public class CarLoan: Loan
    {

    }

    public class ApprovalStatus
    {
        public bool Status { get; set; }
        public string Message { get; set; }
    }

    public class PrimePersonalLoanService : LoanBase<PrimePersonalLoan>
    {
        protected override ApprovalStatus CheckerApprove()
        {
            Console.WriteLine("PrimePersonalLoanService: Checker Approve");
            return new ApprovalStatus
            {
                Status = Loan.CreditScore >= 750,
                Message = Loan.CreditScore < 750 ? "Not enough credit score" : null
            };
        }

        protected override ApprovalStatus MakerApprove()
        {
            Console.WriteLine("PrimePersonalLoanService: Maker Approve");
            return new ApprovalStatus
            {
                Status = Loan.CityDweller,
                Message = Loan.CityDweller ? "Not a city dweller" : null
            };
        }

        protected override ApprovalStatus VerifyDocuments()
        {
            Console.WriteLine("PrimePersonalLoanService: Verify Documents");
            return new ApprovalStatus
            {
                Status = true,
                Message = "Discrepancy in Aadhaar data"
            };
        }

        //  Hook implementation
        protected override ApprovalStatus GetPreClearance()
        {
            Console.WriteLine("PrimePersonalLoanService: Pre-Clearance");
            return new ApprovalStatus
            {
                Status = Loan.AnnualIncome >= 1_000_000,
                Message = Loan.AnnualIncome <= 1_000_000 ? "Not enough annual income" : null
            };
        }
    }

}
