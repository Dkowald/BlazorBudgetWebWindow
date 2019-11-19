namespace BlazorBudgetWebWindow.Pages {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Timers;
    using BlazorBudgetWebWindow.Model;
    using Microsoft.AspNetCore.Components;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "ET001:Type name does not match file name", Justification = "After Razor 3.1 RTM this will no longer be necessary")]
    public class IndexBase : ComponentBase {
        readonly Timer _debounceTimer;
        readonly Timer _recreateChartDelayTimer;
        Int32 _payFrequency = 52;

        public Decimal AnnualSavings { get; private set; }

        public Decimal AnnualTakeHomePay { get; private set; }

        public IList<AnnualTotalItem> AnnualTotalItems { get; private set; }

        public IList<BudgetAllocationItem> BudgetAllocationItems { get; private set; }

        public Decimal MinTakeHomePay { get; set; }

        public IEnumerable<PayFrequencyItem> PayFrequencyItems { get; }

        public Decimal RemainingAllocation { get; private set; }

        public Boolean ShowPieChart { get; private set; } = true;

        public Decimal TakeHomePay { get; set; } = 2000;

        public IndexBase() {
            var budgetAllocationItems = new List<BudgetAllocationItem>();
            budgetAllocationItems.Add(new BudgetAllocationItem { Name = "Housing" });
            budgetAllocationItems.Add(new BudgetAllocationItem { Name = "Food" });
            budgetAllocationItems.Add(new BudgetAllocationItem { Name = "Gas" });
            budgetAllocationItems.Add(new BudgetAllocationItem { Name = "Utilities" });
            budgetAllocationItems.Add(new BudgetAllocationItem { Name = "Phone" });
            budgetAllocationItems.Add(new BudgetAllocationItem { Name = "Internet" });
            budgetAllocationItems.Add(new BudgetAllocationItem { Name = "Savings", IsSavingsItem = true });
            budgetAllocationItems.Add(new BudgetAllocationItem { Name = "Insurance" });
            budgetAllocationItems.Add(new BudgetAllocationItem { Name = "Entertainment" });
            this.BudgetAllocationItems = budgetAllocationItems.OrderBy(x => x.Name).ToList();

            this.AnnualTotalItems = new List<AnnualTotalItem>();

            var payFrequencyItems = new List<PayFrequencyItem>();
            payFrequencyItems.Add(new PayFrequencyItem { Name = "Weekly", PayChecksPerYear = 52 });
            payFrequencyItems.Add(new PayFrequencyItem { Name = "Biweekly", PayChecksPerYear = 26 });
            payFrequencyItems.Add(new PayFrequencyItem { Name = "Semi-Monthly", PayChecksPerYear = 24 });
            payFrequencyItems.Add(new PayFrequencyItem { Name = "Monthly", PayChecksPerYear = 12 });
            this.PayFrequencyItems = payFrequencyItems;

            _debounceTimer = new Timer();
            _debounceTimer.Elapsed += DebounceTimer_Elapsed;
            _debounceTimer.AutoReset = false;
            _debounceTimer.Interval = 400;
            _recreateChartDelayTimer = new Timer();
            _recreateChartDelayTimer.Elapsed += RecreateChartDelayTimer_Elapsed;
            _recreateChartDelayTimer.AutoReset = false;
            _recreateChartDelayTimer.Interval = 5;
        }

        protected void BudgetItemChanged() {
            _debounceTimer.Stop();
            _debounceTimer.Start();
            Calculate();
        }

        protected override void OnAfterRender(Boolean firstRender) {
            if (firstRender) {
                Calculate();
            }
        }

        protected void PayFrequencyChanged(ChangeEventArgs e) {
            _payFrequency = Int32.Parse(e.Value.ToString());
            Calculate();
        }

        protected void TakeHomePayChanged() {
            Calculate();
        }

        void Calculate() {
            InvokeAsync(() => {
                var annualTotalItems = new List<AnnualTotalItem>();
                this.AnnualTakeHomePay = this.TakeHomePay * _payFrequency;
                this.AnnualSavings = _payFrequency * this.BudgetAllocationItems.Where(x => x.IsSavingsItem).Sum(x => x.Amount);
                var totalAllocations = this.BudgetAllocationItems.Sum(x => x.Amount);
                this.RemainingAllocation = this.TakeHomePay - totalAllocations;
                foreach (var item in this.BudgetAllocationItems) {
                    item.MaxValue = item.Amount + this.RemainingAllocation;
                    if (this.TakeHomePay > 0) {
                        item.WidthPercent = (Single)(100 * (item.MaxValue / this.TakeHomePay));
                    } else {
                        item.WidthPercent = 100;
                    }
                    annualTotalItems.Add(new AnnualTotalItem { Amount = item.Amount * _payFrequency, Name = item.Name });
                }
                this.AnnualTotalItems = annualTotalItems.Where(x => x.Amount > 0).OrderBy(x => x.Amount).ToList();
                this.MinTakeHomePay = totalAllocations;
                StateHasChanged();
            });
        }

        void DebounceTimer_Elapsed(Object sender, ElapsedEventArgs e) {
            InvokeAsync(() => {
                _debounceTimer.Stop();
                var t = Task.Run(() => SetChartShowState(!this.ShowPieChart));
                t.Wait();
                StateHasChanged();
            });
        }

        void RecreateChartDelayTimer_Elapsed(Object sender, ElapsedEventArgs e) {
            InvokeAsync(() => {
                _recreateChartDelayTimer.Stop();
                var t = Task.Run(() => SetChartShowState(!this.ShowPieChart));
                t.Wait();
                StateHasChanged();
            });
        }

        void SetChartShowState(Boolean value) {
            this.ShowPieChart = value;
            if (!value) {
                _recreateChartDelayTimer.Start();
            }
        }
    }
}
